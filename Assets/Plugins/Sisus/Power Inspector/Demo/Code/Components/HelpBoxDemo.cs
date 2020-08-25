using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class HelpBoxDemo : MonoBehaviour
	{
		[HelpBox("A new attribute HelpBox can be used to display information above a fields, properties and methods.")]
		public string hasAHelpBox;

		[NonSerialized, ShowInInspector, HelpBox("It is also possible to have HelpBoxes only be shown when specified conditions are met.", false)]
		public bool enableMe = false;

		[NonSerialized, Range(0, 100), ShowInInspector]
		[HelpBox("In addition to info boxes...", Is.Smaller, 33)]
		[HelpBox("...you can show warnings...", "this >= 33 && this < 66", HelpBoxMessageType.Warning)]
		[HelpBox("...as well as errors.", HelpBoxMessageType.Error, Is.LargerOrEqual, 66)]
		public int adjustMe = 0;

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("helpbox");
		}
	}
}