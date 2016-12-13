using System;
using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace getnet.core.Model
{
    public class UnitOfWork : IDisposable
    {
        private getnetContext context = new getnetContext();
        private GenericRepository<Switch> switchRepository;
        private bool disposed = false;

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

        public bool DabaseExists()
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