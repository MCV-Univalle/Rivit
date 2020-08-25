//#define IGNORE_GAMEOBJECT_EDITOR_OVERRIDES

#define DESTROY_DISPOSED_EDITORS
//#define SKIP_DESTROY_GAME_OBJECT_INSPECTOR
#define DESTROY_DISPOSED_EDITOR_SERIALIZED_OBJECTS
//#define NEVER_CACHE_ANY_EDITORS

//#define DEBUG_SERIALIZE
#define DEBUG_GET_EDITOR
//#define DEBUG_DESTROYED_CACHED_EDITORS
//#define DEBUG_DISPOSE
//#define DEBUG_CLEAR
//#define DEBUG_CLEAN_UP

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

#if !NET_STANDARD_2_0
using Sisus.Vexe.FastReflection;
#endif

namespace Sisus
{
	/// <summary>
	/// Class for creating and caching Editors for targets.
	/// Instance and cached will persists through assembly reloads.
	/// </summary>
	public class Editors : IBinarySerializable
	{
		private static Editors instance;

		#if SKIP_DESTROY_GAME_OBJECT_INSPECTOR
		private static Type gameObjectInspectorType;
		private static Type prefabImporterEditorType;
		#elif IGNORE_GAMEOBJECT_EDITOR_OVERRIDES || DEV_MODE
		private static Type gameObjectInspectorType;
		#endif

		#if !NET_STANDARD_2_0
		private MemberGetter<Editor, Object[]> getTargets;
		private MemberGetter<Editor, Object> getContext;
		#else
		private FieldInfo getTargets;
		private FieldInfo getContext;
		#endif

		private Dictionary<EditorKey, Editor> cachedEditors;

		private Dictionary<EditorKey, Editor> cachedEditorsCleaned;
		private List<KeyValuePair<EditorKey, Editor>> cachedEditorsToDispose;
		
		int? IBinarySerializable.DeserializationOrder
		{
			get
			{
				return PersistentSingletonSerialized.DefaultDeserializationOrder;
			}
		}

		public static Editors Instance()
		{
			if(instance == null)
			{
				instance = PersistentSingleton.Get<Editors>();
				if(instance != null)
				{
					instance.Setup(0);
				}
				#if DEV_MODE
				else if(!ApplicationUtility.IsQuitting) { Debug.LogWarning("Editors.Instance() returning null!"); }
				#endif
			}
			return instance;
		}
		
		private void Setup(int count)
		{
			BuildFields();
			cachedEditors = new Dictionary<EditorKey, Editor>(count);
			cachedEditorsCleaned = new Dictionary<EditorKey, Editor>(count);
			cachedEditorsToDispose = new List<KeyValuePair<EditorKey, Editor>>();
		}

		private static EditorKey GetKey(Editor editor)
		{
			#if UNITY_2018_1_OR_NEWER
			return new EditorKey(editor.targets, editor is UnityEditor.Experimental.AssetImporters.AssetImporterEditor);
			#else
			return new EditorKey(editor.targets, false);
			#endif
		}

		public static bool IsCached(Editor editor)
		{
			return IsCached(GetKey(editor));
		}

		public static bool IsCached(Editor editor, out EditorKey key)
		{
			key = GetKey(editor);
			return IsCached(key);
		}

		public static bool IsCached(EditorKey key)
		{
			return Instance().cachedEditors.ContainsKey(key);
		}

		public static bool RemoveFromCache(Editor editor)
		{
			return Instance().cachedEditors.Remove(GetKey(editor));
		}

		public static bool RemoveFromCache(EditorKey key)
		{
			return Instance().cachedEditors.Remove(key);
		}

		/// <summary> Gets an Editor for targets. </summary>
		/// <param name="editor"> [in,out] This will be updated to contain the Editor. </param>
		/// <param name="targets"> The targets for the editor. </param>
		/// <param name="editorType"> (Optional) Type of the editor. </param>
		/// <param name="context"> (Optional) SerializedObject will be created using this if not null. </param>
		/// <param name="cache">
		/// (Optional) True if should cache Editor for later reuse. If false existing editor instance will be Disposed if a new one is created.
		/// </param>
		public static void GetEditor(ref Editor editor, Object[] targets, Type editorType = null, Object context = null, bool cache = true)
		{
			GetEditor(ref editor, targets, editorType, targets.AllSameType(), context, cache);
		}

		/// <summary> Gets an Editor for targets. </summary>
		/// <param name="editor"> [in,out] This will be updated to contain the Editor. </param>
		/// <param name="targets"> The targets for the editor. </param>
		/// <param name="editorType"> (Optional) Type of the editor. </param>
		/// <param name="allTargetsHaveSameType">
		/// True if all targets are of the same type. If false, Editor will be created using only the first target.
		/// </param>
		/// <param name="context"> (Optional) SerializedObject will be created using this if not null. </param>
		/// <param name="cache">
		/// (Optional) True if should cache Editor for later reuse. If false existing editor instance will be Disposed if a new one is created.
		/// </param>
		public static void GetEditor(ref Editor editor, [NotNull]Object[] targets, [CanBeNull]Type editorType, bool allTargetsHaveSameType, Object context = null, bool cache = true)
		{
			Instance().GetEditorInternal(ref editor, targets, editorType, allTargetsHaveSameType, context, cache);
		}

		/// <inheritdoc cref="GetEditor(ref Editor, Object[], Type, bool, Object, bool)"/>
		private void GetEditorInternal(ref Editor editor, [NotNull]Object[] targets, [CanBeNull]Type editorType, bool allTargetsHaveSameType, Object context = null, bool cache = true)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(targets != null);
			Debug.Assert(!targets.ContainsNullObjects(), "Editors.GetEditorInternal called with tagets containing null Objects");
			Debug.Assert(allTargetsHaveSameType == targets.AllSameType());
			Debug.Assert(targets.Length > 0, "GetEditor called with empty targets array! editorType was "+(editorType == null ? "null" : editorType.Name));
			#endif

			if(editor != null)
			{
				#if !NET_STANDARD_2_0
				var previousEditorTargets = getTargets(editor);
				var previousEditorContext = getContext(editor);
				#else
				var previousEditorTargets = getTargets.GetValue(editor) as Object[];
				var previousEditorContext = getContext.GetValue(editor) as Object;
				#endif
				if(targets.ContentsMatch(previousEditorTargets) && context == previousEditorContext)
				{
					return;
				}

				Dispose(ref editor);
			}

			if(!allTargetsHaveSameType)
			{
				GetEditor(ref editor, targets[0], editorType, context, cache);
				return;
			}
			
			#if UNITY_2017_2_OR_NEWER
			bool isAssetImporterEditor = editorType != null && Types.AssetImporterEditor.IsAssignableFrom(editorType);
			var editorKey = new EditorKey(targets, isAssetImporterEditor);
			#else
			var editorKey = new EditorKey(targets, false);
			#endif

			if(cachedEditors.TryGetValue(editorKey, out editor))
			{
				if(editor != null)
				{
					if(!DisposeIfInvalid(ref editor))
					{
						OnBecameActive(editor);

						#if DEV_MODE && DEBUG_GET_EDITOR
						Debug.Log("Editors.GetEditor: for targets " + StringUtils.TypesToString(targets) + " and editorType "+StringUtils.ToString(editorType)+" returning cached: "+editor.GetType().Name+" with key="+editorKey.GetHashCode());
						#endif
						return;
					}
					#if DEV_MODE && DEBUG_DESTROYED_CACHED_EDITORS
					Debug.LogWarning("cachedEditors for targets "+StringUtils.TypeToString(targets)+ " and editorType " + StringUtils.ToString(editorType)+ " with EditorKey hashCode " + editorKey.GetHashCode()+ " contained editor with null targets!\nCachedEditors:\n" + StringUtils.ToString(cachedEditors, "\n"));
					#endif
				}
				#if DEV_MODE && DEBUG_DESTROYED_CACHED_EDITORS
				else { Debug.LogWarning("cachedEditors for targets "+StringUtils.TypesToString(targets) +" and editorType "+StringUtils.ToString(editorType)+ " with EditorKey hashCode " + editorKey.GetHashCode() + " contained a null value!\nCachedEditors:\n" + StringUtils.ToString(cachedEditors, "\n")); }
				#endif
			}

			#if DEV_MODE && DEBUG_GET_EDITOR
			Debug.Log(StringUtils.ToColorizedString("Editors.GetEditor called for ", StringUtils.ToString(targets), " with editorType=", editorType, ", context=", context, ", key=", editorKey.GetHashCode(), ", cache=", cache));
			#endif

			try
			{
				editor = Editor.CreateEditorWithContext(targets, context, editorType);
			}
			#if DEV_MODE
			catch(Exception e)
			{
				Debug.LogError("Editor.CreateEditor for targets " + StringUtils.TypesToString(targets) + " and editorType "+StringUtils.ToString(editorType)+": "+e);
			#else
			catch
			{
			#endif
				return;
			}

			if(editor == null)
			{
				#if DEV_MODE
				Debug.LogWarning("Editor.CreateEditor for targets " + StringUtils.TypesToString(targets) + " and editorType "+StringUtils.ToString(editorType)+" returned null!");
				#endif
				return;
			}
			
			#if DEV_MODE && DEBUG_GET_EDITOR
			Debug.Log("Editors.GetEditor: Created new: "+editor.GetType().Name+" for "+StringUtils.ToString(targets)+" with key="+editorKey.GetHashCode()+", cache="+StringUtils.ToColorizedString(cache));
			#endif

			if(cache)
			{
				cachedEditors[editorKey] = editor;
			}
		}

		public static void OnBecameActive(Editor editor)
		{
			editor.ReloadPreviewInstances();
		}

		private void BuildFields()
		{
			#if !NET_STANDARD_2_0
			getTargets = Types.Editor.GetField("m_Targets", BindingFlags.Instance | BindingFlags.NonPublic).DelegateForGet<Editor, Object[]>();
			getContext = Types.Editor.GetField("m_Context", BindingFlags.Instance | BindingFlags.NonPublic).DelegateForGet<Editor, Object>();
			#else
			getTargets = Types.Editor.GetField("m_Targets", BindingFlags.Instance | BindingFlags.NonPublic);
			getContext = Types.Editor.GetField("m_Context", BindingFlags.Instance | BindingFlags.NonPublic);
			#endif

			#if SKIP_DESTROY_GAME_OBJECT_INSPECTOR
			gameObjectInspectorType = Types.GetInternalEditorType("UnityEditor.GameObjectInspector");
			prefabImporterEditorType = Types.GetInternalEditorType("UnityEditor.PrefabImporterEditor");
			#elif IGNORE_GAMEOBJECT_EDITOR_OVERRIDES || DEV_MODE
			gameObjectInspectorType = Types.GetInternalEditorType("UnityEditor.GameObjectInspector");
			#endif
		}

		/// <summary> Gets an Editor for target. </summary>
		/// <param name="editor"> [in,out] This will be updated to contain the Editor. </param>
		/// <param name="target"> The target for the editor. </param>
		/// <param name="editorType"> (Optional) Type of the editor. </param>
		/// <param name="context"> (Optional) SerializedObject will be created using this if not null. </param>
		/// <param name="cache">
		/// (Optional) True if should cache Editor for later reuse. If false existing editor instance will be Disposed if a new one is created.
		/// </param>
		public static void GetEditor(ref Editor editor, [NotNull]Object target, Type editorType = null, Object context = null, bool cache = true)
		{
			Instance().GetEditorInternal(ref editor, target, editorType, context, cache);
		}

		/// <inheritdoc cref="GetEditor(ref Editor, Object, Type, Object, bool)"/>
		private void GetEditorInternal(ref Editor editor, [NotNull]Object target, Type editorType = null, Object context = null, bool cache = true)
		{
			if(editor != null)
			{
				#if !NET_STANDARD_2_0
				var editorTargets = getTargets(editor);
				if(editorTargets.Length == 1 && editorTargets[0] == target && context == getContext(editor))
				#else
				var editorTargets = getTargets.GetValue(editor) as Object[];
				if(editorTargets.Length == 1 && editorTargets[0] == target && context == getContext.GetValue(editor) as Object)
				#endif
				{
					return;
				}

				Dispose(ref editor);
			}
			
			#if UNITY_2017_2_OR_NEWER
			bool isAssetImporterEditor = editorType != null && Types.AssetImporterEditor.IsAssignableFrom(editorType);
			var editorKey = new EditorKey(target, isAssetImporterEditor);
			#else
			var editorKey = new EditorKey(target, false);
			#endif
			
			if(cachedEditors.TryGetValue(editorKey, out editor))
			{
				if(editor != null)
				{
					if(!DisposeIfInvalid(ref editor))
					{
						OnBecameActive(editor);

						#if DEV_MODE && DEBUG_GET_EDITOR
						Debug.Log("Editors.GetEditor: returning cached: "+editor.GetType().Name+" for "+StringUtils.ToString(target)+" with key="+editorKey.GetHashCode());
						#endif
						return;
					}
					#if DEV_MODE && DEBUG_DESTROYED_CACHED_EDITORS
					Debug.LogWarning("cachedEditors for target "+StringUtils.TypeToString(target)+ " and editorType " + StringUtils.ToString(editorType)+ " with EditorKey hashCode " + editorKey.GetHashCode()+ " contained editor with null targets!\nCachedEditors:\n" + StringUtils.ToString(cachedEditors, "\n"));
					#endif
				}
				#if DEV_MODE && DEBUG_DESTROYED_CACHED_EDITORS
				else { Debug.LogWarning("cachedEditors for target "+StringUtils.TypeToString(target)+ " and editorType " + StringUtils.ToString(editorType)+ " with EditorKey hashCode " + editorKey.GetHashCode()+ " contained a null value!\nCachedEditors:\n" + StringUtils.ToString(cachedEditors, "\n")); }
				#endif
			}

			#if DEV_MODE || IGNORE_GAMEOBJECT_EDITOR_OVERRIDES
			if(editorType == null)
			{
				if(target is GameObject)
				{
					editorType = gameObjectInspectorType;
				}
			}
			#endif

			editor = Editor.CreateEditorWithContext(ArrayPool<Object>.CreateWithContent(target), context, editorType);

			if(editor == null)
			{
				#if DEV_MODE
				Debug.LogWarning("Editor.CreateEditor for target " + StringUtils.TypeToString(target) + " and editorType "+StringUtils.ToString(editorType)+" returned null!");
				#endif
				return;
			}

			#if DEV_MODE && DEBUG_GET_EDITOR
			Debug.Log("Editors.GetEditor: Created new: "+editor.GetType().Name+" for "+StringUtils.ToString(target)+" with key="+editorKey.GetHashCode());
			#endif
			
			if(cache)
			{
				cachedEditors[editorKey] = editor;
			}
		}

		/// <summary>
		/// Disposes the SerializedObject of the Editor and destroys the Editor.
		/// </summary>
		/// <param name="editor"> [in,out] The editor to Dispose. This should not be null when the method is called. </param>
		public static void Dispose(ref Editor editor)
		{
			if(instance == null)
			{
				DisposeStatic(ref editor);
				return;
			}
			instance.Dispose(ref editor, GetKey(editor));
		}

		/// <summary>
		/// Disposes the SerializedObject of the Editor and destroys the Editor.
		/// </summary>
		/// <param name="editor"> [in,out] The editor to Dispose. This should not be null when the method is called. </param>
		/// <param name="key"> Dictionary key for the editor </param>
		private void Dispose(ref Editor editor, EditorKey key)
		{
			#if DEV_MODE
			Debug.Assert(!ReferenceEquals(editor, null), "Dispose called for null editor where ReferenceEquals(editor, "+StringUtils.Null+")="+StringUtils.True);
			#endif

			if(IsCached(key))
			{
				#if !NEVER_CACHE_ANY_EDITORS
				if(!EditorApplication.isCompiling && Validate(editor))
				{
					#if DEV_MODE && DEBUG_DISPOSE
					Debug.Log("Editors.Dispose - <color=green>Keeping</color> cached Editor "+StringUtils.TypeToString(editor)+" of "+StringUtils.TypeToString(editor.target)+" ("+ StringUtils.ToString(editor.target)+") with key="+key.GetHashCode());
					#endif
					editor = null;
					return;
				}
				#endif

				#if DEV_MODE && DEBUG_DISPOSE
				Debug.Log("Editors.Dispose - <color=red>Removing</color> cached Editor "+StringUtils.TypeToString(editor)+" of "+StringUtils.TypeToString(editor.target)+" ("+ StringUtils.ToString(editor.target)+") with key="+key.GetHashCode());
				#endif

				RemoveFromCache(key);
			}
			#if DEV_MODE && DEBUG_DISPOSE
			else { Debug.Log("Editors.Dispose - IsCached("+StringUtils.False+"): Editor "+StringUtils.TypeToString(editor)+" of "+StringUtils.TypeToString(editor.target)+" ("+ StringUtils.ToString(editor.target)+") with key="+key.GetHashCode()+"\ncachedEditors:\n"+StringUtils.ToString(cachedEditors, "\n")); }
			#endif
			
			DisposeStatic(ref editor);
		}

		/// <summary>
		/// Disposes the SerializedObject of the Editor and destroys the Editor.
		/// Won't create a new instance of Editors even if it's missing, and won't
		/// remove editor from cache of existing Editors instance.
		/// </summary>
		/// <param name="editor"> [in,out] The editor to Dispose. This should not be null when the method is called. </param>
		private static void DisposeStatic(ref Editor editor)
		{
			#if DEV_MODE
			Debug.Assert(!ReferenceEquals(editor, null), "Dispose called for null editor where ReferenceEquals(editor, "+StringUtils.Null+")="+StringUtils.True);
			#endif
			
			#if SKIP_DESTROY_GAME_OBJECT_INSPECTOR
			// Check that field m_PreviewCache of GameObjectInspector is not null. If it is, and Destroy is called for an Editor,
			// a NullReferenceException will get thrown. I'm guessing that the field goes null after assembly reloading, because
			// Unity can't serialize Dictionary fields.
			var editorType = editor.GetType();
			if(editorType == gameObjectInspectorType || editorType == prefabImporterEditorType)
			{
				editor = null;
				return;
			}
			#endif

			#if DEV_MODE && DEBUG_DISPOSE
			Debug.Log("Editors.Dispose - <color=red>Destroying</color> Editor "+StringUtils.TypeToString(editor)+" of "+StringUtils.TypeToString(editor.target)+" ("+ StringUtils.ToString(editor.target)+")");
			#endif
			
			var target = editor.target;

			#if DESTROY_DISPOSED_EDITOR_SERIALIZED_OBJECTS
			SerializedObject serializedObject;
			try
			{
				serializedObject = target == null ? null : editor.serializedObject;
			}
			// ArgumentException: Object at index 0 is null can happen when we try to fetch
			// the SerializedObject of an editor target which no longer exists
			#if DEV_MODE
			catch(ArgumentException e)
			{
				Debug.LogError(e);
				serializedObject = null;
			}
			#else
			catch(ArgumentException)
			{
				serializedObject = null;
			}
			#endif

			#endif

			#if DESTROY_DISPOSED_EDITORS
			try
			{
				Object.DestroyImmediate(editor);
			}
			#if DEV_MODE
			catch(NullReferenceException e) // this has happened in rare cases somehow
			{
				Debug.LogError(e);
			#else
			catch(NullReferenceException) // this has happened in rare cases somehow
			{
			#endif
				editor = null;
				return;
			}
			#endif
			
			editor = null;

			// UPDATE: handle nested Editors!
			var nestedEditor = target as Editor;
			if(nestedEditor != null)
			{
				#if DEV_MODE && DEBUG_DISPOSE
				Debug.Log("Editors.Dispose - <color=red>Destroying</color> nestedEditor "+StringUtils.TypeToString(editor));
				#endif
				Dispose(ref nestedEditor);
			}

			#if DESTROY_DISPOSED_EDITOR_SERIALIZED_OBJECTS
			// Destroy the SerializedObject after the Editor because it's
			// possible that OnDisable / OnDestroy methods have references
			// to the serializedObject.
			if(serializedObject != null)
			{
				serializedObject.Dispose();
			}
			#endif
		}

		public static void Clear()
		{
			Clear(Instance().cachedEditors);
		}

		private static void Clear(Dictionary<EditorKey, Editor> cachedEditors)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(cachedEditors != null);
			#endif

			foreach(var editor in cachedEditors.Values)
			{
				if(editor == null)
				{
					#if DEV_MODE && DEBUG_CLEAR
					Debug.LogWarning("Editors.Clear - skipping null Editor");
					#endif
					continue;
				}
				var dispose = editor;
				#if DEV_MODE && DEBUG_CLEAR
				Debug.LogWarning("Editors.Clear - Disposing Editor "+editor.GetType().Name);
				#endif
				DisposeStatic(ref dispose);
			}

			cachedEditors.Clear();
		}

		public static void CleanUp()
		{
			Instance().CleanUpInternal();
		}

		private void CleanUpInternal()
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(cachedEditorsCleaned != null);
			Debug.Assert(cachedEditors != null);
			Debug.Assert(cachedEditorsCleaned != cachedEditors);
			#endif

			foreach(var cached in cachedEditors)
			{
				var editor = cached.Value;
				if(editor == null)
				{
					#if DEV_MODE && DEBUG_CLEAN_UP
					Debug.LogWarning("Editors - removing null Editor");
					#endif
					continue;
				}

				if(editor.targets.ContainsNullObjects())
				{
					#if DEV_MODE && DEBUG_CLEAN_UP
					Debug.LogWarning("Editors - removing Editor "+editor.GetType().Name+" with null targets");
					#endif
					cachedEditorsToDispose.Add(cached);
					continue;
				}

				#if DEV_MODE && DEBUG_CLEAN_UP
				Debug.Log("Editors - keeping Editor "+editor.GetType().Name+".");
				#endif

				cachedEditorsCleaned.Add(cached.Key, editor);
			}
			
			var swap = cachedEditors;
			cachedEditors = cachedEditorsCleaned;
			cachedEditorsCleaned = swap;
			cachedEditorsCleaned.Clear();

			for(int n = cachedEditorsToDispose.Count - 1; n >= 0; n--)
			{
				var dispose = cachedEditorsToDispose[n];
				var editor = dispose.Value;
				Dispose(ref editor, dispose.Key);
			}
			cachedEditorsToDispose.Clear();

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(cachedEditorsCleaned != cachedEditors);
			#endif
		}

		/// <summary> Checks that the given editor has no null target objects. </summary>
		/// <param name="editor"> The editor to check. This cannot be null. </param>
		/// <returns> True if editor is valid, false if has null targets. </returns>
		private static bool Validate([NotNull]Editor editor)
		{
			if(editor.targets.ContainsNullObjects())
			{
				return false;
			}
			return true;
		}

		/// <summary> Disposes editor if it contains null targets. </summary>
		/// <param name="editor"> [in,out] The editor to check. </param>
		/// <returns> True if editor was disposed, false if not. </returns>
		public static bool DisposeIfInvalid([NotNull]ref Editor editor)
		{
			if(editor.targets.ContainsNullObjects())
			{
				Dispose(ref editor);
				return true;
			}
			return false;
		}
		
		public byte[] Serialize(BinaryFormatter formatter)
		{
			if(cachedEditors == null)
			{
				using(var stream = new MemoryStream(54))
				{
					// Editor count
					formatter.Serialize(stream, 0);

					#if DEV_MODE && DEBUG_SERIALIZE
					Debug.Log(StringUtils.ToStringSansNamespace(GetType())+" - Serialized null cachedEditors.");
					#endif
					return stream.ToArray();
				}
			}
			
			int count = cachedEditors.Count;
			using(var stream = new MemoryStream(count * 57))
			{
				// 1: Editor count
				formatter.Serialize(stream, count);

				// 2: instance IDs for Editors
				foreach(var editorData in cachedEditors)
				{
					formatter.Serialize(stream, ObjectIds.Get(editorData.Value));
				}

				#if DEV_MODE && DEBUG_SERIALIZE
				Debug.Log(StringUtils.ToStringSansNamespace(GetType())+" - Serialized "+cachedEditors.Count+" cachedEditors:\n"+StringUtils.ToString(cachedEditors, "\n"));
				#endif
				return stream.ToArray();
			}
		}

		public void DeserializeOverride(BinaryFormatter formatter, MemoryStream stream)
		{
			// 1: Editor count
			int editorCount = (int)formatter.Deserialize(stream);
			Setup(editorCount);
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(cachedEditors.Count == 0, StringUtils.ToStringSansNamespace(GetType())+".DeserializeOverride called with "+cachedEditors.Count+" cachedEditors:\n"+StringUtils.ToString(cachedEditors, "\n"));
			#endif

			// 2: instance IDs for Editors
			for(int n = 0; n < editorCount; n++)
			{
				int instanceId = (int)formatter.Deserialize(stream);
				var editor = ObjectIds.GetTarget(instanceId) as Editor;
				if(editor != null)
				{
					if(!Validate(editor))
					{
						#if DEV_MODE
						Debug.LogWarning("cachedEditors.Add - Disposing editor which failed validation: "+GetKey(editor).GetHashCode()+", "+editor.GetType().Name+StringUtils.ToString(editor.targets)+" with instanceId "+instanceId);
						#endif
						Dispose(ref editor);
						continue;
					}

					try
					{
						cachedEditors.Add(GetKey(editor), editor);
						#if DEV_MODE && DEBUG_DESERIALIZE
						Debug.Log("Editors - Deserialized Editor "+GetKey(editor).GetHashCode()+" for "+editor.GetType().Name+StringUtils.ToString(editor.targets)+" with instanceId "+instanceId);
						#endif
					}
					catch(ArgumentException) //TEMP
					{
						#if DEV_MODE
						var key = GetKey(editor);
						var conflictingEditor = cachedEditors[GetKey(editor)];
						Debug.LogError("cachedEditors.Add - Already contained key "+key.GetHashCode()+" (instanceId="+instanceId+")"
						+"\nFailed To Add: "+(editor == null ? "null" : editor.GetType().Name+" with targets "+StringUtils.ToString(editor.targets))
						+"\nExisting: "+(conflictingEditor == null ? "null" : conflictingEditor.GetType().Name+" with targets "+StringUtils.ToString(conflictingEditor.targets))
						+"\ncachedEditors:\n"+StringUtils.ToString(cachedEditors, "\n"));
						#endif
					}
				}
			}

			#if DEV_MODE && DEBUG_DESERIALIZE
			Debug.Log(StringUtils.ToStringSansNamespace(GetType())+" - Deserialized "+cachedEditors.Count+" cachedEditors:\n"+StringUtils.ToString(cachedEditors, "\n"));
			#endif
		}
	}
}
#endif