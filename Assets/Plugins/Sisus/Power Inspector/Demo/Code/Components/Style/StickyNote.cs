using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo.Style
{
	[AddComponentMenu("")] // hide in add component menu to avoid cluttering it
	public class StickyNote : MonoBehaviour
	{
		[Style("VCS_StickyNote"), TextArea]
		public string note = "(Add note here)";
	}
}