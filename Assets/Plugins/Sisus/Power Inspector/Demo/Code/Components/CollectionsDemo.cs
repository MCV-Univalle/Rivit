#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using System.Collections.Generic;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class CollectionsDemo : MonoBehaviour
	{
		[PHeader("All collections in Power Inspector support drag-n-drop <em>reordering</em> and <em>multi-selection</em>.")]
		public string[] array = { "A1", "A2", "A3" };
		
		[PHeader("Reordering is even possible between collections of <em>different types</em>.", "You can try dragging a member from the array above to this list.")]
		public List<string> list = new List<string>{ "L1", "L2", "L3" };

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowDrawerInfo("collection-drawer");
		}
	}
}