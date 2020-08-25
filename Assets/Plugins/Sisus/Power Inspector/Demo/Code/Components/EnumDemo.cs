using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class EnumDemo : MonoBehaviour
	{
		[PHeader("All <em>enum</em> field popups in Power Inspector contain a <em>filter field</em>.", "A number of new inputs are now also supported:\n<em>Mouse Wheel, Page Up, Page Down, Home and End</em>.")]
		public KeyCode keyCode = KeyCode.A;

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowDrawerInfo("enum-drawer");
		}
	}
}