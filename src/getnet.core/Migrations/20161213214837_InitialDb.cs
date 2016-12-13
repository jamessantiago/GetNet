using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace getnet.core.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantCode = table.Column<string>(maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    SiteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Building = table.Column<string>(maxLength: 100, nullable: true),
                    Details = table.Column<string>(maxLength: 500, nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Owner = table.Column<string>(maxLength: 200, nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.SiteId);
                    table.ForeignKey(
                        name: "FK_Sites_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceHistories",
                columns: table => new
                {
                    DeviceHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Details = table.Column<string>(maxLength: 255, nullable: true),
                    DiscoveryDate = table.Column<DateTime>(nullable: false),
                    Hostname = table.Column<string>(maxLength: 255, nullable: true),
                    LastSeenOnline = table.Column<DateTime>(nullable: false),
                    MAC = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                    Port = table.Column<string>(maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceHistories", x => x.DeviceHistoryId);
                    table.ForeignKey(
                        name: "FK_DeviceHistories_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diagram",
                columns: table => new
                {
                    DiagramId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilePath = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    SiteId = table.Column<int>(nullable: true),
                    Type = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagram", x => x.DiagramId);
                    table.ForeignKey(
                        name: "FK_Diagram_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PointOfContact",
                columns: table => new
                {
                    PointOfContactId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AltPhone = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Organization = table.Column<string>(maxLength: 250, nullable: true),
                    Phone = table.Column<string>(maxLength: 15, nullable: true),
                    SiteId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointOfContact", x => x.PointOfContactId);
                    table.ForeignKey(
                        name: "FK_PointOfContact_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointOfContact_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routers",
                columns: table => new
                {
                    RouterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChassisSerial = table.Column<string>(maxLength: 100, nullable: true),
                    Details = table.Column<string>(maxLength: 500, nullable: true),
                    Hostname = table.Column<string>(nullable: true),
                    Model = table.Column<string>(maxLength: 50, nullable: true),
                    RawManagementIP = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routers", x => x.RouterId);
                    table.ForeignKey(
                        name: "FK_Routers_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routers_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Switches",
                columns: table => new
                {
                    SwitchId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChassisSerial = table.Column<string>(maxLength: 100, nullable: true),
                    Details = table.Column<string>(maxLength: 500, nullable: true),
                    Hostname = table.Column<string>(nullable: true),
                    IsSwitchBlade = table.Column<bool>(nullable: false),
                    Model = table.Column<string>(maxLength: 50, nullable: true),
                    RawManagementIP = table.Column<int>(nullable: false),
                    SiteId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Switches", x => x.SwitchId);
                    table.ForeignKey(
                        name: "FK_Switches_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Switches_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouterRouterConnections",
                columns: table => new
                {
                    RouterId = table.Column<int>(nullable: false),
                    ConnectedRouterId = table.Column<int>(nullable: false),
                    ConnectedRouterPort = table.Column<string>(maxLength: 100, nullable: true),
                    RouterPort = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouterRouterConnections", x => new { x.RouterId, x.ConnectedRouterId });
                    table.ForeignKey(
                        name: "FK_RouterRouterConnections_Routers_ConnectedRouterId",
                        column: x => x.ConnectedRouterId,
                        principalTable: "Routers",
                        principalColumn: "RouterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouterRouterConnections_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "RouterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vlans",
                columns: table => new
                {
                    VlanId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RawVlanIP = table.Column<int>(nullable: false),
                    RawVlanSM = table.Column<int>(nullable: false),
                    RouterId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VlanNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vlans", x => x.VlanId);
                    table.ForeignKey(
                        name: "FK_Vlans_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "RouterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vlans_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouterSwitchConnection",
                columns: table => new
                {
                    RouterId = table.Column<int>(nullable: false),
                    SwitchId = table.Column<int>(nullable: false),
                    RouterPort = table.Column<string>(maxLength: 100, nullable: true),
                    SwitchPort = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouterSwitchConnection", x => new { x.RouterId, x.SwitchId });
                    table.ForeignKey(
                        name: "FK_RouterSwitchConnection_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "RouterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouterSwitchConnection_Switches_SwitchId",
                        column: x => x.SwitchId,
                        principalTable: "Switches",
                        principalColumn: "SwitchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SwitchSwitchConnections",
                columns: table => new
                {
                    SwitchId = table.Column<int>(nullable: false),
                    ConnectedSwitchId = table.Column<int>(nullable: false),
                    ConnectedSwitchPort = table.Column<string>(maxLength: 100, nullable: true),
                    SwitchPort = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchSwitchConnections", x => new { x.SwitchId, x.ConnectedSwitchId });
                    table.ForeignKey(
                        name: "FK_SwitchSwitchConnections_Switches_ConnectedSwitchId",
                        column: x => x.ConnectedSwitchId,
                        principalTable: "Switches",
                        principalColumn: "SwitchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SwitchSwitchConnections_Switches_SwitchId",
                        column: x => x.SwitchId,
                        principalTable: "Switches",
                        principalColumn: "SwitchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Details = table.Column<string>(maxLength: 255, nullable: true),
                    DeviceId1 = table.Column<int>(nullable: true),
                    DiscoveryDate = table.Column<DateTime>(nullable: false),
                    Hostname = table.Column<string>(maxLength: 255, nullable: true),
                    LastSeenOnline = table.Column<DateTime>(nullable: false),
                    MAC = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                    Port = table.Column<string>(maxLength: 100, nullable: true),
                    RawIP = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: true),
                    SiteId = table.Column<int>(nullable: true),
                    SwitchId = table.Column<int>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    VlanId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_Devices_Devices_DeviceId1",
                        column: x => x.DeviceId1,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Switches_SwitchId",
                        column: x => x.SwitchId,
                        principalTable: "Switches",
                        principalColumn: "SwitchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Devices_Vlans_VlanId",
                        column: x => x.VlanId,
                        principalTable: "Vlans",
                        principalColumn: "VlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceId1",
                table: "Devices",
                column: "DeviceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MAC",
                table: "Devices",
                column: "MAC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SiteId",
                table: "Devices",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SwitchId",
                table: "Devices",
                column: "SwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_TenantId",
                table: "Devices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_VlanId",
                table: "Devices",
                column: "VlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceHistories_TenantId",
                table: "DeviceHistories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagram_SiteId",
                table: "Diagram",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PointOfContact_SiteId",
                table: "PointOfContact",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PointOfContact_TenantId",
                table: "PointOfContact",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Routers_Model",
                table: "Routers",
                column: "Model");

            migrationBuilder.CreateIndex(
                name: "IX_Routers_SiteId",
                table: "Routers",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routers_TenantId",
                table: "Routers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RouterRouterConnections_ConnectedRouterId",
                table: "RouterRouterConnections",
                column: "ConnectedRouterId");

            migrationBuilder.CreateIndex(
                name: "IX_RouterSwitchConnection_SwitchId",
                table: "RouterSwitchConnection",
                column: "SwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_LocationId",
                table: "Sites",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Status",
                table: "Sites",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Switches_SiteId",
                table: "Switches",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Switches_TenantId",
                table: "Switches",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchSwitchConnections_ConnectedSwitchId",
                table: "SwitchSwitchConnections",
                column: "ConnectedSwitchId");

            migrationBuilder.CreateIndex(
                name: "IX_Vlans_RouterId",
                table: "Vlans",
                column: "RouterId");

            migrationBuilder.CreateIndex(
                name: "IX_Vlans_TenantId",
                table: "Vlans",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceHistories");

            migrationBuilder.DropTable(
                name: "Diagram");

            migrationBuilder.DropTable(
                name: "PointOfContact");

            migrationBuilder.DropTable(
                name: "RouterRouterConnections");

            migrationBuilder.DropTable(
                name: "RouterSwitchConnection");

            migrationBuilder.DropTable(
                name: "SwitchSwitchConnections");

            migrationBuilder.DropTable(
                name: "Vlans");

            migrationBuilder.DropTable(
                name: "Switches");

            migrationBuilder.DropTable(
                name: "Routers");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
