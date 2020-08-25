using System;
using System.Runtime.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus
{
	public sealed class RuntimePlatform : Platform
	{
		public static readonly RuntimeGUIDrawer GUIDrawer = new RuntimeGUIDrawer();

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
				return true;
			}
		}

		public override string GetPrefs(string key, string defaultValue)
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public override void SetPrefs(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public override void DeletePrefs(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}

		public override float GetPrefs(string key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public override void SetPrefs(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public override bool GetPrefs(string key, bool defaultValue)
		{
			return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 0 ? false : true;
		}

		public override void SetPrefs(string key, bool value)
		{
			PlayerPrefs.SetInt(key, value ? 1 : 0);
		}

		public override bool HasPrefs(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public override Component AddComponent(GameObject gameObject, Type type)
		{
			return gameObject.AddComponent(type);
		}

		public override GameObject CreateGameObject(string name)
		{
			var created = new GameObject(name);
			if(OnGameObjectCreated != null)
			{
				OnGameObjectCreated(created);
			}
			return created;
		}

		public override Object CreateInstance(Type type)
		{
			if(type.IsScriptableObject())
			{
				return ScriptableObject.CreateInstance(type);
			}

			try
			{
				#if DEV_MODE
				Debug.Log("Activator.CreateInstance(" + StringUtils.ToString(type)+")");
				#endif
				return Activator.CreateInstance(type) as Object;
			}
			#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
			catch(Exception e) { Debug.LogWarning(e); }
			#else
			catch { }
			#endif

			try
			{
				var constructor = type.GetConstructors()[0];
				var parameters = constructor.GetParameters();
				int count = parameters.Length;
				var parameterValues = ArrayPool<object>.Create(count);
				for(int n = 0; n < count; n++)
				{
					parameterValues[n] = parameters[n].DefaultValue();
					#if DEV_MODE
					Debug.Log(StringUtils.ToString(type)+" parameterValues["+n+"] = " + StringUtils.ToString(parameterValues[n]) +")");
					#endif
				}
				return Activator.CreateInstance(type, parameterValues) as Object;
			}
			#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
			catch(Exception e) { Debug.LogWarning(e); }
			#else
			catch { }
			#endif

			try
			{
				return FormatterServices.GetUninitializedObject(type) as Object;
			}
			#if DEV_MODE && DEBUG_DEFAULT_VALUE_EXCEPTIONS
			catch(Exception e) { Debug.LogWarning(e); }
			#else
			catch { }
			#endif

			throw new Exception("CreateInstance failed for UnityObject of type "+StringUtils.ToString(type));
		}
		
		public override void SetDirty(Object asset)
		{
			#if UNITY_EDITOR
			Editor.SetDirty(asset);
			#endif
		}

		public override void Destroy(Object target)
		{
			#if UNITY_EDITOR
			Editor.Destroy(target);
			#else
			Object.Destroy(target);
			#endif
		}

		public override void Select(Object target)
		{
			var manager = InspectorUtility.ActiveManager;
			if(manager != null)
			{
				var inspector = manager.ActiveSelectedOrDefaultInspector();
				if(inspector != null)
				{
					inspector.Select(target);
				}
			}
		}

		public override void Select(Object[] targets)
		{
			var manager = InspectorUtility.ActiveManager;
			if(manager != null)
			{
				var inspector = manager.ActiveSelectedOrDefaultInspector();
				if(inspector != null)
				{
					inspector.Select(targets);
				}
			}
		}
	}
}