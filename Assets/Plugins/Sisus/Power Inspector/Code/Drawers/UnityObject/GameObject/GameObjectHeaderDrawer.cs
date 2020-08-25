using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	[Serializable]
	public class GameObjectHeaderDrawer
	{
		public const float OpenInPrefabModeButtonHeight = 30f;

		#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
		private static readonly GUIContent OpenPrefabButtonLabel = new GUIContent("Open In Prefab Mode", "Open in Prefab Mode for full editing support.");
		#endif
		
		[SerializeField]
		private GameObject[] targets;

		#if UNITY_EDITOR
		[SerializeField]
		private Editor editor;
		#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
		private bool isPrefab;
		#endif
		#endif
		
		public void SetTargets(GameObject[] setTargets)
		{
			targets = setTargets;

			#if UNITY_EDITOR
			Editors.GetEditor(ref editor, targets, null, true);

			#if UNITY_2018_3_OR_NEWER
			isPrefab = targets.Length == 1 && targets[0].IsPrefab();
			#endif
			#endif
		}

		public Rect Draw(Rect position)
		{
			#if UNITY_EDITOR
			if(Platform.EditorMode)
			{
				var actualDrawnPosition = EditorGUIDrawer.AssetHeader(position, editor);

				#if UNITY_2018_3_OR_NEWER
				if(isPrefab)
				{
					const float padding = 3f;
					const float doublePadding = padding + padding;

					position.y += position.height - OpenInPrefabModeButtonHeight + padding;
					position.height = OpenInPrefabModeButtonHeight - doublePadding;

					DrawGUI.Active.ColorRect(position, InspectorUtility.Preferences.theme.AssetHeaderBackground);

					position.x += padding;
					position.width -= doublePadding;

					// UPDATE: even if prefab is being drawn in grayed out color
					// due to being inactive, draw the open prefab button without
					// being grayed out, to make it clear that it remains usable.
					var guiColorWas = GUI.color;
					GUI.color = Color.white;
					
					if(GUI.Button(position, OpenPrefabButtonLabel))
					{
						DrawGUI.UseEvent();
						GameObjectDrawer.OpenPrefab(targets[0]);
					}

					GUI.color = guiColorWas;

					actualDrawnPosition.height += OpenInPrefabModeButtonHeight;
				}
				#endif

				return actualDrawnPosition;
			}
			#endif
			DrawGUI.Runtime.GameObjectHeader(position, targets[0]);
			return position;
		}

		public void ResetState()
		{
			targets = null;

			#if UNITY_EDITOR
			if(!ReferenceEquals(editor, null))
			{
				Editors.Dispose(ref editor);
			}
			#endif
		}

		public void OnProjectOrHierarchyChanged(GameObject[] setTargets)
		{
			targets = setTargets;

			#if UNITY_EDITOR
			if(editor == null || Editors.DisposeIfInvalid(ref editor))
			{
				Editors.GetEditor(ref editor, targets);
			}
			#endif
		}
	}
}