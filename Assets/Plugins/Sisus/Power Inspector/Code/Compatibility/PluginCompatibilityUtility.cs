#define DEBUG_USE_COMPATIBILITY_MODE
//#define DEBUG_DONT_USE_COMPATIBILITY_MODE

#if UNITY_EDITOR

using System;
using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
#endif

namespace Sisus.Compatibility
{
	public static class PluginCompatibilityUtility
	{
		#if ODIN_INSPECTOR
		private static Type odinEditorType;
		
		[NotNull]
		private static Type OdinEditorType
		{
			get
			{
				if(odinEditorType == null)
				{
					odinEditorType = TypeExtensions.GetType("Sirenix.OdinInspector.Editor.OdinEditor");
				}
				return odinEditorType;
			}
		}
		#endif

		/// <summary>
		/// Given a type inheriting from UnityEngine.Object, determines whether or not an Editor should be used for drawing a target of this type, based on current compatibility preferences and installed plug-ins. </summary>
		/// <param name="type"> The type of a UnityEngine.Object target. This cannot be null. </param>
		/// <returns> True if should always use Editors for targets of type, false if not. </returns>
		public static bool UseCompatibilityModeForDisplayingTarget([NotNull]Type type)
		{
			#if ODIN_INSPECTOR && PI_ASSERTATIONS
			UnityEngine.Debug.Assert(type != null, "UseCompatibilityModeForDisplayingTarget was called with null type");
			UnityEngine.Debug.Assert(type.IsUnityObject(), "UseCompatibilityModeForDisplayingTarget was called for type that was not UnityEngine.Object: "+type.Name);
			#endif

			switch(InspectorUtility.Preferences.UseEditorsOverDrawers)
			{
				case UseEditorsOverDrawers.OnlyIfHasCustomEditor:
					return false;
				case UseEditorsOverDrawers.Always:
					return true;
				case UseEditorsOverDrawers.BasedOnPlugins:
					#if ODIN_INSPECTOR
					return true;
					#else
					return false;
					#endif
				case UseEditorsOverDrawers.BasedOnPluginPreferences:
					#if ODIN_INSPECTOR
					var odinConfig = GlobalConfig<InspectorConfig>.Instance;
					if(!odinConfig.EnableOdinInInspector)
					{
						#if DEV_MODE && DEBUG_DONT_USE_COMPATIBILITY_MODE
						UnityEngine.Debug.Log("Use Compatibility Mode For "+type.Name+": "+StringUtils.False);
						#endif
						return false;
					}
					var odinInspectorConfiguredEditorType = odinConfig.DrawingConfig.GetEditorType(type);
					if(odinInspectorConfiguredEditorType == OdinEditorType)
					{
						#if DEV_MODE && DEBUG_USE_COMPATIBILITY_MODE
						UnityEngine.Debug.Log("Use Compatibility Mode For "+type.Name+": "+StringUtils.True);
						#endif
						return true;
					}

					#if DEV_MODE && DEBUG_DONT_USE_COMPATIBILITY_MODE
					UnityEngine.Debug.Log("Use Compatibility Mode For "+type.Name+": "+StringUtils.False);
					#endif

					#endif
					return false;
				default:
					throw new IndexOutOfRangeException();
			}
		}
	}
}
#endif