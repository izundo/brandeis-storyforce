using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class EventService
    {
        private readonly IMongoCollection<Event> _events;

        public EventService(IMongoDbDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _events = database.GetCollection<Event>("Events");
        }

        public async Task<List<Event>> GetAsync() =>
            (await _events.FindAsync(s => true)).ToList();

        public async Task<Event> GetAsync(int id) =>
            (await _events.FindAsync<Event>(s => s.Id == id)).FirstOrDefault();

        public async Task<Event> GetByNameAsync(string name) =>
            (await _events.FindAsync<Event>(s => s.Name == name)).FirstOrDefault();

        public async Task<Event> CreateAsync(Event e)
        {
            await _events.InsertOneAsync(e);
            return e;
        }

        public async Task<List<Event>> CreateMultipleAsync(List<Event> events)
        {
            await _events.InsertManyAsync(events);
            return events;
        }

        public async Task UpdateAsync(int id, Event e) =>
            await _events.ReplaceOneAsync(s => s.Id == id, e);

        public async Task RemoveAsync(Event e) =>
            await _events.DeleteOneAsync(s => s.Id == e.Id);

        public async Task RemoveAsync(int id) =>
            await _events.DeleteOneAsync(s => s.Id == id);
    }
}
