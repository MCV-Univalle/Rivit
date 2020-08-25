#if UNITY_EDITOR
using System;
using Sisus.Attributes;
using UnityEngine;

namespace Sisus
{
	[Serializable, DrawerForComponent(typeof(Renderer), true, true)]
	public class RendererDrawer : CustomEditorComponentDrawer
	{
		// calling this instantiates the materials in edit mode
		private static readonly string[] DontDisplayMembers = { "material", "materials" };

		/// <inheritdoc/>
		protected override string[] NeverDisplayMembers()
		{
			return DontDisplayMembers;
		}

		/// <inheritdoc/>
		public override bool DrawBody(Rect position)
		{
			bool guiChangedWas = GUI.changed;
			GUI.changed = false;
			var renderer = Target as Renderer;
			var materials = renderer.sharedMaterials;
			int instanceIdWas = InstanceId;

			bool dirty = base.DrawBody(position);

			if(GUI.changed && !inactive && instanceIdWas == InstanceId && !materials.ContentsMatch(renderer.sharedMaterials))
			{
				// If the materials on the renderer changed then rebuild members of parent
				// in order to update the MaterialDrawers embedded on the GameObjectDrawer
				Inspector.OnNextLayout(parent.RebuildMemberBuildListAndMembers);
			}
			else
			{
				GUI.changed = guiChangedWas;
			}

			return dirty;

		}
	}
}
#endif