using System.ComponentModel.DataAnnotations;

namespace StoryForce.Shared.Models
{
    public class Administrator
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Year")]
        public int? ClassOfYear { get; set; }

        public string AvatarUrl { get; set; }

        [EnumDataType(typeof(PersonType))]
        public PersonType? Type { get; set; }

        public string Role { get; set; }
    }
}
