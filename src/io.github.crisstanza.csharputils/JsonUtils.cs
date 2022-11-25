using System;
using System.Text;
using System.Text.Json;

namespace io.github.crisstanza.csharputils
{
	public class JsonUtils
	{
		private readonly bool debug;

		public JsonUtils(bool debug=true)
		{
			this.debug = debug;
		}

		public byte[] SerializeToArray<T>(T jsonObject)
		{
			return Encoding.UTF8.GetBytes(Serialize(jsonObject) ?? "");
		}

		public string Serialize<T>(T jsonObject)
		{
			JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = this.debug, IgnoreNullValues = !this.debug };
			string jsonString = JsonSerializer.Serialize<T>(jsonObject, options);
			return jsonString;
		}

		public T Deserialize<T>(string jsonString)
		{
			T jsonObject = JsonSerializer.Deserialize<T>(jsonString);
			return jsonObject;
		}
		public object Deserialize(string jsonString, Type returnType)
		{
			object jsonObject = JsonSerializer.Deserialize(jsonString, returnType);
			return jsonObject;
		}
	}
}
