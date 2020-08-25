using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class MultiEditingModesDemo : MonoBehaviour
	{
		[PHeader("You can change the active multi-editing mode by doing the following:\n\n1. <em>Select two or more targets</em> in the Hierarchy or Project view.\n\n2. <em>Click the multi-editing mode button</em> on the inspector toolbar.")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("multi-editing-modes");
		}
	}
}