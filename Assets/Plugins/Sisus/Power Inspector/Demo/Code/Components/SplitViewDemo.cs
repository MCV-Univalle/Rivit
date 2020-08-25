using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class SplitViewDemo : MonoBehaviour
	{
		[PHeader("The <em>Split View</em> button can be used to split the inspector drawer into two views.\n\nYou can also <em>middle-click</em> a target to <em>\"Peek\"</em> (display its data in the split view).")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("split-view");
		}
	}
}