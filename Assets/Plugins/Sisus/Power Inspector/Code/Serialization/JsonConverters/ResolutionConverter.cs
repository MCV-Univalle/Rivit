using System;
using UnityEngine;
using Sisus.Newtonsoft.Json;
using Sisus.Newtonsoft.Json.Linq;

namespace Sisus
{
	public class ResolutionConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Resolution);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var resolution = (Resolution)value;
			writer.WriteStartObject();
			writer.WritePropertyName("height");
			writer.WriteValue((resolution).height);
			writer.WritePropertyName("width");
			writer.WriteValue(resolution.width);
			writer.WritePropertyName("refreshRate");
			writer.WriteValue(resolution.refreshRate);
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jobject = JObject.Load(reader);
			var resolution = default(Resolution);
			var enumerator = jobject.GetEnumerator();
			enumerator.MoveNext();
			resolution.height = enumerator.Current.Value.Value<int>();
			enumerator.MoveNext();
			resolution.width = enumerator.Current.Value.Value<int>();
			enumerator.MoveNext();
			resolution.refreshRate = enumerator.Current.Value.Value<int>();
			return resolution;
		}
	}
}
