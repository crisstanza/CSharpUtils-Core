using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace io.github.crisstanza.csharputils
{
	public class NetworkingUtils
	{
		private RunTimeUtils runTimeUtils;

		public NetworkingUtils()
		{
			this.runTimeUtils = new RunTimeUtils();
		}

		public IPAddress GetLocalAddress(IPAddress subnetMask)
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					if (ip.ToString().StartsWith("192."))
					{
						return ip;
					}
				}
			}
			NetworkInterface[] networkInterfaces;
			try
			{
				networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			}
			catch (NetworkInformationException exc)
			{
				Console.WriteLine(exc.ToString());
				networkInterfaces = null;
			}
			if (networkInterfaces != null)
			{
				foreach (NetworkInterface adapter in networkInterfaces)
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
			}
			RunTimeUtils.ExecResult hosts = this.runTimeUtils.Exec("hostname", "-I");
			if (hosts.ExitCode == 0)
			{
				String[] parts = hosts.Output.Split(' ');
				String ip = parts[0];
				return IPAddress.Parse(ip);
			}
			return null;
		}
	}
}
