//#define DEBUG_KEYBOARD_INPUT

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Handles drawing the Add Component menu that pops open when the Add Component button is pressed.
	/// </summary>
	public class AddComponentMenuDrawer
	{
		public const float Width = 228f;
		public const float TotalHeight = TopPartHeight + ScrollAreaHeight;

		private const string PrefsString = "ComponentSearchString";
		private const string ControlName = "AddCompFilter";
		private const int MaxFullyVisibleMembers = 18;
		
		private const float FilterAreaHeight = 32f;
		private const float NavigationBarHeight = 24f;
		private const float TopPartHeight = FilterAreaHeight + NavigationBarHeight;
		private const float ScrollAreaHeight = 300f;

		public static Action<Type> OnComponentAdded;
		public static AddComponentHistoryTracker addHistory;

		private readonly static Color32 bgColorDark = new Color32(49, 49, 49, 255);
		private readonly static Color32 bgColorLight = new Color32(210, 210, 210, 255);

		private readonly static Color32 bgColorNavigationBarDark = new Color32(62, 62, 62, 255);
		private readonly static Color32 bgColorNavigationBarLight = new Color32(170, 170, 170, 255);

		private static List<IComponentDrawer> conflictingMembers = new List<IComponentDrawer>();

		private static AddComponentMenuDrawer instance;

		private static AddComponentMenuItem activeItem;
		
		private string filter = "";
		private string setFilter;
		private bool goBackLevelNextLayout;
		private bool clearTextNextLayout;
		private Vector2 scrollPos;
		private GUIContent categoryLabel = new GUIContent("Component");

		private Rect filterFieldRect;
		private Rect filterDiscardRect;
		private Rect dividerRect;
		private Rect headerRect;
		private Rect headerLabelRect;
		private Rect backArrowRect;
		private Rect viewRect;
		private Rect contentRect;
		
		private int selectedComponentIndex;
		private int firstVisibleIndex;
		private int lastVisibleIndex = MaxFullyVisibleMembers;

		private bool[] visibleMembersHaveConflicts = new bool[MaxFullyVisibleMembers + 2];

		private bool clearingText;
		
		private IDrawer[] members = new IDrawer[0];
		private IDrawer[] visibleMembers = new IDrawer[0];

		private IGameObjectDrawer target;

		private Action onClosed;

		#if UNITY_EDITOR
		private bool openCreateNewScriptViewNextLayout;
		private Rect createScriptButtonRect;
		#endif

		private IInspector inspector;

		private Vector2 localDrawAreaOffset;

		private static Color32 BgColor
		{
			get
			{
				return DrawGUI.IsProSkin ? bgColorDark : bgColorLight;
			}
		}

		private static Color32 BgColorNavigationBar
		{
			get
			{
				return DrawGUI.IsProSkin ? bgColorNavigationBarDark : bgColorNavigationBarLight;
			}
		}

		public string FilterString
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				setFilter = value;
				SetActiveItem(null);
			}
		}

		public static AddComponentMenuDrawer Create(IInspector setInspector, IGameObjectDrawer target, Rect openPosition, Action onClosed)
		{
			if(instance == null)
			{
				instance = new AddComponentMenuDrawer();
			}
			instance.Setup(setInspector, target, openPosition, onClosed);
			return instance;
		}

		public static AddComponentMenuDrawer CreateNewBackgroundInstance(IInspector setInspector, IGameObjectDrawer target)
		{
			var result = new AddComponentMenuDrawer();
			result.SetupWithoutOpening(setInspector, target);
			return result;
		}

		private AddComponentMenuDrawer()
		{
			if(addHistory == null)
			{
				addHistory = new AddComponentHistoryTracker();
			}
		}
		
		public void SetupWithoutOpening(IInspector setInspector, IGameObjectDrawer setTarget)
		{
			SetTarget(setInspector, setTarget);
			setFilter = "";
		}

		public void Setup(IInspector setInspector, IGameObjectDrawer setTarget, Rect openPosition, Action setOnClosed)
		{
			onClosed = setOnClosed;
			SetTarget(setInspector, setTarget);

			Open(openPosition);
			setFilter = FilterString;
		}

		public void SetTarget(IInspector setInspector, IGameObjectDrawer value)
		{
			target = value;
			inspector = setInspector;
			RebuildIntructionsInChildren();
		}

		public void SetScrollPos(float setY, bool updateVisibleMembers)
		{
			GUI.changed = true;

			scrollPos.y = setY;

			firstVisibleIndex = Mathf.FloorToInt(scrollPos.y / DrawGUI.SingleLineHeight);
			lastVisibleIndex = Mathf.CeilToInt(scrollPos.y / DrawGUI.SingleLineHeight) + MaxFullyVisibleMembers;
			lastVisibleIndex = Mathf.Min(members.Length - 1, lastVisibleIndex);

			if(updateVisibleMembers)
			{
				UpdateVisibleMembers();
			}
		}

		public void RebuildIntructionsInChildren()
		{
			DisposeChildren();
			AddComponentMenuItem[] values;
			if(activeItem == null)
			{
				values = AddComponentMenuItems.GetFiltered(FilterString);
			}
			else
			{
				values = activeItem.children;
			}

			int count = values.Length;
			DrawerArrayPool.Resize(ref members, count);

			for(int n = 0; n < count; n++)
			{
				members[n] = AddComponentMenuItemDrawer.Create(values[n]);
			}
			UpdateContentRect();

			SetScrollPos(0f, false);
			SetSelectedMember(0, false);
			UpdateVisibleMembers();
		}

		private void UpdateContentRect()
		{
			contentRect.height = members.Length * DrawGUI.SingleLineHeight;

			bool hasVerticalScrollBar = contentRect.height > ScrollAreaHeight;
			if(hasVerticalScrollBar)
			{
				//compensate for scrollbar width to avoid horizontal scrollbar appearing
				contentRect.width = Width - DrawGUI.ScrollBarWidth;
			}
			else
			{
				contentRect.width = Width;
			}
		}

		public void UpdateVisibleMembers()
		{
			GUI.changed = true;

			int count = lastVisibleIndex - firstVisibleIndex + 1;
			DrawerArrayPool.Resize(ref visibleMembers, count);
			for(int n = count - 1; n >= 0; n--)
			{
				var member = members[firstVisibleIndex + n];
				visibleMembers[n] = member;

				visibleMembersHaveConflicts[n] = AddComponentUtility.HasConflictingMembers(member.Type, target);
			}
		}

		private void GetDrawPositions(Rect openPosition)
		{
			viewRect = openPosition;
			viewRect.width = Width;
			viewRect.height = TotalHeight;
			viewRect.x = 0f;
			viewRect.y = 0f;
			
			filterFieldRect = viewRect;
			filterFieldRect.height = FilterAreaHeight;
			filterFieldRect.x += 7f;
			filterFieldRect.y = 6f;
			filterFieldRect.width -= 16f + 14f;

			filterDiscardRect = filterFieldRect;
			filterDiscardRect.x += filterFieldRect.width;
			filterDiscardRect.width = 16f;
			filterDiscardRect.y += 1f;

			headerRect = viewRect;
			headerRect.y += FilterAreaHeight;
			headerRect.height = NavigationBarHeight;

			backArrowRect = headerRect;
			backArrowRect.width = 13f;
			backArrowRect.height = 13f;
			backArrowRect.x += 3f;
			backArrowRect.y += 5f;

			#if UNITY_EDITOR
			createScriptButtonRect = headerRect;
			createScriptButtonRect.width = 10f;
			createScriptButtonRect.height = 10f;
			createScriptButtonRect.x = headerRect.width - 18f;
			createScriptButtonRect.y += 4f;
			#endif

			headerLabelRect = headerRect;
			float leftOffset = backArrowRect.xMax - headerRect.x;
			headerLabelRect.x += leftOffset;
			float removeWidth = leftOffset;
			#if UNITY_EDITOR
			removeWidth += createScriptButtonRect.x - createScriptButtonRect.xMax;
			#endif
			headerLabelRect.width -= removeWidth;

			dividerRect = headerRect;
			dividerRect.y = TopPartHeight;
			dividerRect.height = 1f;
			dividerRect.width = Width;

			viewRect.y = TopPartHeight;
			viewRect.height = ScrollAreaHeight;

			contentRect = viewRect;
			contentRect.height = members.Length * DrawGUI.SingleLineHeight;

			bool hasVerticalScrollBar = contentRect.height > ScrollAreaHeight;
			if(hasVerticalScrollBar)
			{
				contentRect.width = Width - DrawGUI.ScrollBarWidth;
			}

			viewRect.y = TopPartHeight;

			localDrawAreaOffset = DrawGUI.GetLocalDrawAreaOffset();
		}

		public bool OnGUI(ref bool addedComponent)
		{
			bool dirty = false;

			var e = Event.current;
			var eventType = e.type;
			switch(eventType)
			{
				case EventType.KeyDown:
					if(OnKeyboardInputGiven(e))
					{
						return true;
					}
					break;
				case EventType.MouseDown:
				case EventType.MouseMove:
					dirty = true;
					break;
				case EventType.Layout:
					if(goBackLevelNextLayout)
					{
						goBackLevelNextLayout = false;
						GoBackLevel();
					}
					#if UNITY_EDITOR
					else if(openCreateNewScriptViewNextLayout)
					{
						openCreateNewScriptViewNextLayout = false;

						string inNamespace = null;
						string saveIn = null;
						if(activeItem != null)
						{
							var type = activeItem.TypeOrAnyChildType();
							if(type != null)
							{
								var existingScript = FileUtility.FindScriptFile(type);
								if(existingScript != null)
								{
									inNamespace = type.Namespace;
									saveIn = FileUtility.GetParentDirectory(UnityEditor.AssetDatabase.GetAssetPath(existingScript));
								}
							}
						}
						
						if(inNamespace == null)
						{
							inNamespace = InspectorUtility.Preferences.defaultNamespace;
						}

						if(saveIn == null)
						{
							saveIn = InspectorUtility.Preferences.defaultScriptPath;
						}

						#if DEV_MODE
						Debug.Log("saveIn="+saveIn+", inNamespace="+inNamespace);
						#endif
						
						if(filter.Length > 0)
						{
							Platform.Active.SetPrefs("PI.CreateScriptWizard/Name", filter);
						}
						Platform.Active.SetPrefs("PI.CreateScriptWizard/Namespace", inNamespace);
						Platform.Active.SetPrefs("PI.CreateScriptWizard/SaveIn", saveIn);
						
						Platform.Active.SetPrefs("PI.CreateScriptWizard/Template", "MonoBehaviour");
						Platform.Active.SetPrefs("PI.CreateScriptWizard/AttachTo", target.UnityObject.GetInstanceID());

						inspector.OnNextLayout(()=>DrawGUI.ExecuteMenuItem(PowerInspectorMenuItemPaths.CreateScriptWizardFromCreateMenu));

						Close();
						return true;
					}
					#endif
					else if(clearTextNextLayout)
					{
						clearTextNextLayout = false;
						ClearText();
					}
					break;
			}

			if(Draw(ref addedComponent))
			{
				dirty = true;
			}

			return dirty;
		}

		#if UNITY_EDITOR
		[UnityEditor.Callbacks.DidReloadScripts, UsedImplicitly]
		private static void OnReadyToAddNewScript()
		{
			if(instance != null && !Platform.Active.IsPlayingOrWillChangePlaymode)
			{
				var scriptToAdd = Platform.Active.GetPrefs("AddComponent.AddOfType", "");
				if(scriptToAdd.Length > 0)
				{
					Platform.Active.DeletePrefs("AddComponent.AddOfType");
					foreach(var type in TypeExtensions.ComponentTypes)
					{
						if(string.Equals(type.FullName, scriptToAdd))
						{
							instance.AddComponent(type);
						}
					}
				}
			}
		}
		#endif

		private bool Draw(ref bool addedComponent)
		{
			if(instance == null)
			{
				return false;
			}

			bool dirty = false;
			var e = Event.current;
			//using raw type, since we consume all click events
			var eventType = e.rawType;
			bool lmbClick = eventType == EventType.MouseDown && e.button == 0;

			if(clearingText)
			{
				if(eventType == EventType.Layout)
				{
					clearingText = false;
				}
				GUI.changed = true;
				dirty = true;
			}
			else
			{
				if(!DrawGUI.EditingTextField)
				{
					DrawGUI.EditingTextField = true;
					GUI.changed = true;
					dirty = true;
				}
				
				if(!string.Equals(GUI.GetNameOfFocusedControl(), ControlName))
				{
					DrawGUI.FocusControl(ControlName);
					GUI.changed = true;
					dirty = true;
				}
			}

			if(DrawFilterField())
			{
				dirty = true;
			}
			
			DrawGUI.Active.ColorRect(headerRect, BgColorNavigationBar);

			#if UNITY_EDITOR
			GUI.Label(createScriptButtonRect, GUIContent.none, "OL Plus");
			if(lmbClick && createScriptButtonRect.Contains(Event.current.mousePosition))
			{
				dirty = true;
				openCreateNewScriptViewNextLayout = true;
			}
			#endif

			bool hasFilter = FilterString.Length > 0;

			if(!hasFilter && activeItem != null)
			{
				GUI.Label(backArrowRect, GUIContent.none, "AC LeftArrow");
				if(lmbClick && backArrowRect.Contains(Event.current.mousePosition))
				{
					#if UNITY_EDITOR
					if(!openCreateNewScriptViewNextLayout)
					#endif
					{
						goBackLevelNextLayout = true;
						dirty = true;
					}
				}
			}
			
			DrawGUI.DrawRect(headerRect, new Color32(24, 24, 24, 255), localDrawAreaOffset);

			GUI.Label(headerLabelRect, categoryLabel, InspectorPreferences.Styles.PopupMenuTitle);

			GUI.DrawTexture(dividerRect, InspectorUtility.Preferences.graphics.horizontalSplitterBg);
			
			var setScrollPos = GUI.BeginScrollView(viewRect, scrollPos, contentRect);
			{
				if(setScrollPos.y != scrollPos.y)
				{
					SetScrollPos(setScrollPos.y, true);
					dirty = true;
				}
				
				var memberRect = contentRect;
				DrawGUI.Active.ColorRect(memberRect, BgColor);

				memberRect.height = DrawGUI.SingleLineHeight;

				//only start drawing from first visible member
				memberRect.y += firstVisibleIndex * DrawGUI.SingleLineHeight;

				int last = lastVisibleIndex - firstVisibleIndex;
				int visibleCount = visibleMembers.Length;
				if(last >= visibleCount)
				{
					#if DEV_MODE
					Debug.LogWarning(ToString()+" - last ("+last+") >= visibleCount ("+visibleCount+ ") with firstVisibleIndex="+ firstVisibleIndex+ " and lastVisibleIndex="+ lastVisibleIndex);
					#endif

					last = visibleCount - 1;
				}

				for(int n = 0; n <= last; n++)
				{
					int memberIndex = firstVisibleIndex + n;

					//TEMP
					if(last >= visibleMembers.Length)
					{
						#if DEV_MODE
						Debug.LogError("n=" + n + ", last=" + last + ", visibleMembers.Length=" + visibleMembers.Length + ", ");
						#endif
						break;
					}

					var member = visibleMembers[n];

					bool selected = memberIndex == selectedComponentIndex;

					if(selected)
					{
						DrawGUI.Active.ColorRect(memberRect, InspectorUtility.Preferences.theme.BackgroundSelected);
					}

					if(memberRect.Contains(e.mousePosition))
					{
						if(memberIndex != selectedComponentIndex)
						{
							SetSelectedMember(memberIndex, false);
							dirty = true;
						}

						if(lmbClick)
						{
							var type = member.Type;

							#if DEV_MODE
							Debug.Log(GetType().Name + " - member " + member.Label.text + " clicked! (custom detection)");
							#endif

							dirty = true;

							if(type == null)
							{
								SetActiveItem((member as AddComponentMenuItemDrawer).Item);
								break;
							}

							addedComponent = true;

							inspector.OnNextLayout(()=>AddComponent(type));
							break;
						}
					}

					if(visibleMembersHaveConflicts[n])
					{
						GUI.enabled = false;
					}

					if(member.Draw(memberRect))
					{
						var type = member.Type;

						#if DEV_MODE
						Debug.Log(GetType().Name + " - member " + member.Label.text + " clicked with eventType " + eventType + "! (member returned true)");
						#endif

						dirty = true;

						if(type == null)
						{
							SetActiveItem((member as AddComponentMenuItemDrawer).Item);
							break;
						}
						
						addedComponent = true;
						inspector.OnNextLayout(()=>AddComponent(type));
						DrawGUI.Use(e);
						break;
					}
					
					GUI.enabled = true;
					memberRect.y += DrawGUI.SingleLineHeight;
				}
				
				GUI.EndScrollView();
			}
			
			return dirty;
		}
		
		private bool DrawFilterField()
		{
			GUI.SetNextControlName(ControlName);
			setFilter = DrawGUI.Active.TextField(filterFieldRect, setFilter, "SearchTextField");
			bool dirty = false;

			if(setFilter.Length == 0)
			{
				GUI.Label(filterDiscardRect, GUIContent.none, "SearchCancelButtonEmpty");
			}
			else
			{
				GUI.Label(filterDiscardRect, GUIContent.none, "SearchCancelButton");
				if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && filterDiscardRect.Contains(Event.current.mousePosition))
				{
					clearTextNextLayout = true;
					dirty = true;
				}
			}

			if(!string.Equals(setFilter, FilterString))
			{
				if(Event.current.type == EventType.Layout)
				{
					FilterString = setFilter;
					dirty = true;
				}
				else
				{
					dirty = true;
				}
			}

			#if UNITY_EDITOR
			GUI.enabled = true;
			#endif

			return dirty;
		}
		
		private void ClearText()
		{
			#if DEV_MODE
			Debug.Log(GetType().Name + ".ClearText()");
			#endif

			clearingText = true;
			FilterString = "";
			KeyboardControlUtility.KeyboardControl = 0;
			DrawGUI.EditingTextField = false;
			GUI.changed = true;
		}
		public void Open(Rect openPosition)
		{
			GetDrawPositions(openPosition);
			UpdateVisibleMembers();
			FilterString = Platform.Active.GetPrefs(PrefsString, FilterString);

			if(FilterString.Length == 0)
			{
				var openMenu = InspectorUtility.Preferences.defaultAddComponentMenuName;
				if(openMenu.Length > 0)
				{
					for(int n = members.Length - 1; n >= 0; n--)
					{
						if(string.Equals(members[n].Name, openMenu))
						{
							SetSelectedMember(n, false);
						}
					}
				}
			}

			KeyboardControlUtility.KeyboardControl = 0;
			GUI.changed = true;
		}

		public void OnClosed()
		{
			Platform.Active.SetPrefs(PrefsString, FilterString);
		}

		public void AddComponent(Type type)
		{
			conflictingMembers.Clear();
			AddComponentUtility.GetConflictingMembers(type, target, ref conflictingMembers);

			int conflictCount = conflictingMembers.Count;

			#if DEV_MODE
			Debug.Log(GetType().Name + ".AddComponent(" + (type == null ? "null" : type.Name) + ") called with conflictCount=" + conflictCount + "!");
			#endif

			bool viewWasLocked = inspector.State.ViewIsLocked;

			if(conflictCount == 0)
			{
				if(OnComponentAdded != null)
				{
					OnComponentAdded(type);
				}

				target.AddComponent(type, true);
				Close();
				return;
			}

			//more reliable method to get components first? then even if Drawer change, component refs will remain the same
			//however locking the inspector should do the same thing
			//var conflictingComponents = conflictingMembers.Select((m)=>m.GetValues()).ToArray();

			//TO DO: handle Transform to RectTransform and vice versa
			//but warn that it requires rebuilding the Components
			//and could result in lost references? Although I guess
			//could even try and retain those too, by seraching through
			//all fields on all objects in hierarchy :D
			if(type == typeof(Transform))
			{
				Debug.LogError("Replacing Transform Not Yet Supported");
				return;
			}
			if(type == typeof(RectTransform))
			{
				Debug.LogError("Replacing RectTransform Not Yet Supported");
				return;
			}

			//TO DO: handle dependencies of all conflicting components too!
			//so first list conflicting components
			//then if they have dependencies, list also "which have dependencies with the following components:"

			string header;
			string msg;
			string ok;
			if(conflictCount == 1)
			{
				header = "Conflicting Component";
				msg = "Component " + type.Name + " cannot be added because it conflicts with existing Component " + conflictingMembers[0].Type.Name;
				ok = "Replace Existing";
			}
			else
			{
				header = "Conflicting Components";
				msg = "Component " + type.Name + " cannot be added because it conflicts with the following exiting Components:" + conflictingMembers[0].Name;
				foreach(var member in conflictingMembers)
				{
					msg += "\n" + member.Type.Name;
				}
				ok = "Replace " + conflictCount + " Existing";
			}

			if(DrawGUI.Active.DisplayDialog(header, msg, ok, "Cancel"))
			{
				inspector.State.ViewIsLocked = true;

				foreach(var member in conflictingMembers)
				{
					var values = member.GetValues();
					for(int n = values.Length - 1; n >= 0; n--)
					{
						var comp = values[n] as Component;
						Platform.Active.Destroy(comp);
					}
				}

				if(OnComponentAdded != null)
				{
					OnComponentAdded(type);
				}

				target.AddComponent(type, true);
				inspector.State.ViewIsLocked = viewWasLocked;

				Close();
			}
		}
		
		/// <summary>
		/// called when this is the selected control and 
		/// keyboard input is given
		/// </summary>
		private bool OnKeyboardInputGiven(Event inputEvent)
		{
			#if DEV_MODE && DEBUG_KEYBOARD_INPUT
			Debug.Log("AddComponentMenuDrawer.OnKeyboardInputGiven("+inputEvent.keyCode+ ") with selectedComponentIndex="+ selectedComponentIndex);
			#endif

			#if UNITY_EDITOR
			if(InspectorUtility.Preferences.keyConfigs.addComponent.DetectAndUseInput(inputEvent))
			{
				GUI.changed = true;
				openCreateNewScriptViewNextLayout = true;
				return true;
			}
			#endif

			switch(inputEvent.keyCode)
			{
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
					GUI.changed = true;
					DrawGUI.Use(inputEvent);
					if(selectedComponentIndex >= 0 && members.Length > selectedComponentIndex)
					{
						var member = members[selectedComponentIndex];
						var type = member.Type;

						#if DEV_MODE && DEBUG_KEYBOARD_INPUT
						Debug.Log("AddComponentMenuDrawer - Activate input given with selected component type: "+StringUtils.ToString(type));
						#endif

						//if selected member is a category, open the category
						if(type == null)
						{
							SetActiveItem((member as AddComponentMenuItemDrawer).Item);
							return true;
						}
						//otherwise selected member is a type; add component of that type
						AddComponent(type);
						return true;
					}
					//Close();
					return false;
				case KeyCode.UpArrow:
					if(selectedComponentIndex > 0)
					{
						SetSelectedMember(selectedComponentIndex - 1, true);
						return true;
					}
					return false;
				case KeyCode.DownArrow:
					int nextIndex = selectedComponentIndex + 1;
					if(nextIndex < members.Length)
					{
						SetSelectedMember(nextIndex, true);
						return true;
					}
					return false;
				case KeyCode.Escape:
					DrawGUI.Use(inputEvent);
					if(FilterString.Length > 0)
					{
						clearTextNextLayout = true;
					}
					else if(activeItem != null)
					{
						GoBackLevel();
					}
					else
					{
						Close();
					}
					KeyboardControlUtility.KeyboardControl = 0;
					DrawGUI.EditingTextField = false;
					return true;
			}
			return false;
		}

		private void SetSelectedMember(int index, bool scrollToShow)
		{
			int count = members.Length;
			if(count == 0)
			{
				selectedComponentIndex = 0;
				AddComponentMenuItemDrawer.activeItem = null;
				return;
			}

			if(index <= 0)
			{
				index = 0;
			}
			else if(index >= count)
			{
				index = count - 1;
			}

			GUI.changed = true;

			selectedComponentIndex = index;
			AddComponentMenuItemDrawer.activeItem = members[index];

			if(scrollToShow)
			{
				float yMin = scrollPos.y;
				float yTargetMin = selectedComponentIndex * DrawGUI.SingleLineHeight;

				if(yTargetMin < yMin)
				{
					SetScrollPos(yTargetMin, true);
				}
				else
				{
					//this seemed to cause a bug where the menu would start scrolling towards the bottom upon being opened
					//but why is this even called?
					float yMax = scrollPos.y + viewRect.height + DrawGUI.SingleLineHeight;
					float yTargetMax = yTargetMin + DrawGUI.SingleLineHeight;

					if(yTargetMax >= yMax)
					{
						float setY = yTargetMax - viewRect.height;
						#if DEV_MODE
						Debug.Log("yTargetMax (" + yTargetMax + ") > yMax (" + yMax + "): setting scrollPos to " + setY + " (from " + scrollPos.y + ")");
						#endif
						SetScrollPos(setY, true);
					}
				}
			}
		}
		
		private void Close()
		{
			if(onClosed != null)
			{
				var callback = onClosed;
				onClosed = null;
				callback();
			}
		}

		private void SetActiveItem(AddComponentMenuItem value)
		{
			activeItem = value;
			UpdateCategoryLabel();
			RebuildIntructionsInChildren();
			GUI.changed = true;
		}

		private void UpdateCategoryLabel()
		{
			if(FilterString.Length > 0)
			{
				categoryLabel.text = "Search";
			}
			else if(activeItem != null)
			{
				categoryLabel.text = activeItem.label;
			}
			else
			{
				categoryLabel.text = "Component";
			}			
		}

		private void GoBackLevel()
		{
			if(activeItem == null)
			{
				return;
			}

			SetActiveItem(activeItem.parent);
		}

		private void DisposeChildren()
		{
			for(int n = members.Length - 1; n >= 0; n--)
			{
				if(members[n] != null)
				{
					members[n].Dispose();
					members[n] = null;
				}
			}
		}
	}
}