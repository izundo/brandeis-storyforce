using System;
using System.ComponentModel.DataAnnotations;

namespace StoryForce.Shared.Models
{
    public class DatabaseEntity
    {
        public DatabaseEntity()
        {
            this.CreatedAt = DateTime.UtcNow;
        }
        
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
