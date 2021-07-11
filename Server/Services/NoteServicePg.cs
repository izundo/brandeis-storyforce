using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class NoteServicePg : DataService<Note>, INoteService
    {
        public NoteServicePg(PgDbContext dbContext) : base(dbContext, dbContext.Notes)
        {
            
        }
    }
}