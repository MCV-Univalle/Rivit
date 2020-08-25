using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	[OnlyComponent]
	public class OnlyComponentDemo : MonoBehaviour
	{
		[PHeader("This component has the <em>OnlyComponent attribute</em>.",
				"This makes it so that no other components can be added to the same GameObject through the inspector.",
				"As you can see, even the <em>Add Component button</em> has been completely <em>hidden</em>.")]
		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("onlycomponent");
		}
	}
}