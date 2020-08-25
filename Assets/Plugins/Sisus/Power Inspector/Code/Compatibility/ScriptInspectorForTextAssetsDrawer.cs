#if UNITY_EDITOR
using UnityEngine;

namespace Sisus.Compatibility
{
	/// <summary> Drawer that add better support for Script Inspector 3 by FLIPBOOK GAMES. </summary>
	public class ScriptInspectorForTextAssetsDrawer : CustomEditorTextAssetDrawer
	{
		/// <inheritdoc/>
		public override bool WantsSearchBoxDisabled
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc/>
		public override bool DrawBody(Rect position)
		{
			var height = Height;
			height -= 55f;
			if(height <= 0f)
			{
				return false;
			}
			GUILayout.BeginHorizontal(GUILayout.Height(height));
			bool dirty = base.DrawBody(position);
			GUILayout.EndHorizontal();
			return dirty;
		}
	}
}
#endif