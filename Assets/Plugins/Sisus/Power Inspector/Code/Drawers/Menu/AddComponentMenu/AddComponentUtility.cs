using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	///	Utility class that holds information about which Types cannot co-exist within the same GameObject instance.
	///	</summary>
	public static class AddComponentUtility
	{
		private static Dictionary<Type,Type[]> conflictingTypes;
		private static HashSet<Type> onlyComponentTypes;
		private static HashSet<Type> invalidComponentTypes;

		private static Dictionary<Type,Type[]> ConflictingTypes
		{
			get
			{
				BuildConflictingTypesDictionaryIfDoesNotExist();
				return conflictingTypes;
			}
		}

		private static HashSet<Type> OnlyComponentTypes
		{
			get
			{
				BuildConflictingTypesDictionaryIfDoesNotExist();
				return onlyComponentTypes;
			}
		}

		/// <summary>
		/// Determines whether or not adding new components to GameObjects
		/// that contain the specified components is allowed.
		/// </summary>
		/// <param name="componentsByTarget"></param>
		/// <returns></returns>
		public static bool CanAddComponents(List<Component[]> componentsByTarget)
		{
			BuildConflictingTypesDictionaryIfDoesNotExist();

			#if DEV_MODE && DEBUG_CAN_ADD_COMPONENTS
			Debug.Log("CanAddComponents("+StringUtils.ToString(componentsByTarget)+") checking "+ onlyComponentTypes.Count+ " onlyComponentTypes: " + StringUtils.ToString(onlyComponentTypes));
			#endif

			for(int t = componentsByTarget.Count - 1; t >= 0; t--)
			{
				var components = componentsByTarget[t];
				for(int c = components.Length - 1; c >= 0; c--)
				{
					var component = components[c];
					if(component != null)
					{
						var type = component.GetType();
						if(onlyComponentTypes.Contains(type) || invalidComponentTypes.Contains(type))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Gets list of existing components drawers on GameObject drawer that prevent component of given type from being added to the target gameobject(s).
		/// </summary>
		/// <param name="type"> Component type to check. </param>
		/// <param name="target"> GameObject drawer. </param>
		/// <param name="conflictingMembers"> Any found conflicting drawers will be added to this list </param>
		public static void GetConflictingMembers(Type type, IGameObjectDrawer target, [NotNull]ref List<IComponentDrawer> conflictingMembers)
		{
			Type[] conflictingTypes;
			if(ConflictingTypes.TryGetValue(type, out conflictingTypes))
			{
				foreach(var componentDrawer in target)
				{
					var memberType = componentDrawer.Type;
					for(int t = conflictingTypes.Length - 1; t >= 0; t--)
					{
						var conflictingType = conflictingTypes[t];
						if(conflictingType.IsAssignableFrom(memberType))
						{
							conflictingMembers.Add(componentDrawer);
							break;
						}
					}
				}
			}
		}

		public static void BuildConflictingTypesDictionaryIfDoesNotExist()
		{
			if(conflictingTypes == null)
			{
				BuildConflictingTypesDictionary();
			}
		}

		private static void BuildConflictingTypesDictionary()
		{
			invalidComponentTypes = TypeExtensions.InvalidComponentTypes;

			conflictingTypes = new Dictionary<Type, Type[]>(20);

			onlyComponentTypes = new HashSet<Type>();

			AddConflictPair(Types.MeshFilter, Types.TextMesh);

			AddConflictPair(Types.Rigidbody, Types.Rigidbody2D);
			AddConflictPair(Types.Rigidbody, Types.Collider2D);
			AddConflictPair(Types.Rigidbody, Types.Joint2D);
			AddConflictPair(Types.Rigidbody, Types.Effector2D);
			AddConflictPair(Types.Rigidbody, Types.PhysicsUpdateBehaviour2D);

			AddConflictPair(Types.Collider, Types.Rigidbody2D);
			AddConflictPair(Types.Collider, Types.Collider2D);
			AddConflictPair(Types.Collider, Types.Joint2D);
			AddConflictPair(Types.Collider, Types.Effector2D);
			AddConflictPair(Types.Collider, Types.PhysicsUpdateBehaviour2D);

			AddConflictPair(Types.ConstantForce, Types.Rigidbody2D);
			AddConflictPair(Types.ConstantForce, Types.Collider2D);
			AddConflictPair(Types.ConstantForce, Types.Joint2D);
			AddConflictPair(Types.ConstantForce, Types.Effector2D);
			AddConflictPair(Types.ConstantForce, Types.PhysicsUpdateBehaviour2D);

			AddDisallowMultiple(Types.AudioListener);
			AddDisallowMultiple(Types.Camera);
			AddDisallowMultiple(Types.FlareLayer);
			AddDisallowMultiple(Types.MeshFilter);
			AddDisallowMultiple(Types.MeshRenderer);
			AddDisallowMultiple(Types.ParticleSystem);
			AddDisallowMultiple(Types.Rigidbody);
			AddDisallowMultiple(Types.Rigidbody2D);
			AddDisallowMultiple(Types.SkinnedMeshRenderer);
			AddDisallowMultiple(Types.TextMesh);
			AddDisallowMultiple(Types.TrailRenderer);
			AddDisallowMultiple(Types.Light);

			// AddComponentMenuItems.Setup handles DisallowMultipleComponent attributes
			// so it also needs to be run before the conflicting types dictionary is
			// fully popuplated
			AddComponentMenuItems.Setup();
		}

		public static bool HasConflictingMembers(Type type, IGameObjectDrawer target)
		{
			if(type == null)
			{
				return false;
			}

			Type[] conflictingTypes;
			if(!ConflictingTypes.TryGetValue(type, out conflictingTypes))
			{
				return false;
			}

			foreach(var componentDrawer in target)
			{
				var memberType = componentDrawer.Type;
				for(int t = conflictingTypes.Length - 1; t >= 0; t--)
				{
					var conflictingType = conflictingTypes[t];
					if(conflictingType.IsAssignableFrom(memberType))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// NOTE: Do not call this before first calling BuildConflictingTypesDictionaryIfDoesNotExist
		/// Or you might get a null reference exception
		/// </summary>
		public static void AddDisallowMultiple(Type a)
		{
			AddConflictWithoutExtendingTypes(a, a);
		}

		public static void AddOnlyComponent(Type onlyComponent)
		{
			#if DEV_MODE && DEBUG_CAN_ADD_COMPONENTS
			Debug.Log("AddOnlyComponent: "+onlyComponent);
			#endif
			onlyComponentTypes.Add(onlyComponent);
		}

		private static void AddConflictPair(Type a, Type b)
		{
			AddConflictPairWithoutExtendingTypes(a, b);
			
			var aExtended = a.GetExtendingComponentTypes(false);
			var bExtended = b.GetExtendingComponentTypes(false);
			
			for(int n = aExtended.Length - 1; n >= 0; n--)
			{
				var ae = aExtended[n];
				AddConflictPairWithoutExtendingTypes(ae, b);
				for(int m = bExtended.Length - 1; m >= 0; m--)
				{
					var be = bExtended[m];
					AddConflictPairWithoutExtendingTypes(ae, be);
				}
			}

			for(int m = bExtended.Length - 1; m >= 0; m--)
			{
				var be = bExtended[m];
				AddConflictPairWithoutExtendingTypes(a, be);
			}
		}

		private static void AddConflictPairWithoutExtendingTypes(Type a, Type b)
		{
			AddConflictWithoutExtendingTypes(a, b);

			#if DEV_MODE
			Debug.Assert(a != b);
			#endif

			AddConflictWithoutExtendingTypes(b, a);
		}

		private static void AddConflictWithoutExtendingTypes(Type a, Type b)
		{
			if(a.IsAbstract || b.IsAbstract)
			{
				return;
			}

			Type[] types;
			if(!conflictingTypes.TryGetValue(a, out types))
			{
				types = new Type[1];
				types[0] = b;
			}
			else
			{
				#if DEV_MODE
				if(Array.IndexOf(types, b) != -1)
				{
					Debug.LogWarning("Type "+b.FullName+" had already been registered to conflict with "+a.FullName);
					return;
				}
				#endif

				int count = types.Length;
				Array.Resize(ref types, count + 1);
				types[count] = b;
			}

			conflictingTypes[a] = types;
		}
	}
}