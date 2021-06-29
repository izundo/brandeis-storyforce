using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public abstract class DataService<TEntity> : IDataService<TEntity> where TEntity : DatabaseEntity
    {
        private readonly PgDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        protected DataService(PgDbContext dbContext, DbSet<TEntity> dbSet)
        {
            _dbContext = dbContext;
            _dbSet = dbSet;
        }

        public virtual Task<List<TEntity>> GetAsync() =>
            _dbSet.AsNoTracking().ToListAsync();

        public virtual Task<TEntity> GetAsync(int id) =>
            _dbSet.FirstOrDefaultAsync(x => x.Id == id);

       
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            var addedEntry = await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return addedEntry.Entity;
        }

        public virtual async Task<List<TEntity>> CreateMultipleAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public virtual async Task UpdateAsync(int id, TEntity entity)
        {
            var itemToUpdate = await _dbSet.FindAsync(id);
            if (itemToUpdate == null)
                throw new NullReferenceException();
            itemToUpdate.Id = id;
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            var itemToRemove = await _dbSet.FindAsync(entity.Id);
            if (itemToRemove == null)
                throw new NullReferenceException();

            _dbSet.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(int id)
        {
            var itemToRemove = await _dbSet.FindAsync(id);
            if (itemToRemove == null)
                throw new NullReferenceException();

            _dbSet.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
        }
    }
}