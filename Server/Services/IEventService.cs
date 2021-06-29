using System.Collections.Generic;
using System.Threading.Tasks;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public interface IEventService : IDataService<Event>
    {
        Task<Event> GetByNameAsync(string name);
    }
}