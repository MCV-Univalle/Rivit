using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class SearchBoxDemo : MonoBehaviour
	{
		[PHeader("The search box can be used to <em>filter what data is shown</em> inside the inspector view.\n\nYou can change between <em>different filtering modes</em> by clicking the maginifying glass icon.\n\nYou can use <em>Ctrl + F</em> to quickly move focus to the search box.")]
		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowFeature("search-box");
		}
	}
}