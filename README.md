# getnet

## Introduction

getnet is yet another IP address management system.  The selling point is that getnet provides the logical grouping of subnets, network infratructure, and network devices.  Information such as where an IP resides and what switch port it is plugged into can be quickly ascertained.

## Development

getnet is currently under initial development.  Some design/functionality goals:

*  .NET Core for cross platform capabilities
*  Both MS SQL and PostgreSQL support
*  material-design-lite by google for UI (instead of bootstrap)
*  Entity framework with unit of work and generic repository design pattern
*  NLog logging extended to support an alert system
*  SSH generalized client with commands that return typed results
*  Sites and services integration
*  DHCP integration
*  Disconnected network import capabilities