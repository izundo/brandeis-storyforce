using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IMongoCollection<Person> _people;

        public PeopleService(IMongoDbDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _people = database.GetCollection<Person>("People");
        }

        public async Task<List<Person>> GetAsync() =>
            (await _people.FindAsync(s => true)).ToList();

        public async Task<Person> GetAsync(int id) =>
            (await _people.FindAsync<Person>(s => s.Id == id)).FirstOrDefault();

        public async Task<Person> GetByEmailOrNameAndYearAsync(string email, string name, int? year)
        {
            if (!string.IsNullOrEmpty(name) && year.HasValue)
            {
                return (await _people.FindAsync<Person>(s => !string.IsNullOrEmpty(s.Name)
                                                             && s.Name.Equals(name)
                                                             && s.ClassOfYear.HasValue
                                                             && s.ClassOfYear.Value == year.Value)).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                return (await _people.FindAsync<Person>(s => !string.IsNullOrEmpty(s.Name)
                                                             && s.Name.Equals(name)
                                                             && !string.IsNullOrEmpty(s.Email)
                                                             && s.Email.Equals(email))).FirstOrDefault();
            }

            return null;
        }

        public async Task<Person> CreateAsync(Person person)
        {
            var existingPerson =
                await GetByEmailOrNameAndYearAsync(person.Email, person.Name, person.ClassOfYear);
            
            if (existingPerson != null)
            {
                return existingPerson;
            }

            await _people.InsertOneAsync(person);
            return person;
        }

        public async Task<List<Person>> CreateMultipleAsync(List<Person> people)
        {
            await _people.InsertManyAsync(people);
            return people;
        }

        public async Task UpdateAsync(int id, Person person) =>
            await _people.ReplaceOneAsync(s => s.Id == id, person);

        public async Task RemoveAsync(Person person) =>
            await _people.DeleteOneAsync(s => s.Id == person.Id);

        public async Task RemoveAsync(int id) =>
            await _people.DeleteOneAsync(s => s.Id == id);

        public Task<Person> GetByEmailAsync(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Person>> GetByAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
