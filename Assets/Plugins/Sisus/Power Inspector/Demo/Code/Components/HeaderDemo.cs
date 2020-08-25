#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class HeaderDemo : MonoBehaviour
	{
		[Header("Headers in Power Inspector support word wrapping, so even longer texts can be shown without information being cut off. ")]
		public bool wordWrappingSupport = true;

		[Header("You can also use\nmanual\nline breaks.")]
		public bool lineBreakSupport = true;

		[Header("<size=15>Rich text markup is <color=green>supported</color></size>")]
		public bool richTextSupport = true;

		[PHeader("The <em>PHeader</em> attribute can be used to add headers before properties and methods.")]
		[ShowInInspector]
		public bool Property
		{
			get;
			set;
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("pheader");
		}
	}
}