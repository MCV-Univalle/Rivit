using System;
using UnityEngine;
using Sisus.OdinSerializer;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class TypeDemo : SerializedMonoBehaviour
	{
		[PHeader("Power Inspector can display <em>Type</em> fields.", "To show a Type field use the <em>ShowInInspector</em> attribute or turn on <em>Show Non-Serialized Fields</em> in preferences.")]
		[ShowInInspector]
		public Type typeField = typeof(Type);

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}
	}
}