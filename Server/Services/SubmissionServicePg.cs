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
                .Include(x => x.SubmittedFiles)
                .Include(x => x.SubmittedBy)
                .Include(x => x.ReviewedBy)
                .Include(x => x.ApprovedBy)
                .Include(x => x.Event)
                .Include(x => x.History)
                .ToListAsync();
        }

        public override Task<Submission> GetAsync(int id)
        {
            return _dbContext
                .Submissions
                .Include(x => x.ApprovedFiles)
                .Include(x => x.RejectedFiles)
                .Include(x => x.SubmittedFiles)
                .Include(x => x.SubmittedBy)
                .Include(x => x.ReviewedBy)
                .Include(x => x.ApprovedBy)
                .Include(x => x.Event)
                .Include(x => x.History)
                .FirstOrDefaultAsync(s => s.Id == id);
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