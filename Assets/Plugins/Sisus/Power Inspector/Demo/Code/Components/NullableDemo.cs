using UnityEngine;
using Sisus.Attributes;
using Sisus.OdinSerializer;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class NullableDemo : SerializedMonoBehaviour
	{
		[PHeader("<em>Nullable</em> is supported.")]
		[ShowInInspector]
		public int? nullableInt = 42;
	}
}