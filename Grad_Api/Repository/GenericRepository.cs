﻿
using Grad_Api.Data;
using Grad_Api.Repositores;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repositores
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GradProjDbContext context;

        public GenericRepository(GradProjDbContext context) {
            this.context = context; 
 
}
        public virtual async  Task<T> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
            
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
