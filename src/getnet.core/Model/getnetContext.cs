using getnet.core.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using getnet;

namespace getnet.core.Model
{

    public interface IModelBuildItem
    {
        void Build(ref ModelBuilder modelBuilder);
    }

    public class getnetContext : DbContext
    {
        private Whistler logger = new Whistler();

        public event EventHandler ConfigurationComplete = delegate { };
        public DbSet<DeviceHistory> DeviceHistories { get; set; }
        public DbSet<Device> Devices { get; set; }
        public bool IsConfigured { get; set; }
        public DbSet<RouterRouterConnection> RouterRouterConnections { get; set; }
        public DbSet<Router> Routers { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Switch> Switches { get; set; }
        public DbSet<SwitchSwitchConnection> SwitchSwitchConnections { get; set; }
        public DbSet<Vlan> Vlans { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString").HasValue())
                {
                    optionsBuilder.UseSqlServer(CoreCurrent.Configuration.GetSecure("Data:SqlServerConnectionString"));
                    IsConfigured = true;
                    logger.Info("Database set to use a MS SQL server connection", WhistlerTypes.DatabaseSetup);
                }
                else if (CoreCurrent.Configuration.GetSecure("Data:NpgsqlConnectionString").HasValue())
                {
                    optionsBuilder.UseNpgsql(CoreCurrent.Configuration["Data:NpgsqlConnectionString"]);
                    IsConfigured = true;
                    logger.Info("Database set to use a PostgreSQL server connection", WhistlerTypes.DatabaseSetup);
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