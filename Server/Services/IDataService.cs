using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoryForce.Server.Services
{
    public interface IDataService<TEntity>
    {
        Task<List<TEntity>> GetAsync();
        Task<TEntity> GetAsync(int id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<List<TEntity>> CreateMultipleAsync(List<TEntity> entities);
        Task UpdateAsync(int id, TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task RemoveAsync(int id);
    }
}