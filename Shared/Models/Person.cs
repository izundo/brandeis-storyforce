using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StoryForce.Shared.Models
{
    public class Person : IdentityUser<int>
    {
        public string Name { get; set; }

        public override string Email { get; set; }

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
