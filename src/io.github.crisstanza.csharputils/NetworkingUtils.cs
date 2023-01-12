using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace io.github.crisstanza.csharputils
{
    public class NetworkingUtils
    {
        public IPAddress GetLocalAddress(IPAddress subnetMask)
        {
			// TODO (Cris Stanza): check command: hostname -I (see firs result)
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
					if (ip.ToString().StartsWith("192.")) {
                    	return ip;
					}
                }
            }
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
