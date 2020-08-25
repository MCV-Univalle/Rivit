//#define DEBUG_BUILD

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus
{
	public static class UseDrawerAttributeUtility
	{
		private static Dictionary<Type, Type> drawersByClassType;
		private static Dictionary<MemberInfo, Type> drawersByMember;

		public static Dictionary<Type, Type> GetCustomDrawersByClassType(IDrawerProvider drawerProvider)
		{
			if(drawersByClassType == null)
			{
				BuildDictionaries(drawerProvider);
			}

			return drawersByClassType;
		}

		public static bool TryGetCustomDrawerForClassMember(IDrawerProvider drawerProvider, MemberInfo member, out Type drawerType)
		{
			if(drawersByMember == null)
			{
				BuildDictionaries(drawerProvider);
			}

			return drawersByMember.TryGetValue(member, out drawerType);
		}

		private static void BuildDictionaries(IDrawerProvider drawerProvider)
		{
			drawersByClassType = new Dictionary<Type, Type>();
			drawersByMember = new Dictionary<MemberInfo, Type>();

			BuildDictionaries(drawerProvider, TypeExtensions.AllVisibleTypes);
			BuildDictionaries(drawerProvider, TypeExtensions.AllInvisibleTypes);
		}

		private static void BuildDictionaries(IDrawerProvider drawerProvider, Type[] types)
		{
			#if DEV_MODE && DEBUG_BUILD
			Debug.Log("UseDrawerAttributeUtility.BuildDictionaries - building from "+types.Length+" types...");
			#endif

			for(int t = types.Length - 1; t >= 0; t--)
			{
				var type = types[t];
				var attributes = type.GetCustomAttributes(false);
				Compatibility.PluginAttributeConverterProvider.ConvertAll(ref attributes);

				for(int a = attributes.Length - 1; a >= 0; a--)
				{
					var attribute = attributes[a];
					var useDrawerAttribute = attribute as IUseDrawer;
					if(useDrawerAttribute == null)
					{
						continue;
					}

					var drawerType = useDrawerAttribute.GetDrawerType(type, drawerProvider.GetClassDrawerType(type));

					#if DEV_MODE && DEBUG_BUILD
					Debug.LogError("IUseDrawer: use drawer "+drawerType.Name+" for class "+type.Name);
					#endif

					try
					{
						drawersByClassType.Add(type, drawerType);
					}
					catch(Exception e)
					{
						Debug.LogError(e);
					}
				}

				var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
				for(int m = members.Length - 1; m >= 0; m--)
				{
					var member = members[m];
					attributes = member.GetCustomAttributes(false);
					Compatibility.PluginAttributeConverterProvider.ConvertAll(ref attributes);

					for(int a = attributes.Length - 1; a >= 0; a--)
					{
						var useDrawerAttribute = attributes[a] as IUseDrawer;
						if(useDrawerAttribute == null)
						{
							continue;
						}

						var memberType = member.DeclaringType;

						var drawerType = useDrawerAttribute.GetDrawerType(memberType, drawerProvider.GetClassMemberDrawerType(memberType));

						#if DEV_MODE && DEBUG_BUILD
						Debug.Log("IUseDrawer: use drawer "+drawerType.Name+" for class member \""+ member.Name +"\" of class "+type.Name);
						#endif

						try
						{
							drawersByMember.Add(member, drawerType);
						}
						catch(Exception e)
						{
							Debug.LogError(e);
						}
					}
				}
			}
		}
	}
}