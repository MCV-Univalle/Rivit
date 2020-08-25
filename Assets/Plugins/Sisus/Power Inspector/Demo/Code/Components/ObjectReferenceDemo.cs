using UnityEngine;
using JetBrains.Annotations;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ObjectReferenceDemo : MonoBehaviour
	{
		[PHeader("All Object fields now have an <em>eyedropper</em> icon.", "Click the icon and then click something in the <em>Scene</em> view to set the value.")]
		public GameObject _gameObject;
		
		[PHeader("Tip: The eyedropper also works with <em>material</em> fields.")]
		public Material _material;

		[PHeader("You can right-click any Component field and select <em>auto-assign</em>.")]
		public Camera _camera;

		[PHeader("Any Object field with the <color=red>NotNull</color> attribute is tinted red when it has no value.")]
		[NotNull]
		public GameObject required;

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.ShowDrawerInfo("object-reference-drawer");
		}
	}
}