﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Financier.DataAccess.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        void SaveChanges();
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
