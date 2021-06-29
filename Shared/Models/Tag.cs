using System.Collections.Generic;

namespace StoryForce.Shared.Models
{
    public class Tag : Filter
    {
        public override string Type { get; } = "Tag";

        public ICollection<StoryFile> StoryFiles { get; set; }
    }
}
