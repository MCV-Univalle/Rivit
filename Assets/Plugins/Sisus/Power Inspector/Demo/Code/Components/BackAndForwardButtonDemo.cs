using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class BackAndForwardButtonDemo : MonoBehaviour
	{
		[PHeader("The toolbar contains new <em>back and forward buttons</em> that allow you to quickly navigate through the history of inspected targets.", "<em>Right-clicking</em> the back or the forward button opens up a <em>context menu</em> containing the full listing of targets in that direction.")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("back-and-forward-buttons");
		}
	}
}