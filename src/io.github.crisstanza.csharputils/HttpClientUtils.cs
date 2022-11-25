using io.github.crisstanza.csharputils.constants;
using System;
using System.Net.Http;
using System.Text;

namespace io.github.crisstanza.csharputils
{
	public class HttpClientUtils
	{
		private readonly string requestUri;
		private readonly StringUtils stringUtils;
		public HttpClientUtils(string requestUri)
		{
			this.requestUri = requestUri;
			this.stringUtils = new StringUtils();
		}
		public void PostJson(string json)
		{
			Console.WriteLine("[PostJson] " + json + " [/PostJson]");
			if (this.stringUtils.IsBlank(this.requestUri))
			{
				Console.WriteLine("[PostJson] no requestUri [/PostJson]");
			}
			else
			{
				HttpClient client = new HttpClient();
				HttpContent content = new StringContent(json, Encoding.UTF8, MediaTypeNamesConstants.APPLICATION_JSON);
				Console.WriteLine("[PostJson] Posting to " + this.requestUri + " [/PostJson]");
				try
				{
					client.PostAsync(this.requestUri, content);
				}
				catch (InvalidOperationException exc)
				{
					Console.WriteLine(exc.Message);
				}
			}
		}
	}
}
