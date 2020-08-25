using UnityEngine;

namespace Sisus
{
	/// <summary> Utility class for setting text color of inspector titlebar in active GUI.skin. </summary>
	public static class GUIStyleUtility
	{
		private static Color? titlebarTextNormalColor;
		private static Color? titlebarToggleNormalColor;

		/// <summary> Sets the inspector titlebar text in current GUI.skin to using the given color. </summary>
		public static void SetInspectorTitlebarTextColor(Color color)
		{
			var guiStyle = GUI.skin.GetStyle("IN TitleText");

			if(!titlebarTextNormalColor.HasValue)
			{
				titlebarTextNormalColor = guiStyle.normal.textColor;
			}

			guiStyle.normal.textColor = color;
			guiStyle.active.textColor = color;
		}

		/// <summary> Sets the inspector titlebar text in current GUI.skin to using the given color. </summary>
		public static void SetInspectorTitlebarPinged()
		{
			var guiStyle = GUI.skin.GetStyle("IN TitleText");
			guiStyle.SetAllBackgrounds(InspectorPreferences.Styles.PingedHeader.normal.background);
		}

		/// <summary> Sets the inspector titlebar text in current GUI.skin to using the given color. </summary>
		public static void ResetInspectorTitlebarBackground()
		{
			var guiStyle = GUI.skin.GetStyle("IN TitleText");
			guiStyle.SetAllBackgrounds(null);
		}

		public static void SetInspectorTitlebarToggleColor(Color color)
		{
			var guiStyle = GUI.skin.toggle;

			if(!titlebarToggleNormalColor.HasValue)
			{
				titlebarToggleNormalColor = guiStyle.normal.textColor;
			}

			guiStyle.normal.textColor = color;
			guiStyle.active.textColor = color;
		}

		/// <summary> Resets the inspector titlebar text back to using its normal color. </summary>
		public static void ResetInspectorTitlebarTextColor()
		{
			var guiStyle = GUI.skin.GetStyle("IN TitleText");
			guiStyle.normal.textColor = InspectorUtility.Preferences.theme.PrefixIdleText;
			guiStyle.active.textColor = InspectorUtility.Preferences.theme.PrefixIdleText;
		}

		public static void ResetInspectorTitlebarToggleColor()
		{
			var guiStyle = GUI.skin.toggle;
			guiStyle.normal.textColor = InspectorUtility.Preferences.theme.PrefixIdleText;
			guiStyle.active.textColor = InspectorUtility.Preferences.theme.PrefixIdleText;
		}
	}
}