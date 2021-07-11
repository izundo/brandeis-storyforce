using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class StoryFileService
    {
        private readonly IMongoCollection<StoryFile> _storyFiles;
        private readonly IMongoCollection<Note> _notes;

        public StoryFileService(IMongoDbDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _storyFiles = database.GetCollection<StoryFile>("StoryFiles");
            _notes = database.GetCollection<Note>("Notes");
        }

        public async Task<List<StoryFile>> GetAsync() =>
            (await _storyFiles.FindAsync(s => true)).ToList();

        public async Task<StoryFile> GetAsync(int id) =>
            (await _storyFiles.FindAsync<StoryFile>(s => s.Id == id)).FirstOrDefault();

        public async Task<StoryFile> CreateAsync(StoryFile storyFile)
        {
            await _storyFiles.InsertOneAsync(storyFile);
            return storyFile;
        }

        public async Task<List<StoryFile>> CreateMultipleAsync(List<StoryFile> storyFiles)
        {
            await _storyFiles.InsertManyAsync(storyFiles);
            return storyFiles;
        }

        public async Task UpdateAsync(int id, StoryFile storyFile) =>
            await _storyFiles.ReplaceOneAsync(s => s.Id == id, storyFile);

        public async Task RemoveAsync(StoryFile storyFile) =>
            await _storyFiles.DeleteOneAsync(s => s.Id == storyFile.Id);

        public async Task RemoveAsync(int id) =>
            await _storyFiles.DeleteOneAsync(s => s.Id == id);

        public async Task RemoveNoteAsync(int id) =>
            await _notes.DeleteOneAsync(s => s.Id == id);
    }
}
