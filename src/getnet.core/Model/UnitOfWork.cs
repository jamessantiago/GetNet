using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using getnet.core.Model.Entities;
using getnet.core.Model.Repositories;

namespace getnet.core.Model
{
    public partial class UnitOfWork : IDisposable
    {
        public IServiceProvider Services;

        private bool disposed = false;

        private Whistler logger = new Whistler();

        public UnitOfWork()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            context.ConfigurationComplete += Context_ConfigurationComplete;
            ConfigurationState = DatabaseConfigurationState.Pending;
        }

        public enum DatabaseConfigurationState
        {
            Configured,
            Unconfigured,
            Pending
        }

        public DatabaseConfigurationState ConfigurationState { get; private set; }

        private getnetContext context
        {
            get
            {
                return Services.GetRequiredService<getnetContext>();
            }
        }
        public bool CheckIfDabaseExists()
        {
            return (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool EnsureDatabaseExists() => context.Database.EnsureCreated();

        public bool EnsureDatabaseIsDeleted() => context.Database.EnsureDeleted();

        public IGenericRepository<T> Repo<T>()
        {
            return Services.GetRequiredService<IGenericRepository<T>>();
        }

        public SiteRepository SiteRepo => Services.GetRequiredService<SiteRepository>();

        public void Transaction(Action action, int retryIntervalMs = 1000, int retryCount = 1)
        {
            context.Database.BeginTransaction();
            try
            {
                Retry.Do(action, TimeSpan.FromMilliseconds(retryIntervalMs), retryCount);
                context.Database.CommitTransaction();
            }
            catch (AggregateException ex)
            {
                context.Database.RollbackTransaction();
                throw ex.InnerExceptions.Last();
            }
        }

        public async void TransactionAsync(Action action, int retryIntervalMs = 1000, int retryCount = 1)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                Retry.Do(action, TimeSpan.FromMilliseconds(retryIntervalMs), retryCount);
                context.Database.CommitTransaction();
            }
            catch (AggregateException ex)
            {
                context.Database.RollbackTransaction();
                throw ex.InnerExceptions.Last();
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public bool TestDatabaseConnection(out Exception ex)
        {
            ex = null;
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State == System.Data.ConnectionState.Open)
                    return true;
                else
                    conn.Open();   // check the database connection
                return true;
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<getnetContext>();
            var q = from t in Assembly.Load(new AssemblyName("getnet.core")).GetTypes()
                    where !typeof(IModelBuildItem).IsAssignableFrom(t) &&
                            t != typeof(IModelBuildItem) &&
                            t.Namespace == "getnet.core.Model.Entities"
                    select t;

            foreach (var t in q.ToList())
            {
                services.AddTransient(Type.GetType(t.FullName), Type.GetType(t.FullName));
            }
            services.AddSingleton(typeof(SiteRepository));
            services.AddSingleton(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }

        private void Context_ConfigurationComplete(object sender, EventArgs e)
        {
            if (context.IsConfigured)
                ConfigurationState = DatabaseConfigurationState.Configured;
            else
                ConfigurationState = DatabaseConfigurationState.Unconfigured;
        }
    }
}