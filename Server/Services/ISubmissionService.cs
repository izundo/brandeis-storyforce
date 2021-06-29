using System.Threading.Tasks;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public interface ISubmissionService : IDataService<Submission>
    {
        Task RemoveWithFilesAsync(int id);
    }
}