using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet;
using System.Threading;
using getnet.core;
using getnet.core.Model;
using Xunit;

namespace getnet.tests
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void CheckDatabaseConnection()
        {
            Current.Configuration["Data:SqlServerConnectionString"] = "test";
            using (UnitOfWork uow = new UnitOfWork())
            {
                Exception ex = null;
                if (uow.ConfigurationState == UnitOfWork.DatabaseConfigurationState.Pending)
                    uow.TestDatabaseConnection(out ex);
                Assert.True(uow.ConfigurationState == UnitOfWork.DatabaseConfigurationState.Configured);
                
                Assert.True(uow.TestDatabaseConnection(out ex));
                Assert.True(uow.CheckIfDabaseExists());
            }
        }
    }
}
