using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Dwo.Interop;

namespace Dwo
{
	public class DhcpServer
	{
		public DhcpIpAddress Address;

		public DhcpServer(DhcpIpAddress address)
		{
			this.Address = address;
		}

		public UInt32 Discovers;
		public UInt32 Offers;
		public UInt32 Requests;
		public UInt32 Acks;
		public UInt32 Naks;
		public UInt32 Declines;
		public UInt32 Releases;
		public DateTime ServerStartTime;
		public UInt32 QtnPctQtnLeases;
		public UInt32 QtnProbationLeases;
		public UInt32 QtnNonQtnLeases;
		public UInt32 QtnExemptLeases;
		public UInt32 QtnCapableClients;
		public UInt32 QtnIASErrors;
		public UInt32 DelayedOffers;
		public UInt32 ScopesWithDelayedOffers;
		public UInt32 Scopes;
		public UInt32 NumAddressesInUse;
		public UInt32 NumAddressesFree;
		public UInt32 NumPendingOffers;


		public void GetDhcpServerStatistics()
		{
			IntPtr iPtr = IntPtr.Zero;
			UInt32 Response = 0;
			DHCP_MIB_INFO_V5 info = new DHCP_MIB_INFO_V5();
			try
			{
				Response = NativeMethods.DhcpGetMibInfoV5(Address.ToString(), out iPtr);

				info = (DHCP_MIB_INFO_V5)Marshal.PtrToStructure<DHCP_MIB_INFO_V5>(iPtr);
			}
			finally
			{
				if (iPtr != IntPtr.Zero)
					NativeMethods.DhcpRpcFreeMemory(iPtr);
			}

			if (Response != 0)
				throw new DhcpException(Response);

			Discovers = info.Discovers;
			Offers = info.Offers;
			Requests = info.Requests;
			Acks = info.Acks;
			Naks = info.Naks;
			Declines = info.Declines;
			Releases = info.Releases;
			ServerStartTime = info.ServerStartTime.Convert();
			QtnPctQtnLeases = info.QtnPctQtnLeases;
			QtnProbationLeases = info.QtnProbationLeases;
			QtnNonQtnLeases = info.QtnNonQtnLeases;
			QtnExemptLeases = info.QtnExemptLeases;
			QtnCapableClients = info.QtnCapableClients;
			QtnIASErrors = info.QtnIASErrors;
			DelayedOffers = info.DelayedOffers;
			ScopesWithDelayedOffers = info.ScopesWithDelayedOffers;
			Scopes = info.Scopes;
			NumAddressesInUse = info.ScopeInfo.NumAddressesInUse;
			NumAddressesFree = info.ScopeInfo.NumAddressesFree;
			NumPendingOffers = info.ScopeInfo.NumPendingOffers;

		}
	}
}