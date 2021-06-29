using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoryForce.Shared.Models
{
    public class Person : DatabaseEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Year")]
        public int? ClassOfYear { get; set; }

        public string AvatarUrl { get; set; }

        [EnumDataType(typeof(PersonType))]
        public PersonType? Type { get; set; }

        public ICollection<Submission> FeaturedSubmissions { get; set; }

        public ICollection<StoryFile> FeaturedStoryFile { get; set; }

        public ICollection<Submission> SubmittedSubmissions { get; set; }

        public ICollection<Submission> ReviewedBySubmissions { get; set; }

        public ICollection<Submission> ApprovedSubmissions { get; set; }

        public ICollection<StoryFile> SubmittedStoryFiles { get; set; }

        public ICollection<StoryFile> RequestedStoryFiles { get; set; }
        
        public ICollection<StoryFile> UpdatedStoryFiles { get; set; }

        public string Role { get; set; }
    }

    public enum PersonType
    {
        Student,
        Class,
        Staff,
        Parent,
        Alumni,
        CommunityMember,
        Other,
        Administrator
    }
}
