using System.Collections.Generic;
using Sisus.Attributes;
using Sisus.OdinSerializer;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class DictionaryDemo : SerializedMonoBehaviour
	{
		[ShowInInspector, PHeader("<em>Dictionary</em> is supported.")]
		public Dictionary<string, string> dictionary = new Dictionary<string, string>(){ {"Key 1", "Value 1" }, { "Key 2", "Value 2" }, { "Key 3", "Value 3" } };

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}
	}
}