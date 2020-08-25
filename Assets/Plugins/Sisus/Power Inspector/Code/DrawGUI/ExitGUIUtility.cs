#define DEBUG_ENABLED

using System;
using System.Reflection;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Unity has method GUIUtility.ExitGUI which intentionally causes an ExitGUIException to be thrown when called.
	/// This is used to break out of the current OnGUI event immediately after something happens that could change
	/// the layout of elements in the GUI, which can avoid warnings about changing the layout in the middle of an OnGUI call.
	/// 
	/// This utility helps detect when an exception is an ExitGUIException and thus should be rethrown.
	/// </summary>
	public static class ExitGUIUtility
	{
		/// <summary> Determine if we should rethrow given exception that was caught in a try-catch. </summary>
		/// <param name="exception"> The caught exception. </param>
		/// <returns> True if should rethrow, false if no. </returns>
		public static bool ShouldRethrowException(Exception exception)
		{
			return IsExitGUIException(exception);
		}

		// <summary> Determine if given exception or its inner exception is an ExitGUIException. </summary>
		/// <param name="exception"> An exception. </param>
		/// <returns> True if exception or its inner exception is ExitGUIException, otherwise false. </returns>
		public static bool IsExitGUIException(Exception exception)
		{
			while (exception is TargetInvocationException && exception.InnerException != null)
			{
				exception = exception.InnerException;
			}

			#if DEV_MODE && DEBUG_ENABLED
			if(exception is ExitGUIException) { Debug.Log("IsExitGUIException("+StringUtils.ToString(exception)+"): "+StringUtils.True); }
			#endif

			return exception is ExitGUIException;
		}

		public static void ExitGUI()
		{
			if(Event.current == null)
			{
				#if DEV_MODE && DEBUG_ENABLED
				Debug.LogWarning("Ignoring ExitGUI call because Event.current was null.");
				#endif
				return;
			}

			GUI.changed = true;
			if(InspectorUtility.ActiveInspectorDrawer != null)
			{
				InspectorUtility.ActiveInspectorDrawer.Repaint();

				if(InspectorUtility.ActiveInspector != null)
				{
					InspectorUtility.ActiveInspector.NowDrawingPart = InspectorPart.None;
				}
			}

			#if DEV_MODE && DEBUG_ENABLED
			Debug.LogWarning("Calling GUIUtility.ExitGUI...");
			#endif

			GUIUtility.ExitGUI();
		}
	}
}