#define SAFE_MODE
#define UPDATE_VALUES_ON_INSPECTOR_UPDATE
#define SHOW_SCREENSHOT_WHILE_SCRIPTS_RELOADING

//#define DEBUG_ON_SELECTION_CHANGE
//#define DEBUG_FOCUS
//#define DEBUG_MOUSEOVERED_INSTANCE
//#define DEBUG_SAVE_STATE
//#define DEBUG_RESTORE_STATE
//#define DEBUG_SPLIT_VIEW
//#define DEBUG_ON_HIERARCHY_CHANGE
//#define DEBUG_PLAY_MODE_CHANGED
//#define DEBUG_ON_SCRIPTS_RELOADED
//#define DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
//#define DEBUG_KEY_DOWN
//#define DEBUG_REPAINT
//#define DEBUG_DRAW_SCREENSHOT

using System;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Sisus
{
	/// <summary>
	/// Class that can be inherited from to handle drawing of inspector views.
	/// </summary>
	public abstract class InspectorDrawerWindow : EditorWindow, ISplittableInspectorDrawer
	{
		[CanBeNull]
		protected IEditorWindowMessageDispenser messageDispenser;

		private float lastUpdateTime;

		[SerializeField]
		protected EditorWindowMinimizer minimizer;

		[SerializeField]
		protected InspectorTargetingMode inspectorTargetingMode = InspectorTargetingMode.All;
		
		private IInspector mainView;
		private IInspector splitView;
		
		[NonSerialized]
		private bool viewIsSplit;

		private bool isDirty = true;
		
		private bool swapSplitSidesOnNextRepaint;
		
		[NonSerialized]
		private bool subscribedToEvents;

		private static InspectorDrawerWindow mouseoveredInstance;

		protected InspectorManager inspectorManager;
		
		private float onUpdateDeltaTime;
		private float onGUIDeltaTime;
		private float lastOnGUITime;

		private bool nowRestoringPreviousState;
		
		private bool nowClosing;
		private bool nowLosingFocus;

		#if UNITY_2017_1_OR_NEWER && SHOW_SCREENSHOT_WHILE_SCRIPTS_RELOADING
		private Texture2D screenshotWhileScriptsReloading;
		#endif

		#if DEV_MODE && DEBUG_ON_SELECTION_CHANGE
		private float lastSelectionChangeTime = -1000f;
		#endif

		/// <summary> Gets a value indicating whether we can split view. </summary>
		/// <value> True if we can split view, false if not. </value>
		public abstract bool CanSplitView
		{
			get;
		}

		/// <summary> Gets the type of the inspectors that are drawn by this class. </summary>
		/// <value> The type of the drawn inspector. </value>
		[NotNull]
		public abstract Type InspectorType
		{
			get;
		}

		/// <inheritdoc />
		public UpdateEvent OnUpdate { get; set; }

		/// <inheritdoc />
		public ISelectionManager SelectionManager
		{
			get;
			protected set;
		}

		/// <inheritdoc />
		public bool UpdateAnimationsNow
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc />
		public float AnimationDeltaTime
		{
			get
			{
				return onGUIDeltaTime;
			}
		}

		/// <inheritdoc />
		public Object UnityObject
		{
			get
			{
				return this;
			}
		}

		/// <inheritdoc />
		public bool HasFocus
		{
			get
			{
				return focusedWindow == this && !nowLosingFocus;
			}
		}

		/// <inheritdoc/>
		public IInspectorManager Manager
		{
			get
			{
				return inspectorManager; 
			}
		}

		/// <inheritdoc/>
		public bool ViewIsSplit
		{
			get
			{
				return viewIsSplit;
			}
		}

		/// <summary> Returns true if cursor is currently over the window's viewport
		/// </summary>
		/// <value> True if mouse is over window, false if not. </value>
		public bool MouseIsOver
		{
			get
			{
				//Using if(mouseOverWindow == this) doesn't seem to be a reliable method.
				//It seemed to be null sometimes when dragging Object references over the window.
				//Also it's set even when mouseovering Window borders or the top Tab.
				return ReferenceEquals(mouseoveredInstance, this);
			}
		}

		/// <inheritdoc/>
		public IInspector MainView
		{
			get
			{
				return mainView;
			}
		}

		/// <inheritdoc/>
		public IInspector SplitView
		{
			get
			{
				return splitView;
			}
		}

		/// <inheritdoc/>
		public InspectorTargetingMode InspectorTargetingMode
		{
			get
			{
				return inspectorTargetingMode;
			}
		}

		/// <summary> Gets the title text displayed on the tab of the EditorWindow. </summary>
		/// <value> The title text. </value>
		protected virtual string TitleText
		{
			get
			{
				return "Inspector";
			}
		}

		private bool SetupDone
		{
			get
			{
				return mainView != null && mainView.SetupDone;
			}
		}
		
		public bool NowClosing
		{
			get
			{
				return nowClosing;
			}
		}		

		[NotNull]
		public static IInspectorDrawer CreateNewWithoutShowing([NotNull] Type windowType, bool addAsTab)
		{
			var manager = InspectorUtility.ActiveManager;
			if(manager == null)
			{
				manager = InspectorManager.Instance();
			}

			// Support adding as new tab next to the default inspector or any other window named "Inspector"
			EditorWindow existingWindow;
			if(focusedWindow != null && string.Equals(focusedWindow.titleContent.text, "Inspector"))
			{
				existingWindow = focusedWindow;
			}
			// If no such window is currently focused, then try to find last inspector drawer of same as the created window.
			else
			{
				existingWindow = manager.GetLastSelectedInspectorDrawer(windowType) as EditorWindow;
			}

			var created = (EditorWindow)CreateInstance(windowType);

			if(existingWindow != null)
			{
				var existingInspectorDrawerWindow = existingWindow as InspectorDrawerWindow;
				//Sometimes instance can refer to an invisible window with an invalid state.
				//This might happen e.g. when the editor is started with the window being open but with
				//scripts containing compile errors. The problem can also be fixed by reverting Layout to
				//factory preferences, but let's handle that manually.
				if(existingInspectorDrawerWindow != null && !existingInspectorDrawerWindow.SetupDone)
				{
					#if DEV_MODE
					Debug.LogError("!existingInstance.SetupDone");
					#endif
					
					//use Close or DestroyImmediate?
					DestroyImmediate(existingInspectorDrawerWindow);
				}
				//if there was an existing InspectorDrawerWindow instance
				//add the new instance as a new tab on the same HostView
				else if(addAsTab)
				{
					existingWindow.AddTab(created);
				}
			}
			
			return (IInspectorDrawer)created;
		}

		/// <inheritdoc/>
		public void FocusWindow()
		{
			#if DEV_MODE && DEBUG_FOCUS
			Debug.Log("FocusWindow");
			#endif
			Focus();
		}

		[NotNull]
		public static InspectorDrawerWindow CreateNew([NotNull]Type windowType, [NotNull]string title, [NotNull]Object[] inspect, bool lockView = false, bool addAsTab = true, Vector2 minSize = default(Vector2), Vector2 maxSize = default(Vector2), bool utility = false)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Event.current != null);
			Debug.Assert(typeof(InspectorDrawerWindow).IsAssignableFrom(windowType));
			#endif

			var drawerWindow = (InspectorDrawerWindow)CreateNewWithoutShowing(windowType, addAsTab);
			if(!drawerWindow.SetupDone)
			{
				drawerWindow.Setup(inspect, lockView, Vector2.zero, minSize.x, minSize.y, maxSize.x, maxSize.y);
			}

			if(utility)
			{
				drawerWindow.ShowUtility();
			}
			else
			{
				drawerWindow.Show();
			}
			
			drawerWindow.FocusWindow();

			return drawerWindow;
		}

		private static InspectorDrawerWindow FindInstance()
		{
			var instances = Resources.FindObjectsOfTypeAll<InspectorDrawerWindow>();
			if(instances.Length > 0)
			{ 
				return instances[0];
			}
			return null;
		}
		
		[UsedImplicitly]
		private void Start()
		{
			#if DEV_MODE
			Debug.Log("Start with SetupDone="+StringUtils.ToColorizedString(SetupDone));
			#endif

			if(EditorUtility.scriptCompilationFailed && !SetupDone)
			{
				#if DEV_MODE
				Debug.LogError("InspectorDrawerWindow instance with probably an invalid state found. Destroy automatically? InspectorUtility.ActiveManager="+ StringUtils.ToString(InspectorUtility.ActiveManager));
				if(DrawGUI.Active.DisplayDialog("InspectorDrawerWindow with invalid state", "InspectorDrawerWindow instance with probably an invalid state found. Destroy automatically?\nInspectorUtility.ActiveManager=" + StringUtils.ToString(InspectorUtility.ActiveManager), "Destroy", "Leave It"))
				{
					DestroyImmediate(this);
				}
				#endif
			}
		}

		private void Setup()
		{
			if(!SetupDone)
			{
				Setup(ArrayPool<Object>.ZeroSizeArray, false, Vector2.zero, -1f, -1f, -1f, -1f);
			}
		}
		
		protected virtual GUIContent GetTitleContent()
		{
			return GetTitleContent(HasFocus && !ApplicationUtility.IsQuitting);
		}

		protected virtual GUIContent GetTitleContent(bool hasFocus)
		{
			return GetTitleContent(inspectorTargetingMode, hasFocus);
		}
		
		protected virtual GUIContent GetTitleContent(InspectorTargetingMode mode, bool hasFocus)
		{
			if(!SetupDone)
			{
				return GUIContentPool.Create(TitleText);
			}
			return GUIContentPool.Create(TitleText, GetWindowIcon(mode, hasFocus));
		}

		private Texture GetWindowIcon()
		{
			return GetWindowIcon(HasFocus);
		}

		private Texture GetWindowIcon(bool hasFocus)
		{
			return GetWindowIcon(inspectorTargetingMode, hasFocus);
		}

		private Texture GetWindowIcon(InspectorTargetingMode mode, bool hasFocus)
		{
			if(!SetupDone)
			{
				return null;
			}

			switch(mode)
			{
				default:
					return hasFocus ? mainView.Preferences.graphics.InspectorIconActive : mainView.Preferences.graphics.InspectorIconInactive;
				case InspectorTargetingMode.Hierarchy:
					return EditorGUIUtility.Load("icons/UnityEditor.SceneHierarchyWindow.png") as Texture;
				case InspectorTargetingMode.Project:
					return EditorGUIUtility.Load("icons/Project.png") as Texture;
			}
		}

		/// <summary> Gets preferences for inspector drawer. </summary>
		/// <returns> The preferences. </returns>
		protected virtual InspectorPreferences GetPreferences()
		{
			return InspectorUtility.Preferences;
		}

		/// <summary> Gets default selection manager. </summary>
		/// <returns> The default selection manager. </returns>
		protected virtual ISelectionManager GetDefaultSelectionManager()
		{
			return EditorSelectionManager.Instance();
		}

		/// <summary> Setups the window and its views so they are ready to be used. </summary>
		/// <param name="inspect"> The targets to inspect in the main view of the window. </param>
		/// <param name="lockView"> True to lock the view, false to leave it unlocked. </param>
		/// <param name="scrollPos"> The scroll position for the main view's viewport. </param>
		/// <param name="minWidth"> (Optional) The minimum width to which the window can be resized. </param>
		/// <param name="minHeight"> (Optional) The minimum height to which the window can be resized. </param>
		/// <param name="maxWidth"> (Optional) The maximum width to which the window can be resized. </param>
		/// <param name="maxHeight"> (Optional) The maximum height to which the window can be resised. </param>
		protected virtual void Setup(Object[] inspect, bool lockView, Vector2 scrollPos, float minWidth = 280f, float minHeight = 130f, float maxWidth = 0f, float maxHeight = 0f)
		{
			#if DEV_MODE
			Debug.Assert(!SetupDone);
			#endif

			Platform.SetEditorMode();

			SelectionManager = GetDefaultSelectionManager();

			inspectorManager = InspectorManager.Instance();

			if(minimizer == null)
			{
				minimizer = new EditorWindowMinimizer(this, SelectionManager, false);
			}
			else
			{
				minimizer.Setup(this, SelectionManager, minimizer.AutoMinimize);
			}

			if(mainView != null)
			{
				inspectorManager.Dispose(ref mainView);
			}
			var preferences = GetPreferences();
			preferences.Setup();
			mainView = CreateInspector(InspectorType, inspect, lockView, scrollPos);
			
			if(splitView != null)
			{
				inspectorManager.Dispose(ref splitView);
			}

			viewIsSplit = false;
			SubscribeToEvents();

			if(minWidth > 0f && minHeight > 0f)
			{
				minSize = new Vector2(minWidth, minHeight);
			}
			if(maxWidth > 0f && maxHeight > 0f)
			{
				maxSize = new Vector2(maxWidth, maxHeight);
			}

			titleContent = GetTitleContent();

			if(HasFocus)
			{
				if(inspectorManager.SelectedInspector != mainView)
				{
					#if DEV_MODE
					Debug.LogWarning("Selecting mainView of newly created InspectorDrawerWindow instance because window had focus");
					#endif
					inspectorManager.Select(mainView, InspectorPart.Viewport, ReasonSelectionChanged.Initialization);
				}
			}

			inspectorManager.OnNextLayout(PreCacheCommonlyUsedDrawer, this);

			if(!nowRestoringPreviousState)
			{
				inspectorManager.OnNextLayout(RebuildDrawerIfTargetsChanged, this);
			}

			SetupMessageDispenser(ref messageDispenser, preferences);
		}

		/// <summary>
		/// Creates a new instance of a message dispenser responsible for displaying messages for the inspector drawer.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		protected virtual void SetupMessageDispenser([CanBeNull]ref IEditorWindowMessageDispenser result, InspectorPreferences preferences)
		{
			var displayMethod = preferences.messageDisplayMethod;
			if(displayMethod.HasFlag(MessageDisplayMethod.Notification))
			{
				if((result as NotificationMessageDispenser) == null)
				{
					result = new NotificationMessageDispenser(this, preferences);
					return;
				}
				result.Setup(this, preferences);
				return;
			}

			if(displayMethod.HasFlag(MessageDisplayMethod.Console))
			{
				if((result as ConsoleMessageDispenser) == null)
				{
					result = new ConsoleMessageDispenser(this);
					return;
				}
				result.Setup(this, preferences);
				return;
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(displayMethod == MessageDisplayMethod.None);
			#endif

			result = null;
		}

		private void SubscribeToEvents()
		{
			autoRepaintOnSceneChange = true;
			wantsMouseMove = true;
			wantsMouseEnterLeaveWindow = true;

			if(!subscribedToEvents)
			{
				subscribedToEvents = true;
				
				Undo.undoRedoPerformed += OnUndoOrRedo;
				PlayMode.OnStateChanged += OnEditorPlaymodeStateChanged;
				EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
				EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;
				
				#if UNITY_2017_1_OR_NEWER
				AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
				AssemblyReloadEvents.afterAssemblyReload += OnScriptsReloaded;
				#endif
				
				#if UNITY_2018_2_OR_NEWER
				EditorApplication.quitting += OnEditorQuitting;
				#endif
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Undo.undoRedoPerformed != null);
			Debug.Assert(PlayMode.OnStateChanged != null);
			Debug.Assert(EditorApplication.hierarchyWindowItemOnGUI != null);
			Debug.Assert(EditorApplication.projectWindowItemOnGUI != null);
			#endif
		}

		private void OnUndoOrRedo()
		{
			#if DEV_MODE
			Debug.Log("Undo detected with Event="+StringUtils.ToString(Event.current));
			#endif
			
			inspectorManager.OnNextLayout(RefreshView);
		}

		private void OnEditorPlaymodeStateChanged(PlayModeStateChange playModeStateChange)
		{
			#if DEV_MODE && DEBUG_PLAY_MODE_CHANGED
			Debug.Log("OnEditorPlaymodeStateChanged("+ playModeStateChange + ")");
			#endif
			
			Editors.CleanUp();

			if(playModeStateChange == PlayModeStateChange.ExitingPlayMode || playModeStateChange == PlayModeStateChange.ExitingEditMode)
			{
				LinkedMemberHierarchy.OnHierarchyChange();

				mainView.RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
				if(viewIsSplit)
				{
					splitView.RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
				}
				mainView.State.ViewIsLocked = true;
			}
			else
			{
				if(playModeStateChange == PlayModeStateChange.EnteredPlayMode)
				{
					MainView.Message(new GUIContent("PLAY MODE\nChanges might not be saved.", MainView.Preferences.graphics.enteringPlayMode), null, MessageType.Info, false);
				}

				mainView.RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
				if(viewIsSplit)
				{
					splitView.RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
				}
				mainView.State.ViewIsLocked = false;

				OnHierarchyOrProjectPossiblyChanged(OnChangedEventSubject.Hierarchy);
			}
		}

		private void HierarchyWindowItemCallback(int instanceId, Rect selectionRect)
		{
			var e = Event.current;
			var type = e.type;
			
			// NOTE: This currently only works via the menu item File/Copy or using the keyboard shortcut Ctrl+C. Copying via right-click menu is not supported.
			if(type == EventType.ValidateCommand && string.Equals(e.commandName, "Copy") && selectionRect.y < 1f)
			{
				#if DEV_MODE
				Debug.Log("Copy selected Hierarchy ValidateCommand detected: " + StringUtils.ToString(e) + " with selectionRect="+ selectionRect);
				#endif

				DrawGUI.Use(e);

				var gameObjects = Selection.gameObjects;
				int count = gameObjects.Length;
				if(count == 1)
				{
					Clipboard.CopyObjectReference(gameObjects[0], Types.GameObject);
				}
				else if(count > 1)
				{
					Clipboard.CopyObjectReferences(gameObjects, Types.GameObject);
				}
			}
			//on middle mouse click on hierarchy view item, open that item in the split view
			else if(e.button == 2 && type == EventType.MouseDown && selectionRect.Contains(e.mousePosition))
			{
				var target = EditorUtility.InstanceIDToObject(instanceId);

				var window = focusedWindow;
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(window != null && window.GetType().Name == "SceneHierarchyWindow");
				#endif

				if(!CanSplitView)
				{
					#if DEV_MODE
					Debug.LogWarning("Ignoring hierarchy item middle click because CanSplitView was false...");
					#endif
					return;
				}

				if(inspectorTargetingMode == InspectorTargetingMode.Project)
				{
					#if DEV_MODE
					Debug.LogWarning("Ignoring hierarchy item middle click because inspectorTargetingMode was set to Project...");
					#endif
					return;
				}

				DrawGUI.Use(e);
				
				var inspect = ArrayPool<Object>.CreateWithContent(target);

				#if DEV_MODE
				Debug.Log("Hierarchy item \""+target.Transform().GetHierarchyPath()+ "\" middle clicked, opening in split view... (viewIsSplit="+StringUtils.ToColorizedString((bool)viewIsSplit)+", splitView="+(splitView == null ? StringUtils.Null : StringUtils.Green("NotNull")) +")");
				#endif
				
				if(!viewIsSplit)
				{
					splitView = CreateInspector(InspectorType, inspect, true);
					#if DEV_MODE
					Debug.Assert(!viewIsSplit);
					#endif
					SetSplitView(true);
				}
				else if(splitView == null)
				{
					#if DEV_MODE
					Debug.LogError("viewIsSplit was true but splitView was null!");
					#endif
					splitView = CreateInspector(InspectorType, inspect, true);
					#if DEV_MODE
					Debug.Assert(!viewIsSplit);
					#endif
					viewIsSplit = false;
					SetSplitView(true);
				}
				else if(!splitView.SetupDone)
				{
					#if DEV_MODE
					Debug.LogError("viewIsSplit was true but splitView.SetupDone was false!");
					#endif
					SetSplitView(false);
					splitView = CreateInspector(InspectorType, inspect, true);
					#if DEV_MODE
					Debug.Assert(!viewIsSplit);
					#endif
					viewIsSplit = false;
					SetSplitView(true);
				}
				else
				{
					splitView.State.ViewIsLocked = true;
					splitView.RebuildDrawers(inspect, false);
					// ran into issue where for some reason there was a "ghost" split view
					// i.e. viewIsSplit was true, and splitView was not null, yet no
					// split view was actually visible.
					viewIsSplit = false;
					SetSplitView(true);
				}

				#if DEV_MODE
				Debug.Assert(viewIsSplit);
				Debug.Assert(splitView != null);
				Debug.Assert(splitView.State.drawers.Length == 1);
				Debug.Assert(splitView.State.drawers.Length == 0 || splitView.State.drawers[0].UnityObject == target);
				Debug.Assert(splitView.State.inspected.Length == 1);
				Debug.Assert(splitView.State.inspected.Length == 0 || splitView.State.inspected[0] == target);
				#endif
			}
			// if window has a search filter, allow clearing it by clicking the element with control held down
			else if(e.button == 1 && e.control && type == EventType.MouseDown && selectionRect.Contains(e.mousePosition))
			{
				var window = focusedWindow;
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(window != null && window.GetType().Name == "SceneHierarchyWindow");
				#endif

				if(window != null)
				{
					var hasSearchFilterProperty = window.GetType().GetProperty("hasSearchFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if(hasSearchFilterProperty != null)
					{
						bool sceneViewHasFilter = (bool)hasSearchFilterProperty.GetValue(window, null);
						if(sceneViewHasFilter)
						{
							var clearSearchMethod = window.GetType().GetMethod("ClearSearchFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
							if(clearSearchMethod != null)
							{
								clearSearchMethod.Invoke(window, null);

								var target = EditorUtility.InstanceIDToObject(instanceId);
								Selection.activeGameObject = target.GameObject();
								DrawGUI.Active.PingObject(target);
								DrawGUI.Use(e);
								return;
							}
							#if DEV_MODE
							else{ Debug.LogError("clearSearchMethod null"); }
							#endif
						}
						#if DEV_MODE && DEBUG_MMB
						else{ Debug.Log("SceneHierarchyWindow sceneViewHasFilter false"); }
						#endif
					}
					#if DEV_MODE
					else{ Debug.LogError("hasSearchFilterProperty null"); }
					#endif
				}
			}
		}
		
		protected IInspector CreateInspector(Type inspectorType, bool lockView = false, Vector2 scrollPos = default(Vector2))
		{
			return CreateInspector(inspectorType, ArrayPool<Object>.ZeroSizeArray, lockView, scrollPos);
		}

		protected IInspector CreateInspector(Type inspectorType, Object[] inspect, bool lockView = false, Vector2 scrollPos = default(Vector2))
		{
			var result = inspectorManager.Create(inspectorType, this, GetPreferences(), inspect, scrollPos, lockView);

			var state = result.State;

			state.OnLockedStateChanged -= SaveState;
			state.OnLockedStateChanged += SaveState;

			state.OnScrollPosChanged -= SaveState;
			state.OnScrollPosChanged += SaveState;

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(state.ViewIsLocked == lockView, ToString() +" state.ViewIsLocked ("+StringUtils.ToColorizedString(state.ViewIsLocked)+") did not match lockView ("+StringUtils.ToColorizedString(lockView)+") after Create");
			Debug.Assert(state.ScrollPos.Equals(scrollPos), "state.ScrollPos "+state.ScrollPos+" != "+scrollPos);
			if(!state.inspected.ContentsMatch(inspect.RemoveNullObjects())) { Debug.LogError("state.inspected ("+StringUtils.TypesToString(state.inspected)+") != inspect.RemoveNullObjects ("+StringUtils.TypesToString(inspect.RemoveNullObjects())+")"); }
			#endif

			return result;
		}
		
		private void ProjectWindowItemCallback(string guid, Rect selectionRect)
		{
			var e = Event.current;
			var type = e.type;

			// NOTE: This currently only works via the menu item File/Copy or using the keyboard shortcut Ctrl+C. Copying via right-click menu is not supported.
			if(type == EventType.ValidateCommand && string.Equals(e.commandName, "Copy") && selectionRect.y < 1f)
			{
				#if DEV_MODE
				Debug.Log("Copy selected ProjectWindow item command detected: " + StringUtils.ToString(e) + " with selectionRect="+ selectionRect);
				#endif

				DrawGUI.Use(e);

				var objects = Selection.objects;
				int count = objects.Length;
				if(count == 1)
				{
					Clipboard.CopyObjectReference(objects[0], objects[0].GetType());
				}
				else if(count > 1)
				{
					Clipboard.CopyObjectReferences(objects, Types.UnityObject);
				}
			}
			//on middle mouse click on hierarchy view item, open that item in the split view
			else if(e.button == 2 && type == EventType.MouseDown && selectionRect.Contains(e.mousePosition))
			{
				var window = focusedWindow;
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(window != null && window.GetType().Name == "ProjectBrowser");
				#endif

				if(!CanSplitView)
				{
					#if DEV_MODE
					Debug.LogWarning("Ignoring project item middle click because CanSplitView was false...");
					#endif
					return;
				}

				if(inspectorTargetingMode == InspectorTargetingMode.Hierarchy)
				{
					#if DEV_MODE
					Debug.LogWarning("Ignoring project item middle click because inspectorTargetingMode was set to Hierarchy...");
					#endif
					return;
				}

				DrawGUI.Use(e); 
				
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var target = AssetDatabase.LoadMainAssetAtPath(path);
				var inspect = ArrayPool<Object>.CreateWithContent(target);

				#if DEV_MODE
				Debug.Log("Project item \"" + path + "\" middle clicked, opening in split view...");
				#endif

				if(splitView == null)
				{
					splitView = CreateInspector(InspectorType, inspect, true);
				}
				else
				{
					splitView.State.ViewIsLocked = true;
					splitView.RebuildDrawers(inspect, false);
				}
				SetSplitView(true);
			}
			// if window has a search filter, allow clearing it by clicking the element with control held down
			else if(e.button == 1 && e.control && type == EventType.MouseDown && selectionRect.Contains(e.mousePosition))
			{
				var window = focusedWindow;
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(window != null && window.GetType().Name == "ProjectBrowser");
				#endif

				if(window != null)
				{
					var searchFilterProperty = window.GetType().GetField("m_SearchFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if(searchFilterProperty != null)
					{
						var searchFilter = searchFilterProperty.GetValue(window);
						if(searchFilter != null)
						{
							var isSearchingMethod = searchFilter.GetType().GetMethod("IsSearching", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
							if(isSearchingMethod != null)
							{
								bool isSearching = (bool)isSearchingMethod.Invoke(searchFilter);
								if(isSearching)
								{
									var clearSearchMethod = window.GetType().GetMethod("ClearSearch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
									if(clearSearchMethod != null)
									{
										clearSearchMethod.Invoke(window, null);

										var path = AssetDatabase.GUIDToAssetPath(guid);
										var target = AssetDatabase.LoadMainAssetAtPath(path);
										Selection.activeObject = target;
										DrawGUI.Use(e);
										DrawGUI.Active.PingObject(target);
										return;
									}
									#if DEV_MODE
									else{ Debug.LogError("clearSearchMethod null"); }
									#endif
								}
								#if DEV_MODE && DEBUG_MMB
								else{ Debug.Log("ProjectBrowser isSearching false"); }
								#endif
							}
							#if DEV_MODE
							else{ Debug.LogError("isSearchingMethod null"); }
							#endif
						}
						#if DEV_MODE
						else{ Debug.LogError("searchFilter null"); }
						#endif
					}
					#if DEV_MODE
					else{ Debug.LogError("searchFilterProperty null"); }
					#endif
				}
			}
		}

		#if UNITY_2018_2_OR_NEWER
		private void OnEditorQuitting()
		{
			titleContent = GetTitleContent(false);
		}
		#endif
		
		private void UpdateCachedValuesFromFields()
		{
			mainView.UpdateCachedValuesFromFields();
			if(viewIsSplit)
			{
				splitView.UpdateCachedValuesFromFields();
			}

			Repaint();
		}

		/// <summary> Called when the window is closed. </summary>
		[UsedImplicitly]
		protected virtual void OnDestroy()
		{
			nowClosing = true;

			if(OnUpdate != null)
			{
				#if DEV_MODE
				Debug.Log("InspectorDrawerWindow.OnDestroy called with OnUpdate containing listeners. Calling it once more now.");
				#endif
				HandleOnUpdate();
			}

			#if DEV_MODE
			Debug.Log("InspectorDrawerWindow.OnDestroy with SetupDone="+SetupDone+", subscribedToEvents="+((bool)subscribedToEvents)+", mainView="+ StringUtils.TypeToString(mainView));
			#endif
			
			if(mainView != null)
			{
				InspectorManager.Instance().Dispose(ref mainView);
			}

			if(splitView != null)
			{
				InspectorManager.Instance().Dispose(ref splitView);
			}

			if(subscribedToEvents)
			{
				Undo.undoRedoPerformed -= OnUndoOrRedo;
				PlayMode.OnStateChanged -= OnEditorPlaymodeStateChanged;
				EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemCallback;
				EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemCallback;
				
				#if UNITY_2017_1_OR_NEWER
				AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
				AssemblyReloadEvents.afterAssemblyReload -= OnScriptsReloaded;
				#endif

				#if UNITY_2018_2_OR_NEWER
				EditorApplication.quitting -= OnEditorQuitting;
				#endif

				subscribedToEvents = false;
			}
		}
		
		[UsedImplicitly]
		private void OnHierarchyChange()
		{
			OnHierarchyOrProjectPossiblyChanged(OnChangedEventSubject.Hierarchy);
		}

		private void OnHierarchyOrProjectPossiblyChanged(OnChangedEventSubject changed)
		{
			//UPDATE: Setup can only be called during the first call to OnGUI
			//while this can sometimes get invoked before that
			if(!SetupDone)
			{
				return;
			}

			#if DEV_MODE && DEBUG_ON_HIERARCHY_CHANGE
			Debug.Log("!!!!!!!! OnHierarchyChange !!!!!!!!!");
			#endif

			LinkedMemberHierarchy.OnHierarchyChange();

			mainView.OnProjectOrHierarchyChanged(changed);
			if(ViewIsSplit)
			{
				splitView.OnProjectOrHierarchyChanged(changed);
			}
		}
		
		[UsedImplicitly]
		private void OnProjectChange()
		{
			//UPDATE: Setup can only be called during the first call to OnGUI
			//while this can sometimes get invoked before that
			if(!SetupDone)
			{
				return;
			}

			OnHierarchyOrProjectPossiblyChanged(OnChangedEventSubject.Project);
		}
		
		protected void RebuildDrawerIfTargetsChanged()
		{
			RebuildDrawers(false);
		}

		private void RebuildDrawers(bool evenIfTargetsTheSame)
		{
			#if SAFE_MODE
			//UPDATE: Setup can only be called during the first call to OnGUI
			//if something should call this method before that, it will most
			//likely result in errors
			if(!SetupDone)
			{
				#if DEV_MODE
				Debug.LogError("RebuildDrawers("+evenIfTargetsTheSame+") was called with SetupDone false! This should not be called before the first OnGUI call!");
				#endif
				//Should we still make the call on next layout, or drop it?
				inspectorManager.OnNextLayout(()=>
				{
					if(!SetupDone)
					{
						Setup();
					}
					RebuildDrawers(evenIfTargetsTheSame);
				}, this);
				return;
			}
			#endif

			bool viewChanged = mainView.RebuildDrawers(evenIfTargetsTheSame);

			if(viewIsSplit)
			{
				if(splitView == null)
				{
					#if DEV_MODE
					Debug.LogError("InspectacularWindow.splitView was true but splitBottom was null!");
					#endif
					SetSplitView(false);
				}

				if(splitView.RebuildDrawers(evenIfTargetsTheSame))
				{
					viewChanged = true;
				}
			}

			if(viewChanged)
			{
				RefreshView();
				SaveState();
			}
		}

		private void AddDefaultInspectorTab()
		{
			this.AddTab(Types.GetInternalEditorType("UnityEditor.InspectorWindow"));
		}
		
		[UsedImplicitly]
		private void Update()
		{
			HandleOnUpdate();
			
			if(LinkedMemberHierarchy.AnyHierarchyHasMissingTargets())
			{
				OnHierarchyOrProjectPossiblyChanged(OnChangedEventSubject.Hierarchy);
			}

			if(inspectorManager != null)
			{
				var mouseDownInfo = inspectorManager.MouseDownInfo;
				if(mouseDownInfo.MouseDownOverDrawer != null)
				{
					if(inspectorManager.MouseDownInfo.NowReordering)
					{
						Repaint();
					}
				}
			}
		}

		private void HandleOnUpdate()
		{
			float time = (float)EditorApplication.timeSinceStartup;
			onUpdateDeltaTime = time - lastUpdateTime;
			if(OnUpdate != null)
			{
				try
				{
					OnUpdate(onUpdateDeltaTime);
					
				}
				#if DEV_MODE
				catch(Exception e)
				{
					Debug.LogError(e);
				#else
				catch(Exception)
				{
				#endif
					lastUpdateTime = time;
					return;
				}
			}
			lastUpdateTime = time;
		}
		
		[UsedImplicitly]
		private void OnInspectorUpdate()
		{
			if(!SetupDone)
			{
				return;
			}
			
			if(isDirty)
			{
				#if DEV_MODE && DEBUG_REPAINT
				Debug.Log("Repaint");
				#endif

				GUI.changed = true;
			}
			
			#if UPDATE_VALUES_ON_INSPECTOR_UPDATE
			UpdateCachedValuesFromFields();
			#endif
		}
		
		[UsedImplicitly]
		private void OnSelectionChange()
		{
			if(!SetupDone)
			{
				return;
			}

			#if DEV_MODE && DEBUG_ON_SELECTION_CHANGE
			float time = Platform.Time;
			Debug.Log(StringUtils.ToColorizedString("On Selection Change: ", StringUtils.ToString(Selection.objects), " with selectTimeDiff=", (Platform.Time - lastSelectionChangeTime), ", mainView.Preferences.MergedMultiEditMode=", mainView.Preferences.MergedMultiEditMode));
			lastSelectionChangeTime = time;
			#endif

			
			inspectorManager.OnNextLayout(SwitchActiveInspectorAndUpdateContent, this);

			SaveState();
		}
		
		/// <summary>
		/// Prewarms some commonly used IDrawer for smoother user experience when selecting
		/// targets with these drawer for the first time
		/// </summary>
		private void PreCacheCommonlyUsedDrawer()
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(mainView != null, "main view null!");
			Debug.Assert(mainView.SetupDone, "main view Setup not done!");
			Debug.Assert(mainView.DrawerProvider != null, "mainView.DrawerProvider null!");
			#endif

			mainView.DrawerProvider.Prewarm(mainView);
		}

		private void SwitchActiveInspectorAndUpdateContent()
		{
			mainView.OnSelectionChange();
			if(viewIsSplit)
			{
				splitView.OnSelectionChange();
			}

			RefreshView();
		}

		[UsedImplicitly]
		private void OnFocus()
		{
			if(ObjectPicker.IsOpen)
			{
				#if DEV_MODE
				Debug.LogWarning("InspectorDrawerWindow.OnFocus was called with ObjectPicker open! mainView.Selected=" + StringUtils.ToColorizedString(mainView.Selected)+ ", mainView.FocusedDrawer=" + StringUtils.ToString(mainView.FocusedDrawer));
				#endif
				//TO DO: restore selected control after focus comes back? and before that, clear it?
				inspectorManager.Select(null, InspectorPart.None, null, ReasonSelectionChanged.LostFocus);
				return;
			}
			
			// this was added to fix issue where if window was focused with the cursor already inside its bounds
			// mouseoveredInstance would never get updated because the MouseEnterWindow was never received
			if(mouseOverWindow == this)
			{
				#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
				if(mouseoveredInstance != this) {  Debug.Log("mouseoveredInstance = "+mouseOverWindow); }
				#endif

				mouseoveredInstance = this;
			}

			if(!SetupDone)
			{
				//NOTE: It's important that this is called before Setup so that Setup will override the icon to use the inactive
				//one if setup has not been done yet since OnFocus gets called on the window on startup even if it does not have focus
				titleContent = GetTitleContent(inspectorTargetingMode, false);
				return;
			}

			var activeInspectorWas = inspectorManager.ActiveInspector;
			inspectorManager.ActiveInspector = mainView;

			//NOTE: It's important that this is called before Setup so that Setup will override the icon to use the inactive
			//one if setup has not been done yet since OnFocus gets called on the window on startup even if it does not have focus
			if(inspectorTargetingMode == InspectorTargetingMode.All)
			{
				titleContent = GetTitleContent(InspectorTargetingMode.All, true);
			}

			// make sure all other InspectorDrawerWindow instances have the inactive icon
			var activeInspectors = inspectorManager.ActiveInstances;
			for(int n = activeInspectors.Count - 1; n >= 0; n--)
			{
				var inspectorDrawer = activeInspectors[n].InspectorDrawer;
				if(!ReferenceEquals(inspectorDrawer, this))
				{
					var updateIconOfDrawer = inspectorDrawer as InspectorDrawerWindow;
					if(updateIconOfDrawer != null)
					{
						updateIconOfDrawer.UpdateWindowIcon();
					}
				}
			}

			#if DEV_MODE && DEBUG_FOCUS
			Debug.Log("OnFocus with selectedInspector="+StringUtils.ToString(inspectorManager.SelectedInspector)+ ", SelectedInspectorPart=" + inspectorManager.SelectedInspectorPart);
			#endif
			
			inspectorManager.selected.OnInspectorDrawerGainedFocus(this);

			var selectedInspector = inspectorManager.SelectedInspector;
			if(selectedInspector != mainView && (!viewIsSplit || splitView != selectedInspector))
			{
				inspectorManager.Select(mainView, InspectorPart.Viewport, ReasonSelectionChanged.GainedFocus);
			}			

			Repaint();
			
			// this is here so that if an EditorWindow tab was docked and not visible
			// and it gains focus, we check that the target that was being inspected
			// is still the selected target, exists etc.
			inspectorManager.OnNextLayout(RebuildDrawerIfTargetsChanged, this);
		
			SubscribeToEvents();

			if(activeInspectorWas != null && activeInspectorWas != mainView)
			{
				inspectorManager.ActiveInspector = activeInspectorWas;
			}
		}		

		[UsedImplicitly]
		private void OnLostFocus()
		{
			nowLosingFocus = true;

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!HasFocus);
			#endif

			DrawGUI.OnNextBeginOnGUI(ResetNowLosingFocus, false);

			#if DEV_MODE && DEBUG_FOCUS
			Debug.Log("OnLostFocus called with SetupDone="+StringUtils.ToColorizedString(SetupDone));
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(SetupDone)
			{
				if(mainView == null)
				{
					Debug.LogError("OnLostFocus was called with SetupDone but mainView null.");
				}
				else if(mainView.Preferences == null)
				{
					Debug.LogError("OnLostFocus was called with SetupDone but mainView.Preferences null.");
				}
			}
			#endif

			titleContent = GetTitleContent(false);

			Repaint();

			if(ReferenceEquals(mouseoveredInstance, this))
			{
				#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
				if(mouseoveredInstance != this)Debug.LogError("Setting mouseoveredInstance to null because OnLostFocus called");
				#endif

				mouseoveredInstance = null;
				GUI.changed = true;
			}

			if(inspectorManager == null)
			{
				return;
			}

			var activeInspectorWas = inspectorManager.ActiveInspector;
			inspectorManager.ActiveInspector = mainView;
			inspectorManager.selected.OnInspectorDrawerLostFocus(this);

			var selectedInspector = inspectorManager.SelectedInspector;

			#if DEV_MODE && DEBUG_FOCUS
			Debug.Log("OnLostFocus with selectedInspector="+StringUtils.ToString(selectedInspector)+ ", SelectedInspectorPart=" + inspectorManager.SelectedInspectorPart);
			#endif

			// handle scenario where alt+tab is used to unfocus the editor application
			// and mouseover effects get left stuck
			var mouseoveredInspector = inspectorManager.MouseoveredInspector;
			if(mouseoveredInspector != null && ReferenceEquals(mouseoveredInspector.InspectorDrawer, this))
			{
				#if DEV_MODE && (DEBUG_FOCUS || DEBUG_MOUSEOVERED_INSTANCE)
				Debug.Log("Clearing mouseovered inspector and drawer because OnLostFocus called");
				#endif
				inspectorManager.SetMouseoveredInspector(null, InspectorPart.None);
			}

			if(activeInspectorWas != null && activeInspectorWas != mainView)
			{
				inspectorManager.ActiveInspector = activeInspectorWas;
			}
		}

		private void ResetNowLosingFocus()
		{
			nowLosingFocus = false;
		}
		
		#if UNITY_2017_1_OR_NEWER
		private void OnBeforeAssemblyReload()
		{
			#if DEV_MODE && DEBUG_ON_SCRIPTS_RELOADED
			Debug.Log("!!!!!!!! OnBeforeAssemblyReload !!!!!!!!!");
			#endif

			Editors.Clear();
		}
		#endif

		protected void OnScriptsReloaded()
		{
			#if DEV_MODE && DEBUG_ON_SCRIPTS_RELOADED
			Debug.Log("!!!!!!!! OnScriptsReloaded !!!!!!!!!");
			#endif
		
			if(!InspectorManager.InstanceExists())
			{
				return;
			}

			var manager = InspectorUtility.ActiveManager;

			nowRestoringPreviousState = true;

			manager.OnNextLayout(OnScriptsReloadedDelayed);
		}

		private void OnScriptsReloadedDelayed()
		{
			if(!SetupDone)
			{
				Setup();
			}
			RestoreState();
		}

		/// <inheritdoc/>
		public void RefreshView()
		{
			//is dirty tells OnGUI to recalculate heights for all components.
			//Once it's done, the Update method is instructed to actually Repaint the view
			isDirty = true;
			GUI.changed = true;
			Repaint();

			#if UNITY_2019_2_OR_NEWER
			rootVisualElement.MarkDirtyRepaint();
			#endif
		}
				
		[UsedImplicitly]
		internal void OnGUI()
		{
			Platform.SetEditorMode();

			if(!SetupDone)
			{
				#if ODIN_INSPECTOR
				// This helps fix issue where OdinEditor had not yet been injected into Unity's internal systems before Power Inspector was requesting custom editor types for inspected components.
				if(!ApplicationUtility.IsReady())
				{
					return;
				}
				#endif

				Setup();
			}

			Manager.ActiveInspector = MainView;
			
			var time = (float)EditorApplication.timeSinceStartup;
			onGUIDeltaTime = time - lastOnGUITime;
			lastOnGUITime = time;

			var e = Event.current;
			var type = e.type;
			switch(type)
			{
				case EventType.MouseMove:
					if(!HasFocus && MouseIsOver)
					{
						GUI.changed = true;
					}
					break;
				case EventType.DragUpdated:
					if(!HasFocus && MouseIsOver)
					{
						GUI.changed = true;
					}
					
					//this is important because MouseEnterWindow doesn'tget called during UnityObject dragging!
					if(DrawGUI.IsUnityObjectDrag)
					{
						if(mouseOverWindow == this)
						{
							#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
							if(mouseoveredInstance != this)Debug.LogError("Custom MouseEnterWindow during UnityObjectDrag");
							#endif

							mouseoveredInstance = this;
							GUI.changed = true;
						}
						else if(mouseoveredInstance == this)
						{
							#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
							Debug.LogError("Custom MouseLeaveWindow during UnityObjectDrag");
							#endif

							mouseoveredInstance = null;
							GUI.changed = true;
						}
					}
					break;
				case EventType.MouseEnterWindow:
					#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
					Debug.Log("MouseEnterWindow with DrawGUI.IsUnityObjectDrag="+ DrawGUI.IsUnityObjectDrag+ ", mouseOverWindow="+StringUtils.ToString(mouseOverWindow));
					#endif
					if(mouseOverWindow == this)
					{
						mouseoveredInstance = this;
					}
					else if(mouseoveredInstance == this)
					{
						mouseoveredInstance = null;
					}
					GUI.changed = true;

					#if DEV_MODE && DEBUG_ABORT_ONGUI
					Debug.LogWarning("Aborting OnGUI for event "+StringUtils.ToString(Event.current)+"...");
					#endif
					return;
				case EventType.MouseLeaveWindow:
					#if DEV_MODE && DEBUG_MOUSEOVERED_INSTANCE
					Debug.Log("MouseLeaveWindow with DrawGUI.IsUnityObjectDrag="+ DrawGUI.IsUnityObjectDrag+ ", mouseOverWindow="+StringUtils.ToString(mouseOverWindow));
					#endif

					// Entering or leaving a window while a mouse button is pressed does not trigger either event, as pressing the mouse button activates drag mode.
					// Instead the MouseLeaveWindow event gets called right after a drag event starts. We want to ignore this event.
					if(DrawGUI.IsUnityObjectDrag)
					{
						#if DEV_MODE
						if(mouseoveredInstance  != null || Manager.MouseoveredInspector != null)
						{
							Debug.LogWarning("MouseLeaveWindow was called but IsUnityObjectDrag was true, so won't clear mouseovered inspector.");
						}
						#endif

						#if DEV_MODE && DEBUG_ABORT_ONGUI
						Debug.LogWarning("Aborting OnGUI for event "+StringUtils.ToString(Event.current)+"...");
						#endif
						return;
					}

					if(mouseoveredInstance == this)
					{
						mouseoveredInstance = null;
					}
					
					var mouseoveredInspector = Manager.MouseoveredInspector;
					if(mouseoveredInspector != null && ReferenceEquals(this, mouseoveredInspector.InspectorDrawer))
					{
						Manager.SetMouseoveredInspector(null, InspectorPart.None);
					}
					GUI.changed = true;

					#if DEV_MODE && DEBUG_ABORT_ONGUI
					Debug.LogWarning("Aborting OnGUI for event "+StringUtils.ToString(Event.current)+"...");
					#endif
					return;
				case EventType.Repaint:
					if(isDirty)
					{
						Repaint();
					}

					if(swapSplitSidesOnNextRepaint)
					{
						swapSplitSidesOnNextRepaint = false;
						if(splitView != null)
						{
							var temp = mainView;
							mainView = splitView;
							splitView = temp;
						}
					}
					break;
				case EventType.KeyDown:
					#if DEV_MODE && DEBUG_KEY_DOWN
					Debug.Log(StringUtils.ToColorizedString("InspectorDrawerWindow.OnGUI.KeyDown with event=", StringUtils.ToString(e), ", HasFocus=", HasFocus));
					#endif

					// Sometimes the KeyDown event can get sent even if the window doesn't have focus.
					// E.g. when the Gradient Editor is open. These inputs should be ignored.
					if(HasFocus)
					{
						if(MainView.Preferences.keyConfigs.closeSelectedView.DetectAndUseInput(e))
						{
							#if DEV_MODE
							Debug.Log("Closing because of input: "+StringUtils.ToString(e));
							#endif
							CloseTab();
							return;
						}
					}
					#if DEV_MODE
					else { Debug.LogWarning("Ignoring KeyDown event for "+e.keyCode+" because InspectorDrawerWindow did not have focus.");}
					#endif
					break;
				case EventType.Layout:
					minimizer.OnLayout();
					break;
			}
			
			Profiler.BeginSample("InspectacularWindow.OnGUI");
			
			DrawGUI.BeginOnGUI(mainView.Preferences, true);

			InspectorUtility.BeginInspectorDrawer(this, this);

			#if UNITY_2017_1_OR_NEWER && SHOW_SCREENSHOT_WHILE_SCRIPTS_RELOADING
			{
				if(EditorApplication.isCompiling)
				{
					// check height to avoid error "[d3d11] attempting to ReadPixels outside of RenderTexture bounds! Reading (0, 0, 645, 644) from (3440, 20)"
					if(screenshotWhileScriptsReloading == null && Screen.height > 20f && position.height > 20f)
					{
						screenshotWhileScriptsReloading = ScreenshotUtility.ScreenshotEditorWindow(this); 
					}

					#if DEV_MODE && DEBUG_DRAW_SCREENSHOT
					Debug.Log("Drawing screenshot...");
					#endif

					var pos = position;
					pos.x = -2f;
					pos.y = 0f;
					pos.width = screenshotWhileScriptsReloading.width;
					pos.height = screenshotWhileScriptsReloading.height;
					GUI.DrawTexture(pos, screenshotWhileScriptsReloading, ScaleMode.StretchToFill);
					return;
				}
				screenshotWhileScriptsReloading = null;
			}
			#endif

			// if window is minimized don't waste resources drawing things, when nothing is visible.
			if(minimizer.Minimized)
			{
				return;
			}

			//trying to fix a bug where the default inspector layout gets affected by things I do in there
			//by making sure all values that could affect it are restored back to normal
			//var indentLevelWas = EditorGUI.indentLevel;
			var labelWidthWas = EditorGUIUtility.labelWidth;
			var matrixWas = GUI.matrix;
			
			var windowRect = position;
			windowRect.x = 0f;
			windowRect.y = 0f;
			
			#if DEV_MODE && PI_ASSERTATIONS
			if(windowRect.width <= 0f) { Debug.LogError(GetType().Name+ " windowRect.width <= 0f: " + windowRect); }
			#endif

			bool mouseIsOverWindow = !Manager.IgnoreAllMouseInputs && MouseIsOver;

			if(mouseIsOverWindow)
			{
				Cursor.CanRequestLocalPosition = windowRect.Contains(Cursor.LocalPosition);
			}
			else
			{
				Cursor.CanRequestLocalPosition = !windowRect.Contains(Cursor.LocalPosition);
			}
			
			EditorGUI.BeginChangeCheck();
			{
				if(viewIsSplit)
				{
					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(CanSplitView);
					#endif

					#if SAFE_MODE || DEV_MODE
					if(splitView == null)
					{
						#if DEV_MODE
						Debug.LogError("splitView was null but viewIsSplit was true");
						#endif
						SetSplitView(true);
					}
					#endif

					var splitPos = windowRect;
					splitPos.height = Mathf.RoundToInt(windowRect.height * 0.5f);
					
					bool anyInspectorPartMouseovered;
					if(mouseIsOverWindow)
					{
						if(Cursor.CanRequestLocalPosition)
						{
							#if DEV_MODE && DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
							if((mainView.MouseoveredPart != InspectorPart.None) != splitPos.Contains(Cursor.LocalPosition)) { Debug.Log("mainView.AnyPartMouseovered = "+StringUtils.ToColorizedString(mainView.MouseoveredPart == InspectorPart.None)+" (cursor inside bounds check)");}
							#endif
							anyInspectorPartMouseovered = splitPos.Contains(Cursor.LocalPosition);
						}
						else
						{
							#if DEV_MODE && DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
							if((mainView.MouseoveredPart != InspectorPart.None) != (mainView.MouseoveredPart != InspectorPart.None)) { Debug.Log("mainView.AnyPartMouseovered = "+StringUtils.ToColorizedString(mainView.MouseoveredPart == InspectorPart.None)+" (could not request mouse pos)");}
							#endif
							anyInspectorPartMouseovered = mainView.MouseoveredPart != InspectorPart.None;
						}
					}
					else
					{
						#if DEV_MODE && DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
						if((mainView.MouseoveredPart != InspectorPart.None) != splitPos.Contains(Cursor.LocalPosition)) { Debug.Log("mainView.AnyPartMouseovered = "+StringUtils.ToColorizedString(mainView.MouseoveredPart == InspectorPart.None)+" (mouseIsOverWindow was false)");}
						#endif
						anyInspectorPartMouseovered = false;
					}
					
					mainView.OnGUI(splitPos, anyInspectorPartMouseovered);
					
					if(!anyInspectorPartMouseovered)
					{
						GUI.Button(splitPos, GUIContent.none, InspectorPreferences.Styles.Blank);
					}
					
					splitPos.y += splitPos.height;

					if(anyInspectorPartMouseovered || !mouseIsOverWindow)
					{
						#if DEV_MODE && DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
						if(splitView.MouseoveredPart != InspectorPart.None) { Debug.Log("anyInspectorPartMouseovered = "+StringUtils.False+" (splitView.AnyPartMouseovered = "+StringUtils.ToColorizedString(splitView.MouseoveredPart == InspectorPart.None)+",  mouseIsOverWindow="+StringUtils.ToColorizedString(mouseIsOverWindow)+")");}
						#endif
						anyInspectorPartMouseovered = false;
					}
					else
					{
						if(Cursor.CanRequestLocalPosition)
						{
							#if DEV_MODE && DEBUG_ANY_INSPECTOR_PART_MOUSEOVERED
							if((splitView.MouseoveredPart != InspectorPart.None) != splitPos.Contains(Cursor.LocalPosition)) { Debug.Log("splitView.AnyPartMouseovered = "+StringUtils.ToColorizedString(splitView.MouseoveredPart == InspectorPart.None)+" (splitPos "+splitPos+" Contains cursorPos "+Cursor.LocalPosition+" test) with Event="+StringUtils.ToString(Event.current));}
							#endif
							anyInspectorPartMouseovered = splitPos.Contains(Cursor.LocalPosition);
						}
						else
						{
							anyInspectorPartMouseovered = splitView.MouseoveredPart != InspectorPart.None;
						}
					}

					var linePos = splitPos;
					linePos.y -= 1f;
					linePos.height = 1f;
					var lineColor = mainView.Preferences.theme.SplitViewDivider;
					DrawGUI.DrawLine(linePos, lineColor);
					linePos.y -= 1f;
					lineColor.a *= 0.5f;
					DrawGUI.DrawLine(linePos, lineColor);

					DrawGUI.BeginArea(splitPos);
					{
						splitPos.y = 0f;

						//it is possible for splitTop.OnGUI to change the splitView state
						if(viewIsSplit)
						{
							try
							{
								splitView.OnGUI(splitPos, anyInspectorPartMouseovered);
							}
							catch(Exception exception)
							{
								if(ExitGUIUtility.ShouldRethrowException(exception))
								{
									OnExitingGUI(labelWidthWas, matrixWas);
									throw;
								}
								#if DEV_MODE
								Debug.LogWarning(ToString()+" "+exception);
								#endif
							}

							if(!anyInspectorPartMouseovered)
							{
								GUI.Button(splitPos, GUIContent.none, InspectorPreferences.Styles.Blank);
							}
						}
					}
					DrawGUI.EndArea();
				}
				else
				{
					if(splitView != null)
					{
						inspectorManager.Dispose(ref splitView);
					}

					bool anyInspectorPartMouseovered;
					if(mouseIsOverWindow)
					{
						if(Cursor.CanRequestLocalPosition)
						{
							anyInspectorPartMouseovered = windowRect.Contains(Cursor.LocalPosition);
						}
						else
						{
							anyInspectorPartMouseovered = mainView.MouseoveredPart != InspectorPart.None;
						}
					}
					else
					{
						anyInspectorPartMouseovered = false;
					}
					
					mainView.OnGUI(windowRect, anyInspectorPartMouseovered);

					if(!anyInspectorPartMouseovered)
					{
						GUI.Button(windowRect, GUIContent.none, InspectorPreferences.Styles.Blank);
					}
				}
			
				//trying to fix a bug where the default inspector layout gets affected by things I do in there
				//by making sure all values that could affect it are restored back to normal
				EditorGUI.indentLevel = 0; //indentLevelWas;
				EditorGUIUtility.labelWidth = labelWidthWas;
				GUI.skin = null;
				GUI.matrix = matrixWas;

				if(EditorGUI.EndChangeCheck())
				{
					RefreshView();
				}
				else if(isDirty && e.type == EventType.Layout) //doing this only on Layout helps with default component editor unfolded update bug
				{
					isDirty = false;
					Repaint();
				}
				else if(GUI.changed)
				{
					Repaint();
				}
			}

			Cursor.CanRequestLocalPosition = true;

			Profiler.EndSample();
		}

		private void OnExitingGUI(float labelWidthWas, Matrix4x4 matrixWas)
		{
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = labelWidthWas;
			GUI.skin = null;
			GUI.matrix = matrixWas;
		}

		/// <inheritdoc/>
		public void OpenSplitView()
		{
			SetSplitView(true);
		}

		/// <inheritdoc/>
		public void CloseSplitView()
		{
			SetSplitView(false);
		}

		/// <inheritdoc/>
		public void SetSplitView(bool enable)
		{
			#if DEV_MODE && DEBUG_SPLIT_VIEW
			Debug.Log(StringUtils.ToColorizedString("SetSplitView(", enable, ") with viewIsSplit=", viewIsSplit, ", splitView=", StringUtils.ToString(splitView), ", Event=", StringUtils.ToString(Event.current)), this);
			#endif
		
			#if SAFE_MODE || DEV_MODE
			if(!CanSplitView)
			{
				#if DEV_MODE
				Debug.LogError("SetSplitView("+StringUtils.ToColorizedString(enable)+") called for "+GetType().Name+" even though CanSplitView="+StringUtils.False);
				#endif
				return;
			}
			#endif

			// temporary fix for issue where peeking would not work
			// with viewIsSplit being true even though there is no
			// visible Split View to be seen
			if(viewIsSplit)
			{
				if(splitView == null)
				{
					#if DEV_MODE
					Debug.LogError("viewIsSplit was "+StringUtils.True+" but splitView was "+StringUtils.Null);
					#endif
					viewIsSplit = false;
				}
				else if(!splitView.SetupDone)
				{
					#if DEV_MODE
					Debug.LogError("viewIsSplit was "+StringUtils.True+" but splitView.SetupDone was "+StringUtils.False);
					#endif
					SetSplitView(false);
				}
			}

			if(enable != viewIsSplit)
			{
				viewIsSplit = enable;
				
				if(enable)
				{
					if(splitView == null)
					{
						splitView = CreateInspector(InspectorType, mainView.State.inspected, true, mainView.State.ScrollPos);
					}
				}
				else
				{
					var selectedPartWas = Manager.SelectedInspectorPart;
					var selectedToolbarItem = splitView.Toolbar.SelectedItem;
					
					if(splitView != null)
					{
						inspectorManager.Dispose(ref splitView);
					}
					inspectorManager.ActiveInspector = mainView;

					inspectorManager.Select(mainView, selectedPartWas, ReasonSelectionChanged.Initialization);
					if(selectedToolbarItem != null)
					{
						var selectItem = mainView.Toolbar.GetItemByType(selectedToolbarItem.GetType());
						if(selectItem != null && selectItem.Selectable)
						{
							mainView.Toolbar.SetSelectedItem(selectItem, ReasonSelectionChanged.Initialization);
						}
					}
				}
				RefreshView();
				SaveState();
			}
			#if DEV_MODE && PI_ASSERTATIONS
			else { Debug.LogWarning("InspectorDrawerWindow.SetSplitView(" + StringUtils.ToColorizedString(enable) + ") called, but viewIsSplit was already "+StringUtils.ToColorizedString((bool)viewIsSplit)); }
			#endif
		}

		/// <inheritdoc/>
		public void ShowInSplitView(Object[] inspect)
		{
			#if SAFE_MODE || DEV_MODE
			if(!CanSplitView)
			{
				#if DEV_MODE
				Debug.LogError("ShowInSplitView("+StringUtils.ToString(inspect)+") called for "+GetType().Name+" even though CanSplitView="+StringUtils.False);
				#endif
				return;
			}
			#endif

			if(splitView != null)
			{
				splitView.State.ViewIsLocked = true;
				splitView.RebuildDrawers(inspect, false);
			}
			else
			{
				splitView = CreateInspector(InspectorType, inspect, true, Vector2.zero);
			}
			SetSplitView(true);
		}
		
		private void SaveState()
		{
			if(!ApplicationUtility.IsReady()) 
			{
				#if DEV_MODE
				Debug.LogWarning("Aborted SaveState because ApplicationUtility.IsReady was false.");
				#endif
				return;
			}

			#if DEV_MODE && DEBUG_SAVE_STATE
			Debug.Log("Saving InspectorDrawerWindow state @ "+ Application.temporaryCachePath + "/temp-PowerInspectorState.data");
			#endif

			if(MainView == null)
			{
				#if DEV_MODE
				Debug.LogError("InspectorDrawerWindow.SaveState called but MainView was null!");
				#endif
				return;
			}

			if(MainView.State == null)
			{
				#if DEV_MODE
				Debug.LogError("InspectorDrawerWindow.SaveState called but MainView.State was null!");
				#endif
				return;
			}

			try
			{
				using(var saveData = new SplittableInspectorDrawerSerializedState(this))
				{
					saveData.Serialize(Application.temporaryCachePath + "/temp-PowerInspectorState.data");
				}
			}
			#if DEV_MODE
			catch(Exception e)
			{
				Debug.LogError(e);
			#else
			catch
			{
				return;
			#endif
			}
		}
		
		private void RestoreState()
		{
			if(!ApplicationUtility.IsReady())
			{
				#if DEV_MODE
				Debug.LogWarning("Aborted RestoreState because ApplicationUtility.IsReady was false.");
				#endif
				return;
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(SetupDone, GetType().Name+".RestoreState called with SetupDone false!");
			Debug.Assert(mainView.SetupDone, GetType().Name+".RestoreState called with mainView.SetupDone false!");
			#endif

			nowRestoringPreviousState = false;

			string path = Application.temporaryCachePath + "/temp-PowerInspectorState.data";
			if(System.IO.File.Exists(path))
			{
				#if DEV_MODE && DEBUG_RESTORE_STATE
				Debug.Log("Restoring "+GetType().Name+" state from "+ path);
				#endif
				
				viewIsSplit = false;
				using(var saveData = SplittableInspectorDrawerSerializedState.Deserialize(path))
				{
					saveData.Deserialize(this);
				}

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(viewIsSplit == (SplitView != null));
				#endif
			}
		}

		public void SwapSplitViewSides()
		{
			swapSplitSidesOnNextRepaint = true;
			Repaint();
		}

		/// <inheritdoc/>
		public void CloseMainView()
		{
			if(!viewIsSplit)
			{
				#if DEV_MODE
				Debug.LogError("CloseMainView was called but SplitView was false!");
				#endif
				return;
			}

			var temp = mainView;
			mainView = splitView;
			splitView = temp;
			SetSplitView(false);

			// auto-unlock view if it is locked and selected targets match inspected targets.
			// This is to get rid of perceived issue of view becoming locked for no good apparent reason
			// if user clicks split view and then closes the main view.
			if(mainView.State.inspected.ContentsMatch(mainView.SelectedObjects))
			{
				mainView.State.ViewIsLocked = false;
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!ViewIsSplit);
			#endif
		}
		
		/// <inheritdoc />
		public void CloseTab()
		{
			if(!ViewIsSplit)
			{
				nowClosing = true;
				Close();
				return;
			}

			if(MainView.Selected || Manager.LastSelectedActiveOrDefaultInspector() == MainView)
			{
				CloseMainView();
			}
			else
			{
				SetSplitView(false);
			}
		}

		/// <inheritdoc />
		public void Message(GUIContent message, Object context = null, MessageType messageType = MessageType.Info, bool alsoLogToConsole = true)
		{
			if(messageDispenser != null)
			{
				messageDispenser.Message(message, context, messageType, alsoLogToConsole);
			}
			#if DEV_MODE
			else
			{
				Debug.Log("Won't show message because messageDispenser null: \""+message.text+"\". This is normal if all messaging has been disabled in the preferences.");
				#if PI_ASSERTATIONS
				Debug.Assert(GetPreferences().messageDisplayMethod == MessageDisplayMethod.None);
				#endif
			}
			#endif
		}

		protected void UpdateWindowIcon()
		{
			if(inspectorTargetingMode == InspectorTargetingMode.All)
			{
				titleContent = GetTitleContent();
			}
		}
	}
}