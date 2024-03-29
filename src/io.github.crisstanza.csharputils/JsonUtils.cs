﻿using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace io.github.crisstanza.csharputils
{
	public class JsonUtils
	{
		public byte[] SerializeToArray<T>(T jsonObject)
		{
			return Encoding.UTF8.GetBytes(Serialize(jsonObject) ?? "");
		}

		public string Serialize<T>(T jsonObject)
		{
			JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
			options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
