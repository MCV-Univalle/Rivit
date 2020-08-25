#if UNITY_EDITOR
using System;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus
{
	[Serializable, DrawerForComponent(typeof(Collider), true, true)]
	public class ColliderDrawer : CustomEditorComponentDrawer
	{
		//calling material creates a new copy of the material and causes memory leaking in the Editor
		private static readonly string[] DontDisplayMembers = {"material"};

		/// <inheritdoc/>
		protected override string[] NeverDisplayMembers()
		{
			return DontDisplayMembers;
		}
	}
}
#endif