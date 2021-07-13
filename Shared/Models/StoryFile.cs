using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace StoryForce.Shared.Models
{
    [BsonIgnoreExtraElements]
    public class StoryFile : DatabaseEntity
    {
        public string Title { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }
        public int EventId { get; set; }

        public List<Note> Notes { get; set; }                                                      

        [EnumDataType(typeof(ApprovalStatusEnum))]
        public ApprovalStatusEnum Status { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Category> Categories { get; set; }

        public Event Event { get; set; }

        public List<Tag> Tags { get; set; }

        public ICollection<Person> FeaturedPeople { get; set; }

        public List<string> Keywords { get; set; }

        public List<AuditDetail> History { get; set; }

        public int SubmissionId { get; set; }
        public int RequestedById { get; set; }
        public int SubmittedById { get; set; }

        public Submission Submission { get; set; }

        public int? ApprovedSubmissionId { get; set; }

        public Submission ApprovedSubmission { get; set; }
        
        public int? RejectedSubmissionId { get; set; }

        public Submission RejectedSubmission { get; set; }

        public string DownloadUrl { get; set; }

        public int? Class { get; set; }

        public ICollection<StoryFileAssignment> StoryFileAssignment { get; set; }

        [BsonIgnore]
        [NotMapped]
        public string GoogleFileId
        {
            get
            {
                if (string.IsNullOrEmpty(this.DownloadUrl))
                {
                    return null;
                }

                var uri = new Uri(this.DownloadUrl);
                var qDictionary = HttpUtility.ParseQueryString(uri.Query);
                return qDictionary["id"];
            }
        }

        public long? Size { get; set; }

        [BsonIgnore]
        [NotMapped]
        public string SizeText
        {
            get
            {
                if (this.Size.HasValue)
                {
                    if (this.Size.Value == 0)
                    {
                        return "Unknown";
                    }

                    return this.Size.Value.ToString();
                }

                return "Unknown";
            }
        }

        public string ThumbnailUrl { get; set; }

        public string Type { get; set; }

        [BsonIgnore]
        [NotMapped]
        public string ImageData { get; set; }

        public DateTime UploadedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Person SubmittedBy { get; set; }

        public Person RequestedBy { get; set; }

        public Person UpdatedBy { get; set; }

        public List<Story> BelongsTo { get; set; }

        public List<Event> Events { get; set; }

        public string GetResizedImageUrl(int width, int height)
        {
            var json = new
            {
                bucket = "storyforce", key = this.Key, edits = new
                {
                    resize = new { width = width, height = height, fit = "cover"}
                }
            };
            var encodedParams = BtoA(JsonConvert.SerializeObject(json));
            return $"https://d13zk917buogwh.cloudfront.net/{encodedParams}";
        }

        public string GetFileType()
        {
            var title = Title.ToLower();

            if (title.EndsWith(".jpg") || title.EndsWith(".jpeg"))
            {
                return "image/jpeg";
            }

            if (title.EndsWith(".png"))
            {
                return "image/png";
            }

            if (title.EndsWith(".mp4") || title.EndsWith(".m4a") || title.EndsWith(".m4p") ||
                title.EndsWith(".m4v"))
            {
                return "video/mp4";
            }

            return "Unknown";
        }

        public string BtoA(string toEncode)
        {
            byte[] bytes = Encoding.GetEncoding(28591).GetBytes(toEncode);
            string toReturn = System.Convert.ToBase64String(bytes);
            return toReturn;
        }        
    }
}
