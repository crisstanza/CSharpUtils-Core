using io.github.crisstanza.csharputils.server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace io.github.crisstanza.csharputils.service
{
	public class HttpListenerService
	{
		private readonly string host;
		private readonly int port;
		private readonly HttpListener server;
		private readonly DateTimeUtils dateTimeUtils;
		private Thread mainThread;

		public HttpListenerService(string host, int port)
		{
			this.host = host;
			this.port = port;
			this.server = new HttpListener();
			this.dateTimeUtils = new DateTimeUtils();
		}
		public void Start(IDefaultController controller, string version, Dictionary<string, string> dependencies, Dictionary<string, string> extras)
		{
			string prefix = string.Format("http://{0}:{1}/", this.host, this.port);
			this.server.Prefixes.Add(prefix);
			this.server.Start();
			Console.WriteLine();
			Console.WriteLine("================================================================================");
			Console.WriteLine("= " + dateTimeUtils.Now());
			Console.WriteLine("=");
			Console.WriteLine("= Version: " + version);
			if (dependencies != null)
			{
				Console.WriteLine("= Dependencies:");
				foreach (KeyValuePair<string, string> dependency in dependencies)
				{
					Console.WriteLine("= - " + dependency.Key + ": " + dependency.Value);
				}
			}
			Console.WriteLine("=");
			Console.WriteLine("= OS: " + RuntimeInformation.OSDescription);
			Console.WriteLine("= SDK: " + RuntimeInformation.FrameworkDescription);
			Console.WriteLine("= Number of processors: " + Environment.ProcessorCount);
			Console.WriteLine("=");
			Console.WriteLine("= Listening on: " + prefix);
			if (extras != null)
			{
				Console.WriteLine("=");
				foreach (KeyValuePair<string, string> extra in extras)
				{
					Console.WriteLine("= " + extra.Key + ": " + extra.Value);
				}
			}
			Console.WriteLine("================================================================================");
			Console.WriteLine();
			mainThread = new Thread(() =>
			{
				while (this.server.IsListening)
				{
					try
					{
						AsyncCallback callback = new AsyncCallback(
							(IAsyncResult result) =>
							{
								new RequestProcessorService(result).ProcessRequest(controller);
							}
						);
						IAsyncResult asyncResult = this.server.BeginGetContext(callback, this.server);
						asyncResult.AsyncWaitHandle.WaitOne();
					}
					catch (HttpListenerException exc)
					{
						Console.WriteLine(exc.Message);
					}
					catch (ThreadInterruptedException exc)
					{
						Console.WriteLine(exc.Message);
					}
				}
				Console.WriteLine("Server stop!");
			});
			mainThread.Start();
		}
		public void Stop()
		{
			this.server.Stop();
			mainThread.Interrupt();
		}
	}
}
