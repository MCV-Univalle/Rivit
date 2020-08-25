#if UNITY_EDITOR
using System;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus
{
	[Serializable, DrawerForComponent(typeof(BoxCollider), false, true)]
	public class BoxColliderDrawer : CustomEditorComponentDrawer
	{
		// Calling material creates a new copy of the material and causes memory leaking in the Editor.
		private static readonly string[] DontDisplayMembers = { "material" };
	
		/// <inheritdoc />
		protected override float EstimatedUnfoldedHeight
		{
			get
			{
				#if UNITY_2019_3_OR_NEWER
				return 148f;
				#else
				return 123f;
				#endif
			}
		}

		/// <inheritdoc/>
		protected override float GetOptimalPrefixLabelWidthForEditor(int indentLevel)
		{
			return 78f;
		}

		/// <inheritdoc/>
		protected override string[] NeverDisplayMembers()
		{
			return DontDisplayMembers;
		}
	}
}
#endif