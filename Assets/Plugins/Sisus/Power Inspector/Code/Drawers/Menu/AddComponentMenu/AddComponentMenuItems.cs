#define SAFE_MODE

//#define DEBUG_SKIP_ADDING
//#define DEBUG_INVALID_DATA

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Sisus
{
	/// <summary>
	/// Handles generating and items for the Add Component menu and can then be queried for the items.
	/// </summary>
	public static class AddComponentMenuItems
	{
		public const string GlobalNamespaceGroupName = "Scripts ";
		private const string GlobalNameSpacePrefix = GlobalNamespaceGroupName + "/";

		private static readonly Type OnlyComponentAttribute = typeof(Attributes.OnlyComponentAttribute);

		private static AddComponentMenuItem[] rootGroups;
		private static Dictionary<string, AddComponentMenuItem> itemsByLabel = new Dictionary<string, AddComponentMenuItem>();
		private static SearchableList searchableList;

		private static AddComponentMenuItem[] itemsFiltered = new AddComponentMenuItem[0];
		private static string lastAppliedFilter = "";

		private static bool itemsGenerated;

		private static List<string> pathBuilder = new List<string>(5);

		private static bool addComponentMenuConfigApplied;

		/// <summary> Gets all AddComponentMenu items that should be shown in the Add Component menu. </summary>
		/// <returns> An array containing all items for the Add Component menu. </returns>
		public static AddComponentMenuItem[] GetAll()
		{
			Setup();
			return rootGroups;
		}

		/// <summary>
		/// Gets AddComponentMenu items that should be shown in the Add Component menu with the provided search filter.
		/// </summary>
		/// <returns> An array containing all items for the Add Component menu that pass the filter. </returns>
		public static AddComponentMenuItem[] GetFiltered(string filter, int maxMismatchThreshold = 0, int maxNumberOfResults = 50)
		{
			int filterLength = filter.Length;
			if(filterLength == 0)
			{
				return GetAll();
			}

			Setup();

			if(!string.Equals(filter, lastAppliedFilter))
			{
				lastAppliedFilter = filter;
				searchableList.Filter = filter;
				var matches = searchableList.GetValues(maxMismatchThreshold);
				int count = Mathf.Min(matches.Length, maxNumberOfResults);

				#if UNITY_EDITOR
				UnityEditor.AssetPreview.SetPreviewTextureCacheSize(maxNumberOfResults);
				#endif

				int oldCount = itemsFiltered.Length;
				if(oldCount != count)
				{
					Array.Resize(ref itemsFiltered, count);
				}

				for(int n = 0; n < count; n++)
				{
					itemsFiltered[n] = GetMenuItem(matches[n]);
				}
			}

			return itemsFiltered;
		}

		/// <summary>
		/// Generates Add Component Menu items, and handles informing AddComponentUtility
		/// of conflicting component data based on DisallowMultipleComponent attributes.
		/// If Setup has already been done, then the call will be ignored.
		/// </summary>
		public static void Setup()
		{
			if(!itemsGenerated)
			{
				GenerateItems();
			}
		}

		public static void Apply(AddComponentMenuConfig config)
		{
			if(addComponentMenuConfigApplied)
			{
				#if DEV_MODE
				Debug.LogWarning("AddComponentMenuItems - Ignoring Apply(AddComponentMenuConfig) because addComponentMenuConfigApplied was already true");
				#endif
				return;
			}
			addComponentMenuConfigApplied = true;

			var groups = config.groups;
			int count = groups.Length;
			for(int n = 0; n < count; n++)
			{
				var group = groups[n];
				GetOrCreateGroup(group.label, group.customIcon);
			}

			var items = config.items;
			count = items.Length;
			for(int n = 0; n < count; n++)
			{
				var item = items[n];
				var type = item.Type;
				if(type == null || !type.IsComponent())
				{
					#if DEV_MODE && DEBUG_INVALID_DATA
					Debug.LogWarning("Skipping item with invalid type for Add Component menu: \"" + item.label + "\" with type "+StringUtils.ToString(type));
					#endif
					continue;
				}

				try
				{
					Add(item.Type, item.label);
				}
				catch(NullReferenceException)
				{
					#if DEV_MODE
					Debug.LogError("Preferences.AddComponentMenuConfig NullReferenceException: item #"+n+" seems to contain invalid data.");
					#else
					continue;
					#endif
				}
			}
		}

		private static void GenerateItems()
		{
			itemsGenerated = true;

			AddComponentUtility.BuildConflictingTypesDictionaryIfDoesNotExist();
			
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int assemblyCount = assemblies.Length;
			
			rootGroups = new AddComponentMenuItem[0];

			Apply(InspectorUtility.Preferences.addComponentMenuConfig);
			
			var componentTypes = TypeExtensions.ComponentTypes;
			for(int n = componentTypes.Length - 1; n >= 0; n--)
			{
				var type = componentTypes[n];
				if(type.IsAbstract || type.IsGenericTypeDefinition)
				{
					continue;
				}
					
				//Special case. For some reason Unity didn't make these abstract
				//but they still should not be shown in the Add Component menu
				if(type == Types.MonoBehaviour
				|| type == Types.Effector2D
				|| type == Types.Behaviour
				|| type == Types.Collider
				|| type == Types.Collider2D
				|| type == Types.Renderer
				#if !UNITY_2019_3_OR_NEWER
				|| type == Types.GUIElement
				#endif
				|| type == Types.Joint
				|| type == Types.Joint2D
				|| type == Types.AnchoredJoint2D
				|| type == Types.PhysicsUpdateBehaviour2D
				|| type == Types.Tree
				|| type == Types.ParticleSystemRenderer
				|| type == Types.AudioBehaviour
				#if UNITY_2017_2_OR_NEWER
				|| type == Types.GridLayout
				#endif
				)
				{
					continue;
				}

				//all GameObjects will always already have a Transform or RectTransform component
				//and only one can be added to a GameObject, so won't show them in the menu
				if(type == Types.Transform
				|| type == Types.RectTransform)
				{
					continue;
				}

				if(Contains(type))
				{
					continue;
				}

				//hide obsolete items from the menu
				if(type.IsDefined(Types.ObsoleteAttribute, false))
				{
					#if DEV_MODE && DEBUG_SKIP_ADDING
					Debug.LogWarning("Skipping adding "+type.Name+" to Add Component menu because it has the Obsolete attribute");
					#endif
					continue;
				}

				Add(type);
			}

			itemsByLabel.Clear();
			for(int n = rootGroups.Length - 1; n >= 0; n--)
			{
				rootGroups[n].GetClassLabelsFlattened(ref pathBuilder, ref itemsByLabel);
			}

			if(searchableList != null)
			{
				SearchableListPool.Dispose(ref searchableList);
			}
			searchableList = SearchableListPool.Create(pathBuilder.ToArray());

			pathBuilder.Clear();

			#if DEV_MODE
			AssertHasNoEmptyGroups(rootGroups);
			#endif

			// if add component menu config contains any invalid items, remove them
			RemoveInvalidItems(ref rootGroups);
		}

		private static void RemoveInvalidItems(ref AddComponentMenuItem[] items)
		{
			for(int n = items.Length - 1; n >= 0; n--)
			{
				var item = items[n];
				if(item.IsGroup)
				{
					RemoveInvalidItems(ref item.children);

					// remove empty group
					if(item.children.Length == 0)
					{
						#if DEV_MODE
						Debug.LogWarning("Removing empty group from Add Component menu: \""+item.label+"\"");
						#endif
						items = items.RemoveAt(n);
					}
				}
				#if DEV_MODE
				else { Debug.Assert(item.type != null && item.type.IsComponent()); }
				#endif
			}
		}

		#if DEV_MODE
		private static void AssertHasNoEmptyGroups(AddComponentMenuItem[] items)
		{
			for(int n = items.Length - 1; n >= 0; n--)
			{
				var test = items[n];
				if(test.IsGroup)
				{
					var children = test.children;
					Debug.Assert(children.Length > 0, "Group "+test.FullLabel()+" had no items!");
					AssertHasNoEmptyGroups(children);
				}
			}
		}
		#endif


		private static bool Contains(Type targetType)
		{
			for(int n = rootGroups.Length - 1; n >= 0; n--)
			{
				if(rootGroups[n].Contains(targetType))
				{
					return true;
				}
			}
			return false;
		}

		private static void Add(Type type)
		{
			string path = PopupMenuUtility.GetFullLabel(type, GlobalNameSpacePrefix);
			if(string.IsNullOrEmpty(path))
			{
				RegisterComponentHiddenInMenu(type);
				return;
			}
			path = StringUtils.SplitPascalCaseToWords(path);
			Add(type, path);
		}

		private static void RegisterComponentHiddenInMenu(Type type)
		{
			if(type.IsDefined(Types.DisallowMultipleComponent, false))
			{
				AddComponentUtility.AddDisallowMultiple(type);
			}

			if(type.IsDefined(OnlyComponentAttribute, true))
			{
				AddComponentUtility.AddOnlyComponent(type);
			}
		}

		private static void Add(Type type, string fullMenuName)
		{
			if(type.IsDefined(Types.DisallowMultipleComponent, false))
			{
				AddComponentUtility.AddDisallowMultiple(type);
			}

			if(type.IsDefined(OnlyComponentAttribute, true))
			{
				AddComponentUtility.AddOnlyComponent(type);
			}

			int split = fullMenuName.LastIndexOf('/');
			
			if(split == -1)
			{
				#if DEV_MODE
				Debug.LogWarning("Adding type "+type.Name+" with fullMenuName \""+fullMenuName+"\" to root");
				#endif

				GetOrCreateRootItem(fullMenuName, type);
			}
			else
			{
				var groupLabels = fullMenuName.Substring(0, split);
				var itemLabel = fullMenuName.Substring(split + 1);

				//TEMP
				#if DEV_MODE
				if(fullMenuName.StartsWith("Unity Engine/", StringComparison.Ordinal) || fullMenuName.StartsWith("Unity Editor/", StringComparison.Ordinal))
				{
					Debug.LogError("Creating Group \""+ fullMenuName.Substring(12)+ "\" for type " + type.Name + " with fullMenuName \"" + fullMenuName + "\".");
				}
				#endif
			
				var group = GetOrCreateGroup(groupLabels, null);
				group.AddChild(itemLabel, type);
				pathBuilder.Clear();
			}
		}

		private static AddComponentMenuItem GetOrCreateGroup(string fullMenuLabel, Texture icon)
		{
			int from = 0;
			for(int to = fullMenuLabel.IndexOf('/'); to != -1; to = fullMenuLabel.IndexOf('/', from))
			{
				var part = fullMenuLabel.Substring(from, to - from);
				pathBuilder.Add(part);
				from = to + 1;
			}
			
			string label = from == 0 ? fullMenuLabel : fullMenuLabel.Substring(from);
			pathBuilder.Add(label);

			return GetOrCreateGroup(ref pathBuilder, null, ref rootGroups, icon);
		}

		private static AddComponentMenuItem GetOrCreateRootItem(string label, Type type)
		{
			for(int n = rootGroups.Length - 1; n >= 0; n--)
			{
				var test = rootGroups[n];
				if(test.type == type)
				{
					return test;
				}
			}
			
			//create the item if it didn't exist
			var newItem = AddComponentMenuItem.Item(type, label, null);
			ArrayExtensions.Add(ref rootGroups, newItem);
			
			return newItem;
		}

		private static AddComponentMenuItem GetOrCreateGroup(ref List<string> groupsLabels, AddComponentMenuItem parent, ref AddComponentMenuItem[] options, Texture icon)
		{
			//first try to find an existing group with given group labels
			string rootGroup = groupsLabels[0];
			for(int i = options.Length - 1; i >= 0; i--)
			{
				var group = options[i];
				if(string.Equals(group.label, rootGroup))
				{
					groupsLabels.RemoveAt(0);
					if(groupsLabels.Count > 0)
					{
						var result = GetOrCreateGroup(ref groupsLabels, group, ref group.children, icon);
						groupsLabels.Clear();
						return result;
					}
					groupsLabels.Clear();
					return group;
				}
			}

			//create the group if it didn't exist
			var newGroup = AddComponentMenuItem.Group(rootGroup, parent, icon);

			#if DEV_MODE
			int countBeforeAdd = options.Length;
			#endif

			ArrayExtensions.Add(ref options, newGroup);

			#if DEV_MODE
			Debug.Assert(options.Length == countBeforeAdd + 1);
			#endif

			if(groupsLabels.Count > 1)
			{
				groupsLabels.RemoveAt(0);
				newGroup = GetOrCreateGroup(ref groupsLabels, parent, ref newGroup.children, icon);
			}
			groupsLabels.Clear();
			return newGroup;
		}
		
		private static AddComponentMenuItem GetMenuItem(string label)
		{
			try
			{
				return itemsByLabel[label];
			}
			catch(Exception e)
			{
				Debug.LogError("AddComponentMenuItems.GetMenuItem("+label+"): "+e);
				return null;
			}

		}
	}
}