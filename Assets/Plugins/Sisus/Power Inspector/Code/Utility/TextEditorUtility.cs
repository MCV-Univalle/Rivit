using System;
using System.Reflection;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Utility class for things related to the TextEditor currently active in the Editor.
	/// Utilizes Reflection to call internal methods in Unity's EditorGUI class.
	/// </summary>
	public static class TextEditorUtility
	{
		#if UNITY_EDITOR
		private static Type editorGUIType = typeof(UnityEditor.EditorGUI);
		private static FieldInfo recycledEditorField;
		private static bool setupDone;
		#endif

		#if UNITY_EDITOR
		private static void Setup()
		{
			recycledEditorField = editorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
			setupDone = true;
		}
		#endif
		
		public static TextEditor GetCurrentTextEditor()
		{
			#if UNITY_EDITOR
			if(!setupDone)
			{
				Setup();
			}
			return recycledEditorField.GetValue(null) as TextEditor;
			#else
			return GUIUtility.GetStateObject(typeof(TextEditor), KeyboardControlUtility.KeyboardControl) as TextEditor;
			#endif
		}

		public static void MoveCursorToTextEditorEnd()
		{
			var textEditor = GetCurrentTextEditor();
			if(textEditor != null)
			{
				textEditor.SelectNone();
				textEditor.MoveTextEnd();
				#if DEV_MODE
				Debug.Log("TextEditorUtility.MoveCursorToTextEditorEnd" + "\ntext ="+textEditor.text+ "\ncursorIndex="+ textEditor.cursorIndex+", selectIndex="+textEditor.selectIndex);
				#endif
			}
		}

		public static void SetText(string text)
		{
			var textEditor = GetCurrentTextEditor();
			if(textEditor != null)
			{
				textEditor.text = text;
				#if DEV_MODE
				Debug.Log("TextEditorUtility.SetText("+ text + ")\ntextEditor.text=" + textEditor.text+ "\ncursorIndex="+ textEditor.cursorIndex+", selectIndex="+textEditor.selectIndex+", DrawGUI.EditingTextField="+ DrawGUI.EditingTextField);
				#endif
			}
		}

		public static void SelectAllText()
		{
			#if DEV_MODE && PI_ASSERTATIONS && UNITY_EDITOR
			bool editingTextFieldWas = UnityEditor.EditorGUIUtility.editingTextField;
			#endif

			var textEditor = GetCurrentTextEditor();
			if(textEditor != null)
			{
				textEditor.SelectAll();
				#if DEV_MODE
				Debug.Log("TextEditorUtility.SelectAllText" + "\ntext="+textEditor.text+ "\ncursorIndex="+ textEditor.cursorIndex+", selectIndex="+textEditor.selectIndex+", DrawGUI.EditingTextField="+ DrawGUI.EditingTextField);
				#endif
			}
			#if DEV_MODE
			else { Debug.Log("TextEditorUtility.SelectAllText - Can't select all text because current TextEditor was null."); }
			#endif

			#if DEV_MODE && PI_ASSERTATIONS && UNITY_EDITOR
			Debug.Assert(editingTextFieldWas == UnityEditor.EditorGUIUtility.editingTextField);
			#endif
		}

		public static bool SetTextLength(int length)
		{
			#if DEV_MODE && PI_ASSERTATIONS && UNITY_EDITOR
			bool editingTextFieldWas = UnityEditor.EditorGUIUtility.editingTextField;
			#endif

			var textEditor = GetCurrentTextEditor();
			if(textEditor != null && textEditor.text.Length > length)
			{
				textEditor.text = textEditor.text.Substring(0, length);
				#if DEV_MODE
				Debug.Log("TextEditorUtility.SetTextLength(1)" + "\ntext =" + textEditor.text + "\ncursorIndex=" + textEditor.cursorIndex + ", selectIndex=" + textEditor.selectIndex);
				#endif

				#if DEV_MODE && PI_ASSERTATIONS && UNITY_EDITOR
				Debug.Assert(editingTextFieldWas == UnityEditor.EditorGUIUtility.editingTextField);
				#endif

				return true;
			}
			#if DEV_MODE
			else if(textEditor == null) { Debug.Log("TextEditorUtility.SetTextLength - Can't set text length because TextEditor was null."); }
			#endif

			#if DEV_MODE && PI_ASSERTATIONS && UNITY_EDITOR
			Debug.Assert(editingTextFieldWas == UnityEditor.EditorGUIUtility.editingTextField);
			#endif

			return false;
		}

		public static bool Insert(char character)
		{
			var textEditor = GetCurrentTextEditor();
			if(textEditor != null)
			{
				textEditor.Insert(character);
				#if DEV_MODE
				Debug.Log("TextEditorUtility.Insert("+StringUtils.ToString(character)+")" + "\ntext =" + textEditor.text + "\ncursorIndex=" + textEditor.cursorIndex + ", selectIndex=" + textEditor.selectIndex);
				#endif
				return true;
			}
			return false;
		}
	}
}