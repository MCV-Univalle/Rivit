using UnityEngine;
using Sisus.Attributes;
using JetBrains.Annotations;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ShyComponent : MonoBehaviour
	{
		[PHeader("Oh no, You found me!",
			"Please let me hide in peace...")]
		[Button]
		private void HideHiddenComponents()
		{
			InspectorUtility.ActiveInspector.Preferences.ShowHiddenComponents = false;
			InspectorUtility.ActiveInspector.ForceRebuildDrawers();
		}


		[UsedImplicitly]
		private void Reset()
		{
			hideFlags = HideFlags.HideInInspector;
		}
	}
}