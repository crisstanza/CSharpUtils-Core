using io.github.crisstanza.csharputils.constants;
using io.github.crisstanza.csharputils.server;
using io.github.crisstanza.csharputils.server.response;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace io.github.crisstanza.csharputils.service
{
	internal class RequestProcessorService
	{
		private readonly IAsyncResult result;
		private readonly HttpListenerUtils httpListenerUtils;
		private readonly FileSystemUtils fileSystemUtils;
		private readonly ArrayUtils arrayUtils;
		private readonly JsonUtils jsonUtils;
		private readonly StringUtils stringUtils;

		public RequestProcessorService(IAsyncResult result)
		{
			this.result = result;
			this.httpListenerUtils = new HttpListenerUtils();
			this.fileSystemUtils = new FileSystemUtils();
			this.arrayUtils = new ArrayUtils();
			this.jsonUtils = new JsonUtils();
			this.stringUtils = new StringUtils();
		}

		public void ProcessRequest(IDefaultController controller)
		{
			HttpListener server = (HttpListener)result.AsyncState;
			if (server.IsListening)
			{
				HttpListenerContext context = server.EndGetContext(result);
				if (context != null)
				{
					string methodName;
					object toBeInvoked;
					object[] toBePassedAsArgument;
					MethodInfo method;
					string[] urlSegments = context.Request.Url.Segments;
					if (urlSegments.Length == 1)
					{
						methodName = "IndexHtml";
						toBeInvoked = this;
						toBePassedAsArgument = null;
					}
					else
					{
						string segments = arrayUtils.Join(1, urlSegments);
						methodName = httpListenerUtils.SegmentToMethod(segments);
						toBeInvoked = ResolveToBeInvoked(methodName, controller);
						toBePassedAsArgument = null;
					}
					method = toBeInvoked.GetType().GetMethod(methodName);
					HttpListenerUtils.OutputBody output;
					if (method == null)
					{
						string page = arrayUtils.Join(1, urlSegments);
						page = page.Replace("/", Path.DirectorySeparatorChar.ToString());
						string currentPath = fileSystemUtils.CurrentPath();
						string pagePath = currentPath + "html" + Path.DirectorySeparatorChar + page;
						toBeInvoked = this;
						if (File.Exists(pagePath))
						{
							methodName = "ServeLocalFile";
							method = toBeInvoked.GetType().GetMethod(methodName);
							toBePassedAsArgument = new object[] { pagePath };
						}
						else
						{
							methodName = "NotFound";
							method = toBeInvoked.GetType().GetMethod(methodName);
							toBePassedAsArgument = new object[] { context };
						}
					}
					else
					{
						ParameterInfo[] parameters = method.GetParameters();
						string httpMethod = context.Request.HttpMethod;
						if (httpMethod == "POST")
						{
							if (parameters.Length > 0)
							{
								ParameterInfo parameter = parameters[0];
								Type type = parameter.ParameterType;
								string inputString = httpListenerUtils.GetPostParameterOrRequestInput(context.Request, "body");
								if (stringUtils.IsBlank(inputString))
								{
									toBePassedAsArgument = new object[] { null };
								}
								else
								{
									Object inputJson = jsonUtils.Deserialize(inputString, type);
									toBePassedAsArgument = new object[] { inputJson };
								}
							}
							else
							{
								toBePassedAsArgument = new object[] { null };
							}
						}
						else
						{
							NameValueCollection queryString = context.Request.QueryString;
							toBePassedAsArgument = new object[parameters.Length];
							for (int i = 0; i < parameters.Length; i++)
							{
								ParameterInfo parameter = parameters[i];
								string parameterName = parameter.Name;
								toBePassedAsArgument[i] = queryString[parameterName];
							}
						}
					}
					try
					{
						output = (HttpListenerUtils.OutputBody)method.Invoke(toBeInvoked, toBePassedAsArgument);
					}
					catch (Exception exc)
					{
						output = this.httpListenerUtils.DefaultErrorOutputBody(exc.ToString());
					}
					if (output.Headers != null)
					{
						for (int i = 0; i < output.Headers.Count; i++)
						{
							KeyValuePair<string, string> header = output.Headers[i];
							context.Response.AddHeader(header.Key, header.Value);
						}
					}
					httpListenerUtils.Write(context.Response, output.ContentType, output.Body, output.Status);
					if (methodName == "Stop")
					{
						controller.End();
					}
				}
			}
		}

		public HttpListenerUtils.OutputBody NotFound(HttpListenerContext context)
		{
			HttpListenerUtils.OutputBody output = new HttpListenerUtils.OutputBody()
			{
				Body = jsonUtils.SerializeToArray((object)new NotFoundResponse() { NotFound = true }),
				ContentType = MediaTypeNamesConstants.APPLICATION_JSON,
				Status = HttpStatusCode.NotFound
			};
			return output;
		}

		public HttpListenerUtils.OutputBody ServeLocalFile(string pagePath)
		{
			string contentType = this.httpListenerUtils.GetContentType(pagePath);
			byte[] body;
			if (this.httpListenerUtils.IsBinaryContentType(contentType))
			{
				body = File.ReadAllBytes(pagePath);
			}
			else
			{
				string contents = File.ReadAllText(pagePath);
				contents = ResolveVariables(contents);
				body = Encoding.UTF8.GetBytes(contents);
			}
			HttpListenerUtils.OutputBody output = new HttpListenerUtils.OutputBody()
			{
				Body = body,
				ContentType = contentType,
				Status = HttpStatusCode.OK
			};
			return output;
		}

		public HttpListenerUtils.OutputBody IndexHtml()
		{
			string currentPath = fileSystemUtils.CurrentPath();
			string page = "index.html";
			string pagePath = currentPath + "html" + Path.DirectorySeparatorChar + page;
			string contents = File.ReadAllText(pagePath);
			contents = ResolveVariables(contents);
			HttpListenerUtils.OutputBody output = new HttpListenerUtils.OutputBody()
			{
				Body = Encoding.UTF8.GetBytes(contents),
				ContentType = MediaTypeNamesConstants.TEXT_HTML,
				Status = HttpStatusCode.OK
			};
			return output;
		}
		private string ResolveVariables(string contents)
		{
			contents = contents.Replace("${version}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
			return contents;
		}
		public HttpListenerUtils.OutputBody FaviconIco()
		{
			string currentPath = fileSystemUtils.CurrentPath();
			string icon = "favicon.ico";
			string iconPath = currentPath + "html" + Path.DirectorySeparatorChar + icon;
			HttpListenerUtils.OutputBody output = new HttpListenerUtils.OutputBody()
			{
				Body = File.ReadAllBytes(iconPath),
				ContentType = MediaTypeNamesConstants.IMAGE_ICON,
				Status = HttpStatusCode.OK
			};
			return output;
		}

		private object ResolveToBeInvoked(string methodName, IDefaultController controller)
		{
			if (methodName == "FaviconIco")
			{
				return this;
			}
			return controller;
		}
	}
}
