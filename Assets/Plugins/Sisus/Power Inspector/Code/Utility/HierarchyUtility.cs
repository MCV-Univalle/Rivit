using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Sisus
{
	public static class HierarchyUtility
	{
		private readonly static Stack<Transform> aParents = new Stack<Transform>(3);
		private readonly static Stack<Transform> bParents = new Stack<Transform>(3);
		private readonly static List<GameObject> getRoot = new List<GameObject>();
		private readonly static List<Component> getComponents = new List<Component>();

		public static int CompareHierarchyOrder(Transform a, Transform b)
		{
			if(a == b)
			{
				return 0;
			}
			if(a == null)
			{
				return -1;
			}
			if(b == null)
			{
				return 1;
			}

			for(var current = a; current != null; current = current.parent)
			{
				aParents.Push(current);
			}
			for(var current = b; current != null; current = current.parent)
			{
				bParents.Push(current);
			}

			int aCount = aParents.Count;
			int bCount = bParents.Count;
			int minCount = Mathf.Min(aCount, bCount); 
			for(int n = minCount; n > 0; n--)
			{
				var aParent = aParents.Pop();
				var bParent = bParents.Pop();
				if(aParent != bParent)
				{
					return aParent.GetSiblingIndex().CompareTo(bParent.GetSiblingIndex());
				}
			}
			return a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
		}

		public static int CompareHierarchyOrder(GameObject a, GameObject b)
		{
			if(a == b)
			{
				return 0;
			}
			if(a == null)
			{
				return -1;
			}
			if(b == null)
			{
				return 1;
			}
			return CompareHierarchyOrder(a.transform, b.transform);
		}

		public static int CompareHierarchyOrder(Component a, Component b)
		{
			if(a == b)
			{
				return 0;
			}

			if(a == null)
			{
				return 1;
			}
			if(b == null)
			{
				return -1;
			}

			var transA = a.transform;
			var transB = b.transform;

			if(transA != transB)
			{
				return CompareHierarchyOrder(transA, transB);
			}
			
			transA.GetComponents(Types.Component, getComponents);
			for(int n = getComponents.Count - 1; n >= 0; n--)
			{
				var comp = getComponents[n];
				if(comp == a)
				{
					getComponents.Clear();
					return 1;
				}
				if(comp == b)
				{
					getComponents.Clear();
					return -1;
				}
			}
			getComponents.Clear();
			throw new InvalidOperationException();
		}

		public static int CompareHierarchyOrder(Object a, Object b)
		{
			if(a == b)
			{
				return 0;
			}

			if(a == null)
			{
				return 1;
			}
			if(b == null)
			{
				return -1;
			}

			var transA = a.Transform();
			var transB = b.Transform();

			if(transA != transB)
			{
				return CompareHierarchyOrder(transA, transB);
			}
			
			//since a != b, but transA == transB
			//at least a or b must a component
			transA.GetComponents(Types.Component, getComponents);
			for(int n = getComponents.Count - 1; n >= 0; n--)
			{
				var comp = getComponents[n];
				if(comp == a)
				{
					getComponents.Clear();
					return 1;
				}
				if(comp == b)
				{
					getComponents.Clear();
					return -1;
				}
			}
			getComponents.Clear();
			throw new InvalidOperationException();
		}

		public static Component PreviousComponent(this Component origin)
		{
			#if DEV_MODE
			Debug.Assert(getComponents.Count == 0);
			#endif

			var transform = origin.Transform();
			Component result;
			
			if(origin == transform)
			{
				var previousTransform = transform.Previous();
				GetComponents(previousTransform);
				result = getComponents[getComponents.Count - 1];
				getComponents.Clear();
				return result;
			}
			
			origin.GetComponents(getComponents);
			int index = getComponents.IndexOf(origin);
			result = getComponents[index - 1];
			getComponents.Clear();
			return result;
		}

		private static void GetComponents(Component subject)
		{
			#if DEV_MODE
			Debug.Assert(getComponents.Count == 0);
			#endif
			subject.GetComponents(getComponents);
		}

		public static Component NextComponent(this Component origin)
		{
			#if DEV_MODE
			Debug.Assert(getComponents.Count == 0);
			#endif

			Component result;
			GetComponents(origin);
			int nextIndex = getComponents.IndexOf(origin) + 1;
			if(nextIndex < getComponents.Count)
			{
				result = getComponents[nextIndex];
				getComponents.Clear();
				return result;
			}

			getComponents.Clear();
			return origin.transform.Next();
		}

		/// <summary>
		/// Gets previous transform in scene hierarchy or prefab hierarchy relative to origin Transform which is visible in the hierarchy and inspector.
		/// </summary>
		/// <param name="origin"> Get previous transform relative to this. </param>
		/// <returns> Previous transform. Can be null (if hierarchy contains no visible transforms). </returns>
		[CanBeNull]
		public static Transform PreviousVisibleInInspector([NotNull]this Transform origin, bool checkHideInInspector)
		{
			Transform previous;
			do
			{
				previous = origin.Previous();
				if(previous.gameObject.hideFlags != HideFlags.HideInHierarchy && (!checkHideInInspector || !previous.hideFlags.HasFlag(HideFlags.HideInInspector)))
				{
					return previous;
				}
			}
			while(previous != origin);
			
			return !origin.gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy) && (!checkHideInInspector || !origin.hideFlags.HasFlag(HideFlags.HideInInspector)) ? origin : null;
		}

		public static Transform Previous(this Transform origin)
		{
			var parent = origin.parent;
			if(parent != null)
			{
				int index = origin.GetSiblingIndex();
				if(index > 0)
				{
					return GetLastLeaf(parent.GetChild(index - 1));
				}
				return parent;
			}

			var gameObject = origin.gameObject;
			//check if it is a scene object
			var scene = gameObject.scene;
			if(!scene.IsValid())
			{
				//get last leaf child in prefab
				return GetLastLeaf(origin);
			}

			//get scene root objects
			scene.GetRootGameObjects(getRoot);
			int rootIndex = getRoot.IndexOf(gameObject);

			Transform result;

			//if not first transform in scene
			if(rootIndex > 0)
			{
				//get last leaf in previous root transform
				result = getRoot[rootIndex - 1].transform;
				getRoot.Clear();
				return result;
			}

			int loadedSceneCount = SceneManager.sceneCount;
			
			//if more than one scene is loaded return last leaf in previous scene
			if(loadedSceneCount > 1)
			{
				//get index of current scene in loaded scenes
				int originSceneIndex;
				for(originSceneIndex = 0; originSceneIndex < loadedSceneCount; originSceneIndex++)
				{
					if(scene == SceneManager.GetSceneAt(originSceneIndex))
					{
						break;
					}
				}
				
				//go backwards in loaded scenes list until first non-empty scene is found
				for(int sceneIndex = originSceneIndex - 1; sceneIndex >= 0; sceneIndex--)
				{
					if(TryGetLastLeaf(SceneManager.GetSceneAt(sceneIndex), out result))
					{
						getRoot.Clear();
						return result;
					}
				}

				//loop around and check loaded scenes with index larger than that of the loaded scene
				for(int sceneIndex = loadedSceneCount - 1; sceneIndex > originSceneIndex; sceneIndex--)
				{
					if(TryGetLastLeaf(SceneManager.GetSceneAt(sceneIndex), out result))
					{
						getRoot.Clear();
						return result;
					}
				}
			}

			//get last leaf in current scene hierarchy
			result = getRoot[getRoot.Count - 1].transform;
			getRoot.Clear();
			return result;
		}

		/// <summary>
		/// Given a scene, which can be invalid, tries to find the last leaf Transform inside it.
		/// </summary>
		/// <param name="scene"> The scene to check. Can be invalid. </param>
		/// <param name="result"> The last leaf Transform. Can be null. </param>
		/// <returns></returns>
		private static bool TryGetLastLeaf(this Scene scene, [CanBeNull]out Transform result)
		{
			if(scene.IsValid())
			{
				var root = scene.GetRootGameObjects();
				if(root.Length > 0)
				{
					//get last left in scene
					result = GetLastLeaf(root[root.Length - 1].transform);
					return true;
				}
			}
			result = null;
			return false;
		}

		[NotNull]
		private static Transform GetLastLeaf(Transform origin)
		{
			int childCount = origin.childCount;
			return childCount == 0 ? origin : GetLastLeaf(origin.GetChild(childCount - 1));
		}

		/// <summary>
		/// Gets next transform in scene hierarchy or prefab hierarchy relative to origin Transform which is visible in the hierarchy and inspector.
		/// </summary>
		/// <param name="origin"> Get next transform relative to this. </param>
		/// <returns> Next transform. Can be null (if hierarchy contains no visible transforms). </returns>
		[CanBeNull]
		public static Transform NextVisibleInInspector([NotNull]this Transform origin, bool checkHideInInspector)
		{
			Transform next;
			do
			{
				next = origin.Next();
				if(!next.gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy) && (!checkHideInInspector || !next.hideFlags.HasFlag(HideFlags.HideInInspector)))
				{
					return next;
				}
			}
			while(next != origin);
			
			return !origin.hideFlags.HasFlag(HideFlags.HideInHierarchy) && (!checkHideInInspector || !origin.hideFlags.HasFlag(HideFlags.HideInInspector)) ? origin : null;
		}

		/// <summary>
		/// Gets next transform in scene hierarchy or prefab hierarchy relative to origin Transform.
		/// </summary>
		/// <param name="origin"> Get next transform relative to this. </param>
		/// <returns> Next transform. Can not be null. </returns>
		[NotNull]
		public static Transform Next([NotNull]this Transform origin)
		{
			int childCount = origin.childCount;

			// if origin Transform has children, get first child
			if(childCount > 0)
			{
				return origin.GetChild(0);
			}

			var parent = origin.parent;

			// if origin Transform has no children but has a parent, return next sibling
			if(parent != null)
			{
				#if DEV_MODE
				var result = Next(parent, origin.GetSiblingIndex());
				Debug.Log(origin.name+".Next(): "+ result.name + " (next child of parent)");
				#endif

				return Next(parent, origin.GetSiblingIndex());
			}

			var gameObject = origin.gameObject;
			var scene = gameObject.scene;

			// If origin Transform has no children, no parent and is a prefab, return itself.
			if(!scene.IsValid())
			{
				#if DEV_MODE
				Debug.Log(origin.name+".Next(): "+ origin.GetChild(0).name+" (parent, because scene not valid)");
				#endif

				return origin;
			}

			#if DEV_MODE
			Debug.Log(origin.name+".Next(): "+ scene.GetNext(origin).name+" (next in scene)");
			#endif

			// If origin transform is a root scene object, return next Transform in scene
			return scene.GetNext(origin);
		}

		/// <summary>
		/// Gets next child transform of parent. If previous child was last child, then moves
		/// on to siblings etc.
		/// </summary>
		/// <param name="parent"> Parent whose child transform will be returned </param>
		/// <param name="originChildIndex"> Zero-based index of previous child transform. </param>
		/// <returns></returns>
		[NotNull]
		private static Transform Next([NotNull]Transform parent, int originChildIndex)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(originChildIndex >= 0);
			#endif

			int lastIndex = parent.childCount - 1;

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(originChildIndex <= lastIndex);
			#endif

			// if there are more children, get next one
			if(originChildIndex < lastIndex)
			{
				return parent.GetChild(originChildIndex + 1);
			}

			// if there are no more children, get next in grand parent
			var grandParent = parent.parent;
			if(grandParent != null)
			{
				return Next(grandParent, parent.GetSiblingIndex());
			}
			
			var gameObject = parent.gameObject;
			var scene = gameObject.scene;

			// parent is prefab root, select last Transform in prefabs.
			if(!scene.IsValid())
			{
				//return prefab root
				return GetLastLeaf(parent);
			}

			// return next transform in the scene
			return scene.GetNext(parent);
		}
				
		private static bool TryGetFirst(Scene scene, out Transform result)
		{
			if(scene.IsValid())
			{
				var root = scene.GetRootGameObjects();
				if(root.Length > 0)
				{
					result = root[0].transform;
					return true;
				}
			}
			result = null;
			return false;
		}

		/// <summary>
		/// Gets next transform in given scene.
		/// </summary>
		/// <param name="scene"> Scene from which to get next transform. This should be a valid scene. </param>
		/// <param name="originInRoot"> Origin transform related to which the next transform should be returned. This should not be null, and it should be a transform which exists inside the scene. </param>
		/// <returns></returns>
		[NotNull]
		private static Transform GetNext(this Scene scene, [NotNull]Transform originInRoot)
		{
			if(!scene.IsValid())
			{
				throw new NullReferenceException("Scene.GetNext(Transform) called with invalid scene!");
			}

			var root = scene.GetRootGameObjects();

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(root.Length > 0);
			#endif

			int nextIndex = Array.IndexOf(root, originInRoot.gameObject) + 1;
			
			//if not the last root transform then return next in root
			if(nextIndex < root.Length)
			{
				return root[nextIndex].transform;
			}
			
			int loadedSceneCount = SceneManager.sceneCount;
			
			//if more than one scene is loaded return first transform in next scene
			if(loadedSceneCount > 1)
			{
				int originSceneIndex;
				for(originSceneIndex = 0; originSceneIndex < loadedSceneCount; originSceneIndex++)
				{
					if(scene == SceneManager.GetSceneAt(originSceneIndex))
					{
						break;
					}
				}

				//go foward in loaded scenes list until first non-empty scene is found
				for(int sceneIndex = originSceneIndex + 1; sceneIndex < loadedSceneCount; sceneIndex++)
				{
					Transform result;
					if(TryGetFirst(SceneManager.GetSceneAt(sceneIndex), out result))
					{
						return result;
					}
				}

				//loop around and check loaded scenes with index smaller than that of the loaded scene
				for(int sceneIndex = 0; sceneIndex < originSceneIndex; sceneIndex++)
				{
					Transform result;
					if(TryGetFirst(SceneManager.GetSceneAt(sceneIndex), out result))
					{
						return result;
					}
				}
			}

			//get first transform in current scene hierarchy
			return root[0].transform;
		}
		
		public static GameObject FindByHierarchyPath(string hierarchyPath, int gameObjectWithPathOrdinal = 1)
		{
			int nameIndex = hierarchyPath.LastIndexOf('/');
			string name = nameIndex == -1 ? hierarchyPath : hierarchyPath.Substring(nameIndex + 1);
			int instanceCounter = 0;
			int count = SceneManager.sceneCount;
			for(int s = 0; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);

				for(int o = getRoot.Count - 1; o >= 0; o--)
				{
					var gameObject = getRoot[o];
					if(string.Equals(gameObject.name, name, StringComparison.CurrentCulture))
					{
						if(string.Equals(gameObject.transform.GetHierarchyPath(), hierarchyPath, StringComparison.CurrentCulture))
						{
							instanceCounter++;
							if(instanceCounter == gameObjectWithPathOrdinal)
							{
								getRoot.Clear();
								return gameObject;
							}
						}
					}
				}

				getRoot.Clear();
			}
			return null;
		}

		public static Component FindComponentByHierarchyPath(string hierarchyPath, Type componentType, int gameObjectWithPathOrdinal = 1, int componentOfTypeOrdinal = 1)
		{
			var gameObject = FindByHierarchyPath(hierarchyPath, gameObjectWithPathOrdinal);
			if(gameObject == null)
			{
				return null;
			}

			gameObject.GetComponents(componentType, getComponents);
			int index = componentOfTypeOrdinal - 1;
			var result = getComponents.Count <= index ? null : getComponents[index];
			getComponents.Clear();
			return result;
		}

		public static string GetHierarchyPath(this Transform transform)
		{
			var sb = StringBuilderPool.Create();
			GetHierarchyPath(transform, ref sb);
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static string GetHierarchyPathWithType(this Object gameObjectOrComponent)
		{
			var sb = StringBuilderPool.Create();
			GetHierarchyPath(gameObjectOrComponent.Transform(), ref sb);
			sb.Append('/');
			sb.Append(StringUtils.ToStringSansNamespace(gameObjectOrComponent.GetType()));
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static void GetHierarchyPath(this Transform transform, ref StringBuilder sb)
		{
			var parent = transform.parent;
			if(parent != null)
			{
				GetHierarchyPath(parent, ref sb);
				sb.Append('/');
			}
			sb.Append(transform.name);
		}

		public static string GetRelativeHierarchyPath(this Transform parent, Transform child)
		{
			var sb = StringBuilderPool.Create();
			GetRelativeHierarchyPath(parent, child, ref sb);
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static string GetRelativeHierarchyPathWithType(this Transform parent, Object child)
		{
			var sb = StringBuilderPool.Create();
			GetRelativeHierarchyPathWithType(parent, child, ref sb);
			return StringBuilderPool.ToStringAndDispose(ref sb);
		}

		public static void GetRelativeHierarchyPathWithType(this Transform root, Object gameObjectOrComponent, ref StringBuilder sb)
		{
			var transform = gameObjectOrComponent.Transform();
			if(transform != root)
			{
				GetRelativeHierarchyPath(root, transform.parent, ref sb);
				sb.Append('/');
			}
			sb.Append(transform.name);
			sb.Append('/');
			sb.Append(StringUtils.ToStringSansNamespace(gameObjectOrComponent.GetType()));
		}

		public static void GetRelativeHierarchyPath(this Transform root, Transform child, ref StringBuilder sb)
		{
			#if DEV_MODE
			Debug.Assert(root != null);
			Debug.Assert(child != null);
			Debug.Assert(child.parent != null || child == root);
			#endif

			try
			{
				if(child != root)
				{
					GetRelativeHierarchyPath(root, child.parent, ref sb);
					sb.Append('/');
				}
				sb.Append(child.name);
			}
			catch(NullReferenceException)
			{
				#if DEV_MODE
				Debug.Log("GetRelativeHierarchyPath(root="+StringUtils.ToString(root)+", child="+StringUtils.ToString(child)+") child was not root but parent was null?");
				#endif
			}
		}

		public static string GetHierarchyPathAndInstanceOrdinals([NotNull]Object subject, out int gameObjectWithPathOrdinal, out int componentOfTypeOrdinal)
		{
			var transform = subject.Transform();
			var gameObject = transform.gameObject;
			string hierarchyPath = GetHierarchyPathAndInstanceOrdinal(gameObject, out gameObjectWithPathOrdinal);
			if(subject == gameObject)
			{
				componentOfTypeOrdinal = -1;
			}
			else
			{
				componentOfTypeOrdinal = GetComponentOfTypeOrdinal(gameObject, subject as Component);
			}
			return hierarchyPath;
		}

		public static string GetHierarchyPathAndInstanceOrdinal([NotNull]GameObject subject, out int nthInstanceWithSamePath)
		{
			int gameObjectWithPathOrdinal = 1;
			var transform = subject.transform;
			string hierarchyPath = transform.GetHierarchyPath();
			int nameIndex = hierarchyPath.LastIndexOf('/');
			string name = nameIndex == -1 ? hierarchyPath : hierarchyPath.Substring(nameIndex + 1);
			int count = SceneManager.sceneCount;
			for(int s = 0; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);

				for(int o = getRoot.Count - 1; o >= 0; o--)
				{
					var gameObject = getRoot[o];
					if(string.Equals(gameObject.name, name, StringComparison.CurrentCulture))
					{
						if(gameObject == subject)
						{
							getRoot.Clear();
							nthInstanceWithSamePath = gameObjectWithPathOrdinal;
							return hierarchyPath;
						}
						if(string.Equals(gameObject.transform.GetHierarchyPath(), hierarchyPath, StringComparison.CurrentCulture))
						{
							gameObjectWithPathOrdinal++;
						}
					}
				}
				getRoot.Clear();
			}
			nthInstanceWithSamePath = - 1;
			return hierarchyPath;
		}

		public static int GetComponentOfTypeOrdinal([NotNull]GameObject gameObject, [NotNull]Component component)
		{
			gameObject.GetComponents(component.GetType(), getComponents);
			int result = getComponents.IndexOf(component) + 1;
			getComponents.Clear();
			return result;
		}

		public static void FindComponentsOfExactType(Type type, [NotNull]List<Component> targetsOfType)
		{
			for(int s = 0, count = SceneManager.sceneCount; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);
				for(int g = 0, gcount = getRoot.Count; g < gcount; g++)
				{
					getRoot[g].GetComponentsOfExactTypeInChildren(type, targetsOfType);
				}
				getRoot.Clear();
			}
		}

		public static int FindComponentsOfExactlySameType(Component component, [NotNull]List<Component> targetsOfType)
		{
			var type = component.GetType();
			FindComponentsOfExactType(type, targetsOfType);
			int lastIndex = targetsOfType.Count - 1;
			if(lastIndex > -1)
			{
				for(int n = lastIndex; n >= 0; n--)
				{
					if(targetsOfType[n] == component)
					{
						return n;
					}
				}
			}
			return -1;
		}

		/// <summary> Gets all Components in hierarchy that implement the given interface type. </summary>
		/// <param name="interfaceType"> Type of the interface. </param>
		/// <returns> The found components, in no particular order. </returns>
		public static Component[] FindComponentsImplementingInterface(Type interfaceType)
		{
			FindComponentsImplementingInterface(interfaceType, getComponents);
			var results = getComponents.ToArray();
			getComponents.Clear();
			return results;
		}

		/// <summary> Gets all Components in hierarchy that implement the given interface type. </summary>
		/// <param name="interfaceType"> Type of the interface. </param>
		/// <param name="targetsOfType"> List into which found Components are added. </param>
		public static void FindComponentsImplementingInterface(Type interfaceType, [NotNull]List<Component> targetsOfType)
		{
			var types = interfaceType.GetImplementingComponentTypes(false);
			for(int t = 0, count = types.Length; t < count; t++)
			{
				var type = types[t];
				var found = Object.FindObjectsOfType(type);
				for(int c = 0, ccount = found.Length; c < ccount; c++)
				{
					var comp = found[c] as Component;
					if(comp != null)
					{
						targetsOfType.Add(comp);
					}
				}
			}
		}
		
		public static bool TryGetPreviousOfType(GameObject gameObject, out GameObject result)
		{
			Transform previousTransform;
			if(TryGetPreviousOfType(gameObject.transform, out previousTransform))
			{
				result = previousTransform.gameObject;
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryGetPreviousOfType(Transform transform, out Transform result)
		{
			result = transform.Previous();
			return true;
		}

		public static bool TryGetPreviousOfType(Component component, out Component result)
		{
			if(component is Transform)
			{
				result = component.transform.Previous();
				return true;
			}

			var type = component.GetType();

			// handle prefabs
			if(!component.IsSceneObject())
			{
				result = null;

				var root = component.transform.root;
				if(TryGetPreviousOfType(root, component, type, ref result))
				{
					if(result != null)
					{
						return true;
					}
					
					if(TryGetLastOfType(root, type, ref result))
					{
						return result != component;
					}
				}
				return false;
			}

			result = null;
			for(int s = SceneManager.sceneCount - 1; s >= 0; s--)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);
				for(int g = 0, gcount = getRoot.Count; g < gcount; g++)
				{
					if(TryGetPreviousOfType(getRoot[g].transform, component, type, ref result))
					{
						if(result != null)
						{
							return true;
						}
						
						for(g = g + 1; g < gcount; g++)
						{
							if(TryGetLastOfType(getRoot[g].transform, type, ref result))
							{
								getRoot.Clear();
								return true;
							}
						}
						getRoot.Clear();
						return TryGetLastOfType(type, out result) && result != component;
					}
				}
				getRoot.Clear();
			}
			return false;
		}

		private static bool TryGetPreviousOfType(Transform currentTransform, Component target, Type type, ref Component result)
		{
			currentTransform.gameObject.GetComponents(type, getComponents);
			int targetIndex = getComponents.IndexOf(target);
			if(targetIndex != -1)
			{
				if(targetIndex > 0)
				{
					result = getComponents[targetIndex - 1];
					getComponents.Clear();
					return true;
				}
				getComponents.Clear();

				for(int n = 0, count = currentTransform.childCount; n < count; n++)
				{
					if(TryGetLastOfType(currentTransform, type, ref result))
					{
						return true;
					}
				}
				return true;
			}
			getComponents.Clear();
			for(int n = 0, count = currentTransform.childCount; n < count; n++)
			{
				if(TryGetPreviousOfType(currentTransform.GetChild(n), target, type, ref result))
				{
					if(result != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool TryGetLastOfType(Transform currentTransform, Type type, ref Component result)
		{
			currentTransform.gameObject.GetComponents(type, getComponents);
			int count = getComponents.Count;
			if(count > 0)
			{
				result = getComponents[count - 1];
				getComponents.Clear();
				return true;
			}
			return false;
		}

		private static bool TryGetLastOfType(Type type, out Component result)
		{
			result = null;
			for(int s = 0, count = SceneManager.sceneCount; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);
				for(int g = 0, gcount = getRoot.Count; g < gcount; g++)
				{
					for(g = g + 1; g < gcount; g++)
					{
						if(TryGetFirstOfType(getRoot[g].transform, type, ref result))
						{
							getRoot.Clear();
							return true;
						}
					}
				}
				getRoot.Clear();
			}
			result = null;
			return false;
		}

		public static bool TryGetNextOfType([NotNull]GameObject gameObject, [CanBeNull]out GameObject result)
		{
			result = gameObject.transform.Next().gameObject;
			return true;
		}

		public static bool TryGetNextOfType([NotNull]Transform transform, [CanBeNull]out Transform result)
		{
			result = transform.Next();
			return true;
		}

		public static bool TryGetNextOfType([NotNull]Component nextFrom, [CanBeNull]out Component result)
		{
			if(nextFrom is Transform)
			{
				result = nextFrom.transform.Next();
				return true;
			}
			
			var type = nextFrom.GetType();

			// handle prefabs
			if(!nextFrom.IsSceneObject())
			{
				result = null;

				var root = nextFrom.transform.root;
				if(TryGetNextOfType(root, nextFrom, type, ref result))
				{
					if(result != null)
					{
						return true;
					}
					
					if(TryGetFirstOfType(root, type, ref result))
					{
						return result != nextFrom;
					}
				}
				return false;
			}

			result = null;
			for(int s = 0, count = SceneManager.sceneCount; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);
				for(int g = 0, gcount = getRoot.Count; g < gcount; g++)
				{
					if(TryGetNextOfType(getRoot[g].transform, nextFrom, type, ref result))
					{
						// if found next of type and it wasn't nextFrom
						if(result != null)
						{
							#if DEV_MODE
							Debug.Log("TryGetNextOfType found " + result.name);
							#endif
							return true;
						}

						#if DEV_MODE
						Debug.Log("TryGetNextOfType found nextFrom \""+ nextFrom+"\" under root object #"+g);
						#endif
						
						// find next instance in remaining root objects
						for(g += 1; g < gcount; g++)
						{
							if(TryGetFirstOfType(getRoot[g].transform, type, ref result))
							{
								#if DEV_MODE
								Debug.Log("TryGetNextOfType found \"" + result.name+ "\" under root object #" + g);
								#endif
								getRoot.Clear();
								return true;
							}
						}
						getRoot.Clear();

						// find first instance in scene
						return TryGetFirstOfType(type, out result) && result != nextFrom;
					}
				}
				getRoot.Clear();
			}
			return false;
		}

		/// <summary>
		/// Tries to find instance of given component type in transform or any of its children.
		/// </summary>
		/// <param name="currentTransform"> Transform to search. </param>
		/// <param name="nextFrom"> Component requesting the search. If next component found is the target, then returns true but sets result to null. </param>
		/// <param name="type"> Component type. </param>
		/// <param name="result"> Next component of type that was found, or null if next component of type was target. </param>
		/// <returns> True if found an instance of type in hierarchy. </returns>
		private static bool TryGetNextOfType([NotNull]Transform currentTransform, [NotNull]Component nextFrom, [NotNull]Type type, [CanBeNull]ref Component result)
		{
			currentTransform.gameObject.GetComponents(type, getComponents);
			int targetIndex = getComponents.IndexOf(nextFrom);
			if(targetIndex != -1)
			{
				for(int n = targetIndex + 1, count = getComponents.Count; n < count; n++)
				{
					var test = getComponents[n];
					if(type.Equals(test.GetType()))
					{
						result = test;
						getComponents.Clear();
						#if DEV_MODE
						Debug.Log("TryGetNextOfType found " + result.name);
						#endif
						return true;
					}
				}
				getComponents.Clear();

				// search children
				for(int n = 0, count = currentTransform.childCount; n < count; n++)
				{
					result = currentTransform.GetChild(n).gameObject.GetComponentOfExactTypeInChildren(type);
					if(result != null)
					{
						#if DEV_MODE
						Debug.Log("TryGetNextOfType found " + result.name + " via TryGetFirstOfType on child #" + n + " of " + currentTransform.name);
						#endif
						return true;
					}
				}

				#if DEV_MODE
				Debug.Log("Returning true even though result is null, because found self.");
				#endif

				return true;
			}
			getComponents.Clear();

			bool foundNextFrom = false;
			for(int n = 0, count = currentTransform.childCount; n < count; n++)
			{
				if(TryGetNextOfType(currentTransform.GetChild(n), nextFrom, type, ref result))
				{
					if(result != null)
					{
						#if DEV_MODE
						Debug.Log("TryGetNextOfType found " + result.name + " via child #" + n + " of "+ currentTransform.name);
						#endif
						return true;
					}
					foundNextFrom = true;
				}				
			}
			return foundNextFrom;
		}
	
		private static bool TryGetFirstOfType(Type type, out Component result)
		{
			for(int s = 0, count = SceneManager.sceneCount; s < count; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				scene.GetRootGameObjects(getRoot);
				for(int g = 0, gcount = getRoot.Count; g < gcount; g++)
				{
					for(g = g + 1; g < gcount; g++)
					{
						result = getRoot[g].GetComponentOfExactTypeInChildren(type);
						if(result != null)
						{
							getRoot.Clear();
							return true;
						}
					}
				}
				getRoot.Clear();
			}
			result = null;
			return false;
		}

		private static bool TryGetFirstOfType([NotNull]Transform inTransformOrChildren, [NotNull]Type type, [CanBeNull]ref Component result)
		{
			result = inTransformOrChildren.gameObject.GetComponentOfExactTypeInChildren(type);
			return result != null;
		}
	}
}