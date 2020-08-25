//#define DEBUG_SELECT
#define DEBUG_DESTROY

using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Sisus
{
	[InitializeOnLoad]
	public sealed class EditorPlatform : Platform
	{
		private static readonly EditorGUIDrawer GUIDrawer = new EditorGUIDrawer();
		
		public override DrawGUI GUI
		{
			get
			{
				return GUIDrawer;
			}
		}

		public override bool IsPlayingOrWillChangePlaymode
		{
			get
			{
				return EditorApplication.isPlayingOrWillChangePlaymode;
			}
		}

		static EditorPlatform()
		{
			Editor = new EditorPlatform();
		}

		public override string GetPrefs(string key, string defaultValue)
		{
			return EditorPrefs.GetString(key, defaultValue);
		}

		public override void SetPrefs(string key, string value)
		{
			EditorPrefs.SetString(key, value);
		}

		public override void DeletePrefs(string key)
		{
			EditorPrefs.DeleteKey(key);
		}

		public override float GetPrefs(string key, float defaultValue)
		{
			return EditorPrefs.GetFloat(key, defaultValue);
		}

		public override void SetPrefs(string key, float value)
		{
			EditorPrefs.SetFloat(key, value);
		}

		public override bool GetPrefs(string key, bool defaultValue)
		{
			return EditorPrefs.GetBool(key, defaultValue);
		}

		public override void SetPrefs(string key, bool value)
		{
			EditorPrefs.SetBool(key, value);
		}

		public override bool HasPrefs(string key)
		{
			return EditorPrefs.HasKey(key);
		}

		public override Component AddComponent(GameObject gameObject, Type type)
		{
			#if DEV_MODE && DEBUG_ADD_COMPONENT
			Debug.Log("\"" + gameObject.name + "\".AddComponent("+type.Name+")");
			#endif
			
			#if UNITY_2018_1_OR_NEWER
			//ObjectFactory handles Undo registration and applies default values from the project
			return ObjectFactory.AddComponent(gameObject, type);
			#else
			return Undo.AddComponent(gameObject, type);
			#endif
		}

		public override GameObject CreateGameObject(string name)
		{
			#if UNITY_2018_1_OR_NEWER
			//ObjectFactory handles Undo registration and applies default values from the project
			var created = ObjectFactory.CreateGameObject(name);
			#else
			var created = new GameObject(name);
			Undo.RegisterCreatedObjectUndo(created, "CreateGameObject(\""+name+"\")");
			#endif

			if(OnGameObjectCreated != null)
			{
				OnGameObjectCreated(created);
			}

			return created;
		}

		public override Object CreateInstance(Type type)
		{
			#if UNITY_2018_1_OR_NEWER
			//ObjectFactory handles Undo registration and applies default values from the project
			return ObjectFactory.CreateInstance(type);
			#else
			var created = ScriptableObject.CreateInstance(type);
			Undo.RegisterCreatedObjectUndo(created, "CreateInstance("+type.Name+")");
			return created;
			#endif
		}

		public override void SetDirty(Object asset)
		{
			EditorUtility.SetDirty(asset);
		}

		public override void Destroy(Object target)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(target != null);
			Debug.Assert(!Types.EditorWindow.IsAssignableFrom(target.GetType()));
			#endif

			#if DEV_MODE && DEBUG_DESTROY
			Debug.Log("Destroy("+target.GetType().Name+")");
			#endif

			#if DEV_MODE
			if(target is EditorWindow)
			{
				Debug.LogWarning("Destroy was called for EditorWindow target. EditorWindow.Close should be used instead in most instances.");
			}
			#endif

			if(!Application.isPlaying)
			{
				Undo.DestroyObjectImmediate(target);
				return;
			}
			Object.Destroy(target);
		}

		public override void Select(Object target)
		{
			#if DEV_MODE && DEBUG_SELECT
			Debug.Log("Select("+StringUtils.ToString(target)+")");
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(target is Transform) { Debug.LogWarning("Select called for transform, not GameObject. Intentional?"); }
			#endif

			var manager = InspectorUtility.ActiveManager;
			if(manager != null)
			{
				var inspector = manager.ActiveSelectedOrDefaultInspector();
				if(inspector != null)
				{
					inspector.Select(target);
					return;
				}
			}

			Selection.activeObject = target;
		}

		public override void Select(Object[] targets)
		{
			#if DEV_MODE && DEBUG_SELECT
			Debug.Log("Select("+StringUtils.ToString(targets)+")");
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(targets.Length > 0 && targets[0] is Transform) { Debug.LogWarning("Select called for transforms, not GameObjects. Intentional?"); }
			#endif

			var manager = InspectorUtility.ActiveManager;
			if(manager != null)
			{
				var inspector = manager.ActiveSelectedOrDefaultInspector();
				if(inspector != null)
				{
					inspector.Select(targets);
					return;
				}
			}
			Selection.objects = targets;
		}
	}
}