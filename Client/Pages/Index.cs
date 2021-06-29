using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Google.Apis.Http;
using HeyRed.Mime;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.JSInterop;
using Sentry;
using StoryForce.Client.UI;
using StoryForce.Shared.ViewModels;
using UploadFile = StoryForce.Shared.ViewModels.UploadFile;
using Microsoft.AspNetCore.Components.Web;

namespace StoryForce.Client.Pages
{
    public partial class Index
    {
        private IEnumerable<IBrowserFile> selectedFiles;
        private string uploadStatus = string.Empty;
        public System.Threading.CancellationTokenSource cancellationTokenSource;
        string Output { get; set; }
        private static string nl = "<br>";
        public long max;
        public long value;
        public bool CanCancel { get; set; }
        public bool IsCancelDisabled => !CanCancel;
        private string modalClass = string.Empty;
        private string successClass = "hide";
        private string modalFooterClass = "hide";
        private string modalDisplay = "none";
        private string modalDisplayRequired = "none";
        private string modalProgressClass = string.Empty;
        private string errorMessageClass = "hide";
        private string submissionFailedMessage = string.Empty;
        private bool showBackdrop = false;
        private int selectedPersonIndex = 0;
        private int submittedCount = 0;
        private static Action<string, string, string, long, string> action;
        private Interop _interop;
        private string _fileKeyPrefix;
        private string[] _googleDocTypes;
        private string filenameVali = string.Empty;

        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        public BlazorFilesSubmission Submission { get; set; }
        
        public UploadByUrl NewUploadByUrl { get; set; }

        public IEnumerable<Person> Students { get; set; }
        public IEnumerable<Person> Staff { get; set; }
        public IEnumerable<Event> Events { get; set; }    

        public IEnumerable<int> Classes
        {
            get
            {
                return Enumerable.Range(DateTime.Now.Year - 10, 19).ToList();

            }
        }

       

        public Index()
        {
            this.ResetPageData();
            this._googleDocTypes = new[]
            {
                "application/vnd.google-apps.document",
                "application/vnd.google-apps.presentation",
                "application/vnd.google-apps.spreadsheet",
                "application/vnd.google-apps.drawing"
            };

            //this.Students = new List<Person>
            //{
            //    new Person{ Name = "Nataan Hong", ClassOfYear = 2021 },
            //    new Person{ Name = "Ezra Rosen", ClassOfYear = 2021 },
            //    new Person{ Name = "Aviv Shakked", ClassOfYear = 2021 },
            //    new Person{ Name = "David Flores", ClassOfYear = 2021 }
            //};
            //this.Events = new List<Event>
            //{
            //    new Event {Name = "First Event", CreatedAt = DateTime.UtcNow, Id = "1"},
            //    new Event {Name = "Second Event", CreatedAt = DateTime.UtcNow, Id = "2"},
            //    new Event {Name = "Third Event", CreatedAt = DateTime.UtcNow, Id = "3"}
            //};
        }
        private void KeyPress(KeyboardEventArgs args)
        {
            if (args.Key == "Enter" || args.Code == "Enter")
            {
                return;
            }
        }
        
        private async Task ClickHandler()
        {
            var isInValid = this.Submission.UploadFiles.Any(x => string.IsNullOrEmpty(x.Description));
            if (isInValid)
            {
                filenameVali = String.Empty;
                foreach ( var item in  this.Submission.UploadFiles.Where(m => m.Description == null))
                {
                    filenameVali +=  item.Title + ",";                    
                }

                CloseModalWindow();
                ShowModalRequiredWindow();
                return;
            }

            await this.HandleSubmission(editContext);
        }
        
        private void ResetPageData()
        {
            this.Submission = new BlazorFilesSubmission();
            this.NewUploadByUrl = new UploadByUrl();
            this._fileKeyPrefix = DateTime.UtcNow.Ticks.ToString();
        }

        private async Task PopulateUserDataFromLocalStorage()
        {
            var name = await GetStringFromLocalStorage("name");
            if (!string.IsNullOrEmpty(name))
            {
                this.Submission.SubmittedBy.Name = name;
            }

            var email = await GetStringFromLocalStorage("email");
            if (!string.IsNullOrEmpty(email))
            {
                this.Submission.SubmittedBy.Email = email;
            }
        }
        private EditContext editContext;

        protected override async Task OnInitializedAsync()
        {
            editContext = new(Submission);
            this._interop = new Interop(JS);
            SentrySdk.Init(Configuration.GetSection("Sentry:Dsn").Value);
            action = AddGoogleFile;

            await PopulateUserDataFromLocalStorage();
            var people = await Http.GetFromJsonAsync<IEnumerable<Person>>("api/people/");
            this.Students = people.Where(p => p.Type == PersonType.Student);
            this.Staff = people.Where(p => p.Type == PersonType.Staff);
            this.Events = await Http.GetFromJsonAsync<IEnumerable<Event>>("api/events/");
        }

        [Parameter]
        public EventCallback OnFileRemoved { get; set; }

        protected async Task HandleSubmission(EditContext editContext)
        {
            ShowModalWindow();

            await this.AddSubmissionAsync(editContext);
        }

        protected async Task OnFileInputChange(InputFileChangeEventArgs e)
        {
            var format = "image/png";
            selectedFiles = e.GetMultipleFiles(10);
            foreach (var file in selectedFiles)
            {
                var uploadFile = new UploadFile
                {
                    Title = file.Name,
                    Key = $"{this._fileKeyPrefix}-{file.Name}",
                    Size = file.Size,
                    MimeType = file.ContentType,
                    StorageProvider = StorageProvider.LocalFileSystem,
                    FeaturedPeople = new List<Person>() { new() }
                };
                if (file.ContentType.StartsWith("image/"))
                {
                    var resizedImageFile = await file.RequestImageFileAsync(format,
                        250, 250);
                    var buffer = new byte[resizedImageFile.Size];
                    await resizedImageFile.OpenReadStream().ReadAsync(buffer);
                    uploadFile.PreviewUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                }

                this.Submission.UploadFiles.Add(uploadFile);
                this.StateHasChanged();
            }
        }

        protected async Task InitGooglePicker()
        {
            var clientId = this.Configuration.GetSection("Google:ClientId").Value;
            var appId = this.Configuration.GetSection("Google:ProjectId").Value;
            var developerKey = this.Configuration.GetSection("Google:DevKey").Value;
            await JS.InvokeVoidAsync("loadPicker", clientId, appId, developerKey);
        }

        void OnPersonChange(object args, UploadFile file, int index, Person person)
        {
            //person;
        }

        void OnEventChange(object value, UploadFile file)
        {
            file.EventName = value.ToString();
        }

        private string GetGoogleAppDownloadUrl(string fileId, string googleMimeType)
        {
            /* populate download url of the file if it is a google document

            https://docs.google.com/document/d/DOC_FILE_ID/export/docx
            https://docs.google.com/presentation/d/PRESENTATION_ID/export/pptx
            https://docs.google.com/spreadsheets/d/FILE_ID/export/xlsx
            https://docs.google.com/drawings/d/FILE_ID/export/png

            */

            switch (googleMimeType)
            {
                case "application/vnd.google-apps.document":
                    return $"https://docs.google.com/document/d/{fileId}/export?format=pdf";

                case "application/vnd.google-apps.presentation":
                    return $"https://docs.google.com/presentation/d/{fileId}/export/pdf";

                case "application/vnd.google-apps.spreadsheet":
                    return $"https://docs.google.com/spreadsheets/d/{fileId}/export?format=pdf";

                case "application/vnd.google-apps.drawing":
                    return $"https://docs.google.com/drawings/d/{fileId}/export/png";

                case { } t when t.StartsWith("image/"):
                    return $"https://lh3.googleusercontent.com/d/{fileId}";

                default:
                    return $"https://www.googleapis.com/drive/v3/files/{fileId}?alt=media";
            }
        }

        private string GetGoogleDocExt(string googleMimeType)
        {
            switch (googleMimeType)
            {
                case "application/vnd.google-apps.document":
                    return "docx";

                case "application/vnd.google-apps.presentation":
                    return "pptx";

                case "application/vnd.google-apps.spreadsheet":
                    return "xlsx";

                case "application/vnd.google-apps.drawing":
                    return "png";

                default:
                    return "";
            }
        }

        protected void AddGoogleFile(string fileName, string fileId, string mimeType, long size, string gDriveOAuthToken)
        {
            this.Submission.GDriveOAuthToken = gDriveOAuthToken;

            var uploadFile = new UploadFile
            {
                Title = fileName,
                Key = $"{this._fileKeyPrefix}-{fileName}",
                Size = size,
                MimeType = mimeType,
                StorageProvider = StorageProvider.GoogleDrive,
                ProviderFileId = fileId
            };

            if (this._googleDocTypes.Contains(mimeType))
            {
                uploadFile.Key = $"{uploadFile.Key}.{GetGoogleDocExt(mimeType)}";
            }
            else
            {
                uploadFile.MimeType = MimeTypesMap.GetMimeType(fileName);
            }

            uploadFile.PreviewUrl = $"https://lh3.googleusercontent.com/d/{fileId}=w250-h250-p-k-nu-iv1" +
                                    (gDriveOAuthToken != null
                                        ? "?access_token=" + gDriveOAuthToken
                                        : string.Empty);

            uploadFile.DownloadUrl = GetGoogleAppDownloadUrl(fileId, mimeType);

            this.Submission.UploadFiles.Add(uploadFile);
            this.StateHasChanged();
        }

        [JSInvokable]
        public static void AddToFileList(string fileName, string fileId, string mimeType, long size, string gDriveOAuthToken)
        {
            action.Invoke(fileName, fileId, mimeType, size, gDriveOAuthToken);
        }

        protected void AddFeaturedPerson(UploadFile file)
        {
            file.FeaturedPeople.Add(new());
        }

        protected void RemoveFeaturedPerson(UploadFile file, Person person)
        {
            file.FeaturedPeople.Remove(person);
        }

        private string GetFileNameFromUrl(string url)
        {
            var parts = url.Split('/');
            var lastPart = parts[parts.Length - 1];
            if (lastPart.Contains("."))
            {
                return Path.GetFileName(new Uri(url).AbsolutePath);
            }
            return "UrlFile";
        }

        protected void AddUrl()
        {
            this.Submission.UploadFiles.Add(new UploadFile()
            {
                DownloadUrl = NewUploadByUrl.DownloadUrl,
                Description = NewUploadByUrl.Description,
                StorageProvider = StorageProvider.Url
            });
            this.NewUploadByUrl = new();
        }

        protected async Task RemoveFile(UploadFile file)
        {
            this.Submission.UploadFiles.Remove(file);
            await OnFileRemoved.InvokeAsync(file);
        }

        protected async Task Loadile(UploadFile file)
        {
            max = file.Size.Value;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (this.Submission.UploadFiles.Count > 1)
            {
                Output += nl;
            }

            this.StateHasChanged();
            Console.WriteLine(Output);
        }

        protected void DuplicateFileAttributes(UploadFile file)
        {
            var files = this.Submission.UploadFiles;
            foreach (var f in files)
            {
                f.Description = file.Description;
                var copiedFeaturedPeople = new List<Person>();

                foreach (var person in file.FeaturedPeople)
                {
                    copiedFeaturedPeople.Add(new Person { 
                        Id = person.Id, 
                        Name = person.Name, 
                        ClassOfYear = person.ClassOfYear, 
                        Type = person.Type 
                    });
                }

                f.FeaturedPeople = copiedFeaturedPeople;

                f.Event = file.Event;
                f.Class = file.Class;
                f.RequestedBy = file.RequestedBy;
            }
        }


        private void ShowModalWindow()
        {
            modalDisplay = "block";
            modalClass = "Show";
            showBackdrop = true;
            this.modalProgressClass = string.Empty;
            this.successClass = "hide";
            this.modalFooterClass = "hide";
            this.StateHasChanged();
        }

        private void ShowModalRequiredWindow()
        {
            modalDisplayRequired = "block";
            modalClass = "Show";
            showBackdrop = true;
            this.StateHasChanged();
        }

        private void CloseModalWindow()
        {
            modalDisplay = "none";
            modalClass = "";
            showBackdrop = false;
            this.StateHasChanged();
        }
        
        private void CloseValidateModalWindow()
        {
            modalDisplayRequired = "none";
            modalClass = "";
            filenameVali = string.Empty;
            showBackdrop = false;
            this.StateHasChanged();
        }

        private void ShowUploadSuccessMessage()
        {
            modalDisplay = "block";
            showBackdrop = true;
            this.successClass = string.Empty;
            this.modalFooterClass = string.Empty;
            this.modalProgressClass = "hide";
            this.StateHasChanged();
        }

        private void ShowErrorMessage()
        {
            this.submissionFailedMessage = "Your upload has failed, but we are on it. Please try again later.";
            this.errorMessageClass = string.Empty;
            this.modalFooterClass = string.Empty;
            this.modalProgressClass = "hide";
            this.successClass = "hide";
            this.StateHasChanged();
        }       

        private async Task SaveDataToLocalStorage()
        {
            await SaveStringToLocalStorage("name", this.Submission.SubmittedBy.Name);
            await SaveStringToLocalStorage("email", this.Submission.SubmittedBy.Email);
        }

        private async Task AddSubmissionAsync(EditContext editContext)
        {
            var model = (BlazorFilesSubmission)editContext.Model;
            await SaveDataToLocalStorage();

            // Upload local files
            var localFiles = this.Submission.UploadFiles
                .Where(f => f.StorageProvider == StorageProvider.LocalFileSystem).ToList();

            if (localFiles.Any())
            {
                var descriptions = localFiles.Select(f => f.Description).ToArray();
                await _interop.UploadFiles("api/file/UploadFileChunk", "uploadFiles", descriptions,
                    this._fileKeyPrefix);
            }

            
            this.Submission.UploadFiles.ForEach(file =>
            {
                // remove blank featured people
                file.FeaturedPeople.RemoveAll(person => string.IsNullOrEmpty(person.Name) && !file.Class.HasValue);                             
            });

            // Upload Url-based files
            var uploadByUrlsFiles =
                this.Submission.UploadFiles.Where(f => f.StorageProvider == StorageProvider.GoogleDrive)
                    .Select(file => new UploadByUrl
                    {
                        DownloadUrl = file.DownloadUrl,
                        Title = file.Title,
                        Key = file.Key,
                        MimeType = file.MimeType,
                        Description = file.Description,
                        Size = file.Size,
                        AccessToken = this.Submission.GDriveOAuthToken,
                        FeaturedPeople = file.FeaturedPeople,
                        Event = file.Event
                    }).ToList();

            var urlFiles = new List<UploadByUrl>();
            foreach (var file in this.Submission.UploadFiles.Where(file => file.StorageProvider == StorageProvider.Url))
            {
                var fileName = GetFileNameFromUrl(file.DownloadUrl);
                file.Title = fileName;
                file.Key = $"{this._fileKeyPrefix}-{fileName}";
                file.MimeType = MimeTypesMap.GetMimeType(fileName);
                file.Description = file.Description;
                file.Size = 0;
                urlFiles.Add(new UploadByUrl
                {
                    DownloadUrl = file.DownloadUrl,
                    Title = file.Title,
                    Key = file.Key,
                    MimeType = file.MimeType,
                    Description = file.Description,
                    Size = file.Size,
                    AccessToken = null,
                    FeaturedPeople = file.FeaturedPeople,
                    Event = file.Event
                });
            }

            //uploadByUrlsFiles.AddRange(urlFiles);

            var uploadByUrlsResponse = await Http.PostAsJsonAsync<UrlsWithAccessToken>("api/s3/UploadByUrls",
                new UrlsWithAccessToken { UploadByUrls = uploadByUrlsFiles.ToArray() });
            if (!uploadByUrlsResponse.IsSuccessStatusCode)
            {
                SentrySdk.CaptureException(new Exception(
                    $"{uploadByUrlsResponse.ReasonPhrase} urls={uploadByUrlsFiles.Select(f => f.DownloadUrl)}"));
                this.ShowErrorMessage();
                return;
            }

            var submissionResponse = await Http.PostAsJsonAsync<BlazorFilesSubmission>("api/submissions/simple", model);
            if (!submissionResponse.IsSuccessStatusCode)
            {
                SentrySdk.CaptureException(new Exception(submissionResponse.ReasonPhrase));
                this.ShowErrorMessage();
                return;
            }

            this.ShowUploadSuccessMessage();
            this.ResetPageData();
            await this.PopulateUserDataFromLocalStorage();

        }

        private async Task SaveStringToLocalStorage(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            await LocalStorage.SetItemAsync<string>(key, value);
        }

        private async Task<string> GetStringFromLocalStorage(string key)
        {
            return await LocalStorage.GetItemAsync<string>(key);
        }       
    }
}
