using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace getnet.core.Model
{
    public class UnitOfWork : IDisposable
    {
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
        public enum DatabaseConfigurationState
        {
            Configured,
            Unconfigured,
            Pending
        }
        public DatabaseConfigurationState ConfigurationState { get; private set; }

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
        public bool CheckIfDabaseExists()
        {
            return (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
        }

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