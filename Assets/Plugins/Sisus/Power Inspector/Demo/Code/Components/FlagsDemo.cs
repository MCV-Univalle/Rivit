using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class FlagsDemo : MonoBehaviour
	{
		[PHeader("<em>Enum Flags</em> are supported.")]
		public MyFlags flags = MyFlags.F | MyFlags.L | MyFlags.A | MyFlags.G | MyFlags.S;

		[Button]
		private void ShowCode()
		{
			DemoUtility.ShowCode(this);
		}
	}

	[Flags]
	public enum MyFlags
	{
		None = 0,

		F = (1 << 1),
		L = (1 << 2),
		A = (1 << 3),
		G = (1 << 4),
		S = (1 << 5)
	}
}