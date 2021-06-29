using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using StoryForce.Shared.Models;

namespace StoryForce.Shared.ViewModels
{
    public class BlazorFilesSubmission
    {
        public BlazorFilesSubmission()
        {
            this.SubmittedBy = new Person();
            this.FileMetaDataList = new List<FileMeta>();
            this.UploadFiles = new List<UploadFile>();            
        }

        [Required]
        [Display(Name = "Files")]
        [ValidateComplexType]
        public List<UploadFile> UploadFiles { get; set; }

        [Required]
        [Display(Name = "Urls")]
        [ValidateComplexType]
        public List<UploadFile> UploadByUrls
        {
            get { return this.UploadFiles.Where(f => f.StorageProvider == StorageProvider.Url).ToList(); }
        }

        public string GDriveOAuthToken { get; set; }

        public List<FileMeta> FileMetaDataList { get; set; }

        public string[] FilesUploadedToServerDisk { get; set; }

        public Event Event { get; set; }

        [Required]
        [ValidateComplexType]
        public Person SubmittedBy { get; set; }

        public Person RequestedBy { get; set; }       

        public Submission ConvertToEntity()
        {
            var createdAt = DateTime.UtcNow;
            return new Submission
            {
                Title = $"{this.SubmittedBy.Name}-{createdAt.ToShortTimeString()} {createdAt.ToShortDateString()}",
                Description = $"{UploadFiles.Count} files by {this.SubmittedBy.Name}",
                CreatedAt = createdAt,
                SubmittedFiles = UploadFiles.Select((file, index) => new StoryFile()
                {
                    Title = file.Title,
                    Key = file.Key,
                    Description = file.Description,
                    Size = file.Size,
                    Type = file.MimeType,
                    DownloadUrl = file.DownloadUrl,
                    ThumbnailUrl = file.ThumbnailUrl,
                    UpdatedAt = createdAt,
                    SubmittedBy = this.SubmittedBy,
                    FeaturedPeople = file.FeaturedPeople,
                    RequestedBy = file.RequestedBy,
                    Event = file.Event                    
                }).ToList(),
            };
        }
    }
}
