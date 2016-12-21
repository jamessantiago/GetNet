using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet;
using System.Threading;
using getnet.core;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.Helpers;
using Xunit;

namespace getnet.tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void CheckDatabaseTest()
        {
            var w = new Whistler();
            CoreCurrent.Configuration["Data:SqlServerConnectionString"] = "Server=.\\SQLEXPRESS;Database=getnetTests;Integrated Security=true";
            using (UnitOfWork uow = new UnitOfWork())
            {
                w.Info("test", "test");
                bool configured, exists = false, tested, deleted = false;

                Exception testException = null;
                Exception createException = null;
                Exception deleteException = null;
                
                tested = uow.TestDatabaseConnection(out testException);
                configured = uow.ConfigurationState == UnitOfWork.DatabaseConfigurationState.Configured;

                try
                {
                    exists = uow.EnsureDatabaseExists();
                    tested = uow.TestDatabaseConnection(out testException);

                } catch (Exception e)
                {
                    createException = e;
                } 

                try
                {
                    deleted = uow.EnsureDatabaseIsDeleted();
                } catch (Exception ee)
                {
                    deleteException = ee;
                }

                if (testException != null)
                    throw testException;
                if (createException != null)
                    throw createException;
                if (deleteException != null)
                    throw deleteException;

                w.Fatal("test2", "test2");

                Assert.True(configured);
                Assert.True(tested);
                Assert.True(exists);
                Assert.True(deleted);
            }
        }

        [Fact]
        public void CreateSiteTest()
        {
            CoreCurrent.Configuration.SetSecure("Data:SqlServerConnectionString", "Server=.\\SQLEXPRESS;Database=getnetTests;Integrated Security=true");
            
            using (UnitOfWork uow = new UnitOfWork())
            {
                Exception testException;
                uow.TestDatabaseConnection(out testException);
                uow.EnsureDatabaseExists();
                uow.Repo<Site>().Insert(new Site()
                {
                    Name = "Site Test",
                    Priority = Priority.P1,
                    Building = "10",
                    Details = "This is a site test",
                    Owner = "Owner",
                    Status = SiteStatus.Unkown
                });
                uow.Save();
                Assert.NotNull(uow.Repo<Site>().Get(d => d.Name == "Site Test").First());
                uow.EnsureDatabaseIsDeleted();
            }
        }
    }
}
