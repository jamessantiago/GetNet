using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using getnet.core.Model.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace getnet.core.Model
{
    public class getnetContext : DbContext
    {
        
        public DbSet<Switch> Switches { get; set; }
        public DbSet<Router> Routers { get; set; }
        public DbSet<Vlan> Vlans { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<RouterRouterConnection> RouterRouterConnections { get; set; }
        public DbSet<SwitchSwitchConnection> SwitchSwitchConnections { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceHistory> DeviceHistories { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("testconfig");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var buildItems = Assembly.Load(new AssemblyName("getnet.core"))
                .GetTypes()
                .Where(d => typeof(IModelBuildItem).IsAssignableFrom(d) && d != typeof(IModelBuildItem));

            foreach (var item in buildItems) {
                var instance = Activator.CreateInstance(item, false) as IModelBuildItem;
                instance.Build(ref modelBuilder);
            }
        }
    }

    public interface IModelBuildItem
    {
        void Build(ref ModelBuilder modelBuilder);
    }
}
