using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus
{
	/// <summary>
	/// The Platform class helps with creating platform-based branching for methods,
	/// reducing the need to use preprocessor directives for platform dependent compilation
	/// and resulting in code that works across different platforms.
	/// 
	/// E.g. if you write the GUI drawing logic of your Custom Editors using the GUI
	/// property of this class, then it will incorporate all the benefits of using the
	/// editor-only EditorGUI class in the editor, while switching to use the runtime-supported
	/// GUI class in builds.
	/// 
	/// To use it, you need to set Platform.Active to refer to the correct plaform once at the
	/// beginning of each method call chain (like the beginning of your OnGUI method), and then
	/// everything else can just use Platform.Active to gain access to the right methods for the
	/// current platform.
	/// </summary>
	public abstract class Platform
	{
		// not using a direct readonly assignment of EditorPlatform so that
		// it can be located inside an Editor folder
		public static Platform Editor;
		public static readonly RuntimePlatform Runtime = new RuntimePlatform();

		public static Action<GameObject> OnGameObjectCreated;

		/// <summary>
		/// Should be set to refer to Editor or Runtime at the beginning of
		/// each method call chain (like the beginning of your OnGUI method).
		/// </summary>
		public static Platform Active;

		public abstract DrawGUI GUI { get; }

		public abstract bool IsPlayingOrWillChangePlaymode { get; }

		public abstract string GetPrefs(string key, string defaultValue);
		public abstract void SetPrefs(string key, string value);
		public abstract void DeletePrefs(string key);

		public abstract float GetPrefs(string key, float defaultValue);

		public void SetPrefs(string key, int value, int defaultValue)
		{
			if(value.Equals(defaultValue))
			{
				DeletePrefs(key);
			}
			else
			{
				SetPrefs(key, value);
			}
		}

		public abstract void SetPrefs(string key, float value);

		public void SetPrefs(string key, float value, float defaultValue)
		{
			if(value.Equals(defaultValue))
			{
				DeletePrefs(key);
			}
			else
			{
				SetPrefs(key, value);
			}
		}

		public abstract bool GetPrefs(string key, bool defaultValue);
		public abstract void SetPrefs(string key, bool value);

		public void SetPrefs(string key, bool value, bool defaultValue)
		{
			if(value == defaultValue)
			{
				DeletePrefs(key);
			}
			else
			{
				SetPrefs(key, value);
			}
		}

		public abstract bool HasPrefs(string key);

		public abstract Component AddComponent([NotNull]GameObject target, [NotNull]Type type);
		public abstract GameObject CreateGameObject(string name);
		public abstract Object CreateInstance([NotNull]Type type);

		public abstract void SetDirty([NotNull]Object asset);
		public abstract void Destroy([NotNull]Object target);

		public abstract void Select([CanBeNull]Object target);
		public abstract void Select([NotNull]Object[] targets);
		
		#if UNITY_EDITOR
		public static void SetEditorMode()
		{
			Active = Editor;
		}
		#endif

		public static void SetRuntimeMode()
		{
			Active = Runtime;
		}

		public static bool EditorMode
		{
			get
			{
				#if UNITY_EDITOR
				return Active != Runtime;
				#else
				return false;
				#endif
			}
		}

		public static float Time
		{
			get
			{
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					return (float)UnityEditor.EditorApplication.timeSinceStartup;
				}
				#endif
				return UnityEngine.Time.realtimeSinceStartup;
			}
		}

		public static bool IsCompiling
		{
			get
			{
				#if UNITY_EDITOR
				return UnityEditor.EditorApplication.isCompiling;
				#else
				return false;
				#endif
			}
		}
	}
}