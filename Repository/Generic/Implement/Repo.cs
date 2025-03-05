using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBMSDATA1.Context;
using BBMSDATA1.Repository.Generic.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BBMSDATA1.Repository.Generic.Implement
{
    public class Repo<T> : IRepo<T> where T : class
    {
  

        private readonly MYDBContext context;
        private readonly DbSet<T> dbSet;
        public Repo(MYDBContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
           await dbSet.AddAsync(entity);
           await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
           var entity= await GetByIdAsync(id);
            if(entity != null)
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync();

            }
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with id {id} not found.");
            }
            return entity;
        }

        public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int page, int pageSize)
        {
            return await context
                .Set<T>()
                .Skip((page- 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();

        }
    }
}
