//#define DEBUG_ENABLED

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sisus.Attributes;

namespace Sisus
{
	/// <summary>
	/// Class that handles creating, caching and returning default drawer providers for inspectors.
	/// </summary>
	public static class DefaultDrawerProviders
	{
		private static Dictionary<Type, IDrawerProvider> drawerProvidersByInspectorType;
		
		[CanBeNull]
		public static IDrawerProvider GetForInspector(Type inspectorType)
		{
			if(drawerProvidersByInspectorType == null)
			{
				drawerProvidersByInspectorType = new Dictionary<Type, IDrawerProvider>(1);

				FindDrawerProviderForAttributesInTypes(TypeExtensions.AllVisibleTypes);
				FindDrawerProviderForAttributesInTypes(TypeExtensions.AllInvisibleTypes);

				// Also add derived types of inspector types
				var addDerived = new List<KeyValuePair<Type, IDrawerProvider>>();
				foreach(var drawerByType in drawerProvidersByInspectorType)
				{
					var exactInspectorType = drawerByType.Key;
					var derivedInspectorTypes = exactInspectorType.IsInterface ? TypeExtensions.GetImplementingTypes(exactInspectorType, true) : TypeExtensions.GetExtendingTypes(exactInspectorType, true);
					for(int n = derivedInspectorTypes.Length - 1; n >= 0; n--)
					{
						#if DEV_MODE && DEBUG_ENABLED
						UnityEngine.Debug.Log("derived inspector type "+derivedInspectorTypes[n].Name+" DrawerProvider is "+drawerByType.Value.GetType().Name);
						#endif

						addDerived.Add(new KeyValuePair<Type, IDrawerProvider>(derivedInspectorTypes[n], drawerByType.Value));
					}
				}
				for(int n = addDerived.Count - 1; n >= 0; n--)
				{
					var add = addDerived[n];
					var derivedInspectorType = add.Key;
					if(!drawerProvidersByInspectorType.ContainsKey(derivedInspectorType))
					{
						drawerProvidersByInspectorType.Add(derivedInspectorType, add.Value);
					}
				}
			}

			IDrawerProvider drawerProvider;
			return drawerProvidersByInspectorType.TryGetValue(inspectorType, out drawerProvider) ? drawerProvider : null;
		}

		private static void FindDrawerProviderForAttributesInTypes([NotNull]Type[] types)
		{
			for(int n = types.Length - 1; n >= 0; n--)
			{
				var type = types[n];

				var attributes = type.GetCustomAttributes(false);
				foreach(var attribute in attributes)
				{
					var drawerProviderFor = attribute as DrawerProviderForAttribute;
					if(drawerProviderFor != null)
					{
						var inspectorType = drawerProviderFor.inspectorType;
						if(inspectorType == null)
						{
							UnityEngine.Debug.LogError(drawerProviderFor.GetType().Name + " on class "+type.Name+" NullReferenceException - inspectorType was null!");
							continue;
						}

						IDrawerProvider drawerProvider;
						if(!drawerProvidersByInspectorType.TryGetValue(inspectorType, out drawerProvider) || !drawerProviderFor.isFallback)
						{
							bool reusedExistingInstance = false;
							foreach(var createdDrawerProvider in drawerProvidersByInspectorType.Values)
							{
								if(createdDrawerProvider.GetType() == type)
								{
									drawerProvidersByInspectorType.Add(inspectorType, createdDrawerProvider);
									reusedExistingInstance = true;
									break;
								}
							}
							
							if(!reusedExistingInstance)
							{
								#if DEV_MODE && DEBUG_ENABLED
								UnityEngine.Debug.Log("Creating new DrawerProvider instance of type "+type.Name+" for inspector"+inspectorType.Name);
								#endif

								var drawerProviderInstance = (IDrawerProvider)type.CreateInstance();

								#if DEV_MODE && PI_ASSERTATIONS
								UnityEngine.Debug.Assert(drawerProviderInstance != null);
								#endif

								drawerProvidersByInspectorType.Add(inspectorType, drawerProviderInstance);

								#if DEV_MODE && PI_ASSERTATIONS
								UnityEngine.Debug.Assert(drawerProvidersByInspectorType[inspectorType] != null);
								#endif
							}
						}
					}
				}
			}
		}
	}
}