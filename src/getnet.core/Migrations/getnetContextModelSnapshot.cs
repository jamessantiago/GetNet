using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using getnet.core.Model;
using getnet.core.Model.Entities;

namespace getnet.core.Migrations
{
    [DbContext(typeof(getnetContext))]
    partial class getnetContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("getnet.core.Model.Entities.Device", b =>
                {
                    b.Property<int>("DeviceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Details")
                        .HasMaxLength(255);

                    b.Property<int?>("DeviceId1");

                    b.Property<DateTime>("DiscoveryDate");

                    b.Property<string>("Hostname")
                        .HasMaxLength(255);

                    b.Property<DateTime>("LastSeenOnline");

                    b.Property<string>("MAC")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15);

                    b.Property<string>("Port")
                        .HasMaxLength(100);

                    b.Property<int>("RawIP");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("SiteId");

                    b.Property<int?>("SwitchId");

                    b.Property<int?>("TenantId");

                    b.Property<int>("Type");

                    b.Property<int?>("VlanId");

                    b.HasKey("DeviceId");

                    b.HasIndex("DeviceId1");

                    b.HasIndex("MAC")
                        .IsUnique();

                    b.HasIndex("SiteId");

                    b.HasIndex("SwitchId");

                    b.HasIndex("TenantId");

                    b.HasIndex("VlanId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.DeviceHistory", b =>
                {
                    b.Property<int>("DeviceHistoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Details")
                        .HasMaxLength(255);

                    b.Property<DateTime>("DiscoveryDate");

                    b.Property<string>("Hostname")
                        .HasMaxLength(255);

                    b.Property<DateTime>("LastSeenOnline");

                    b.Property<string>("MAC")
                        .HasMaxLength(50);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15);

                    b.Property<string>("Port")
                        .HasMaxLength(100);

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("TenantId");

                    b.Property<int>("Type");

                    b.HasKey("DeviceHistoryId");

                    b.HasIndex("TenantId");

                    b.ToTable("DeviceHistories");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Diagram", b =>
                {
                    b.Property<int>("DiagramId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FilePath")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int?>("SiteId");

                    b.Property<string>("Type")
                        .HasMaxLength(100);

                    b.HasKey("DiagramId");

                    b.HasIndex("SiteId");

                    b.ToTable("Diagram");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("LocationId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.PointOfContact", b =>
                {
                    b.Property<int>("PointOfContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AltPhone")
                        .HasMaxLength(15);

                    b.Property<string>("Email")
                        .HasMaxLength(250);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("Organization")
                        .HasMaxLength(250);

                    b.Property<string>("Phone")
                        .HasMaxLength(15);

                    b.Property<int?>("SiteId");

                    b.Property<int?>("TenantId");

                    b.HasKey("PointOfContactId");

                    b.HasIndex("SiteId");

                    b.HasIndex("TenantId");

                    b.ToTable("PointOfContact");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Router", b =>
                {
                    b.Property<int>("RouterId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChassisSerial")
                        .HasMaxLength(100);

                    b.Property<string>("Details")
                        .HasMaxLength(500);

                    b.Property<string>("Hostname");

                    b.Property<string>("Model")
                        .HasMaxLength(50);

                    b.Property<int>("RawManagementIP");

                    b.Property<int?>("SiteId");

                    b.Property<int?>("TenantId");

                    b.HasKey("RouterId");

                    b.HasIndex("Model");

                    b.HasIndex("SiteId");

                    b.HasIndex("TenantId");

                    b.ToTable("Routers");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.RouterRouterConnection", b =>
                {
                    b.Property<int>("RouterId");

                    b.Property<int>("ConnectedRouterId");

                    b.Property<string>("ConnectedRouterPort")
                        .HasMaxLength(100);

                    b.Property<string>("RouterPort")
                        .HasMaxLength(100);

                    b.HasKey("RouterId", "ConnectedRouterId");

                    b.HasIndex("ConnectedRouterId");

                    b.ToTable("RouterRouterConnections");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.RouterSwitchConnection", b =>
                {
                    b.Property<int>("RouterId");

                    b.Property<int>("SwitchId");

                    b.Property<string>("RouterPort")
                        .HasMaxLength(100);

                    b.Property<string>("SwitchPort")
                        .HasMaxLength(100);

                    b.HasKey("RouterId", "SwitchId");

                    b.HasIndex("SwitchId");

                    b.ToTable("RouterSwitchConnection");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Site", b =>
                {
                    b.Property<int>("SiteId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Building")
                        .HasMaxLength(100);

                    b.Property<string>("Details")
                        .HasMaxLength(500);

                    b.Property<int?>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Owner")
                        .HasMaxLength(200);

                    b.Property<int>("Priority");

                    b.Property<int>("Status");

                    b.HasKey("SiteId");

                    b.HasIndex("LocationId");

                    b.HasIndex("Status");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Switch", b =>
                {
                    b.Property<int>("SwitchId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChassisSerial")
                        .HasMaxLength(100);

                    b.Property<string>("Details")
                        .HasMaxLength(500);

                    b.Property<string>("Hostname");

                    b.Property<bool>("IsSwitchBlade");

                    b.Property<string>("Model")
                        .HasMaxLength(50);

                    b.Property<int>("RawManagementIP");

                    b.Property<int?>("SiteId");

                    b.Property<int?>("TenantId");

                    b.HasKey("SwitchId");

                    b.HasIndex("SiteId");

                    b.HasIndex("TenantId");

                    b.ToTable("Switches");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.SwitchSwitchConnection", b =>
                {
                    b.Property<int>("SwitchId");

                    b.Property<int>("ConnectedSwitchId");

                    b.Property<string>("ConnectedSwitchPort")
                        .HasMaxLength(100);

                    b.Property<string>("SwitchPort")
                        .HasMaxLength(100);

                    b.HasKey("SwitchId", "ConnectedSwitchId");

                    b.HasIndex("ConnectedSwitchId");

                    b.ToTable("SwitchSwitchConnections");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Tenant", b =>
                {
                    b.Property<int>("TenantId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("TenantCode")
                        .HasMaxLength(3);

                    b.HasKey("TenantId");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Vlan", b =>
                {
                    b.Property<int>("VlanId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RawVlanIP");

                    b.Property<int>("RawVlanSM");

                    b.Property<int?>("RouterId");

                    b.Property<int?>("TenantId");

                    b.Property<int>("VlanNumber");

                    b.HasKey("VlanId");

                    b.HasIndex("RouterId");

                    b.HasIndex("TenantId");

                    b.ToTable("Vlans");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Device", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Device")
                        .WithMany("Devices")
                        .HasForeignKey("DeviceId1");

                    b.HasOne("getnet.core.Model.Entities.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId");

                    b.HasOne("getnet.core.Model.Entities.Switch", "Switch")
                        .WithMany()
                        .HasForeignKey("SwitchId");

                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");

                    b.HasOne("getnet.core.Model.Entities.Vlan", "Vlan")
                        .WithMany("Devices")
                        .HasForeignKey("VlanId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.DeviceHistory", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Diagram", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Site", "Site")
                        .WithMany("Diagrams")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.PointOfContact", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Site", "Site")
                        .WithMany("PointOfContacts")
                        .HasForeignKey("SiteId");

                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Router", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Site")
                        .WithMany("Routers")
                        .HasForeignKey("SiteId");

                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.RouterRouterConnection", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Router", "ConnectedRouter")
                        .WithMany("InRouterRouterConnections")
                        .HasForeignKey("ConnectedRouterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("getnet.core.Model.Entities.Router", "Router")
                        .WithMany("OutRouterRouterConnections")
                        .HasForeignKey("RouterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("getnet.core.Model.Entities.RouterSwitchConnection", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Router", "Router")
                        .WithMany("RouterSwitchConnections")
                        .HasForeignKey("RouterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("getnet.core.Model.Entities.Switch", "Switch")
                        .WithMany("RouterSwitchConnections")
                        .HasForeignKey("SwitchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Site", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Switch", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Site")
                        .WithMany("Switches")
                        .HasForeignKey("SiteId");

                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("getnet.core.Model.Entities.SwitchSwitchConnection", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Switch", "ConnectedSwitch")
                        .WithMany("OutSwitchSwitchConnections")
                        .HasForeignKey("ConnectedSwitchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("getnet.core.Model.Entities.Switch", "Switch")
                        .WithMany("InSwitchSwitchConnections")
                        .HasForeignKey("SwitchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("getnet.core.Model.Entities.Vlan", b =>
                {
                    b.HasOne("getnet.core.Model.Entities.Router", "Router")
                        .WithMany("Vlans")
                        .HasForeignKey("RouterId");

                    b.HasOne("getnet.core.Model.Entities.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");
                });
        }
    }
}
