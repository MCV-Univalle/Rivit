using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class TransformDrawerDemo : MonoBehaviour
	{
		[PHeader("The transform drawer in Power Inspector is highly customizable.", "You can try the different presets below to tailor the drawer for your tastes.")]
		[PSpace(5f)]
		[Button("Classic Look", "Try It")]
		private void ClassicLook()
		{
			Preferences.ClassicLook();
			InspectorUtility.ActiveInspector.UnfoldAllComponents();
		}

		[Button("Compact Look", "Try It")]
		private void CompactLook()
		{
			Preferences.CompactLook();
			InspectorUtility.ActiveInspector.UnfoldAllComponents();
		}

		[Button("Iconographic Look", "Try It")]
		private void IconographicLook()
		{
			Preferences.IconographicLook();
			InspectorUtility.ActiveInspector.UnfoldAllComponents();
		}

		[Button("Colorful X/Y/Z Tint", "Try It")]
		private void ColorfulXYZTint()
		{
			Preferences.ColorfulXYZTint();
			InspectorUtility.ActiveInspector.UnfoldAllComponents();
		}

		[Button("Colorful X/Y/Z Icons", "Try It")]
		private void ColorfulXYZIcons()
		{
			Preferences.ColorfulXYZIcons();
			InspectorUtility.ActiveInspector.UnfoldAllComponents();
		}

		[PSpace(5f)]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowDrawerInfo("transform-drawer");
		}

		private InspectorPreferences Preferences
		{
			get
			{
				return InspectorUtility.ActiveInspector.Preferences;
			}
		}
	}
}