using System;
using System.Collections.Generic;
using System.Linq;
using StoryForce.Shared.Models;

namespace StoryForce.Shared.Dtos
{
    public class SubmissionDto : DatabaseEntity
    {
        private List<Person> _featuredPersons;

        public SubmissionDto()
        {
            this.SubmittedFiles = new List<StoryFile>();
            this.FeaturedPeople = new List<Person>();
        }

        public string CreatedAtShortString => this.CreatedAt.ToString("h:mm tt M/d/yy");

        public Person SubmittedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public Person ReviewedBy { get; set; }

        public Person ApprovedBy { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<Note> Notes { get; set; }

        public ApprovalStatusEnum Status { get; set; }

        public List<StoryFile> SubmittedFiles { get; set; }

        public List<StoryFile> ApprovedFiles { get; set; }

        public List<StoryFile> RejectedFiles { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Category> Categories { get; set; }

        public Event Event { get; set; }

        public List<Tag> Tags { get; set; }

        public List<Person> FeaturedPeople
        {
            get => this._featuredPersons;
            set => this._featuredPersons = value;
        }

        public List<string> Keywords { get; set; }

        public List<AuditDetail> History { get; set; }
        
        public string GetFileTypes()
        {
            return string.Join(", ", SubmittedFiles.Select(f => f.GetFileType()));
        }

        public Submission ConvertToEntity()
        {
            return
                new Submission
                {
                    Id = this.Id,
                    ApprovedBy = this.ApprovedBy,
                    Title = this.Title,
                    Description = this.Description,
                    ApprovedFiles = this.ApprovedFiles,
                    Event = this.Event,
                    FeaturedPeople = this.FeaturedPeople,
                    History = this.History,
                    RejectedFiles = this.RejectedFiles,
                    SubmittedFiles = this.SubmittedFiles,
                    SubmittedBy = this.SubmittedBy,
                    CreatedAt = this.CreatedAt,
                    ReviewedBy = this.ReviewedBy,
                    UpdatedAt = this.UpdatedAt                    
                };
        }

        public static SubmissionDto ConvertFromEntity(Submission entity)
        {
            return
                new SubmissionDto
                {
                    Id = entity.Id,
                    ApprovedBy = entity.ApprovedBy,
                    Title = entity.Title,
                    Description = entity.Description,
                    ApprovedFiles = entity.ApprovedFiles.ToList(),
                    Event = entity.Event,
                    FeaturedPeople = entity.FeaturedPeople.ToList(),
                    History = entity.History.ToList(),
                    RejectedFiles = entity.RejectedFiles.ToList(),
                    SubmittedFiles = entity.SubmittedFiles.ToList(),
                    SubmittedBy = entity.SubmittedBy,
                    CreatedAt = entity.CreatedAt,
                    ReviewedBy = entity.ReviewedBy,
                    UpdatedAt = entity.UpdatedAt,
                };
        }

    }
}
