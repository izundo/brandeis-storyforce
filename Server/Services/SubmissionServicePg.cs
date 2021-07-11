using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class SubmissionServicePg : DataService<Submission>, ISubmissionService
    {
        private readonly PgDbContext _dbContext;

        public SubmissionServicePg(PgDbContext dbContext) : base(dbContext, dbContext.Submissions)
        {
            _dbContext = dbContext;
        }

        public override Task<List<Submission>> GetAsync()
        {
            return _dbContext
                .Submissions
                .Include(x => x.ApprovedFiles)
                .Include(x => x.RejectedFiles)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Notes)
                .Include(x => x.SubmittedBy)
                .Include(x => x.ReviewedBy)
                .Include(x => x.ApprovedBy)
                .Include(x => x.Event)
                .Include(x => x.History)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Tags)
                .Include(x => x.NoteFile)
                .ToListAsync();
        }

        public override Task<Submission> GetAsync(int id)
        {
            return _dbContext
                .Submissions
                .Include(x => x.ApprovedFiles)
                .Include(x => x.RejectedFiles)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Notes)
                .Include(x => x.SubmittedBy)
                .Include(x => x.ReviewedBy)
                .Include(x => x.ApprovedBy)
                .Include(x => x.Event)
                .Include(x => x.History)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Tags)
                .Include(x => x.NoteFile)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<List<Submission>> GetBySubmittedByIdAsync(int id)
             => await _dbContext
                .Submissions
                .Include(x => x.ApprovedFiles)
                .Include(x => x.RejectedFiles)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Notes)
                .Include(x => x.SubmittedBy)
                .Include(x => x.ReviewedBy)
                .Include(x => x.ApprovedBy)
                .Include(x => x.Event)
                .Include(x => x.History)
                .Include(x => x.SubmittedFiles).ThenInclude(m => m.Tags)
                .Include(x => x.NoteFile)
                .Where(m => m.SubmittedBy.Id == id)
                .ToListAsync();

        public async Task<List<Submission>> GetBySubmittedByInputValueAsync(string value)
        {
            var list = _dbContext.Submissions.Include(m => m.SubmittedFiles).ThenInclude(x => x.Tags).Include(m=>m.SubmittedBy).Include(m=>m.ReviewedBy)
                .Where(m => m.Title.ToLower().Contains(value.ToLower())
            || m.Description.ToLower().Contains(value.ToLower())
            || m.SubmittedFiles.Any(sub => sub.Title.ToLower().Contains(value.ToLower()))
            || m.SubmittedFiles.Any(sub => sub.Description.ToLower().Contains(value.ToLower())
            || m.SubmittedFiles.Any(sub => sub.Tags.Any(tag => tag.Name.ToLower().Contains(value.ToLower()))))
            ).ToList();
            return list;
        }
        public async Task RemoveWithFilesAsync(int id)
        {
            await using var session = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.StoryFiles.RemoveRange(_dbContext.StoryFiles.Where(f => f.SubmissionId == id).ToArray());
                _dbContext.Submissions.Remove(await _dbContext.Submissions.FindAsync(id));
                await _dbContext.SaveChangesAsync();
                await session.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting Submission and its StoryFiles: " + e.Message);
                await session.RollbackAsync();
            }
        }
    }

}