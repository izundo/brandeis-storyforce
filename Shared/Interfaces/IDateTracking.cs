using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryForce.Shared.Interfaces
{
    public interface IDateTracking
    {
        string ModifiedBy { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? LastModifiedDate { get; set; }
    }
}
