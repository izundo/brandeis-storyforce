using System.Collections.Generic;
using System.Threading.Tasks;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public interface IPeopleService : IDataService<Person>
    {
        Task<Person> GetByEmailOrNameAndYearAsync(string email, string name, int? year);
        Task<Person> GetByEmailAsync(string email);
        Task<List<Person>> GetByAllAsync();
    }
}