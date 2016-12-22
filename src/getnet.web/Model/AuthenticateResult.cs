using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace getnet.Model
{
    public class AuthenticateResult : IdentityResult
    {
        public AuthenticateResult(bool succeeeded, string error)
        {
            Succeeded = succeeeded;
            if (error.HasValue())
                Failed(new IdentityError[] { new IdentityError() { Code = "", Description = error } });
        }
    }
}
