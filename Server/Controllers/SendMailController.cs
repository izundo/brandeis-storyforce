using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using StoryForce.Server.Services;
using StoryForce.Shared.ViewModels;

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMailController : Controller
    {
        private readonly SendMailJobService _sendMailJobService;

        private readonly SubmissionService _submissionService;
      
        public SendMailController(SendMailJobService sendMailJobService, SubmissionService submissionService)
        {            
            _sendMailJobService = sendMailJobService ;
            _submissionService = submissionService;

        }

        [HttpPost]
        public async Task<ActionResult> SendMail(SendMailRequest sendMailRequest)
        {
            await _sendMailJobService.SendEmailAsync(sendMailRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> getByEmail(string email)
        {
            var task = await _submissionService.GetByRequestedByEmailAsync(email);
            if (task == null) return BadRequest();
            return Ok(task);
        }
    }
}
