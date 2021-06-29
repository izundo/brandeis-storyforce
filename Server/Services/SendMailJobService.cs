using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using StoryForce.Server.Models.Options;
using StoryForce.Shared.ViewModels;

namespace StoryForce.Server.Services
{
    public class SendMailJobService : ISendMailJobService, IHostedService, IDisposable
    {
        //The BufferBlock<T> is a thread-safe producer/consumer queue
        private readonly BufferBlock<SendMailRequest> _mailMessagesQueue;
        private readonly ILogger _logger;
        private Task _sendTask;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IOptionsMonitor<SendGridOptions> _optionsMonitor;
        public SendMailJobService(IOptionsMonitor<SendGridOptions> optionsMonitor, ILogger<SendMailJobService> logger)
        {
            this._optionsMonitor = optionsMonitor;
            this._logger = logger;
            this._mailMessagesQueue = new BufferBlock<SendMailRequest>();
        }

        // This public method will be used by other services (such as a Controller or an application service)
        // to send messages, which will be actually delivered asynchronously in background.
        public async Task SendEmailAsync(SendMailRequest sendMailRequest)
        {
            // We just enqueue the message in memory. Delivery will be attempted in background (see the DeliverAsync method)
            await this._mailMessagesQueue.SendAsync(sendMailRequest);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting background e-mail delivery");
            _cancellationTokenSource = new CancellationTokenSource();
            // The StartAsync method just needs to start a background task (or a timer)
            _sendTask = DeliverAsync(_cancellationTokenSource.Token);
            return Task.CompletedTask;
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            CancelSendTask();

            //Next, we wait for sendTask to end, but no longer than what the web host allows
            await Task.WhenAny(_sendTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private void CancelSendTask()
        {
            try
            {
                if (_cancellationTokenSource == null) return;
                _logger.LogInformation("Stopping e-mail background delivery");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            catch
            {
                // ignored
            }
        }

        public async Task DeliverAsync(CancellationToken token)
        {
            _logger.LogInformation("E-mail background delivery started");

            while (!token.IsCancellationRequested)
            {
                SendMailRequest request = null;
                try
                {
                    // Let's wait for a message to appear in the queue
                    // If the token gets canceled, then we'll stop waiting
                    // since an OperationCanceledException will be thrown
                    request = await _mailMessagesQueue.ReceiveAsync(token);

                    // As soon as a message is available, we'll send it
                    var options = this._optionsMonitor.CurrentValue;
                    var apiKey = options.ApiKey;

                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress(options.Sender);
                    var to = new EmailAddress(request.To); //new EmailAddress("steve@izundo.com");
                    var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, null, request.Content);
                    var response = await client.SendEmailAsync(msg, token);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"E-mail sent to {request.To}");
                    }
                    else
                    {
                        _logger.LogError($"Couldn't send an e-mail to {request.To}");
                    }
                    
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Couldn't send an e-mail to {request?.To}");
                }
            }

            _logger.LogInformation("E-mail background delivery stopped");
        }

        public void Dispose()
        {
            CancelSendTask();
        }
    }

}