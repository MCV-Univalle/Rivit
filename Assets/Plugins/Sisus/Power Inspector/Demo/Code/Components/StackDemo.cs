using System.Collections.Generic;
using UnityEngine;
using Sisus.OdinSerializer;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class StackDemo : SerializedMonoBehaviour
	{
		[ShowInInspector]
		public Stack<string> stack = new Stack<string>(new[]{ "1", "2", "3" });
	}
}