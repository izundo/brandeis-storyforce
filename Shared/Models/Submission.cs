using System;
using System.Collections.Generic;

namespace StoryForce.Shared.Models
{
    public class Submission : DatabaseEntity
    {
        public Submission()
        {
            this.SubmittedFiles = new List<StoryFile>();
            this.FeaturedPeople = new List<Person>();
        }
        
        public Person SubmittedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public Person ReviewedBy { get; set; }
        
        public Person ApprovedBy { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<Person> FeaturedPeople { get; set; }

        public Event Event { get; set; }

        public ICollection<AuditDetail> History { get; set; }
        
        public ICollection<StoryFile> SubmittedFiles { get; set; }
        
        public ICollection<StoryFile> ApprovedFiles { get; set; }
        
        public ICollection<StoryFile> RejectedFiles { get; set; }
    }
}
