//#define DEBUG_CONVERT

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sisus.Compatibility
{
	/// <summary>
	/// A class that can be extended to add support in Power Inspector for attributes provided by third party plugins.
	/// 
	/// This can be used specify how to convert third-party attributes to Power Inspector attributes.
	/// </summary>
	public abstract class PluginAttributeConverterProvider
	{
		/// <summary> Gets all active plugin attribute converters. </summary>
		[NotNull]
		public static readonly Dictionary<Type, AttributeConverter> converters;

		/// <summary> List of plugin attribute aliases for Power Inspector supported attribute types. </summary>
		[NotNull]
		public static readonly Dictionary<Type, Type[]> aliases;

		[NotNull]
		public static readonly Type[] ignoredEditors;

		/// <summary> Static constructor initializes converters. </summary>
		static PluginAttributeConverterProvider()
		{
			var allTypes = typeof(PluginAttributeConverterProvider).GetExtendingNonUnityObjectClassTypes(true);

			int count = allTypes.Length;
			int capacity = count * 10;
			converters = new Dictionary<Type, AttributeConverter>(capacity);
			aliases = new Dictionary<Type, Type[]>(capacity);

			var editorsToIgnore = new HashSet<Type>();

			#if DEV_MODE
			UnityEngine.Debug.Log("!!!!!!!!!!!!!!!!!!!! PluginAttributeConverterProvider found " + count + " PluginAttributeConverterProviders");
			#endif

			for(int n = count - 1; n >= 0; n--)
			{
				PluginAttributeConverterProvider converterProvider;
				try
				{
					converterProvider = (PluginAttributeConverterProvider)allTypes[n].CreateInstance();
				}
				catch(Exception e)
				{
					UnityEngine.Debug.LogError(e);
					continue;
				}

				if(!converterProvider.IsActive)
				{
					continue;
				}

				converterProvider.AddConverters(Register);

				#if DEV_MODE
				UnityEngine.Debug.Log(converterProvider.GetType().Name+".AddConverters: now has "+converters.Count+" converters");
				#endif

				var ignoreEditors = converterProvider.IgnoreEditors;
				for(int e = ignoreEditors.Length - 1; e >= 0; e--)
				{
					var type = TypeExtensions.GetType(ignoreEditors[e]);

					if(type == null)
					{
						UnityEngine.Debug.LogWarning(converterProvider.GetType().Name + " Editor type by name not found: "+ ignoreEditors[e]);
						continue;
					}

					editorsToIgnore.Add(type);
				}

				ignoredEditors = new Type[editorsToIgnore.Count];
				editorsToIgnore.CopyTo(ignoredEditors);
			}
		}

		private static void Register([NotNull]Type pluginAttributeType, [NotNull]Type supportedAttributeType, [NotNull]AttributeConverter converter)
		{
			converters[pluginAttributeType] = converter;

			Type[] array;
			if(aliases.TryGetValue(supportedAttributeType, out array))
			{
				aliases[supportedAttributeType] = array.Add(pluginAttributeType);
			}
			else
			{
				aliases[supportedAttributeType] = new Type[] { pluginAttributeType };
			}
		}

		public static IEnumerator<Type> GetAttributeTypeAndEachAlias(Type attributeType)
		{
			yield return attributeType;

			Type[] aliases;
			if(PluginAttributeConverterProvider.aliases.TryGetValue(attributeType, out aliases))
			{
				for(int n = aliases.Length - 1; n >= 0; n--)
				{
					yield return aliases[n];
				}
			}
		}

		/// <summary>
		/// Converts each attribute inside array that has a registered converter to an attribute supported by Power Inspector.
		/// </summary>
		/// <param name="attributes"></param>
		public static void ConvertAll([NotNull]ref object[] attributes)
		{
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];

				AttributeConverter converter;
				if(converters.TryGetValue(attribute.GetType(), out converter))
				{
					attributes[n] = converter(attribute);

					#if DEV_MODE && DEBUG_CONVERT
					UnityEngine.Debug.Log("Converted "+ attribute.GetType().FullName + " to "+ attributes[n].GetType().FullName);
					#endif
				}
			}
		}

		public static bool TryConvert<T>([NotNull]object attribute, out T result) where T : class
		{
			result = attribute as T;
			if(result != null)
			{
				return true;
			}

			AttributeConverter converter;
			if(converters.TryGetValue(attribute.GetType(), out converter))
			{
				result = converter(attribute) as T;
				if(result != null)
				{
					return true;
				}
			}

			return false;
		}

		public static bool TryConvert([NotNull]object attribute, [NotNull]Type convertToAttributeType, out object result)
		{
			if(attribute.GetType() == convertToAttributeType)
			{
				result = attribute;
				return true;
			}

			AttributeConverter converter;
			if(converters.TryGetValue(attribute.GetType(), out converter))
			{
				var converted = converter(attribute);
				if(converted != null && converted.GetType() == convertToAttributeType)
				{
					result = converted;
					return true;
				}
			}

			result = null;
			return false;
		}

		/// <summary> Is the plugin currently installed and active? </summary>
		/// <value> True if plugin is installed and active, false if not. </value>
		public virtual bool IsActive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Returns list of full names of custom editors that should be ignored by Power Inspector.
		/// 
		/// Sometimes plugins can contain a custom Editor used by all Objects in Unity.
		/// By default Power Inspector will also use this Custom Editor, resulting in some functionality
		/// provided by Power Inspector's custom drawers to be lost.
		/// 
		/// When the functionality of a plugin is already supported with the attribute conversion feature,
		/// it can often be desirable to prevent the base editor from being used in Power Inspector.
		/// </summary>
		public virtual string[] IgnoreEditors
		{
			get
			{
				return new string[0];
			}
		}

		/// <summary>
		/// Add converters that allow converting plugin attributes to Power Inspector supported attributes.
		/// </summary>
		/// <param name="add">
		/// Delegate for registering information that can be used to convert a plugin attribute instance to a Power Inspector supported attribute instance.
		/// </param>
		public abstract void AddConverters([NotNull]RegisterAttributeConverter add);		
	}
}