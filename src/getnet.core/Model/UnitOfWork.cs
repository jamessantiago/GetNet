using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace getnet.core.Model
{
    public class UnitOfWork : IDisposable
    {
        private Whistler logger = new Whistler();

        public UnitOfWork()
        {
            context.ConfigurationComplete += Context_ConfigurationComplete;
            ConfigurationState = DatabaseConfigurationState.Pending;
        }

        private void Context_ConfigurationComplete(object sender, EventArgs e)
        {
            if (context.IsConfigured)
                ConfigurationState = DatabaseConfigurationState.Configured;
            else
                ConfigurationState = DatabaseConfigurationState.Unconfigured;
        }

        private getnetContext context = new getnetContext();
        private bool disposed = false;
        private GenericRepository<Switch> switchRepository;
        private GenericRepository<Site> siteRepository;
        private GenericRepository<AlertRule> alertRuleRepository;
        public enum DatabaseConfigurationState
        {
            Configured,
            Unconfigured,
            Pending
        }
        public DatabaseConfigurationState ConfigurationState { get; private set; }

        public GenericRepository<AlertRule> AlertRuleRepository
        {
            get
            {
                if (this.alertRuleRepository == null)
                {
                    this.alertRuleRepository = new GenericRepository<AlertRule>(context);
                }
                return alertRuleRepository;
            }
        }

        public GenericRepository<Switch> SwitchRepository
        {
            get
            {
                if (this.switchRepository == null)
                {
                    this.switchRepository = new GenericRepository<Switch>(context);
                }
                return switchRepository;
            }
        }

        public GenericRepository<Site> SiteRepository
        {
            get
            {
                if (this.siteRepository == null)
                {
                    this.siteRepository = new GenericRepository<Site>(context);
                }
                return siteRepository;
            }
        }

        public bool CheckIfDabaseExists()
        {
            return (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
        }

        public bool EnsureDatabaseExists() => context.Database.EnsureCreated();

        public bool EnsureDatabaseIsDeleted() => context.Database.EnsureDeleted();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
    }
}