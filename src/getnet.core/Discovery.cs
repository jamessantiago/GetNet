using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model;
using getnet.core.Model.Entities;
using getnet.core.ssh;

namespace getnet.core
{
    public static partial class Discovery
    {
        private static UnitOfWork uow = new UnitOfWork();
        private static Whistler logger = new Whistler(typeof(Discovery).FullName);
    }
}
