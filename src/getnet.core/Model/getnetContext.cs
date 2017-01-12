using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace getnet.core.Model
{
    public interface IModelBuildItem
    {
        void Build(ref ModelBuilder modelBuilder);
    }

    public class getnetContext : DbContext
    {
        private Whistler logger = new Whistler(typeof(getnetContext).FullName);

        public event EventHandler ConfigurationComplete = delegate { };

        public DbSet<AlertRule> AlertRules { get; set; }
        public DbSet<DeviceHistory> DeviceHistories { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DhcpPool> DhcpPools { get; set; }
        public DbSet<DhcpSubnet> DhcpSubnet { get; set; }
        public DbSet<Diagram> Diagrams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<HotPath> HotPaths { get; set; }
        public DbSet<Location> Locations { get; set; }
        public bool IsConfigured { get; set; }
        public DbSet<NetworkDeviceConnection> NetworkDeviceConnections { get; set; }
        public DbSet<NetworkDevice> NetworkDevices { get; set; }
        public DbSet<PointOfContact> PointOfContacts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Subnet> Subnets { get; set; }
        public DbSet<TaskSchedule> TaskSchedules { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Vlan> Vlans { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=GetNet;Integrated Security=true");
            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GetNet;Pooling=true;");
            try
            {
                if (CoreCurrent.Configuration["Data:DataStore"] == "MSSQL" && CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString").HasValue())
                {
                    optionsBuilder.UseSqlServer(CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString"));
                    IsConfigured = true;
                }
                else if (CoreCurrent.Configuration["Data:DataStore"] == "Postgres" && CoreCurrent.Configuration.GetSecure("Data:PostgresConnectionString").HasValue())
                {
                    optionsBuilder.UseNpgsql(CoreCurrent.Configuration.GetSecure("Data:PostgresConnectionString"));
                    IsConfigured = true;
                }
                else
                {
                    IsConfigured = false;
                    logger.Warn("There is no connection string in the configuration available to configure the database", WhistlerTypes.DatabaseSetup);
                }
            }
            catch { }
            finally
            {
                ConfigurationComplete(this, EventArgs.Empty);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var buildItems = Assembly.Load(new AssemblyName("getnet.core"))
                .GetTypes()
                .Where(d => typeof(IModelBuildItem).IsAssignableFrom(d) && d != typeof(IModelBuildItem));

            foreach (var item in buildItems)
            {
                var instance = Activator.CreateInstance(item, false) as IModelBuildItem;
                instance.Build(ref modelBuilder);
            }
        }
    }
}