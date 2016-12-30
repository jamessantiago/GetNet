# GetNet

GetNet is a highly focused network management tool.  Core components include network discovery, monitoring, dhcp, sites and services, and ip management.

![GetNet Logo](/media/logo/logo_112.png)

## Development

GetNet is currently under initial development.  Some design/functionality goals:

*  .NET Core for cross platform capabilities
*  Both MS SQL and PostgreSQL support
*  material-design-lite by google for UI (instead of bootstrap)
*  Entity framework with unit of work and generic repository design pattern
*  NLog logging extended to support an alert system
*  SSH generalized client with commands that return typed results
*  Sites and services integration
*  DHCP integration
*  Disconnected network import capabilities

## Target Environment

GetNet should more or less be generalized to most networks.  Here is the current network architecture GetNet is being built around:

![GNS Simulated Network](/gns3/eigrpn%20net%20with%20gre%20tunnels%20to%20user%20sites/screenshot.png)