using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class KeyboardNavigationDemo : MonoBehaviour
	{
		[PHeader("In Power Inspector <em>up is up</em> and <em>down is down</em>!",
		"Try changing the focused field using the <em>arrow keys</em> on your keyboard.")]
		public Rect rect1 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect2 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect3 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect4 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect5 = new Rect(0f, 1f, 2f, 3f);
	}
}