using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo.Style
{
	[AddComponentMenu(""), RequireComponent(typeof(StickyNote))] // hide in add component menu to avoid cluttering it
	public class LearnMore : MonoBehaviour
	{
		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(GetComponent<StickyNote>());
		}

		[Button]
		private void learnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("style");
		}
	}
}