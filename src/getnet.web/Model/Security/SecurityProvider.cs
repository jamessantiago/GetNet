using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace getnet.Model.Security
{
    public abstract class SecurityProvider
    {
        public virtual bool IsAdmin => InGroups(CoreCurrent.Configuration["Security:AdminGroups"]);
        public virtual bool IsViewer => InGroups(CoreCurrent.Configuration["Security:ViewGroups"]);

        internal virtual bool InReadGroups(ISecurableModule settings)
        {
            return IsViewer || (settings != null && (InGroups(settings.ViewGroups) || InAdminGroups(settings)));
        }

        internal virtual bool InAdminGroups(ISecurableModule settings)
        {
            return IsAdmin || (settings != null && InGroups(settings.AdminGroups));
        }

        private bool InGroups(string groupNames)
        {
            if (groupNames.IsNullOrEmpty() || Current.User.AccountName.IsNullOrEmpty()) return false;
            return groupNames == "*" || InGroups(groupNames, Current.User.AccountName);
        }

        public abstract bool InGroups(string groupNames, string accountName);
        public abstract bool ValidateUser(string userName, string password);

        public virtual List<string> GetGroupMembers(string groupName)
        {
            return new List<string>();
        }
        public virtual void PurgeCache() { }

        public List<IPNetwork> InternalNetworks;
        protected SecurityProvider()
        {
            InternalNetworks = new List<IPNetwork>();
            if (CoreCurrent.Configuration["Security:InternalNetworks"].HasValue())
            {
                foreach (var n in CoreCurrent.Configuration["Security:InternalNetworks"].Split(';'))
                {
                    IPNetwork net = null;
                    if (IPNetwork.TryParse(n, out net))
                    {
                        InternalNetworks.Add(net);
                    }
                }
            }
        }

        public bool IsInternalIP(string ip)
        {
            IPAddress addr;
            return IPAddress.TryParse(ip, out addr) && InternalNetworks.Any(n => IPNetwork.Contains(n,  addr));
        }
    }

    public interface ISecurableModule
    {
        bool Enabled { get; }
        string ViewGroups { get; }
        string AdminGroups { get; }
    }
}