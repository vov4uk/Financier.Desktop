﻿using Financier.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Financier.DataAccess
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DbContext ctx;

        public BaseRepository(DbContext context)
        {
            ctx = context;
        }

        public virtual ValueTask<EntityEntry<T>> AddAsync(T entity)
        {
            return ctx.Set<T>().AddAsync(entity);
        }

        public virtual EntityEntry<T> Add(T entity)
        {
            return ctx.Set<T>().Add(entity);
        }

        public virtual Task AddRangeAsync(IEnumerable<T> entities)
        {
            return ctx.Set<T>().AddRangeAsync(entities);
        }

        public virtual Task<List<T>> GetAllAsync()
        {
            return ctx.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var result = ctx.Set<T>().AsNoTracking().Where(i => true);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            Logger.Info(result.ToQueryString());

            return await result.ToListAsync();
        }

        public virtual Task<List<T>> LastAsync(int last)
        {
            return ctx.Set<T>().OrderByDescending(x => x).Take(last).ToListAsync();
        }

        public virtual async Task<T> FindByAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var result = ctx.Set<T>().AsNoTracking().Where(predicate);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result?.FirstOrDefaultAsync();
        }

        public virtual async Task<List<T>> FindManyAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var result = ctx.Set<T>().AsNoTracking().Where(predicate);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result?.ToListAsync();
        }

        public virtual async Task<List<TResult>> FindManyAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> projection,
            params Expression<Func<T, object>>[] includes)
        {
            var result = ctx.Set<T>().AsNoTracking().Where(predicate);

            foreach (var includeExpression in includes)
            {
                result = result.Include(includeExpression);
            }

            if (result != null)
            {
                var tresult = result.Select(projection);
                Logger.Info(tresult.ToQueryString());

                return await tresult.ToListAsync();
            }

            return default;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                ctx.Set<T>().Attach(entity);
                ctx.Entry(entity).State = EntityState.Modified;

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> identity,
            params Expression<Func<T, object>>[] includes)
        {
            var results = ctx.Set<T>().Where(identity);

            foreach (var includeExpression in includes)
                results = results.Include(includeExpression);
            try
            {
                ctx.Set<T>().RemoveRange(results);
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            ctx.Set<T>().Remove(entity);
            return await Task.FromResult(true);
        }
    }
}