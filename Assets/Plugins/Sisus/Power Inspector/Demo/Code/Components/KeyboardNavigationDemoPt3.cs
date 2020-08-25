using Sisus.Attributes;
using UnityEngine;

namespace Sisus.Demo
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class KeyboardNavigationDemoPt3 : MonoBehaviour
	{
		[PHeader("<em>Ctrl + Left</em> and <em>Ctrl + Right</em> can be used to jump to the next or previous component of the same type in the scene.")]
		public Rect rect1 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect2 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect3 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect4 = new Rect(0f, 1f, 2f, 3f);
		public Rect rect5 = new Rect(0f, 1f, 2f, 3f);
	}
}