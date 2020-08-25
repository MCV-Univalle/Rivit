using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ViewMenuDemo : MonoBehaviour
	{
		[PHeader("Clicking the <em>view menu</em> button on the toolbar opens up the view menu.",
			"From the menu you can <em>change options</em> related to what and how data is displayed inside the inspector view.",
			"Try selecting <em>Hidden Components > Show</em> from the view menu.")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("view-menu");
		}
	}
}