using System;

namespace getnet.Model
{
    [Flags]
    public enum Roles
    {
        None = 0,
        Anonymous = 1 << 1,
        Authenticated = 1 << 2,
        
        LocalRequest = 1 << 3,
        InternalRequest = 1 << 4,
        ApiRequest = 1 << 5,

        GlobalViewers = 1 << 6,
        GlobalAdmin = 1 << 7
    }
}