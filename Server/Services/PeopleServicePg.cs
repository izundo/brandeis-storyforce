using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class PeopleServicePg : DataService<Person>, IPeopleService
    {
        private readonly PgDbContext _dbContext;

        public PeopleServicePg(PgDbContext dbContext): base(dbContext, dbContext.Persons)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Person> GetByEmailOrNameAndYearAsync(string email, string name, int? year)
        {
            if (!string.IsNullOrEmpty(name) && year.HasValue)
            {
                return await _dbContext.Persons.FirstOrDefaultAsync(s => !string.IsNullOrEmpty(s.Name)
                                                                         && s.Name.Equals(name)
                                                                         && s.ClassOfYear.HasValue
                                                                         && s.ClassOfYear.Value == year.Value);
            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                return await _dbContext.Persons.FirstOrDefaultAsync(s => !string.IsNullOrEmpty(s.Name)
                                                                         && s.Name.Equals(name)
                                                                         && !string.IsNullOrEmpty(s.Email)
                                                                         && s.Email.Equals(email));
            }

            return null;
        }

        public override async Task<Person> CreateAsync(Person person)
        {
            var existingPerson =
                await GetByEmailOrNameAndYearAsync(person.Email, person.Name, person.ClassOfYear);

            if (existingPerson != null)
            {
                return existingPerson;
            }

            var addedEntry = await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();
            return addedEntry.Entity;
        }
        
    }
}