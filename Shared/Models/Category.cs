using System.Collections.Generic;

namespace StoryForce.Shared.Models
{
    public class Category : Filter
    {
        public override string Type { get; } = "Category";

        public Category ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; }

        public ICollection<StoryFile> StoryFiles { get; set; }
    }
}
