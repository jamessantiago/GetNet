using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;

namespace getnet
{
    public static partial class Current
    {
        internal static void SetDbConfigurationState()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                bool configured, tested;

                Exception testException = null;

                tested = uow.TestDatabaseConnection(out testException);
                configured = uow.ConfigurationState == UnitOfWork.DatabaseConfigurationState.Configured;
                tested = uow.TestDatabaseConnection(out testException);

                ConfigurationRequired = configured;
                if (testException != null)
                    DatabaseConnectionError = testException;
            }
        }

        public static bool ConfigurationRequired { get; private set; }

        public static Exception DatabaseConnectionError { get; private set; }
    }
}
