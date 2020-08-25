using System;
using UnityEngine;

namespace Sisus
{
	public static class FloatExtensions
	{
		public static bool Approximately(this float a, float b)
		{
			return Math.Abs(a - b) <= Mathf.Epsilon;
		}

		public static bool ApproximatelyZero(this float a)
		{
			return Math.Abs(a) <= Mathf.Epsilon;
		}
	}
}