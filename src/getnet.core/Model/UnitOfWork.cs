using System;

namespace getnet.core.Model
{
    public class UnitOfWork : IDisposable
    {
        private getnetContext context = new getnetContext();
        //private GenericRepository<Example> exampleRepository;
        private bool disposed = false;

        //public GenericRepository<example> ExampleRepository
        //{
        //    get
        //    {
        //        if (this.exampleRepository == null)
        //        {
        //            this.exampleRepository = new GenericRepository<Example>(context);
        //        }
        //        return exampleRepository;
        //    }
        //}        

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