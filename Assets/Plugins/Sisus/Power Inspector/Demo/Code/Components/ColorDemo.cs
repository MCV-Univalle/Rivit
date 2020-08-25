using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class ColorDemo : MonoBehaviour
	{
		[PHeader("Color and Color32 fields in Power Inspector have new context menu items:\n<em>Set Value</em>, <em>Set Alpha</em> and <em>Copy As</em>.")]
		public Color color;
	}
}