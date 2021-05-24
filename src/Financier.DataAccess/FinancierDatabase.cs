using Financier.DataAccess.DataBase.Scripts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;

namespace Financier.DataAccess
{
    public class FinancierDatabase
    {

        private readonly DbConnection _connection;

        public FinancierDatabase()
            : this(
                new DbContextOptionsBuilder<FinancierDataContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();

        protected FinancierDatabase(DbContextOptions<FinancierDataContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<FinancierDataContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new FinancierDataContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                List<Task> createTasks = new List<Task>();

                ResourceSet create = SQL_create_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                foreach (DictionaryEntry entry in create)
                {
                    string resourceKey = entry.Key.ToString();
                    object resource = entry.Value;
                    createTasks.Add(context.Database.ExecuteSqlRawAsync(Convert.ToString(resource)));
                }
                Task.WaitAll(createTasks.ToArray());


                var alter = SQL_alter_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
                foreach (var entry in alter)
                {
                    context.Database.ExecuteSqlRaw(entry.Value);
                }

                var view = SQL_views_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
                foreach (var entry in view)
                {
                    context.Database.ExecuteSqlRaw(entry.Value);
                }

                context.SaveChanges();
            }
        }
    }
}
