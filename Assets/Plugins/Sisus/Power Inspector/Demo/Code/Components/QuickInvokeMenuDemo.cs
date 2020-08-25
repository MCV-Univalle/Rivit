using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class QuickInvokeMenuDemo : MonoBehaviour
	{
		[PHeader("The <em>Quick Invoke Menu</em>\nbutton can be used to\nquickly invoke methods on\nany components and assets.",
		"By default only <em>public methods</em> will be listed, but if you <em>right-click</em> the button <em>non-public</em> ones will be included as well.")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("quick-invoke-menu");
		}

		public void PrintDocumentationUrl()
		{
			Debug.Log(PowerInspectorDocumentation.GetFeatureUrl("quick-invoke-menu"));
		}

		private void CountToFive()
		{
			for(int n = 1; n <= 5; n++)
			{
				Debug.Log(n);
			}
		}
	}
}