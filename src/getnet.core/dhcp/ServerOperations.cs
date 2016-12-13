using System;
using System.Collections.Generic;
using System.Text;

using Dwo;

namespace Dwo.Operations
{
    /// <summary>
    /// Provides a set of DHCP server operations.  One server operation may involve one or more SMO method calls to complete the task.
    /// </summary>
    public static class ServerOperations
    {
        #region Subnet Ops...
        /// <summary>
        /// Enumerate subnets on DHCP server
        /// </summary>
        /// <param name="server">Server IP address to enumerate</param>
        /// <returns>A list of DhcpSubnets</returns>
        public static List<DhcpSubnet> EnumSubnets(DhcpIpAddress server)
        {
            return DhcpSubnet.EnumAll(server);
        }

        /// <summary>
        /// Creates a DHCP subnet on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address</param>
        /// <param name="ipRange">Default address pool associated with subnet</param>
        /// <remarks>ipRange parameter can be of type <see cref="DhcpIpRange"/> or <see cref="BootpIpRange"/></remarks>
        public static void CreateSubnet(DhcpIpAddress server, DhcpSubnet network, DhcpIpRange ipRange)
        {
            
            if (ipRange.Type == DhcpSubnetElementDataType.DhcpExcludedIpRanges ||
                ipRange.Type == DhcpSubnetElementDataType.DhcpReservedIps)
                throw new Exception("SubnetElementData incorrect type");

            //create subnet
            network.Create(server);

            //assign default range
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network.Address;
            se.Data = ipRange;
            se.Add(server);

            //zero out DNS option
            DhcpOptionValue ov = new DhcpOptionValue();
            ov.ClassType = DhcpOptionClassType.Dhcp;
            ov.OptionId = 81;
            DhcpOptionScope os = new DhcpOptionScope();
            os.type = DhcpOptionScopeType.Subnet;
            os.SubnetIp = network.Address;
            ov.OptionScope = os;
            ov.OptionData = new DhcpOptionDataDword(new UInt32[] { 0 });
            ov.Set(server);

        }

        /// <summary>
        /// Update DHCP subnet on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet object to update</param>
        /// <remarks>Operation will update network.Name, network.Comment, and network.State on existing subnet matching network.Address</remarks>
        public static void UpdateSubnet(DhcpIpAddress server, DhcpSubnet network)
        {
            //update subnet
            network.Update(server);
        }

        /// <summary>
        /// Updates default DHCP subnet address pool (range)
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to update pool on</param>
        /// <param name="ipRange">New default address pool associated with subnet</param>
        /// <remarks>
        /// ipRange parameter can be of type <see cref="DhcpIpRange"/> or <see cref="BootpIpRange"/><br/>
        /// Removes current address pool and replaces with ipRange.  This can be used to change address pool client type (Dhcp, Bootp, or Both)
        /// </remarks>
        public static void UpdateSubnetRange(DhcpIpAddress server, DhcpIpAddress network, DhcpIpRange ipRange)
        {
            if (ipRange.Type == DhcpSubnetElementDataType.DhcpExcludedIpRanges ||
                ipRange.Type == DhcpSubnetElementDataType.DhcpReservedIps)
                throw new Exception("SubnetElementData incorrect type");

            //Remove old ipRange
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = new BootpIpRange(DhcpSubnetElementDataType.DhcpIpRangesDhcpBootp);
            DhcpSubnetElementData[] array = se.EnumAll(server);
            se.Data = array[0];
            se.Remove(server, true);

            try
            {
                //Add new ranage
                se.Data = ipRange;
                se.Add(server);
            }
            catch (Exception e)
            {
                //rollback
                se.Data = array[0];
                se.Add(server);
                throw e;
            }
        }

        /// <summary>
        /// Deletes DHCP subnet on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address identifying subnet</param>
        public static void DeleteSubnet(DhcpIpAddress server, DhcpIpAddress network)
        {
            DhcpSubnet sub = new DhcpSubnet();
            sub.Address = network;
            sub.Delete(server, true);
        }

        /// <summary>
        /// Creates a subnet exclusion address pool
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to create pool in</param>
        /// <param name="ipExRange">IP range use to created pool</param>
        /// <remarks>ipExRange.Type must equal <see cref="DhcpSubnetElementDataType.DhcpExcludedIpRanges"/></remarks>
        public static void CreateSubnetExclusion(DhcpIpAddress server, DhcpIpAddress network, DhcpIpRange ipExRange)
        {
            if(ipExRange.Type != DhcpSubnetElementDataType.DhcpExcludedIpRanges)
                throw new Exception("SubnetElementData incorrect type");

            //Add Exclusion Range
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = ipExRange;
            se.Add(server);
        }

        /// <summary>
        /// Deletes a subnet exclusion address pool
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to delete pool from</param>
        /// <param name="ipExRange">IP range to delete</param>
        /// <remarks>ipExRange.Type must equal <see cref="DhcpSubnetElementDataType.DhcpExcludedIpRanges"/></remarks>
        public static void DeleteSubnetExclusion(DhcpIpAddress server, DhcpIpAddress network, DhcpIpRange ipExRange)
        {
            if (ipExRange.Type != DhcpSubnetElementDataType.DhcpExcludedIpRanges)
                throw new Exception("SubnetElementData incorrect type");

            //Add Exclusion Range
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = ipExRange;
            se.Remove(server, true);
        }

        /// <summary>
        /// Search subnet for client leases matching terms
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to search</param>
        /// <param name="terms">Client filter terms</param>
        /// <remarks>Client filter terms are .NET regex patterns</remarks>
        /// <returns>A list of client leases matching terms</returns>
        public static List<DhcpClient> SubnetFindAllClients(DhcpIpAddress server, DhcpIpAddress network, DhcpClientFilter terms)
        {
            return DhcpClient.EnumAll(server, network).FindAll(terms.Filter);
        }

        /// <summary>
        /// Search subnet for first client lease matching terms
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to search</param>
        /// <param name="terms">Client filter terms</param>
        /// <remarks>Client filter terms are .NET regex patterns</remarks>
        /// <returns>First matching client lease or null if no match</returns>
        public static DhcpClient SubnetFindOneClient(DhcpIpAddress server, DhcpIpAddress network, DhcpClientFilter terms)
        {
            return DhcpClient.EnumAll(server, network).Find(terms.Filter);
        }

        /// <summary>
        /// Search <b>all</b> subnets on server looking for client leases matching terms
        /// </summary>
        /// <param name="server">Server IP address to search</param>
        /// <param name="terms">Client filter terms</param>
        /// <remarks>Client filter terms are .NET regex patterns</remarks>
        /// <returns>A list of client leases matching terms</returns>
        public static List<DhcpClient> ServerFindAllClients(DhcpIpAddress server, DhcpClientFilter terms)
        {
            List<DhcpSubnet> subnets = DhcpSubnet.EnumAll(server);

            List<DhcpClient> clients = new List<DhcpClient>();
            foreach (DhcpSubnet net in subnets)
            {
                 clients.AddRange(DhcpClient.EnumAll(server, net.Address).FindAll(terms.Filter));
            }
            return clients;
        }

        /// <summary>
        /// Search <b>all</b> subnets on server looking for first client lease matching terms
        /// </summary>
        /// <param name="server">Server IP address to search</param>
        /// <param name="terms">Client filter terms</param>
        /// <remarks>Client filter terms are .NET regex patterns</remarks>
        /// <returns>First matching client lease or null if no match</returns>
        public static DhcpClient ServerFindOneClient(DhcpIpAddress server, DhcpClientFilter terms)
        {
            List<DhcpSubnet> subnets = DhcpSubnet.EnumAll(server);

            DhcpClient client = null;
            foreach (DhcpSubnet net in subnets)
            {
                client = DhcpClient.EnumAll(server, net.Address).Find(terms.Filter);
                if(client != null)
                    return client;
            }

            return client;
        }

        /// <summary>
        /// Enumerate all address pools belonging to subnet
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address</param>
        /// <returns>An array of address pools.  The first pool will be the default address pool followed by zero or more exclusion pools</returns>
        public static DhcpIpRange[] EnumPools(DhcpIpAddress server, DhcpIpAddress network)
        {
            List<DhcpIpRange> pools = new List<DhcpIpRange>(3);
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            //use DhcpIpRangesDhcpBootp to get Main Range of any type.
            se.Data = new DhcpIpRange(DhcpSubnetElementDataType.DhcpIpRangesDhcpBootp);
            DhcpSubnetElementData[] mainpool = se.EnumAll(server);
            pools.Add((DhcpIpRange)mainpool[0]);

            se.Data = new DhcpIpRange(DhcpSubnetElementDataType.DhcpExcludedIpRanges);
            DhcpSubnetElementData[] exPools = se.EnumAll(server);
            foreach (DhcpSubnetElementData pool in exPools)
                pools.Add((DhcpIpRange)pool);

            return pools.ToArray();
        }

		/// <summary>
		/// Gets lease time (option 51)
		/// </summary>
		/// <param name="server"></param>
		/// <param name="subnet"></param>
		/// <returns>lease time</returns>
		public static TimeSpan GetDHCPLeaseTime(DhcpIpAddress server, DhcpIpAddress subnet)
		{

			//try
			//{
				var scope = new Dwo.DhcpOptionScope()
				{
					type = Dwo.DhcpOptionScopeType.Subnet,					
					SubnetIp = subnet					
				};
				var lease = Dwo.Operations.ServerOperations.GetOptionValue(server, 051, Dwo.DhcpOptionClassType.Dhcp, scope);
				var value = ((Dwo.DhcpOptionDataDword)lease.OptionData).Data[0];
				TimeSpan leaseTime = new TimeSpan(0, 0, (int)value);
				return leaseTime;
			//}
			//catch {
			//    return new TimeSpan(0);
			//}
		}

		/// <summary>
		/// Sets lease time (option 51)
		/// </summary>
		/// <param name="server"></param>
		/// <param name="subnet"></param>
		/// <returns>lease time</returns>
		public static TimeSpan SetDHCPLeaseTime(DhcpIpAddress server, DhcpIpAddress subnet, uint TimeInSecs)
		{

			//try
			//{
				var scope = new Dwo.DhcpOptionScope()
				{
					type = Dwo.DhcpOptionScopeType.Subnet,
					SubnetIp = subnet
				};
				Dwo.DhcpOptionValue lease = null;
				try
				{
					lease = Dwo.Operations.ServerOperations.GetOptionValue(server, 051, Dwo.DhcpOptionClassType.Dhcp, scope);
				}
				catch
				{
				}
				if (lease == null)
				{
					lease = new DhcpOptionValue()
					{
						ClassType = DhcpOptionClassType.Dhcp,
						OptionId = 051,
						OptionScope = scope,
						OptionData = new DhcpOptionDataDword(new uint[] { 0 })
					};
				}
				((Dwo.DhcpOptionDataDword)lease.OptionData).Data[0] = TimeInSecs;
				Dwo.Operations.ServerOperations.SetOptionValue(server, lease);
			
				var value = ((Dwo.DhcpOptionDataDword)lease.OptionData).Data[0];
				TimeSpan leaseTime = new TimeSpan(0, 0, (int)value);
				return leaseTime;
			//}
			//catch
			//{
				//return new TimeSpan(0);
			//}
		}
        #endregion

        #region Lease Ops...
        /// <summary>
        /// Enumerate all client leases on subnet
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to enumerate</param>
        /// <returns>A list of client leases belonging to subnet</returns>
        public static List<DhcpClient> EnumLeases(DhcpIpAddress server, DhcpIpAddress network)
        {
            return DhcpClient.EnumAll(server, network);
        }

        /// <summary>
        /// Creates a client lease on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="lease">Client lease info to create</param>
        /// <remarks>
        /// Client lease will be assigned to a existing subnet based on the binary AND operation of lease.IpAddress &amp; lease.SubnetMask
        /// </remarks>
        public static void CreateLease(DhcpIpAddress server, DhcpClient lease)
        {
            lease.Create(server);
        }

        /// <summary>
        /// Updates a client on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="lease">Client lease to match and update</param>
        /// <remarks>
        /// Client lease will be updated on a existing subnet based on the binary AND operation of lease.IpAddress &amp; lease.SubnetMask<br/>
        /// Operation will update lease.Mac, lease.Name, lease.Comment, and lease.LeaseExpires on existing lease matching lease.IpAddress and lease.SubnetMask
        /// </remarks>
        public static void UpdateLease(DhcpIpAddress server, DhcpClient lease)
        {
            lease.Update(server);
        }

        /// <summary>
        /// Deletes client lease from server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="leaseIp">Client lease IP address, used to identify and delete</param>
        public static void DeleteLease(DhcpIpAddress server, DhcpIpAddress leaseIp)
        {
            DhcpSearchInfo term = new DhcpSearchInfo();
            term.Type = DhcpSearchInfoType.IpAddress;
            term.IpAddress = leaseIp;
            DhcpClient.Delete(server, term);
        }

        #endregion

        #region Rez Ops...

        /// <summary>
        /// Creates a DHCP IP reservation on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to create reservation on</param>
        /// <param name="res">IP reservation info</param>
        public static void CreateReservation(DhcpIpAddress server, DhcpIpAddress network, DhcpReservation res)
        {
            //get subnet object for mask
            DhcpSubnet snet = new DhcpSubnet();
            snet.Address = network;
            snet.Get(server);

            //set internal res and lease objects
            DhcpIpReservation r = new DhcpIpReservation();
            DhcpClient c = new DhcpClient();
            r.ReservedIp = c.IpAddress = res.ReservedIp;
            r.ReservedMac = c.MacAddress = res.ReservedMac;
            r.bAllowedClientTypes = res.bAllowedClientTypes;
            c.SubnetMask = snet.Mask;
            c.Name = res.Name;
            c.Comment = res.Comment;

            //create element (reservation) to subnet
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = (DhcpSubnetElementData)r;
            se.Add(server);

            //update lease data
            c.Update(server);

            //zero out DNS option
            DhcpOptionValue ov = new DhcpOptionValue();
            ov.ClassType = DhcpOptionClassType.Dhcp;
            ov.OptionId = 81;
            DhcpOptionScope os = new DhcpOptionScope();
            os.type = DhcpOptionScopeType.Reservation;
            os.SubnetIp = network;
            os.ReservationIp = res.ReservedIp;
            ov.OptionScope = os;
            ov.OptionData = new DhcpOptionDataDword(new UInt32[] { 0 });
            ov.Set(server);

        }

        /// <summary>
        /// Updates a DHCP IP reservation on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to update reservation on</param>
        /// <param name="res">Updated IP reservation info</param>
        /// <remarks>Updates res.ReservedMac, res.bAllowedClientTypes, res.Name, and res.Comment based on match of res.ReservedIp</remarks>
        public static void UpdateReservation(DhcpIpAddress server, DhcpIpAddress network, DhcpReservation res)
        {
            //get subnet object for mask
            DhcpSubnet snet = new DhcpSubnet();
            snet.Address = network;
            snet.Get(server);

            DhcpIpReservation r = new DhcpIpReservation();
            DhcpClient c = new DhcpClient();
            r.ReservedIp = c.IpAddress = res.ReservedIp;
            r.ReservedMac = c.MacAddress = res.ReservedMac;
            r.bAllowedClientTypes = res.bAllowedClientTypes;
            c.SubnetMask = snet.Mask;
            c.Name = res.Name;
            c.Comment = res.Comment;
            
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = (DhcpSubnetElementData)r;
            se.Remove(server, false);
            se.Add(server);
            c.Update(server);
            
            
        }

        /// <summary>
        /// Deletes a DHCP IP reservation on server
        /// </summary>
        /// <param name="server">server IP address</param>
        /// <param name="network">Subnet network IP address to delete reservation from</param>
        /// <param name="resIp">Reservation IP address to delete</param>
        /// <remarks>Lease associated with reservation will also be deleted</remarks>
        public static void DeleteReservation(DhcpIpAddress server, DhcpIpAddress network, DhcpIpAddress resIp)
        {
            DhcpIpReservation r = new DhcpIpReservation();
            r.ReservedIp = resIp;
            r.ReservedMac = new DhcpMacAddress();

            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = (DhcpSubnetElementData)r;
            se.Remove(server, true);

        }

        /// <summary>
        /// Enumerate IP Reservations owned by subnet on server
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="network">Subnet network IP address to enumerate</param>
        /// <returns>A list of reservations on subnet</returns>
        public static List<DhcpReservation> EnumReservations(DhcpIpAddress server, DhcpIpAddress network)
        {
            DhcpSubnetElement se = new DhcpSubnetElement();
            se.SubnetAddress = network;
            se.Data = new DhcpIpReservation();
            DhcpSubnetElementData[] resArray = se.EnumAll(server);
            List<DhcpReservation> ResList = new List<DhcpReservation>(resArray.Length);

            DhcpClient c;
            DhcpSearchInfo term = new DhcpSearchInfo();
            term.Type = DhcpSearchInfoType.IpAddress;
            foreach (DhcpIpReservation res in resArray)
            {
                term.IpAddress = res.ReservedIp;
                c = DhcpClient.Get(server, term);

                DhcpReservation resinfo = new DhcpReservation();
                resinfo.Name = c.Name;
                resinfo.Comment = c.Comment;
                resinfo.ReservedIp = res.ReservedIp;
                resinfo.ReservedMac = res.ReservedMac;
                resinfo.bAllowedClientTypes = res.bAllowedClientTypes;

                ResList.Add(resinfo);
            }
            return ResList;
        }
        #endregion

        #region Option Ops...

        /// <summary>
        /// Sets an DHCP option's value(s) based on a particluar scope
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="value">DHCP option value</param>
        /// <remarks>This method overwrites any previous data set on value.OptionId</remarks>
        public static void SetOptionValue(DhcpIpAddress server, DhcpOptionValue value)
        {
            value.Set(server);
        }

        /// <summary>
        /// Retrieves a DHCP option's value(s) 
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="optionId">DHCP option identifier (ex. 006 = DNS Servers)</param>
        /// <param name="classType">DHCP option class type (Dhcp, Bootp, or Routing)</param>
        /// <param name="scope">Get option value from scope (Server, Subnet, or Reservation)</param>
        /// <returns>A DHCP option value</returns>
        public static DhcpOptionValue GetOptionValue(DhcpIpAddress server, UInt32 optionId, DhcpOptionClassType classType, DhcpOptionScope scope)
        {
            DhcpOptionValue value = new DhcpOptionValue();
            value.OptionId = optionId;
            value.ClassType = classType;
            value.OptionScope = scope;
            value.Get(server);
            return value;
        }

        /// <summary>
        /// Removes a DHCP option's value(s)
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="value">DHCP option value</param>
        /// <remarks>Completes remove all values from option.  Option becomes unset</remarks>
        public static void RemoveOptionValue(DhcpIpAddress server, DhcpOptionValue value)
        {
            value.Remove(server);
        }

        /// <summary>
        /// Enumerate all DHCP option's values set in scope
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="classType">DHCP option class type (Dhcp, Bootp, or Routing</param>
        /// <param name="scope">Option scope (Server, Subnet, or Reservation) to enumerate</param>
        /// <returns>An array of DHCP option's values current set in scope</returns>
        public static DhcpOptionValue[] EnumOptionValues(DhcpIpAddress server, DhcpOptionClassType classType, DhcpOptionScope scope)
        {
            return DhcpOptionValue.EnumAll(server, classType, scope);
        }

        /// <summary>
        /// Enumerate all DHCP Options available on server (global set of DHCP options)
        /// </summary>
        /// <param name="server">Server IP address</param>
        /// <param name="classType">DHCP option class type (Dhcp, Bootp, or Routing)</param>
        /// <returns>An array of global DHCP options available on server</returns>
        /// <remarks>Only Dhcp class types seems to return a set of possible DHCP options.  The other classes share these same DHCP options</remarks>
        public static DhcpOption[] EnumOptions(DhcpIpAddress server, DhcpOptionClassType classType)
        {
            return DhcpOption.EnumAll(server, classType);
        }
        #endregion

		#region Server Ops


		#endregion Server Ops
	}
}
