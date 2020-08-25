using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class RangeDemo : MonoBehaviour
	{
		[PHeader("The <em>Range</em> attribute drawer has been enhanced with a <em>tooltip</em> letting you exactly what the value will be set to before you even click.")]
		[Range(0f, 100f)]
		public int range;
	}
}