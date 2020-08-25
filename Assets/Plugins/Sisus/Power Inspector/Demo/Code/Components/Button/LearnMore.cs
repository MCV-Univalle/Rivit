using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo.Button
{
	[RequireComponent(typeof(ButtonDemo)), AddComponentMenu("")]
	public class LearnMore : MonoBehaviour
	{
		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(GetComponent<ButtonDemo>());
		}

		[Button]
		private void learnMore()
		{
			PowerInspectorDocumentation.ShowAttribute("button");
		}
	}
}