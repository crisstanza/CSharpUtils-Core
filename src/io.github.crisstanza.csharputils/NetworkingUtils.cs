using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace io.github.crisstanza.csharputils
{
	public class NetworkingUtils
	{
		public IPAddress GetLocalAddress(IPAddress subnetMask)
		{
			foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
			{
				foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
				{
					if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						if (unicastIPAddressInformation.IPv4Mask.Equals(subnetMask))
						{
							return unicastIPAddressInformation.Address;
						}
					}
				}
			}
			return null;
		}
	}
}
