using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using getnet.core.Model.Entities;

namespace getnet.core.Model.Repositories
{
    public class SiteRepository : GenericRepository<Site>
    {
        public SiteRepository(getnetContext context) : base(context)
        {
        }

        public void DoesNothing()
        {

        }
    }
}
