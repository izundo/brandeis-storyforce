using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class EventServicePg : DataService<Event>, IEventService
    {
        private readonly PgDbContext _dbContext;

        public EventServicePg(PgDbContext dbContext) : base(dbContext, dbContext.Events)
        {
            _dbContext = dbContext;
        }
        
        public Task<Event> GetByNameAsync(string name)
        {
            return _dbContext.Events.FirstOrDefaultAsync(x => x.Name == name);
        }
        
    }
}