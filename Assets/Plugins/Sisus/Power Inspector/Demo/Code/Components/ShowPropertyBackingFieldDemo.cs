using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ShowPropertyBackingFieldDemo : MonoBehaviour
	{
		[field: PHeader("Properties with the <em>[field: SerializeField]</em> attribute are <em>serialized and shown</em> in Power Inspector with names matching the property.")]
		[field: SerializeField]
		public int SerializedProperty
		{
			get;
			private set;
		}

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}

		[Button]
		private void LearnMore()
		{
			PowerInspectorDocumentation.OpenUrl(PowerInspectorDocumentation.BaseUrl + "getting-started/class-member-visibility");
		}
	}
}