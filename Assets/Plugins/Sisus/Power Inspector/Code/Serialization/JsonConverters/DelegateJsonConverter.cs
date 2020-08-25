using System;
using System.Collections.Generic;
using System.Text;
using Sisus.Newtonsoft.Json;
using Sisus.Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;

namespace Sisus
{
	public class DelegateJsonConverter : JsonConverter
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
			#if DEV_MODE
			UnityEngine.Debug.Log(GetType().Name+ ".CanConvert("+StringUtils.ToStringSansNamespace(objectType)+"): " + StringUtils.ToColorizedString(typeof(Delegate).IsAssignableFrom(objectType)));
			#endif

			return typeof(Delegate).IsAssignableFrom(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string serializedString;
			if(value == null)
			{
				serializedString = "null";
			}
			else
			{
				//List<Object> unityObjects;
				//var bytes = OdinSerializer.SerializationUtility.SerializeValueWeak(value, OdinSerializer.DataFormat.Binary, out unityObjects);
				//serializedString = Encoding.UTF8.GetString(bytes);

				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All,
					ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
					{
						IgnoreSerializableInterface = false,
						IgnoreSerializableAttribute = false,
					},
					Formatting = Formatting.Indented,
				};
				serializedString = JsonConvert.SerializeObject(value, settings);
			}

			writer.WriteRawValue(serializedString);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var token = JToken.ReadFrom(reader);
			string stringData = token.Value<string>(); //reader.Value as string;

			

			#if DEV_MODE
			UnityEngine.Debug.Log(GetType().Name+".ReadJson with stringData="+StringUtils.ToString(stringData)+ ", reader.ReadAsString()="+StringUtils.ToString(reader.ReadAsString())+ ", token.Value<string>()="+StringUtils.ToString(token.Value<string>()));
			#endif

			//stringData = token.Value<string>();


			//var jobject = JObject.Load(reader);
			//var enumerator = jobject.GetEnumerator();
			//enumerator.MoveNext();

			if(string.IsNullOrEmpty(stringData) || string.Equals(stringData, "null"))
			{
				return null;
			}

			//var bytes = Encoding.UTF8.GetBytes(stringData);
			//List<Object> unityObjects = null;
			//OdinSerializer.SerializationUtility.DeserializeValueWeak(bytes, OdinSerializer.DataFormat.Binary, unityObjects);

			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
				{
					IgnoreSerializableInterface = false,
					IgnoreSerializableAttribute = false,
				},
				Formatting = Formatting.Indented,
			};
			return JsonConvert.DeserializeObject(stringData, settings);
		}
	}
}