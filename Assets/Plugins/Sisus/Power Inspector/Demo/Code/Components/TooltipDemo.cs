#pragma warning disable CS0414 //we are interested in seeing how the class member look in the inspector, so this warning isn't valid here

using JetBrains.Annotations;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class TooltipDemo : MonoBehaviour
	{
		[Tooltip("Tooltips in Power Inspector are relocated behind a dedicated tooltip icon by default.\n\nThis helps users know when tooltip information is available, and avoids issue of tooltips popping open when not desirable (like when the intention is to drag or right-click prefix labels).")]
		public bool tooltipIcons;
		
		///<summary>
		/// Power Inspector can also display tooltips generated from the XML documentation comments of fields.
		///</summary>
		public bool tooltipsFromXmlComments;

		///<summary>
		/// Generating tooltips from XML documentation comments of properties is supported as well.
		///</summary>
		[ShowInInspector]
		public bool PropertySupport
		{
			get;
			set;
		}

		///<summary>
		/// Generating tooltips from XML documentation comments of methods is supported too.
		///</summary>
		/// <param name="message"> Message to log to console. </param>
		/// <returns> False if provided message was null or empty, otherwise true. </returns>
		[ShowInInspector]
		public bool MethodSupport(string message)
		{
			if(string.IsNullOrEmpty(message))
			{
				return false;
			}
			Debug.Log(message);
			return true;
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("tooltips");
		}


		[UsedImplicitly]
		private void OnValidate()
		{
			UpdateValues();
		}

		[UsedImplicitly]
		private void Reset()
		{
			UpdateValues();
		}

		private void UpdateValues()
		{
			if(InspectorUtility.ActiveInspector == null)
			{
				return;
			}

			var preferences = InspectorUtility.ActiveInspector.Preferences;
			tooltipIcons = preferences.enableTooltipIcons;
			tooltipsFromXmlComments = preferences.enableTooltipsFromXmlComments;
			PropertySupport = true;
		}
	}
}