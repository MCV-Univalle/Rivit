using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo.HideTransformInInspector
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	[RequireComponent(typeof(ReadOnlyTransform))]
	public class LearnMore : MonoBehaviour
	{
		[PHeader("The above component has the <em>HideTransformInInspector</em> attribute.",
				"This causes the Transform component to be hidden in the inspector.")]
		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(GetComponent<ReadOnlyTransform>());
		}

		[Button]
		private void learnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("hidetransformininspector");
		}
	}
}