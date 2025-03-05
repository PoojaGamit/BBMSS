using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBMSDATA1.Repository.Generic.Interface
{
    public interface IRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetPagedResponseAsync(int page, int pageSize);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
