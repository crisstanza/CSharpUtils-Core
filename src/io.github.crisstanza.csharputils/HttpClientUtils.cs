using io.github.crisstanza.csharputils.constants;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace io.github.crisstanza.csharputils
{
	public class HttpClientUtils
	{
		private readonly HttpClient client = new HttpClient();
		private readonly string requestUri;
		public HttpClientUtils(string requestUri)
		{
			this.client = new HttpClient();
			this.requestUri = requestUri;
		}
		public void PostJson(string json)
		{
			HttpContent content = new StringContent(json, Encoding.UTF8, MediaTypeNamesConstants.APPLICATION_JSON);
			this.client.PostAsync(this.requestUri, content);
		}
		public string Get()
		{
			Task<string> result = this.Get_();
			if (result == null)
			{
				return null;
			}
			return result.Result;
		}

		private async Task<string> Get_()
		{
			try
			{
				using HttpResponseMessage response = await this.client.GetAsync(this.requestUri);
				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync();
			}
			catch (InvalidOperationException exc)
			{
				Console.WriteLine(exc.Message);
			}
			catch (HttpRequestException exc)
			{
				Console.WriteLine(exc.Message);
			}
			return null;
		}
	}
}
