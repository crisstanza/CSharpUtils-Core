using io.github.crisstanza.csharputils.constants;
using io.github.crisstanza.csharputils.server.response;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace io.github.crisstanza.csharputils
{
	public class HttpListenerUtils
	{
		public class OutputBody
		{
			public byte[] Body { get; set; }
			public string ContentType { get; set; }
			public HttpStatusCode Status { get; set; }
			public List<KeyValuePair<string, string>> Headers { get; set; }
		}

		private readonly StreamUtils streamUtils;
		private readonly StringUtils stringUtils;
		private readonly JsonUtils jsonUtils;

		public HttpListenerUtils()
		{
			this.streamUtils = new StreamUtils();
			this.stringUtils = new StringUtils();
			this.jsonUtils = new JsonUtils();
		}

		public string SegmentToMethod(string segment)
		{
			StringBuilder method = new StringBuilder();
			char[] array = segment.ToCharArray();
			int length = array.Length;
			bool upper = true;
			for (int i = 0; i < length; i++)
			{
				char character = array[i];
				if (character == '-' || character == '/' || character == '.')
				{
					upper = true;
				}
				else
				{
					if (upper)
					{
						character = Char.ToUpper(character);
						upper = false;
					}
					method.Append(character);
				}
			}
			return method.ToString();
		}
		public string GetPostParameterOrRequestInput(HttpListenerRequest request, string parameterName)
		{
			String rawInput = GetRawRequestInput(request);
			NameValueCollection parameters = HttpUtility.ParseQueryString(rawInput);
			string parameterInput = parameters.Get(parameterName);
			string input;
			if (parameterInput == null)
			{
				input = WebUtility.UrlDecode(rawInput);
			}
			else
			{
				input = WebUtility.UrlDecode(parameterInput);

			}
			return input;
		}

		public OutputBody DefaultJsonOutputBody(ADefaultResponse output)
		{
			return new OutputBody()
			{
				Body = this.jsonUtils.SerializeToArray((object)output),
				ContentType = MediaTypeNamesConstants.APPLICATION_JSON,
				Status = HttpStatusCode.OK
			};
		}
		public OutputBody DefaultJsonOutputBody(string output)
		{
			return new OutputBody()
			{
				Body = Encoding.UTF8.GetBytes(output),
				ContentType = MediaTypeNamesConstants.APPLICATION_JSON,
				Status = HttpStatusCode.OK
			};
		}
		public OutputBody DefaultTextOutputBody(string output)
		{
			return new OutputBody()
			{
				Body = Encoding.UTF8.GetBytes(output),
				ContentType = MediaTypeNamesConstants.TEXT_PLAIN,
				Status = HttpStatusCode.OK
			};
		}
		public OutputBody DefaultErrorOutputBody(string output)
		{
			return new OutputBody()
			{
				Body = Encoding.UTF8.GetBytes(output),
				ContentType = MediaTypeNamesConstants.TEXT_PLAIN,
				Status = HttpStatusCode.InternalServerError
			};
		}

		public OutputBody DefaultDownloadOutputBody(string output, string name)
		{
			return new OutputBody()
			{
				Body = Encoding.UTF8.GetBytes(output),
				ContentType = MediaTypeNamesConstants.APPLICATION_OCTET_STREAM,
				Headers = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Content-Disposition", "attachment; filename=\"" + name + "\"") },
				Status = HttpStatusCode.OK
			};
		}

		public string GetRawRequestInput(HttpListenerRequest request)
		{
			String input = this.streamUtils.ReadToEnd(request.InputStream, request.ContentEncoding);
			return input;
		}
		public void Write(HttpListenerResponse response, string contentType, string buffer, HttpStatusCode statusCode)
		{
			Write(response, contentType, Encoding.UTF8.GetBytes(buffer ?? ""), statusCode);
		}
		public void Write(HttpListenerResponse response, string contentType, byte[] buffer, HttpStatusCode statusCode)
		{
			if (contentType != null)
			{
				response.ContentType = contentType;
			}
			response.ContentLength64 = buffer == null ? 0 : buffer.Length;
			response.StatusCode = (int)statusCode;
			Stream output = response.OutputStream;
			output.Write(buffer ?? new byte[0], 0, (int)response.ContentLength64);
			output.Close();
		}

		internal string GetContentType(string pagePath)
		{
			if (pagePath.EndsWith(".js"))
			{
				return MediaTypeNamesConstants.TEXT_JAVASCRIPT;
			}
			else if (pagePath.EndsWith(".css"))
			{
				return MediaTypeNamesConstants.TEXT_CSS;
			}
			return null;
		}
	}
}
