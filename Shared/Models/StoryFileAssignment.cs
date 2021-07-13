using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StoryForce.Shared.Interfaces;

namespace StoryForce.Shared.Models
{
    [Table("StoryFileAssignment")]
    public class StoryFileAssignment : DatabaseEntity, IDateTracking
    {
        public StoryFile StoryFile { get; set; }
        public Person AssignedTo { get; set; }
        public string Note { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public FileStatus FileStatus { get; set; }

    }

    public enum FileStatus
    {
        New,
        RequiresReview,
        Approved,
        AssignEd
    }
}
