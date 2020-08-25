using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class KeyboardNavigationDemoPt2 : MonoBehaviour
	{
		[PHeader("You can also use <em>Ctrl + Up</em> and <em>Ctrl + Down</em> to jump to the <em>next or previous component</em>.")]
		public Rect rect1 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect2 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect3 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect4 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect5 = new Rect(0f, 1f, 2f, 3f);
	}
}