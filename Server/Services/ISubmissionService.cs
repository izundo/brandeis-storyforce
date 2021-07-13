using System.Collections.Generic;
using System.Threading.Tasks;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public interface ISubmissionService : IDataService<Submission>
    {
        Task RemoveWithFilesAsync(int id);
        Task<List<Submission>> GetBySubmittedByIdAsync(int submittedId);
        Task<List<Submission>> GetBySubmittedByInputValueAsync(string  value);
    }
}