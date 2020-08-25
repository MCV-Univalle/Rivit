//#define DEBUG_SET_HAS_FOCUS
#define DEBUG_SET_IS_QUITTING

using System;
using UnityEngine;
using JetBrains.Annotations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	/// <summary>
	/// Class that tracks current cursor position and broadcasts OnPositionChanged
	///	event whenever it changes. It can also be polled for the current cursor position
	///	in local GUILayout space as well as screen space.
	///	
	///	Relies on DrawGUI.OnBeginOnGUI which only gets called if there's at least one
	///	class that has an OnGUI method and calls DrawGUI.BeginOnGUI at the start of said method.
	/// </summary>
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public static class ApplicationUtility
	{
		public static Action OnApplicationQuitting;
		public static Action OnApplicationGainedFocus;
		public static Action OnApplicationLostFocus;

		private static bool initialized = false;

		private static bool hasFocus = true;
		private static bool isQuitting = false;

		#if ODIN_INSPECTOR && UNITY_EDITOR
		private static bool OdinEditorIsReady;
		private static double OdinEditorIsReadyTime;
		#endif

		#if !UNITY_EDITOR || !UNITY_2018_1_OR_NEWER
		private static MonoBehaviourEventCatcher eventCatcher;
		#endif

		public static bool IsQuitting
		{
			get
			{
				return isQuitting;
			}

			private set
			{
				if(isQuitting != value)
				{
					#if DEV_MODE && DEBUG_SET_IS_QUITTING
					Debug.Log("ApplicationUtility.HasFocus = "+StringUtils.ToColorizedString(value));
					#endif

					isQuitting = value;

					if(value)
					{
						if(OnApplicationQuitting != null)
						{
							OnApplicationQuitting();
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether Application currently has focus.
		/// </summary>
		/// <value> True if Application is focused, false if not. </value>
		public static bool HasFocus
		{
			get
			{
				return hasFocus;
			}

			private set
			{
				if(hasFocus != value)
				{
					#if DEV_MODE && DEBUG_SET_HAS_FOCUS
					Debug.Log("ApplicationUtility.HasFocus = "+StringUtils.ToColorizedString(value));
					#endif

					hasFocus = value;

					if(hasFocus)
					{
						if(OnApplicationGainedFocus != null)
						{
							OnApplicationGainedFocus();
						}
					}
					else if(OnApplicationLostFocus != null)
					{
						OnApplicationLostFocus();
					}
				}
			}
		}
		
		#if UNITY_EDITOR
		/// <summary>
		/// This is called on editor load because of usage of the InitializeOnLoad attribute.
		/// </summary>
		[UsedImplicitly]
		static ApplicationUtility()
		{
			EditorApplication.delayCall += Initialize;
		}
		#endif

		/// <summary>
		/// This is called when entering play mode or when the game is loaded.
		/// </summary>
		[RuntimeInitializeOnLoadMethod, UsedImplicitly]
		private static void RuntimeInitializeOnLoad()
		{
			Initialize();
		}

		private static void Initialize()
		{
			#if UNITY_EDITOR
			if(EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += Initialize;
				return;
			}
			#endif

			isQuitting = false;

			if(initialized)
			{
				return;
			}
			initialized = true;
			
			#if UNITY_EDITOR
			#if UNITY_2018_1_OR_NEWER
			EditorApplication.quitting += SetApplicationIsQuitting;
			#endif
			#else
			eventCatcher = InvisibleComponentProvider.GetOrCreate<MonoBehaviourEventCatcher>("ApplicationUtility/MonoBehaviourEventCatcher");
			eventCatcher.OnApplicationQuitting += SetApplicationIsQuitting;
			#endif
			
			#if UNITY_EDITOR
			EditorApplication.update += UpdateApplicationHasFocus;
			#else
			eventCatcher.OnApplicationLostFocus += HandleOnApplicationLostFocus;
			eventCatcher.OnApplicationGainedFocus += HandleOnApplicationGainedFocus;
			#endif
		}

		#if !UNITY_EDITOR || UNITY_2018_1_OR_NEWER
		private static void SetApplicationIsQuitting()
		{
			IsQuitting = true;
		}
		#endif
		
		#if UNITY_EDITOR
		private static void UpdateApplicationHasFocus()
		{
			HasFocus = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
		}
		#else
		private static void HandleOnApplicationLostFocus()
		{
			HasFocus = false;
		}

		private static void HandleOnApplicationGainedFocus()
		{
			HasFocus = true;
		}
		#endif

		/// <summary>
		/// Determines whether or not Application is now fully loaded and in a state where all main systems are ready to be used.
		/// 
		/// Returns false if application is compiling, making a build, changing play mode state or quitting.
		/// </summary>
		/// <returns> True if application is fully ready. </returns>
		public static bool IsReady()
		{
			#if UNITY_EDITOR
			if(EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer || PlayMode.NowChangingState)
			{
				return false;
			}
			#endif
			
			#if ODIN_INSPECTOR && UNITY_EDITOR
			// Fix for issue where using Editor.CreateEditor immediately after switching to play mode would not return the OdinEditor when expected.
			if(!OdinEditorIsReady)
			{
				// the issue only seems to occur when entering play mode
				if(Application.isPlaying)
				{
					if(OdinEditorIsReadyTime <= 0d)
					{
						OdinEditorIsReadyTime = EditorApplication.timeSinceStartup + 0.5d;
						return false;
					}
					else if(EditorApplication.timeSinceStartup < OdinEditorIsReadyTime)
					{
						return false;
					}
				}
				OdinEditorIsReady = true;
			}
			#endif

			return !IsQuitting;
		}
	}
}