using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using HeyRed.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StoryForce.Server.Services;
using StoryForce.Server.ViewModels;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;
using StoryForce.Shared.Services;
using StoryForce.Shared.ViewModels;
using File = Google.Apis.Drive.v3.Data.File;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly IStoryFileService _storyFileService;
        private readonly IPeopleService _peopleService;
        private readonly IEventService _eventService;
        private readonly ISendMailJobService _sendMailJobService;
        private readonly IConfiguration _configuration;
        private readonly ITagService _tagService;
        readonly string[] scopes = { DriveService.Scope.Drive };
        DriveService _gDriveService;
        private FileService _fileService;
        private WebClient _webClient;
        private string UPLOAD_DIRECTORY;

        public SubmissionsController(IConfiguration configration
            , ISubmissionService submissionService
            , IStoryFileService storyFileService
            , IPeopleService peopleService
            , IEventService eventService
            , ISendMailJobService sendMailJobService
            , ITagService tagService
            )
        {
            _configuration = configration;
            _submissionService = submissionService;
            _storyFileService = storyFileService;
            _peopleService = peopleService;
            _gDriveService = InitGoogleDriveService();
            _eventService = eventService;
            _tagService = tagService;
            this._sendMailJobService = sendMailJobService;
            _fileService = new FileService();
            _webClient = new WebClient();
            this.UPLOAD_DIRECTORY = Path.Combine(Path.GetTempPath(), "uploads");
        }

        [HttpGet]
        public async Task<ActionResult<List<SubmissionDto>>> Get()
        {
            var submissions = await _submissionService.GetAsync();

            return submissions.Select(submission => SubmissionDto.ConvertFromEntity(submission)).ToList();
        }

        [HttpGet("{id}", Name = "GetSubmission")]
        public async Task<ActionResult<SubmissionDto>> Get(int id)
        {
            var submission = await _submissionService.GetAsync(id);

            if (submission == null)
            {
                return NotFound();
            }

            return SubmissionDto.ConvertFromEntity(submission);
        }

        [HttpPost("simple")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)] //2GB:1024 * 1024 * 1024 * 2
        public async Task<ActionResult<SubmissionDto>> CreateFromSimple(BlazorFilesSubmission submission)
        {
            var convertedSubmission = submission.ConvertToEntity();

            convertedSubmission.SubmittedBy = await _peopleService.GetByEmailOrNameAndYearAsync(submission.SubmittedBy.Email, submission.SubmittedBy.Name, null) ?? submission.SubmittedBy;

            convertedSubmission.FeaturedPeople = await Task.WhenAll(convertedSubmission
                .FeaturedPeople
                .Select(async person => await _peopleService.GetByEmailOrNameAndYearAsync(person.Email, person.Name, null) ?? person));

            foreach (var storyFile in convertedSubmission.SubmittedFiles)
            {
                storyFile.SubmittedBy = convertedSubmission.SubmittedBy;
                if (storyFile.RequestedBy != null)
                {
                    storyFile.RequestedBy = await _peopleService.GetAsync(storyFile.RequestedBy.Id);
                }

                storyFile.FeaturedPeople = await Task.WhenAll(storyFile
                    .FeaturedPeople
                    .Select(async person => await _peopleService.GetByEmailOrNameAndYearAsync(person.Email, person.Name, null) ?? person));

                if (storyFile.Event != null && !string.IsNullOrEmpty(storyFile.Event.Name))
                {
                    storyFile.Event = await _eventService.GetByNameAsync(storyFile.Event.Name) ?? storyFile.Event;
                }
            }

            var insertedSubmission = await _submissionService.CreateAsync(convertedSubmission);
            foreach (var item in insertedSubmission.SubmittedFiles)
            {
                var createTag = new Tag();
                var createTagdto = new CreateTagDto();
                if (item.RequestedBy == null)
                {
                    createTagdto.Name = "Pending";
                    createTag = await _tagService.CreateAsync(createTagdto.ToEntity());
                    {
                        var tagStoryFile = await _storyFileService.GetAsync(item.Id);
                        if (tagStoryFile == null)
                            return BadRequest($"Story file with id '{createTagdto.StoryFileId}' not found.");
                        tagStoryFile.Tags.Add(createTag);
                        await _storyFileService.UpdateAsync(item.Id, item);
                    }
                }
            }

            var requestedPeople = submission.UploadFiles.Select(x => x.RequestedBy).ToArray();

            if (requestedPeople.Any(x => x != null))
            {
                var requestedPersons = submission.UploadFiles.Select(x => x.RequestedBy?.Email).Distinct().Select(r => requestedPeople.FirstOrDefault(s => s.Email == r)); ;

                foreach (var requestedPerson in requestedPersons)
                {
                    if (requestedPerson == null) continue;

                    await this._sendMailJobService.SendEmailAsync(new SendMailRequest()
                    {
                        To = requestedPerson.Email,
                        Subject = "[StoryForce] Requested documents was uploaded",
                        Content = @$"Dear {requestedPerson.Name}, 
                                <br>
                                The documents you request was uploaded, please check it out <a href='{Request.Scheme}://{Request.Host}/showFile'>here</a>"
                    });

                }
            }

            return CreatedAtRoute("GetSubmission", new { id = insertedSubmission.Id }, insertedSubmission);
        }

        [HttpPost("blazor")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)] //2GB:1024 * 1024 * 1024 * 2
        public async Task<ActionResult<SubmissionDto>> CreateFromBlazor(BlazorFilesSubmission submission)
        {
            var googleDriveId = this._configuration.GetSection("Google:Drive:DriveId").Value;

            var convertedSubmission = submission.ConvertToEntity();
            convertedSubmission.SubmittedBy = await _peopleService.GetByEmailOrNameAndYearAsync(submission.SubmittedBy.Email, submission.SubmittedBy.Name, null) ?? submission.SubmittedBy;

            for (var index = 0; index < submission.UploadFiles.Count; index++)
            {
                var currentFile = submission.UploadFiles[index];
                if (currentFile.StorageProvider == StorageProvider.LocalFileSystem && currentFile.Size == 0)
                {
                    continue;
                }

                File newFile = new File
                {
                    Name = currentFile.Title,
                    Description = convertedSubmission.SubmittedFiles != null && convertedSubmission.SubmittedFiles.Count > 0
                        ? convertedSubmission.SubmittedFiles.ElementAt(index).Description
                        : string.Empty,
                    Parents = new List<string>() { googleDriveId }
                };

                Stream readStream = null;

                if (currentFile.StorageProvider == StorageProvider.LocalFileSystem)
                {
                    newFile.MimeType = currentFile.MimeType;
                    readStream = new MemoryStream();
                }
                else
                {
                    newFile.MimeType = MimeTypesMap.GetMimeType(newFile.Name);
                    string downloadUrl = string.Empty;

                    if (currentFile.StorageProvider == StorageProvider.GoogleDrive)
                    {
                        downloadUrl =
                            $"https://lh3.googleusercontent.com/d/{currentFile.ProviderFileId}" +
                            (submission.GDriveOAuthToken != null
                                ? "?access_token=" + submission.GDriveOAuthToken
                                : string.Empty);
                    }

                    if (currentFile.StorageProvider == StorageProvider.Url)
                    {
                        downloadUrl = currentFile.DownloadUrl;
                    }

                    readStream = await _webClient.OpenReadTaskAsync(new Uri(downloadUrl));
                }

                var tempFilename = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
                await using (var stream = new FileStream(tempFilename, FileMode.CreateNew))
                {
                    const int chunkSize = 1024;
                    var buffer = new byte[chunkSize];
                    var bytesRead = 0;
                    do
                    {
                        bytesRead = await readStream.ReadAsync(buffer, 0, buffer.Length);
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    } while (bytesRead > 0);

                    await readStream.DisposeAsync();
                }

                var uploadStream = new FileStream(tempFilename, FileMode.Open);

                var request = _gDriveService.Files.Create(newFile, uploadStream, currentFile.MimeType);
                request.ChunkSize = 1 * 1024 * 1024; //1MB per chunk
                request.Fields = "id, name, webViewLink, webContentLink, thumbnailLink, createdTime, size";

                request.ResponseReceived += (file) =>
                {
                    var storyFile = convertedSubmission.SubmittedFiles.SingleOrDefault(f => f.Title == file.Name);
                    storyFile.DownloadUrl = file.WebContentLink;
                    storyFile.ThumbnailUrl = file.ThumbnailLink;
                    storyFile.UpdatedAt = convertedSubmission.CreatedAt;
                    storyFile.SubmissionId = convertedSubmission.Id;
                    storyFile.SubmittedBy = convertedSubmission.SubmittedBy;
                    storyFile.Size = file.Size;
                };

                await request.UploadAsync();
            }

            convertedSubmission.FeaturedPeople = await Task.WhenAll(convertedSubmission
                .FeaturedPeople
                .Select(async person => await _peopleService.GetByEmailOrNameAndYearAsync(person.Email, person.Name, null) ?? person));

            foreach (var storyFile in convertedSubmission.SubmittedFiles)
            {
                storyFile.SubmittedBy = convertedSubmission.SubmittedBy;
                if (storyFile.RequestedBy != null)
                {
                    storyFile.RequestedBy = await _peopleService.GetAsync(storyFile.RequestedBy.Id);
                }

                storyFile.FeaturedPeople = await Task.WhenAll(storyFile
                    .FeaturedPeople
                    .Select(async person => await _peopleService.GetByEmailOrNameAndYearAsync(person.Email, person.Name, null) ?? person));
            }


            if (convertedSubmission.Event != null && !string.IsNullOrEmpty(convertedSubmission.Event.Name))
            {
                convertedSubmission.Event = await _eventService.GetByNameAsync(convertedSubmission.Event.Name) ?? submission.Event;
            }

            var insertedSubmission = await _submissionService.CreateAsync(convertedSubmission);

            return CreatedAtRoute("GetSubmission", new { id = insertedSubmission.Id }, insertedSubmission);
        }

        private async Task SaveFileChunkToDisk(IFormFile file)
        {
            var tempFilename = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
            await using var stream = new FileStream(tempFilename, FileMode.CreateNew);
            await file.CopyToAsync(stream);
        }

        private async Task UploadFileToGoogleDrive(Submission converted, Person submitter, UploadFile currentFile, string readFilePath)
        {
            var googleDriveId = this._configuration.GetSection("Google:Drive:DriveId").Value;

            File newFile = new File
            {
                Name = currentFile.Title,
                Description = currentFile.Description,
                Parents = new List<string>() { googleDriveId }
            };
            var uploadStream = new FileStream(readFilePath, FileMode.Open);

            var request = _gDriveService.Files.Create(newFile, uploadStream, currentFile.MimeType);
            request.ChunkSize = 1 * 1024 * 1024; //1MB per chunk
            request.Fields = "id, name, webViewLink, webContentLink, thumbnailLink, createdTime, size";

            request.ResponseReceived += (file) =>
            {
                var storyFile = converted.SubmittedFiles.SingleOrDefault(f => f.Title == file.Name);
                storyFile.DownloadUrl = file.WebContentLink;
                storyFile.ThumbnailUrl = file.ThumbnailLink;
                storyFile.UpdatedAt = converted.CreatedAt;
                storyFile.SubmissionId = converted.Id;
                storyFile.SubmittedBy = submitter;
                storyFile.Size = file.Size;
            };

            await request.UploadAsync();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)] //2GB:1024 * 1024 * 1024 * 2
        public async Task<ActionResult<SubmissionDto>> Create([FromForm] FilesSubmission submission)
        {
            var googleDriveId = this._configuration.GetSection("Google:Drive:DriveId").Value;
            var converted = submission.ConvertToEntity();
            var submitter = await _peopleService.CreateAsync(converted.SubmittedBy);
            converted.SubmittedBy = submitter;

            for (var index = 0; index < submission.FormFiles.Count; index++)
            {
                var currentFile = submission.FormFiles[index];
                File newFile = new File
                {
                    Name = currentFile.FileName,
                    Description = converted.SubmittedFiles != null && converted.SubmittedFiles.Count > 0
                        ? converted.SubmittedFiles.ElementAt(index).Description
                        : string.Empty,
                    Parents = new List<string>() { googleDriveId }
                };

                Stream readStream;

                if (currentFile.Length > 0)
                {
                    newFile.MimeType = currentFile.ContentType;
                    readStream = currentFile.OpenReadStream();
                }
                else
                {
                    newFile.MimeType = MimeTypesMap.GetMimeType(newFile.Name);
                    var fileMeta = submission.FileMetaDataList[index];
                    string downloadUrl = string.Empty;
                    if (fileMeta.StorageProvider == StorageProvider.GoogleDrive)
                    {
                        downloadUrl =
                            $"https://lh3.googleusercontent.com/d/{fileMeta.FileId}" +
                            (submission.GDriveOAuthToken != null
                                ? "?access_token=" + submission.GDriveOAuthToken
                                : string.Empty);
                    }
                    else if (fileMeta.StorageProvider == StorageProvider.Url)
                    {
                        downloadUrl = fileMeta.DownloadUrl;
                    }

                    readStream = await _webClient.OpenReadTaskAsync(new Uri(downloadUrl));
                }

                var tempFilename = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
                await using (var stream = new FileStream(tempFilename, FileMode.CreateNew))
                {
                    const int chunkSize = 1024;
                    var buffer = new byte[chunkSize];
                    var bytesRead = 0;
                    do
                    {
                        bytesRead = await readStream.ReadAsync(buffer, 0, buffer.Length);
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    } while (bytesRead > 0);

                    await readStream.DisposeAsync();
                }

                var uploadStream = new FileStream(tempFilename, FileMode.Open);

                var request = _gDriveService.Files.Create(newFile, uploadStream, currentFile.ContentType);
                request.ChunkSize = 1 * 1024 * 1024; //1MB per chunk
                request.Fields = "id, name, webViewLink, webContentLink, thumbnailLink, createdTime, size";

                request.ResponseReceived += (file) =>
                {
                    var storyFile = converted.SubmittedFiles.SingleOrDefault(f => f.Title == file.Name);
                    storyFile.DownloadUrl = file.WebContentLink;
                    storyFile.ThumbnailUrl = file.ThumbnailLink;
                    storyFile.UpdatedAt = converted.CreatedAt;
                    storyFile.SubmissionId = converted.Id;
                    storyFile.SubmittedBy = submitter;
                    storyFile.Size = file.Size;
                };

                await request.UploadAsync();
            }

            // await _peopleService.CreateMultipleAsync(converted.FeaturedPeople);
            foreach (var person in converted.FeaturedPeople)
            {
                var addedPerson = await _peopleService.CreateAsync(person);
                person.Id = addedPerson.Id;
            }

            await _storyFileService.CreateMultipleAsync(converted.SubmittedFiles.ToList());

            if (converted.Event != null && !string.IsNullOrEmpty(converted.Event.Name))
            {
                await _eventService.CreateAsync(converted.Event);
            }

            await _submissionService.CreateAsync(converted);

            return CreatedAtRoute("GetSubmission", new { id = converted.Id }, converted);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SubmissionDto updatedSubmission)
        {
            var submission = await _submissionService.GetAsync(id);

            if (submission == null)
            {
                return NotFound();
            }

            await _submissionService.UpdateAsync(id, updatedSubmission.ConvertToEntity());

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var submission = await _submissionService.GetAsync(id);

            if (submission == null)
            {
                return NotFound();
            }

            await _submissionService.RemoveAsync(id);

            return NoContent();
        }


        private DriveService InitGoogleDriveService()
        {
            var serviceAccount = this._configuration.GetSection("Google:ServiceAccount:Email").Value;
            var privateKey = this._configuration.GetSection("Google:ServiceAccount:Key").Value;
            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccount)
                {
                    Scopes = scopes
                }.FromPrivateKey(privateKey));

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Brandeis Story Force",
            });

            return service;
        }
        public class CreateTagDto
        {
            public string Name { get; set; }

            public int StoryFileId { get; set; }
            public Tag ToEntity()
            {
                return new Tag()
                {
                    Name = this.Name,
                    CreatedAt = DateTime.Now
                };
            }
        }
    }
}
