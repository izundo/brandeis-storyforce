using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class TagServicePg : DataService<Tag>, ITagService
    {
        public TagServicePg(PgDbContext dbContext) : base(dbContext, dbContext.Tags)
        {

        }
    }
}