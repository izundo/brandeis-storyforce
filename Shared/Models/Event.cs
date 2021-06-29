using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoryForce.Shared.Models
{
    public class Event : Filter
    {
        public int? Category { get; set; }

        public override string Type { get; } = "Event";

        public DateTime? Date { get; set; }

        [Display(Name = "Event Year")]
        [Range(1960, 3000)]

        public int? Year { get; set; }

        public ICollection<StoryFile> StoryFiles { get; set; }
    }
}
