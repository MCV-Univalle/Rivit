#pragma warning disable CS0414 //we are interested in seeing how the fields look in the inspector, so this warning isn't valid here

using System.Collections.Generic;
using UnityEngine;
using Sisus.OdinSerializer;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class HashSetDemo : SerializedMonoBehaviour
	{
		[PHeader("<em>HashSet</em> is supported.")]
		[ShowInInspector]
		public HashSet<int> hashSet = new HashSet<int> { 1, 2, 3 };
	}
}