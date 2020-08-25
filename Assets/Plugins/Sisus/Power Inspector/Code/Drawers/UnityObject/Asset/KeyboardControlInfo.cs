#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0
using Sisus.Vexe.FastReflection;
#endif

namespace Sisus
{
	/// <summary>
	/// Editor-only class that uses reflection to access internal data in some built-in classes.
	/// </summary>
	public class KeyboardControlInfo
	{
		private MethodInfo getKeyboardRect;
		private object[] GetKeyboardRectParams = {0, default(Rect)};
		
		#if (UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0
		private MethodCaller<object, object> canHaveKeyboardFocus;
		#else
		private MethodInfo canHaveKeyboardFocus;
		#endif

		public KeyboardControlInfo()
		{
			var editorGUIUtilityType = typeof(EditorGUIUtility);
			getKeyboardRect = editorGUIUtilityType.GetMethod("Internal_GetKeyboardRect", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);
			#if (UNITY_EDITOR || UNITY_STANDALONE) && !NET_STANDARD_2_0
			canHaveKeyboardFocus = editorGUIUtilityType.GetMethod("CanHaveKeyboardFocus", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static).DelegateForCall();
			#else
			canHaveKeyboardFocus = editorGUIUtilityType.GetMethod("CanHaveKeyboardFocus", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);
			#endif
		}

		/// <summary>
		/// Gets Rect describing the position and size of the inspector control that currently has keyboard focus
		/// </summary>
		/// <value>
		/// Rect (position and size)
		/// </value>
		public Rect KeyboardRect
		{
			get
			{
				GetKeyboardRectParams[0] = GUIUtility.keyboardControl;
				
				getKeyboardRect.Invoke(null, GetKeyboardRectParams);
				//second parameter has the out modifier
				return (Rect)GetKeyboardRectParams[1];
			}
		}
		
		public bool CanHaveKeyboardFocus(int id)
		{
			return (bool)canHaveKeyboardFocus.InvokeWithParameter(null, id);
		}
	}
}
#endif