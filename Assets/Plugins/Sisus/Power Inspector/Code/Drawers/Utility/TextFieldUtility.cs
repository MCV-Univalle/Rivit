//#define DEBUG_EDITING_TEXT_FIELD
#define DEBUG_SYNC_EDITING_TEXT_FIELD

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Sisus
{
	public static class TextFieldUtility
	{
		/// <summary> Call this when a text field control is clicked. </summary>
		/// <param name="subject"> The clicked text field drawer. </param>
		/// <param name="inputEvent"> The current input event. </param>
		public static void OnControlClicked(ITextFieldDrawer subject, Event inputEvent)
		{
			bool canStartEditing = CanStartEditing(subject);

			// if field was already selected when it was clicked, don't use the event
			// this way Unity can handle positioning the cursor in a specific point on the text field etc.
			if(subject.Selected)
			{
				#if DEV_MODE && DEBUG_EDITING_TEXT_FIELD
				Debug.Log(StringUtils.ToColorizedString("TextFieldUtility.OnControlClicked - DrawGUI.EditingTextField = ", canStartEditing, " (because was selected)"));
				#endif

				DrawGUI.EditingTextField = canStartEditing; 
				return;
			}

			DrawGUI.Use(inputEvent);
			subject.HandleOnClickSelection(inputEvent, ReasonSelectionChanged.ControlClicked);
			if(canStartEditing)
			{
				#if DEV_MODE && DEBUG_EDITING_TEXT_FIELD
				Debug.Log(StringUtils.ToColorizedString("TextFieldUtility.OnControlClicked - Calling StartEditingField (not selected and can start editing)"));
				#endif

				subject.StartEditingField();
			}
			else
			{
				#if DEV_MODE && DEBUG_EDITING_TEXT_FIELD
				Debug.Log(StringUtils.ToColorizedString("TextFieldUtility.OnControlClicked - DrawGUI.EditingTextField = ", false, " (because was can start editing was ", false, ")"));
				#endif

				DrawGUI.EditingTextField = false;
			}
		}

		/// <summary>
		/// Determines whether it is possible to start editing the text field drawer at this time,
		/// given the current state of the drawer and the inspector view.
		/// 
		/// Returns false if subject is read only or there's a multi-selection (editing multiple text fields
		/// simultaneously is not supported).
		/// </summary>
		/// <param name="subject"> The subject. </param>
		/// <returns> True if we can start editing, false if not. </returns>
		public static bool CanStartEditing(ITextFieldDrawer subject)
		{
			return !subject.ReadOnly && !InspectorUtility.ActiveManager.HasMultiSelectedControls;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// If DrawGUI.EditingTextField and EditorGUIUtility.editingTextField values are not in sync,
		/// then figures out which should take precedence over the other, and syncs their values accordingly.
		/// </summary>
		public static void SyncEditingTextField()
		{
			if(DrawGUI.EditingTextField == EditorGUIUtility.editingTextField)
			{
				return;
			}

			var manager = InspectorUtility.ActiveManager;
			if(manager == null)
			{
				DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
				return;
			}

			var lastInputEvent = DrawGUI.LastInputEvent();
			if(lastInputEvent == null)
			{
				DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
				return;
			}

			if(lastInputEvent.isMouse)
			{
				// something else than Power Inspector was probably clicked
				if(manager.MouseoveredInspector == null)
				{
					#if DEV_MODE && DEBUG_SYNC_EDITING_TEXT_FIELD
					Debug.Log("DrawGUI.editingTextField = "+StringUtils.ToColorizedString(EditorGUIUtility.editingTextField)+" with lastInputEvent="+StringUtils.ToString(lastInputEvent)+" because MouseoveredInspector="+StringUtils.Null);
					#endif
					DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
				}
				// editing text fields is not allowed when multiple controls are selected
				else if(manager.HasMultiSelectedControls)
				{
					#if DEV_MODE && DEBUG_SYNC_EDITING_TEXT_FIELD
					Debug.Log("DrawGUI.EditingTextField = "+StringUtils.False+" with lastInputEvent="+StringUtils.ToString(lastInputEvent)+" because HasMultiSelectedControls="+StringUtils.True);
					#endif
					DrawGUI.EditingTextField = false;
				}
				else
				{
					#if DEV_MODE && DEBUG_SYNC_EDITING_TEXT_FIELD
					Debug.LogWarning("EditorGUIUtility.editingTextField = "+StringUtils.ToColorizedString(DrawGUI.EditingTextField)+" with lastInputEvent="+StringUtils.ToString(lastInputEvent));
					#endif
					EditorGUIUtility.editingTextField = DrawGUI.EditingTextField;
				}
			}
			else if(manager.SelectedInspector == null || !manager.SelectedInspector.InspectorDrawer.HasFocus)
			{
				#if DEV_MODE && DEBUG_SYNC_EDITING_TEXT_FIELD
				Debug.Log("DrawGUI.editingTextField = "+StringUtils.ToColorizedString(EditorGUIUtility.editingTextField)+" with lastInputEvent="+StringUtils.ToString(lastInputEvent)+" because MouseoveredInspector="+StringUtils.Null);
				#endif
				DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
			}
			else
			{
				#if DEV_MODE && DEBUG_SYNC_EDITING_TEXT_FIELD
				Debug.LogWarning("EditorGUIUtility.editingTextField = "+StringUtils.ToColorizedString(DrawGUI.EditingTextField)+" with lastInputEvent="+StringUtils.ToString(lastInputEvent));
				#endif
				EditorGUIUtility.editingTextField = DrawGUI.EditingTextField;
			}
		}
		#endif
	}
}