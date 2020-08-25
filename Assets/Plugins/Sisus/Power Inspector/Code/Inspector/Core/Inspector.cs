#define SAFE_MODE
#define DISABLE_DRAWING_WHILE_COMPILING

//skip showing Settings menus in the this?
//this make sense since Unity force opens default Inspector window
//when a settings item is selected anyways
#define DONT_DISPLAY_SETTINGS

#define DEBUG_ON_MOUSE_DOWN
//#define DEBUG_REBUILD_DRAWERS
//#define DEBUG_ABORT_REBUILD_DRAWERS
#define DEBUG_ADD_COMPONENT
#define DEBUG_FINISH_ADDING_COMPONENTS
//#define DEBUG_ON_SELECTION_CHANGE
//#define DEBUG_MOUSEOVER_DETECTION
//#define DEBUG_GET_MOUSEOVERED_PART_UPDATED
//#define DEBUG_DRAG
//#define DEBUG_ON_NEXT_INSPECTED_VIEW_CHANGED
//#define DEBUG_ON_NEXT_INSPECTED_VIEW_CHANGED_DETAILED
//#define DEBUG_PROJECT_OR_HIERARCHY_CHANGED
//#define DEBUG_PREVIEW_AREA
//#define DEBUG_SCROLL_TO_SHOW
//#define DEBUG_SELECTED_PART_CHANGED

using System;
using UnityEngine;
using JetBrains.Annotations;
using Sisus.Attributes;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Sisus
{
	public delegate void Setup<TInspector>(TInspector inspector, IInspectorDrawer drawer) where TInspector : class, IInspector;
	public delegate void SetupForInspected<TInspector>(TInspector inspector, InspectorPreferences preferences, IInspectorDrawer drawer, Object[] inspected, Vector2 scrollPos, bool viewIsLocked) where TInspector : IInspector;

	public abstract class Inspector : IInspector
	{
		[SerializeField]
		private bool drawPreviewArea;
		private IInspectorDrawer inspectorDrawer;
		private InspectorPart nowDrawingPart = InspectorPart.None;
		private InspectorState state = new InspectorState();
		private InspectorPreferences preferences;
		private Action onNextInspectedViewChanged;

		#if !POWER_INSPECTOR_LITE
		[CanBeNull]
		private IInspectorToolbar toolbar;
		#endif

		/// <summary>
		/// Has setup phase been done for the Inspector? Set to true when Setup is called
		/// during the first OnGUI call for the Inspector's drawer.
		/// Set to false when the Inspector is disposed or during the reloading of the Inspector's assembly.
		/// </summary>
		[NonSerialized]
		private bool setupDone;

		/// <inheritdoc />
		public IInspectorDrawer InspectorDrawer
		{
			get
			{
				return inspectorDrawer;
			}
		}

		/// <inheritdoc/>
		public IDrawer FocusedDrawer
		{
			get
			{
				var manager = Manager;
				return manager.SelectedInspector != this ? null : manager.FocusedControl;
			}
		}

		/// <inheritdoc/>
		public IDrawerProvider DrawerProvider
		{
			get;
			private set;
		}
		
		/// <summary> Gets or sets delegate that is invoked when the filter is changed, right before its effects are applied for filtering the view. </summary>
		/// <value> Callback invoked when the filter is changing. </value>
		public Action<SearchFilter> OnFilterChanging { get; set; }

		/// <inheritdoc />
		public Object[] SelectedObjects
		{
			get
			{
				return inspectorDrawer.SelectionManager.Selected;
			}
		}

		/// <summary> Gets currently selected GameObjects. </summary>
		/// <value> The selected game objects. </value>
		private GameObject[] SelectedGameObjects
		{
			get
			{
				return ArrayPool<Object>.Cast<GameObject>(SelectedObjects);
			}
		}

		/// <summary> Gets or sets the part of the Inspector that is currently being drawn during an OnGUI event. </summary>
		/// <value> During OnGUI events returns the part of the inspector being drawn, otherwise returns None. </value>
		public InspectorPart NowDrawingPart
		{
			get
			{
				return nowDrawingPart;
			}

			set
			{
				switch(value)
				{
					case InspectorPart.None:
						GUISpace.Current = Space.Window;
						break;
					case InspectorPart.Viewport:
						GUISpace.Current = Space.Local;
						break;
					default:
						GUISpace.Current = Space.Inspector;
						break;
				}

				nowDrawingPart = value;
			}
		}

		/// <inheritdoc/>
		public InspectorPreferences Preferences
		{
			get
			{
				return preferences;
			}

			protected set
			{
				preferences = value;
			}
		}

		/// <inheritdoc/>
		public InspectorState State
		{
			get
			{
				return state;
			}
		}

		/// <inheritdoc/>
		public bool HasFilter
		{
			get
			{
				return state.filter.HasFilter;
			}
		}

		/// <inheritdoc/>
		public IInspectorToolbar Toolbar
		{
			get
			{
				#if !POWER_INSPECTOR_LITE
				return toolbar;
				#else
				return null;
				#endif
			}
		}

		/// <inheritdoc/>
		public float PreviewAreaHeight
		{
			get
			{
				return DrawPreviewArea ? PreviewDrawer.Height : 0f;
			}
		}

		/// <inheritdoc/>
		public float ToolbarHeight
		{
			get
			{
				var toolbar = Toolbar;
				return toolbar != null ? toolbar.Height : 0f;
			}
		}

		/// <inheritdoc/>
		public bool Selected
		{
			get
			{
				return Manager.SelectedInspector == this;
			}
		}

		/// <inheritdoc/>
		public InspectorPart MouseoveredPart
		{
			get
			{
				var manager = Manager;
				return manager.MouseoveredInspector == this ? manager.MouseoveredInspectorPart : InspectorPart.None;
			}
		}

		/// <inheritdoc/>
		public float LastInputTime { get; set; }

		/// <summary> Gets the part of the Inspector which is currently selected. </summary>
		/// <value> If the inspector is selected, returns the selected part, else returns None. </value>
		private InspectorPart SelectedPart
		{
			get
			{
				var manager = Manager;
				return manager.SelectedInspector == this ? manager.SelectedInspectorPart : InspectorPart.None;
			}
		}

		/// <summary> Gets a value indicating whether the preview area should be drawn. </summary>
		/// <value> True if preview area should be drawn, false if not. </value>
		protected bool DrawPreviewArea
		{
			get
			{
				return drawPreviewArea;
			}
		}

		/// <summary> Gets the class responsible for drawing the preview area of the inspector. </summary>
		/// <value>
		/// The preview drawer, or null if the inspector doesn't have a preview area
		/// (in which case DrawPreviewArea should return false).
		/// </value>
		[CanBeNull]
		protected abstract PreviewDrawer PreviewDrawer
		{
			get;
		}

		/// <summary>
		/// How many OnGUI Layout events to wait between updating the cached values of all displayed controls
		/// via their MemberInfos. </summary>
		/// <value> Layout event interval for updating cached values. </value>
		protected virtual int UpdateCachedValuesInterval
		{
			get
			{
				return 10;
			}
		}

		/// <inheritdoc/>
		[NotNull]
		public IInspectorManager Manager
		{
			get
			{
				if(inspectorDrawer == null)
				{
					#if DEV_MODE
					Debug.LogWarning(ToString()+".Manager called with InspectorDrawer null. Has the containing EditorWindow been closed?");
					#endif
					return InspectorUtility.ActiveManager;
				}
				return inspectorDrawer.Manager;
			}
		}

		/// <summary> Gets a value indicating whether setup phase has been done for the Inspector. </summary>
		/// <value> True if setup done, false if not. </value>
		public bool SetupDone
		{
			get
			{
				return setupDone;
			}
		}
		
		/// <summary> Initializes the Inspector for use with the given drawer, preferences and state data. </summary>
		/// <param name="drawer"> The drawer. </param>
		/// <param name="setPreferences"> The preferences. </param>
		/// <param name="inspected"> The targets to show on the inspector. </param>
		/// <param name="scrollPos"> The viewport scroll position. </param>
		/// <param name="viewIsLocked"> True if view is locked. </param>
		public virtual void Setup([NotNull]IInspectorDrawer drawer, [NotNull]InspectorPreferences setPreferences, Object[] inspected, Vector2 scrollPos, bool viewIsLocked)
		{
			DrawerProvider = DefaultDrawerProviders.GetForInspector(GetType());

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(DrawerProvider != null, "DefaultDrawerProviders failed to find a DrawerProvider for "+GetType().Name);
			Debug.Assert(!(DrawerProvider is DrawerProviderBase) || ((DrawerProviderBase)DrawerProvider).drawersFor != null, "DrawerProvider.drawersFor was null for " + GetType().Name);
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(drawer != null);
			Debug.Assert(setPreferences != null);
			#endif

			Preferences = setPreferences;
			inspectorDrawer = drawer;
			drawer.Manager.ActiveInspector = this;
			State.Setup(this, scrollPos, viewIsLocked, null);
			
			UndoHandler.Instance();

			setPreferences.Setup();

			if(Event.current != null)
			{
				DrawGUI.Setup(setPreferences);
			}

			#if !POWER_INSPECTOR_LITE
			if(toolbar == null)
			{
				toolbar = CreateToolbar();
			}
			if(toolbar != null)
			{
				SetupToolbar(toolbar);
			}
			#endif
			
			if(InspectorUtility.IsSafeToChangeInspectorContents)
			{
				RebuildDrawers(inspected, true);
			}
			else
			{
				drawer.Manager.OnNextLayout(() => RebuildDrawers(inspected, true), drawer);
			}

			setupDone = true;
		}
		
		/// <inheritdoc/>
		public void Message([NotNullOrEmpty]string message, Object context = null, MessageType messageType = MessageType.Info, bool alsoLogToConsole = true)
		{
			Message(new GUIContent(message), context, messageType, alsoLogToConsole);
		}

		/// <inheritdoc/>
		public abstract void Message(GUIContent message, Object context = null, MessageType messageType = MessageType.Info, bool alsoLogToConsole = true);

		/// <inheritdoc/>
		public virtual void Select(Object target)
		{
			inspectorDrawer.SelectionManager.Select(target);
		}

		/// <inheritdoc/>
		public virtual void Select(Object[] targets)
		{
			inspectorDrawer.SelectionManager.Select(targets);
		}

		/// <inheritdoc/>
		public virtual void OnSelectedPartChanged(InspectorPart from, InspectorPart to, ReasonSelectionChanged reason)
		{
			#if DEV_MODE && DEBUG_SELECTED_PART_CHANGED
			Debug.Log("OnSelectedPartChanged(from="+from+", to="+to+", reason="+reason+")");
			#endif

			if(to == InspectorPart.Toolbar)
			{
				Toolbar.OnSelected(reason);
			}
			else if(from == InspectorPart.Toolbar)
			{
				Toolbar.OnDeselected(reason);
			}
		}

		/// <inheritdoc/>
		public virtual void Dispose()
		{
			if(state != null)
			{
				state.Dispose();
			}
			else
			{
				#if DEV_MODE
				Debug.LogError("ResetState was called with state being null, so this null check is still needed.");
				#endif
				state = new InspectorState();
			}

			toolbar.Dispose();

			setupDone = false;
			inspectorDrawer = null;
			preferences = null;
			onNextInspectedViewChanged = null;
			NowDrawingPart = InspectorPart.None;
			toolbar.Dispose();
		}

		/// <inheritdoc/>
		public void OnNextInspectedChanged(Action action)
		{
			#if DEV_MODE && DEBUG_ON_NEXT_INSPECTED_VIEW_CHANGED
			Debug.Log("OnNextInspectedChanged(" + StringUtils.ToString(action)+")");
			#endif
			onNextInspectedViewChanged += action;

			#if DEV_MODE && PI_ASSERTATIONS
			if(onNextInspectedViewChanged.GetInvocationList().Length >= 3)
			{ Debug.LogWarning("onNextInspectedViewChanged had "+ onNextInspectedViewChanged.GetInvocationList().Length+" invocation list targets. Seems suspicious. Intentional?\n"+StringUtils.ToString(onNextInspectedViewChanged)); }
			#endif
		}

		/// <inheritdoc/>
		public void CancelOnNextInspectedChanged(Action action)
		{
			#if DEV_MODE && DEBUG_ON_NEXT_INSPECTED_VIEW_CHANGED
			Debug.Log("CancelOnNextInspectedChanged(" + StringUtils.ToString(action)+")");
			#endif
			onNextInspectedViewChanged -= action;
		}

		/// <summary>
		/// Gets clickable drawer inside the inspector view whose click-to-select area is currently situated under the cursor.
		/// If cursor overlaps multiple drawers' click-to-select areas, members are prioritized over parents.
		/// </summary>
		/// <returns> Mouseovered drawer. </returns>
		public IDrawer MouseoverClickableControl()
		{
			return IgnoreViewportMouseInputs() ? null : Manager.MouseoveredSelectable;
		}

		/// <summary>
		/// Gets right-clickable drawers of the inspector whose right click area is currently positioned under the cursor.
		/// </summary>
		/// <returns> Mouseovered drawers. </returns>
		public IDrawer MouseoverRightClickableControl()
		{
			return IgnoreViewportMouseInputs() ? null : Manager.MouseoveredRightClickable;
		}

		/// <inheritdoc/>
		public bool SendEvent(Event e)
		{
			return inspectorDrawer.SendEvent(e);
		}

		/// <inheritdoc/>
		public void Select(IDrawer drawer, ReasonSelectionChanged reason)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			if(drawer != null)
			{
				if(!drawer.Selectable) { Debug.LogError(ToString()+".Select called for drawer that is not selectable: "+drawer); }
				if(drawer.Inactive) { Debug.LogError(ToString()+".Select called for drawer that is Inactive: "+drawer); }
				if(!drawer.ShouldShowInInspector) { Debug.LogError(ToString()+".Select called for drawer that is not shown in inspector: "+drawer); }
			}
			#endif

			Manager.Select(this, InspectorPart.Viewport, drawer, reason);
		}

		/// <inheritdoc/>
		public void AddToSelection(IDrawer drawer, ReasonSelectionChanged reason)
		{
			#if DEV_MODE
			Debug.Assert(drawer.Inspector == this);
			#endif

			Manager.AddToSelection(drawer, reason);
		}

		/// <inheritdoc/>
		public void RemoveFromSelection(IDrawer drawer, ReasonSelectionChanged reason)
		{
			#if DEV_MODE
			Debug.Assert(drawer.Inspector == this);
			#endif

			Manager.RemoveFromSelection(drawer, reason);
		}

		/// <inheritdoc/>
		public bool IgnoreViewportMouseInputs()
		{
			return !ViewportMouseovered() || Manager.IgnoreAllMouseInputs;
		}

		/// <inheritdoc/>
		public bool IgnorePreviewAreaMouseInputs()
		{
			return MouseoveredPart == InspectorPart.PreviewArea || Manager.IgnoreAllMouseInputs;
		}

		/// <summary> Determines if we should ignore all scroll bar mouse inputs. </summary>
		/// <returns> True if should ignore, false if not. </returns>
		public bool IgnoreScrollBarMouseInputs()
		{
			return MouseoveredPart != InspectorPart.Scrollbar || Manager.IgnoreAllMouseInputs;
		}

		/// <inheritdoc/>
		public bool IgnoreToolbarMouseInputs()
		{
			return MouseoveredPart != InspectorPart.Toolbar || Manager.IgnoreAllMouseInputs;
		}

		/// <summary> Determines if viewport of this inspector is currently mouseovered. </summary>
		/// <returns> True if it succeeds, false if it fails. </returns>
		private bool ViewportMouseovered()
		{
			return MouseoveredPart == InspectorPart.Viewport;
		}

		/// <inheritdoc/>
		public void FoldAllComponents()
		{
			SetAllComponentsUnfolded(false);
		}

		/// <inheritdoc/>
		public void FoldAllComponents([CanBeNull]IComponentDrawer skipCollapsingThis)
		{
			SetAllComponentsUnfolded(false, skipCollapsingThis);
		}

		/// <inheritdoc/>
		public void UnfoldAllComponents()
		{
			SetAllComponentsUnfolded(true);
		}

		/// <summary> Sets unfolded state of all Components currently shown in the inspector. </summary>
		/// <param name="setUnfolded"> Unfold all if true, fold all if false. </param>
		/// <param name="skipThisOne"> (Optional) Skip altering the unfolded state of this Component. </param>
		public void SetAllComponentsUnfolded(bool setUnfolded, IComponentDrawer skipThisOne = null)
		{
			for(int n = state.drawers.Length - 1; n >= 0; n--)
			{
				var go = state.drawers[n] as IGameObjectDrawer;
				if(go != null)
				{
					var members = go.VisibleMembers;
					for(int c = go.LastVisibleCollectionMemberIndex; c >= 0; c--)
					{
						var comp = members[c] as IComponentDrawer;
						if(comp != null && comp != skipThisOne && ((!UnityObjectDrawerUtility.HeadlessMode && comp.Foldable) || setUnfolded))
						{
							comp.SetUnfolded(setUnfolded, false, false);
						}
					}
				}
			}

			RefreshView();
		}

		/// <inheritdoc/>
		public void RefreshView()
		{
			try
			{
				inspectorDrawer.RefreshView();
			}
			#if DEV_MODE
			catch(NullReferenceException e)
			{
				Debug.LogError(e);
			#else
			catch(NullReferenceException)
			{
			#endif
				GUI.changed = true;
			}
		}

		/// <inheritdoc/>
		public bool IsAboveViewport(float verticalPoint)
		{
			// new temp fix for issues in preferences window
			if(state.CanHaveScrollBar())
			{
				return false;
			}

			//return verticalPoint < state.ScrollPos.y;
			return verticalPoint < DrawGUI.ActiveScrollViewScrollPosition.y;
		}

		/// <inheritdoc/>
		public bool IsBelowViewport(float verticalPoint)
		{
			// new temp fix for issues in preferences window
			if(state.CanHaveScrollBar())
			{
				return false;
			}

			return verticalPoint > DrawGUI.ActiveScrollViewScrollPosition.y + Screen.height;
		}

		/// <inheritdoc/>
		public bool IsOutsideViewport(Rect color)
		{
			float start = color.yMin;
			float end = color.yMax;
			if(IsAboveViewport(start))
			{
				return IsAboveViewport(end);
			}
			if(IsBelowViewport(start))
			{
				return IsBelowViewport(end);
			}
			return false;
		}

		#if !POWER_INSPECTOR_LITE
		/// <inheritdoc/>
		public bool StepBackInSelectionHistory()
		{
			return state.selectionHistory.StepBackInSelectionHistory(this);
		}
		#endif

		#if !POWER_INSPECTOR_LITE
		/// <inheritdoc/>
		public bool StepForwardInSelectionHistory()
		{
			return state.selectionHistory.StepForwardInSelectionHistory(this);
		}
		#endif

		/// <inheritdoc />
		public void ScrollToShow(Object target)
		{
			var show = state.drawers.FindDrawer(target);
			if(show != null)
			{
				ScrollToShow(show);
			}
		}

		/// <inheritdoc />
		public void ScrollToShow(IDrawer drawer)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(drawer != null);
			#endif

			#if DEV_MODE && DEBUG_SCROLL_TO_SHOW
			Debug.Log("ScrollToShow("+target+")");
			#endif

			var pos = drawer.ClickToSelectArea;
			pos.height = drawer.Height;
			ScrollToShow(pos);
		}

		/// <inheritdoc />
		public void ScrollToShow(Rect area)
		{
			state.ScrollToShow(area);
		}

		/// <inheritdoc/>
		public void SelectAndShow(GameObject gameObject, ReasonSelectionChanged reason)
		{
			Manager.Select(this, InspectorPart.Viewport, reason);

			var gos = state.drawers.Members;
			for(int n = gos.Length - 1; n >= 0; n--)
			{
				var go = gos[n];
				if(Array.IndexOf(go.UnityObjects, gameObject) != -1)
				{
					Select(go, reason);
					ScrollToShow(go);
					if(!IsSelected(gameObject))
					{
						Select(gameObject);
					}
					return;
				}
			}

			//if GameObject was not found among Drawer
			
			//make sure that the target is not selected to avoid the possibility of an infinite loop
			if(!IsSelected(gameObject))
			{
				//make sure that this view is not locked
				state.ViewIsLocked = false;

				//select the GameObject and once that happens, call self again recursively
				OnNextInspectedChanged(()=> SelectAndShow(gameObject, reason));
				Select(gameObject);
			}
		}

		/// <inheritdoc/>
		public void SelectAndShow([CanBeNull]Component component, ReasonSelectionChanged reason)
		{
			#if DEV_MODE
			Debug.Log(ToString()+".Show(\""+StringUtils.ToString(component)+"\")", component);
			#endif

			var show = state.drawers.FindDrawer(component);
			if(show != null)
			{
				Manager.Select(this, InspectorPart.Viewport, show, reason);
				ScrollToShow(show);
				var gameObject = show.Transform.gameObject;
				if(!IsSelected(gameObject))
				{
					Select(gameObject);
				}
			}
			else if(component == null)
			{
				Debug.LogWarning("SelectAndShow was called with null Component parameter but drawers contained no MissingScriptDrawer (ReasonSelectionChanged: " + reason+")");
			}
			#if UNITY_EDITOR
			//if component not found among inspected drawers
			//make sure that the target is not selected to avoid the possibility of an infinite loop
			else if(!IsSelected(component))
			{
				//make sure that this view is not locked
				state.ViewIsLocked = false;

				//select the Component and once that happens, call self again recursively
				OnNextInspectedChanged(()=>SelectAndShow(component, reason));
				Select(component);
			}
			#endif
		}

		/// <inheritdoc/>
		public bool IsSelected(Object target)
		{
			var selected = SelectedObjects;
			return selected.Length == 1 && selected[0] == target;
		}

		/// <inheritdoc/>
		public bool IsSelected(IDrawer target)
		{
			return Manager.IsSelected(target);
		}

		/// <inheritdoc/>
		public void SelectAndShow(Object target, ReasonSelectionChanged reason)
		{
			var show = state.drawers.FindDrawer(target);
			if(show != null)
			{
				Manager.Select(this, InspectorPart.Viewport, show, reason);
				ScrollToShow(show);
				if(!IsSelected(target))
				{
					Select(target);
				}
			}
			//if target not found among inspected drawers
			//make sure that the target is not selected to avoid the possibility of an infinite loop
			else if(!IsSelected(target))
			{
				//make sure that this view is not locked
				state.ViewIsLocked = false;

				//select the Object and once that happens, call self again recursively
				OnNextInspectedChanged(()=>SelectAndShow(target, reason));
				Select(target);
			}
		}

		/// <inheritdoc/>
		public void SelectAndShow(LinkedMemberInfo memberInfo, ReasonSelectionChanged reason)
		{
			var show = state.drawers.FindDrawer(memberInfo);

			if(show != null)
			{
				Select(show, reason);
				ScrollToShow(show);
			}
			//if target not found among inspected drawers
			else
			{
				var target = memberInfo.UnityObject;

				//make sure that the target is not selected to avoid the possibility of an infinite loop
				if(target != null && !IsSelected(target))
				{
					//make sure that this view is not locked
					state.ViewIsLocked = false;

					//select the Object and once that happens, call self again recursively
					OnNextInspectedChanged(()=>SelectAndShow(memberInfo, reason));
					Select(memberInfo.UnityObject);
				}
			}
		}

		/// <summary>
		/// Tries to find Drawer for target field in members.
		/// This is a slow method, and using DrawerGroup.FindDrawer(Component) should be used instead when possible.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>
		/// The found Drawer. Null if none was found.
		/// </returns>
		public IDrawer FindDrawerForField(System.Reflection.FieldInfo field)
		{
			return ParentDrawerUtility.FindDrawerForField(state.drawers.Members, field);
		}

		/// <inheritdoc />
		public void OnSelectionChange()
		{
			#if DEV_MODE && DEBUG_ON_SELECTION_CHANGE
			Debug.LogWarning("OnSelectionChange SelectedObjects=" + StringUtils.ToString(SelectedObjects)+", inspected="+ StringUtils.ToString(State.inspected)+" - will call RebuildDrawersIfTargetsChanged");
			#endif

			//reset when selection changes?
			if(!State.ViewIsLocked)
			{
				// Delaying until the next layout even helps make the inspector feel more responsive since draw positions can get cached immediately.
				// If the delay is removed things using GUILayout such as custom editors can be drawn with incorrect dimensions for one frame.
				OnNextLayout(RebuildDrawersIfTargetsChanged);
			}
		}

		/// <inheritdoc />
		public void ForceRebuildDrawers()
		{
			RebuildDrawers(true);
		}

		/// <inheritdoc />
		public void RebuildDrawersIfTargetsChanged()
		{
			RebuildDrawers(false);
		}

		/// <inheritdoc />
		public bool RebuildDrawers(bool evenIfTargetsTheSame)
		{
			#if DEV_MODE
			Debug.Assert(SetupDone, ToString()+".RebuildDrawers(" + evenIfTargetsTheSame + ") was called with SetupDone false!");
			Debug.Assert(preferences != null, ToString()+".RebuildDrawers("+evenIfTargetsTheSame+") was called with Preferences null!");
			#endif

			// If Setup hasn't been called before RebuildDrawers gets invoked
			// we need to do this step to avoid errors.
			preferences.Setup();

			if(!state.ViewIsLocked)
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(!SelectedObjects.ContainsObjectsOfType(typeof(Transform)), "targets from "+inspectorDrawer.SelectionManager.GetType().Name+" with contained transforms: "+StringUtils.ToString(SelectedObjects));
				#endif
				return RebuildDrawers(SelectedObjects, evenIfTargetsTheSame);
			}
			
			return RebuildDrawers(state.inspected, evenIfTargetsTheSame);
		}

		/// <summary> Rebuild drawers for the given target. </summary>
		/// <param name="target">
		/// (Optional)Target. Can be UnityEngine.Object or instance of any other class. </param>
		/// <returns> True if inspected drawers changed, false if not. </returns>
		public bool RebuildDrawers([CanBeNull]object target)
		{
			if(target == null)
			{
				return RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
			}
			return RebuildDrawers(target, target.GetType());
		}
		
		/// <inheritdoc />
		public bool RebuildDrawers(object target, Type type)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(target != null || type != null);
			#endif

			if(target == null)
			{
				if(type == null)
				{
					return RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
				}
			}
			else
			{
				var unityObject = target as Object;
				if(unityObject != null)
				{
					Select(unityObject);
					return true;
				}
			}

			state.drawers.DisposeMembersAndClearVisibleMembers();

			IDrawer created;
			if(target != null)
			{
				created = DrawerProvider.GetForField(target, target.GetType(), null, null, GUIContent.none, false);
			}
			else
			{
				created = ClassDrawer.Create(type, null, this);
			}
			
			if(created == null)
			{
				return false;
			}

			Select(null as Object);

			state.inspected = ArrayPool<Object>.ZeroSizeArray;

			state.assetMode = false;

			state.ScrollPos = Vector2.zero;

			state.drawers.SetMembers(DrawerArrayPool.Create(created));
			
			var previewDrawer = PreviewDrawer;
			if(previewDrawer != null)
			{
				previewDrawer.SetTargets(ArrayPool<Object>.ZeroSizeArray, state.drawers);
				drawPreviewArea = false;
			}
			
			SendOnNextInspectedViewChangedEvent();

			return true;
		}

		/// <inheritdoc />
		public bool RebuildDrawers(Object[] targets, bool evenIfTargetsTheSame)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!targets.ContainsObjectsOfType(typeof(Transform)), GetType().Name+".RebuildDrawers - targets contained Transforms! They should be GameObjects!");
			#endif

			#if UNITY_EDITOR
			if(targets.Length > 0)
			{
				if(UnityEditor.EditorApplication.isCompiling)
				{
					#if DEV_MODE
					Debug.LogWarning(StringUtils.ToColorizedString("RebuildDrawers(", targets, ", ", evenIfTargetsTheSame,") replacing with empty array because EditorApplication.isCompiling."));
					#endif
					targets = ArrayPool<Object>.ZeroSizeArray;
				}
				else if(PlayMode.NowChangingState)
				{
					#if DEV_MODE
					Debug.LogWarning(StringUtils.ToColorizedString("RebuildDrawers(", targets, ", ", evenIfTargetsTheSame,") replacing with empty array because PlayMode.NowChangingState."));
					#endif
					targets = ArrayPool<Object>.ZeroSizeArray;
				}
			}
			#endif

			targets = targets.RemoveNullObjects();

			#if DEV_MODE
			Debug.Assert(!targets.ContainsNullObjects());
			#endif

			bool inspectedTargetsChanged;

			if(targets == state.inspected)
			{
				if(!evenIfTargetsTheSame)
				{
					#if DEV_MODE && DEBUG_ABORT_REBUILD_DRAWERS
					Debug.Log("RebuildDrawers("+this+", "+StringUtils.ToString(targets)+"): aborting because targets == state.inspected and evenIfTargetsTheSame is " + StringUtils.False);
					#endif

					#if DEV_MODE
					Debug.Assert(!state.drawers.Members.ContainsNullMembers(), "drawer:\n"+StringUtils.ToString(state.drawers.Members, "\n"));
					#endif

					if(state.drawers.Members.ContainsNullMembers())
					{
						state.drawers.SetMembers(state.drawers.Members.RemoveNullMembers());
					}
					return false;
				}
				inspectedTargetsChanged = false;
			}
			else
			{
				if(!state.inspected.ContentsMatch(targets))
				{
					inspectedTargetsChanged = true;
				}
				else
				{
					if(!evenIfTargetsTheSame)
					{
						#if DEV_MODE && DEBUG_ABORT_REBUILD_DRAWERS
						Debug.Log("RebuildDrawers("+this+", "+StringUtils.ToString(targets)+"): aborting because state.inspected.ContentsMatch(targets) and evenIfTargetsTheSame is " + StringUtils.False+". Event="+StringUtils.ToString(Event.current));
						#endif
						return false;
					}
					inspectedTargetsChanged = false;
				}
			}

			#if DEV_MODE && DEBUG_REBUILD_DRAWERS
			Debug.Log(" --- RebuildDrawers("+this+", "+StringUtils.ToString(targets)+") --- ");
			#endif
			
			Manager.ActiveInspector = this;

			int count = targets.Length;
			var target = count > 0 ? targets[0] : null;
			var go = target as GameObject;
			var isGameObject = go != null;

			bool isAsset;
			#if UNITY_EDITOR
			bool isModel;
			#endif
			if(!isGameObject)
			{
				isAsset = true;
				#if UNITY_EDITOR
				isModel = false;
				#endif

				#if UNITY_EDITOR && DONT_DISPLAY_SETTINGS
				if(count == 1 && target != null)
				{
					string assetPath = UnityEditor.AssetDatabase.GetAssetPath(target);
					if(assetPath.StartsWith("ProjectSettings/"))
					{
						#if DEV_MODE
						Debug.LogWarning("RebuildDrawers Abort - Settings asset detected: "+assetPath);
						#endif
						return false;
					}

					//this MIGHT also be a settings page...
					if(assetPath.Length == 0)
					{
						switch(target.GetType().FullName)
						{
							case "UnityEditor.MonoManager":
								#if DEV_MODE
								Debug.LogWarning("RebuildDrawers Abort - Settings asset detected: " + target.GetType().FullName);
								#endif
								return false;
						}
						
						#if DEV_MODE
						Debug.LogWarning("RebuildDrawers - Non-GameObject with no asset path detected, but probably not a settings object: " + target.GetType().FullName);
						#endif
					}
				}
				#endif
			}
			else
			{
				isAsset = go.IsPrefab();
				#if UNITY_EDITOR
				isModel = go.IsModel();
				#endif
			}
			state.assetMode = isAsset;
			state.ScrollPos = Vector2.zero;
			
			bool animateInitialUnfoldingWas = preferences.animateInitialUnfolding;
			if(!inspectedTargetsChanged)
			{
				// temporarily disable unfolding animations when targets have not been changed.
				// It can look strange when Components get folded and unfolded in front of the user's eyes.
				preferences.animateInitialUnfolding = false;
			}

			state.drawers.DisposeMembersAndClearVisibleMembers();
			
			if(count > 0)
			{
				IDrawer[] setMembers;
				IDrawer created;
				if(count > 1 && UserSettings.MergedMultiEditMode)
				{
					if(isGameObject)
					{
						#if UNITY_EDITOR
						if(isModel)
						{
							created = ModelDrawer.Create(ArrayPool<Object>.Cast<GameObject>(targets), state.drawers, this);
						}
						else
						#endif
						{
							created = DrawerProvider.GetForGameObjects(this, ArrayPool<Object>.Cast<GameObject>(targets), state.drawers);
						}
					}
					else
					{
						created = DrawerProvider.GetForAssets(this, targets, state.drawers);
					}
					
					setMembers = created == null ? ArrayPool<IDrawer>.ZeroSizeArray : DrawerArrayPool.Create(created);
				}
				else
				{
					setMembers = DrawerArrayPool.Create(count);
					
					for(int n = 0; n < count; n++)
					{
						target = targets[n];

						if(isGameObject)
						{
							go = target as GameObject;
							#if UNITY_EDITOR
							// ModelDrawer should only be used for the prefab root of models, not the sub-assets such as the rig.
							if(isModel && isAsset && UnityEditor.AssetDatabase.IsMainAsset(targets[0]))
							{
								created = ModelDrawer.Create(targets, state.drawers, this);
							}
							else
							#endif
							{
								created = DrawerProvider.GetForGameObject(this, go, state.drawers);
							}
						}
						else
						{
							var asset = targets[n];
							created = DrawerProvider.GetForAsset(this, asset, state.drawers);
						}

						setMembers[n] = created;
					}
				}

				for(int n = setMembers.Length - 1; n >= 0; n--)
				{
					if(setMembers[n] == null)
					{
						setMembers = setMembers.RemoveAt(n);
					}
				}

				state.drawers.SetMembers(setMembers);
			}
			else
			{
				state.drawers.Clear();
				state.ViewIsLocked = false; //unlock the view now that it has nothing to show?
			}
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!targets.ContainsNullObjects(), "targets contained null entries");
			Debug.Assert(!targets.ContainsObjectsOfType(typeof(Transform)), "targets contained transforms");
			#endif

			state.inspected = targets;

			if(PreviewDrawer != null)
			{
				// UPDATE: AnimationClipEditor throws an exception if HasPreviewGUI is called when Event.current is null.
				// So delay rebuilding the preview drawer until the the next event in this situation.
				// Event.current can be null e.g. if drawers are rebuilt via the OnHierarchyChange event.
				if(Event.current == null)
				{
					// Reset state until then. This might be important in case previous targets were destroyed.
					PreviewDrawer.ResetState();

					OnNextLayout(()=>
					{
						PreviewDrawer.SetTargets(targets, state.drawers);
						UpdateDrawPreviewArea();
					});
				}
				else
				{
					PreviewDrawer.SetTargets(targets, state.drawers);
					UpdateDrawPreviewArea();
				}
			}

			if(!HasFilter && UserSettings.EditComponentsOneAtATime && inspectedTargetsChanged && !UnityObjectDrawerUtility.HeadlessMode && isGameObject)
            {
                FoldAllComponents();
            }

			preferences.animateInitialUnfolding = animateInitialUnfoldingWas;
			
			if(inspectedTargetsChanged)
			{
				state.OnInspectedChanged(this);
			}
			
			#if DEV_MODE
			if(onNextInspectedViewChanged != null && !inspectedTargetsChanged)
			{
				Debug.LogWarning("RebuildDrawers: inspectedTargetsChanged="+StringUtils.False+" and onNextInspectedViewChanged="+StringUtils.ToString(onNextInspectedViewChanged)+". Still sending event now.");
			}
			#endif

			// immediately update dimensions for a more responsive feel
			// this helps especially with drawers using GUILayout like custom editors.
			if(Event.current != null)
			{
				var splittable = inspectorDrawer as ISplittableInspectorDrawer;
				var inspectorWindowRect = inspectorDrawer.position;
				inspectorWindowRect.x = 0f;
				inspectorWindowRect.y = 0f;

				if(splittable != null && splittable.SplitView == this)
				{
					inspectorWindowRect.height *= 0.5f;
					inspectorWindowRect.y += inspectorWindowRect.height;
				}
				// draw instructions immediately for a more responsive feel
				state.UpdateDimensions(true, false, inspectorWindowRect, ToolbarHeight, PreviewAreaHeight);
			}

			inspectorDrawer.Repaint();

			SendOnNextInspectedViewChangedEvent();

			return true;
		}

		private void SendOnNextInspectedViewChangedEvent()
		{
			var apply = onNextInspectedViewChanged;
			if(apply != null)
			{
				onNextInspectedViewChanged = null;

				#if DEV_MODE && DEBUG_ON_NEXT_INSPECTED_VIEW_CHANGED_DETAILED
				Debug.Log("Applying onNextInspectedViewChanged Action: " + StringUtils.ToString(apply));
				#endif

				apply();
			}
		}

		/// <inheritdoc/>
		public void OnNextLayout(Action action)
		{
			var inspectorDrawer = InspectorDrawer;
			inspectorDrawer.Manager.OnNextLayout(action, inspectorDrawer);
		}

		/// <inheritdoc/>
		public void OnNextLayout(IDrawerDelayableAction action)
		{
			var inspectorDrawer = InspectorDrawer;
			inspectorDrawer.Manager.OnNextLayout(action, inspectorDrawer);
		}

		/// <inheritdoc/>
		public InspectorPart GetMouseoveredPartUpdated(ref bool anyInspectorPartMouseovered)
		{
			var currentEvent = Event.current;
			var manager = Manager;
			bool ignoreAllMouseInputs = manager.IgnoreAllMouseInputs;

			#if DEV_MODE && DEBUG_GET_MOUSEOVERED_PART_UPDATED
			if(Event.current.isMouse && Event.current.control) { Debug.Log(StringUtils.ToColorizedString("GetMouseoveredPartUpdated(anyInspectorPartMouseovered=", anyInspectorPartMouseovered, ") with ignoreAllMouseInputs=", ignoreAllMouseInputs, ", Cursor.CanRequestLocalPosition=", Cursor.CanRequestLocalPosition, ", MouseoveredPart=", MouseoveredPart)); }
			#endif

			InspectorPart mouseoveredInspectorPart;
			if(!anyInspectorPartMouseovered || ignoreAllMouseInputs)
			{
				anyInspectorPartMouseovered = false;
				mouseoveredInspectorPart = InspectorPart.None;
			}
			else
			{
				if(!Cursor.CanRequestLocalPosition)
				{
					mouseoveredInspectorPart = MouseoveredPart;
				}
				else
				{
					var cursorPos = currentEvent.mousePosition;
					var windowRect = state.WindowRect;

					float toolbarHeight = ToolbarHeight;
					if(toolbarHeight > 0f && cursorPos.y < toolbarHeight)
					{
						if(cursorPos.y >= 0f)
						{
							mouseoveredInspectorPart = InspectorPart.Toolbar;
						}
						//new test
						else
						{
							mouseoveredInspectorPart = InspectorPart.Other;
						}
					}
					else
					{
						var previewAreaHeight = PreviewAreaHeight;
						if(previewAreaHeight > 0f && cursorPos.y > windowRect.height - previewAreaHeight)
						{
							if(cursorPos.y <= windowRect.height)
							{
								mouseoveredInspectorPart = InspectorPart.PreviewArea;
							}
							//new test
							else
							{
								mouseoveredInspectorPart = InspectorPart.Other;
							}
						}
						else if(state.HasScrollBar && cursorPos.x >= state.ScrollBarRect.x)
						{
							mouseoveredInspectorPart = InspectorPart.Scrollbar;
						}
						else
						{
							var zeroBasedWindowRect = windowRect;
							zeroBasedWindowRect.x = 0f;
							zeroBasedWindowRect.y = 0f;
							if(zeroBasedWindowRect.Contains(cursorPos))
							{
								mouseoveredInspectorPart = InspectorPart.Viewport;
							}
							else
							{
								#if DEV_MODE
								Debug.LogWarning(StringUtils.ToColorizedString(ToString()+".GetMouseoveredPartUpdated - zeroBasedWindowRect.Contains(cursorPos)=", false, " and anyInspectorPartMouseovered=", anyInspectorPartMouseovered, ". Returning previous MouseoveredPart value ", MouseoveredPart, ".\nmousePos=", cursorPos, ", windowRect=", windowRect, ", toolbarHeight=", toolbarHeight, ", previewAreaHeight=", previewAreaHeight));
								#endif
								mouseoveredInspectorPart = MouseoveredPart;
							}
						}
					}
				}
			}

			#if DEV_MODE && DEBUG_GET_MOUSEOVERED_PART_UPDATED
			if(Event.current.isMouse && Event.current.control) { Debug.Log(StringUtils.ToColorizedString(ToString()+"GetMouseoveredPartUpdated: ", mouseoveredInspectorPart, " (was: ", MouseoveredPart,") with anyInspectorPartMouseovered=", anyInspectorPartMouseovered, ") with ignoreAllMouseInputs=", ignoreAllMouseInputs, ", Cursor.CanRequestLocalPosition=", Cursor.CanRequestLocalPosition, "mousePos=", currentEvent.mousePosition)); }
			#endif

			return mouseoveredInspectorPart;
		}

		protected virtual void GetDrawPositions(Rect inspectorDimensions, out Rect toolbarRect, out Rect viewportRect, out Rect previewAreaRect)
		{
			toolbarRect = inspectorDimensions;
			viewportRect = inspectorDimensions;
			previewAreaRect = inspectorDimensions;

			toolbarRect.height = ToolbarHeight;
			previewAreaRect.height = PreviewAreaHeight;
			viewportRect.height = viewportRect.height - toolbarRect.height - previewAreaRect.height;

			viewportRect.y = toolbarRect.yMax;
			previewAreaRect.y = viewportRect.yMax;
		}
		
		public void OnContextClick(Event e)
		{
			#if DEV_MODE
			Debug.Log("OnContextClick("+StringUtils.ToString(e)+ ") with IsUnusedRightClickEvent="+StringUtils.ToColorizedString(Manager.RightClickInfo.IsUnusedRightClickEvent()));
			#endif

			var manager = Manager;

			if(!manager.RightClickInfo.IsUnusedRightClickEvent())
			{
				#if DEV_MODE
				Debug.LogWarning("Ignoring OnContextClick because RightClickInfo.IsUnusedRightClickEvent=" + StringUtils.False);
				#endif
				return;
			}

			var clickedPart = manager.MouseoveredInspectorPart;
			if(clickedPart == InspectorPart.Toolbar)
			{
				Toolbar.OnRightClick(e);
				ExitGUIUtility.ExitGUI();
				return;
			}

			SendOnRightClickEvent(e);
		}

		public void OnMouseDown(Event e)
		{
			var manager = inspectorDrawer.Manager;
			
			var clickedPart = manager.MouseoveredInspectorPart;

			#if DEV_MODE
			Debug.Assert(clickedPart != InspectorPart.None && !manager.IgnoreAllMouseInputs);
			#endif

			#if DEV_MODE && DEBUG_ON_MOUSE_DOWN
			Debug.Log(StringUtils.ToColorizedString(ToString()+".OnMouseDown(", e.button, ") @ ", e.mousePosition, " for ", ToString(), " with clickedPart=", clickedPart, ", ViewportMouseovered=", ViewportMouseovered(), ", IgnoreMouseInputs=", IgnoreViewportMouseInputs(), ", IgnoreAllMouseInputs=", Manager.IgnoreAllMouseInputs, ", IgnoreScrollBarMouseInputs=", IgnoreScrollBarMouseInputs(), ", mouseovered=", MouseoverClickableControl(), ", KeyboardControl=", KeyboardControlUtility.KeyboardControl, ", JustClickedControl=", KeyboardControlUtility.JustClickedControl,", e.type=", e.type, ", e.rawType=", e.rawType));
			#endif

			manager.Select(this, clickedPart, ReasonSelectionChanged.ThisClicked);

			// give controls time to react to selection changes, editing text field changes etc.
			// before cached values are updated
			ResetNextUpdateCachedValues();

			inspectorDrawer.Repaint();
			
			switch(e.button)
			{
				case 0:
				{
					var selectedWas = FocusedDrawer;
					var mouseovered = MouseoverClickableControl();

					#if DEV_MODE && DEBUG_ON_MOUSE_DOWN
					Debug.Log(StringUtils.ToColorizedString(ToString()+".OnMouseDown(", 0, ") with mouseovered=", StringUtils.ToString(mouseovered), ", selectedWas=", StringUtils.ToString(selectedWas), ", Event=", e, ", KeyboardControl=", KeyboardControlUtility.KeyboardControl, ", JustClickedControl=", KeyboardControlUtility.JustClickedControl));
					#endif

					manager.MouseDownInfo.OnPressingMouseDown(this, mouseovered);
					
					if(mouseovered != null)
					{
						if(e.clickCount == 2 && mouseovered.OnDoubleClick(e))
						{
							GUI.changed = true;
						}

						#if DEV_MODE && DEBUG_ON_MOUSE_DOWN
						Debug.Log(ToString()+" calling OnClick of "+ mouseovered+ " with ClickToSelectArea=" + mouseovered.ClickToSelectArea+ ", MouseoveredPart=" + mouseovered.MouseoveredPart);
						#endif

						if(mouseovered.OnClick(e))
						{
							#if DEV_MODE && DEBUG_ON_MOUSE_DOWN
							Debug.Log(ToString()+" OnClick of "+mouseovered+" returned "+StringUtils.True+ ". Event="+StringUtils.ToString(e));
							#endif
							GUI.changed = true;
						}
						// If a field was clicked but wasn't selected (e.g. it has Selectable false)
						// and the click didn't result in the selected drawer changing at all then deselect current field?
						else if(e.type != EventType.Used)
						{
							var selectedNow = FocusedDrawer;
							if(selectedNow != null && mouseovered != selectedNow && selectedNow == selectedWas)
							{
								#if DEV_MODE && DEBUG_ON_MOUSE_DOWN
								Debug.LogWarning("Deselecting field "+selectedWas+ " because another field was clicked: " + mouseovered);
								#endif

								Select(null, ReasonSelectionChanged.OtherClicked);
								KeyboardControlUtility.KeyboardControl = 0;
								DrawGUI.EditingTextField = false;
							}
						}
					}
					else
					{
						if(selectedWas != null)
						{
							Select(null, ReasonSelectionChanged.OtherClicked);
							KeyboardControlUtility.KeyboardControl = 0;
							DrawGUI.EditingTextField = false;
						}
						else
						{
							KeyboardControlUtility.KeyboardControl = 0;
							DrawGUI.EditingTextField = false;
						}

						if(clickedPart == InspectorPart.Toolbar)
						{
							if(!IgnoreToolbarMouseInputs())
							{
								if(Toolbar.OnClick(e))
								{
									ExitGUIUtility.ExitGUI();
								}
							}
							#if DEV_MODE
							else { Debug.LogWarning("Ignoring toolbar click because IgnoreToolbarMouseInputs was true."); }
							#endif
						}
					}

					manager.MouseDownInfo.OnPressedMouseDown(this, mouseovered);
				}
				break;
				case 1:
				{
					if(!manager.RightClickInfo.IsUnusedRightClickEvent())
					{
						#if DEV_MODE
						Debug.LogWarning("Ignoring MouseDown(1) because RightClickInfo.IsUnusedRightClickEvent=" + StringUtils.False);
						#endif
						return;
					}

					if(clickedPart == InspectorPart.Toolbar)
					{
						if(Toolbar.OnRightClick(e))
						{
							ExitGUIUtility.ExitGUI();
						}
					}
					SendOnRightClickEvent(e);
				}
				break;
				case 2:
				{
					if(clickedPart == InspectorPart.Toolbar)
					{
						if(Toolbar.OnMiddleClick(e))
						{
							ExitGUIUtility.ExitGUI();
						}
					}

					var mouseovered = MouseoverClickableControl();
					if(mouseovered != null)
					{
						mouseovered.OnMiddleClick(e);
					}
				}
				break;
			}
		}

		private void SendOnRightClickEvent(Event e)
		{
			if(e.type == EventType.Used)
			{
				return;
			}
			
			var mouseovered = MouseoverRightClickableControl();
			if(mouseovered != null)
			{
				#if DEV_MODE
				Debug.Assert(MouseoveredPart == InspectorPart.Viewport);
				#endif

				if(ContextMenuUtility.MenuIsOpening)
				{
					#if DEV_MODE
					Debug.LogWarning(ToString()+" - Ignoring SendOnRightClickEvent because ContextMenuUtility.MenuIsOpening already true.");
					#endif
					return;
				}

				if(mouseovered.OnRightClick(e))
				{
					GUI.changed = true;
				}
			}
		}

		private void UpdateMouseoverClickableControl()
		{
			#if DEV_MODE && DEBUG_MOUSEOVER_DETECTION
			Debug.Log(StringUtils.ToColorizedString("UpdateMouseoverClickableControl with InspectorDrawer.position=", InspectorDrawer.position, ", inspectorDrawer.MouseIsOver=", InspectorDrawer.MouseIsOver, ", CanRequestLocalPosition = ", Cursor.CanRequestLocalPosition, ", IgnoreViewportMouseInputs=", IgnoreViewportMouseInputs()));
			#endif

			if(Cursor.CanRequestLocalPosition)
			{
				if(IgnoreViewportMouseInputs())
				{
					Manager.SetMouseoveredSelectable(this, null);
					return;
				}
			}

			state.OnNextLayoutForVisibleDrawers(0, HandleMouseoverDetection);
		}
		
		private void HandleMouseoverDetection(IDrawer drawer)
		{
			#if DEV_MODE && DEBUG_MOUSEOVER_DETECTION
			Debug.Log(StringUtils.ToColorizedString("HandleMouseoverDetection with CanRequestLocalPosition=", Cursor.CanRequestLocalPosition, ", IgnoreViewportMouseInputs=", IgnoreViewportMouseInputs()));
			#endif

			if(!Cursor.CanRequestLocalPosition)
			{
				return;
			}

			if(drawer.DetectMouseover(Event.current.mousePosition))
			{
				Manager.SetMouseoveredSelectable(this, drawer);

				//once we've found the mouseover target, no need to check the rest
				state.ClearOnNextLayoutForVisibleDrawers(0);
			}
			else if(MouseoverClickableControl() == drawer)
			{
				Manager.SetMouseoveredSelectable(this, null);
			}
		}

		private void UpdateMouseoverRightClickableControl()
		{
			if(IgnoreViewportMouseInputs())
			{
				Manager.SetMouseoveredRightClickable(this, null);
			}
			else
			{
				state.OnNextLayoutForVisibleDrawers(1, HandleRightClickAreaMouseoverDetection);
			}
		}
		
		private void HandleRightClickAreaMouseoverDetection(IDrawer drawers)
		{
			if(!Cursor.CanRequestLocalPosition)
			{
				return;
			}

			if(drawers.DetectRightClickAreaMouseover(Event.current.mousePosition))
			{
				Manager.SetMouseoveredRightClickable(this, drawers);

				//once we've found the mouseover target, no need to check the rest
				state.ClearOnNextLayoutForVisibleDrawers(1);
			}
			else if(Manager.MouseoveredSelectable == drawers)
			{
				Manager.SetMouseoveredRightClickable(this, null);
			}
		}

		public void OnCursorPositionOrLayoutChanged()
		{
			#if DEV_MODE
			//if(Manager.MouseDownInfo.IsDrag()) { Debug.Log(StringUtils.ToColorizedString("OnCursorPositionOrLayoutChanged with ViewportMouseovered=", ViewportMouseovered(), ", MouseoveredInspector=", Manager.MouseoveredInspector, ", MouseoveredInspectorPart=", Manager.MouseoveredInspectorPart, ", InspectorDrawer.MouseIsOver=", InspectorDrawer.MouseIsOver, ", MouseDownInfo.IsDrag()=", Manager.MouseDownInfo.IsDrag())); }
			#endif

			if(ViewportMouseovered())
			{
				UpdateMouseoverClickableControl();
				UpdateMouseoverRightClickableControl();

				if(Manager.MouseDownInfo.IsDrag())
				{
					Manager.MouseDownInfo.Reordering.OnCursorMovedOrInspectorLayoutChanged();
				}
			}
		}

		private void ApplyForEachControl(Action<IDrawer> action)
		{
			state.drawers.ApplyForEachControl(action);
		}

		protected bool DrawViewport(Rect viewportRect)
		{
			#if DISABLE_DRAWING_WHILE_COMPILING
			// new test: don't draw viewport unless application is in a stable and fully ready state
			if(!ApplicationUtility.IsReady())
			{
				var messageRect = viewportRect;
				var message = new GUIContent("");

				#if UNITY_EDITOR
				if(UnityEditor.BuildPipeline.isBuildingPlayer)
				{
					message.text = "Building";
				}
				else if(UnityEditor.EditorApplication.isCompiling)
				{
					message.text = "Compiling";
				}
				else if(PlayMode.NowChangingState)
				{
					switch(PlayMode.CurrentState)
					{
						case PlayModeState.ExitingEditMode:
							message.text = "Entering Play Mode...";
							break;
						case PlayModeState.ExitingPlayMode:
							message.text = "Exiting Play Mode...";
							break;
					}
				}
				else if(ApplicationUtility.IsQuitting)
				{
					message.text = "Quitting";
				}
				else
				{
					message.text = "";
				}
				#else
				message.text = "";
				#endif

				switch(Mathf.RoundToInt(Platform.Time) % 3)
				{
					case 0:
						message.text += ".";
						break;
					case 1:
						message.text += "..";
						break;
					default:
						message.text += "...";
						break;
				}

				var style = new GUIStyle(InspectorPreferences.Styles.Centered);
				style.fontStyle = FontStyle.Bold;
				style.fontSize *= 2;

				GUI.Label(messageRect, message, style);

				return false;
			}
			#endif

			bool doScrollView = State.CanHaveScrollBar();

			if(doScrollView)
			{
				BeginScrollView(viewportRect);
			}
			else
			{
				GUI.BeginGroup(viewportRect);
			}
			bool dirty = DrawViewport();
			if(doScrollView)
			{
				DrawGUI.EndScrollView();
			}
			else
			{
				GUI.EndGroup();
			}
			return dirty;
		}

		protected void BeginScrollView(Rect viewportRect)
		{
			var manager = Manager;
			var mouseoveredInspectorPart = manager.MouseoveredInspectorPart;
			
			var setScrollPos = state.ScrollPos;
			DrawGUI.BeginScrollView(viewportRect, state.contentRect, ref setScrollPos);
			if(!manager.IgnoreAllMouseInputs && mouseoveredInspectorPart != InspectorPart.None)
			{
				state.ScrollPos = setScrollPos;
			}
		}
		
		protected bool DrawViewport()
		{
			bool dirty = false;
			var guiInstructionDrawRect = state.contentRect;
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(guiInstructionDrawRect.width > 0f, guiInstructionDrawRect.ToString());
			if(guiInstructionDrawRect.height <= 0f && State.drawers.Length > 0)
			{
				Debug.LogWarning("DrawViewport called with guiInstructionDrawRect.height "+guiInstructionDrawRect.height+", Event=" + StringUtils.ToString(Event.current));
			}
			#endif

			var manager = Manager;
			var mouseDownInfo = manager.MouseDownInfo;
			var mouseoveredClickable = MouseoverClickableControl();
			if(mouseoveredClickable != null)
			{
				if(!mouseDownInfo.IsDrag())
				{
					mouseoveredClickable.OnMouseoverBeforeDraw();
				}
			}

			int count = state.drawers.Length;
			for(var i = 0; i < count; i++)
			{
				var drawers = state.drawers[i];
				if(drawers != null && drawers.ShouldShowInInspector)
				{
					GUI.changed = false;

					NowDrawingPart = InspectorPart.Viewport;

					if(HasFilter && Event.current.type == EventType.Repaint) 
					{
						var filterHighlightColor = InspectorUtility.Preferences.theme.FilterHighlight;
						filterHighlightColor.a = 1f;
						drawers.ApplyInVisibleChildren((drawer)=>drawer.DrawFilterHighlight(State.SearchFilter, filterHighlightColor));
					}

					if(drawers.Draw(guiInstructionDrawRect) || GUI.changed)
					{
						dirty = true;
					}

					NowDrawingPart = InspectorPart.Other;

					float itemHeight = drawers.Height;
					guiInstructionDrawRect.y += itemHeight;
					guiInstructionDrawRect.height -= itemHeight;
				}
			}

			var mouseDownOverControl = mouseDownInfo.Inspector != this ? null : mouseDownInfo.MouseDownOverDrawer;
			bool mouseDownOverInspectorControl = mouseDownOverControl != null;

			bool viewportMouseovered = ViewportMouseovered() && !manager.IgnoreAllMouseInputs;
			
			bool selected = manager.SelectedInspector == this;

			// don't call DrawSelectionRect when reordering, because it can look strange with
			// Collection members that follow the cursor position, and because OnBeingReordered
			// is already being called, and can be used to do the same thing that would normally
			// be done in DrawSelectionRect if that is desired.
			if(selected && !mouseDownInfo.NowReordering)
			{
				var selectedControl = manager.FocusedControl;

				if(!manager.HasMultiSelectedControls)
				{
					if(selectedControl != null && !selectedControl.BeingAnimated())
					{
						selectedControl.DrawSelectionRect();
					}
				}
				else
				{
					if(selectedControl != null && manager.IsFocusedButNotSelected(selectedControl) && !selectedControl.BeingAnimated())
					{
						//TO DO: Change selected line color?
						var guiWasEnabled = GUI.enabled;
						GUI.enabled = false;
						
						selectedControl.DrawSelectionRect();

						GUI.enabled = guiWasEnabled;
					}
				
					var multiSelected = manager.MultiSelectedControls;
					for(int n = multiSelected.Count - 1; n >= 0; n--)
					{
						var drawer = multiSelected[n];
						if(!drawer.BeingAnimated())
						{
							drawer.DrawSelectionRect();
						}
					}
				}
			}
			
			#if DEV_MODE && DEBUG_DRAG
			if(DrawGUI.IsUnityObjectDrag) Debug.Log(StringUtils.ToColorizedString("DrawViewport with mouseDownOverControl=", mouseDownOverInspectorControl, ", viewportMouseovered=", viewportMouseovered, ", mouseDownInfo.NowReordering=", mouseDownInfo.NowReordering, ", mouseDownInfo.NowDraggingPrefix=", mouseDownInfo.NowDraggingPrefix, ", MouseoverClickableControl=", MouseoverClickableControl(), ", mouseDownInfo.IsDrag()=", mouseDownInfo.IsDrag(), ", .MouseoveredDropTarget.Parent=", mouseDownInfo.Reordering.MouseoveredDropTarget.Parent, ", Event=", Event.current));
			#endif
			
			if(mouseDownOverInspectorControl)
			{
				var draggedReferences = DrawGUI.Active.DragAndDropObjectReferences;
				if(mouseDownInfo.NowReordering)
				{
					GUI.changed = true;

					var reordering = mouseDownInfo.Reordering;
					reordering.Drawer.OnBeingReordered(state.WindowRect.y);

					var reorderingDropTargetParent = reordering.Parent;
					if(reorderingDropTargetParent != null)
					{
						reorderingDropTargetParent.OnMemberDrag(mouseDownInfo, draggedReferences);
					}

					var dropTarget = reordering.MouseoveredDropTarget.Parent;
					if(dropTarget != null && reordering.MouseoveredDropTarget.Inspector == this)
					{
						dropTarget.OnSubjectOverDropTarget(mouseDownInfo, draggedReferences);
					}

					if(draggedReferences.Length > 0)
					{
						mouseDownOverControl.OnDrag(Event.current);
						if(mouseoveredClickable != null)
						{
							mouseoveredClickable.OnMouseoverDuringDrag(mouseDownInfo, draggedReferences);
						}
					}
				}
				else if(mouseDownInfo.NowDraggingPrefix)
				{
					mouseDownInfo.DraggingPrefixOfDrawer.OnPrefixDragged(Event.current);
					GUI.changed = true;
				}
				else
				{
					mouseDownOverControl.OnDrag(Event.current);
					if(mouseoveredClickable != null)
					{
						mouseoveredClickable.OnMouseoverDuringDrag(mouseDownInfo, draggedReferences);
					}
				}
			}
			else if(viewportMouseovered)
			{
				var mouseoveredRightClickable = MouseoverRightClickableControl();
				if(mouseoveredRightClickable != null)
				{
					mouseoveredRightClickable.OnRightClickAreaMouseover();
				}

				if(mouseoveredClickable != null && !mouseoveredClickable.BeingAnimated())
				{
					if(mouseDownInfo.IsDrag())
					{
						mouseoveredClickable.OnMouseoverDuringDrag(mouseDownInfo, DrawGUI.Active.DragAndDropObjectReferences);
					}
					else
					{
						mouseoveredClickable.OnMouseover();
					}
				}

				if(DrawGUI.IsUnityObjectDrag)
				{
					#if DEV_MODE && PI_ASSERTATIONS
					if(!mouseDownInfo.MouseButtonIsDown && DrawGUI.LastInputEventType != EventType.DragPerform && DrawGUI.LastInputEventType != EventType.DragExited)
					{ Debug.LogError(StringUtils.ToColorizedString("MouseButtonIsDown was ", false, " even though IsDrag() was ", true, " with ObjectReferences=", StringUtils.ToString(DrawGUI.Active.DragAndDropObjectReferences), ", mouseDownInfo.IsClick=", mouseDownInfo.IsClick, ", LastInputEventType=", DrawGUI.LastInputEventType+", LastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()))); }
					#endif

					var dropTargetInfo = mouseDownInfo.Reordering.MouseoveredDropTarget;
					var dropTarget = mouseDownInfo.Reordering.MouseoveredDropTarget.Parent;
					if(dropTarget != null && dropTargetInfo.Inspector == this)
					{
						dropTarget.OnSubjectOverDropTarget(mouseDownInfo, DrawGUI.Active.DragAndDropObjectReferences);
					}
				}
			}
			
			if(Event.current.rawType == EventType.DragPerform)
			{
				OnNextLayout(DrawGUI.ClearDragAndDropObjectReferences);
			}
			
			return dirty;
		}
		
		public void OnValidateCommand(Event e)
		{
			var selectedControl = FocusedDrawer;

			#if DEV_MODE
			Debug.Assert(SelectedPart != InspectorPart.None, ToString()+".OnValidateCommand was called but selected part was None");
			#endif

			if(string.Equals(e.commandName, "Find"))
			{
				toolbar.OnFindCommandGiven();
				return;
			}

			if(SelectedPart != InspectorPart.Viewport)
			{
				return;
			}

			switch(e.commandName)
			{
				case "Cut":
					if(!DrawGUI.EditingTextField && selectedControl != null)
					{
						GUI.changed = true;
						DrawGUI.Use(e);

						// Copy exceptions should be handled internally by drawer but making sure.
						try
						{
							selectedControl.CutToClipboard();
						}
						#if DEV_MODE
						catch(Exception exception)
						{
							Debug.LogWarning(exception.ToString());
						#else
						catch(Exception)
						{
						#endif
							
							// This should handle detecting that the operation failed and displaying a fitting message to the user.
							Clipboard.SendCopyToClipboardMessage(selectedControl.GetFieldNameForMessages());
						}
					}
					break;
				case "Copy":
					if(!DrawGUI.EditingTextField && selectedControl != null)
					{
						GUI.changed = true;
						DrawGUI.Use(e);

						// Copy exceptions should be handled internally by drawer but making sure.
						try
						{
							selectedControl.CopyToClipboard();
						}
						#if DEV_MODE
						catch(Exception exception)
						{
							Debug.LogWarning(exception.ToString());
						#else
						catch(Exception)
						{
						#endif
							// This should handle detecting that the operation failed and displaying a fitting message to the user.
							Clipboard.SendCopyToClipboardMessage(selectedControl.GetFieldNameForMessages());
						}
					}
					break;
				case "Paste":
					if(!DrawGUI.EditingTextField && selectedControl != null)
					{
						var manager = Manager;
						var type = selectedControl.Type;
						if(manager.HasMultiSelectedControls)
						{
							if(Clipboard.CanPasteAs(selectedControl.Type))
							{
								GUI.changed = true;
								DrawGUI.Use(e);
								var multiselected = manager.MultiSelectedControls;
								for(int n = multiselected.Count - 1; n >= 0; n--)
								{
									multiselected[n].PasteFromClipboard();
								}

								ExitGUIUtility.ExitGUI();
							}
							else
							{
								var arrayType = type.MakeArrayType();
								if(Clipboard.CanPasteAs(arrayType))
								{
									var setValues = Clipboard.Paste(arrayType) as Array;
									GUI.changed = true;
									DrawGUI.Use(e);
									var multiselected = manager.MultiSelectedControls;
									int valueCount = setValues.Length;

									Exception rethrowException = null;

									for(int n = Mathf.Min(multiselected.Count, valueCount) - 1; n >= 0; n--)
									{
										try
										{
											multiselected[n].SetValue(setValues.GetValue(n));
										}
										catch(Exception exception)
										{
											#if DEV_MODE
											Debug.LogWarning(exception.ToString());
											#endif

											if(ExitGUIUtility.ShouldRethrowException(exception))
											{
												rethrowException = exception;
											}

											// This should handle detecting that the operation failed and displaying a fitting message to the user.
											Clipboard.SendPasteFromClipboardMessage(selectedControl.GetFieldNameForMessages());
										}
									}

									if(rethrowException != null)
									{
										throw rethrowException;
									}
									ExitGUIUtility.ExitGUI();
								}
								else
								{
									Clipboard.SendOperationFailedMessage("");
								}
							}
						}
						else if(Clipboard.CanPasteAs(selectedControl.Type))
						{
							GUI.changed = true;
							DrawGUI.Use(e);

							// Paste exceptions should be handled internally by drawer but making sure.
							try
							{
								selectedControl.PasteFromClipboard();
								ExitGUIUtility.ExitGUI();
							}
							catch(Exception exception)
							{
								#if DEV_MODE
								Debug.LogWarning(exception.ToString());
								#endif

								if(ExitGUIUtility.ShouldRethrowException(exception))
								{
									throw;
								}

								// This should handle detecting that the operation failed and displaying a fitting message to the user.
								Clipboard.SendPasteFromClipboardMessage(selectedControl.GetFieldNameForMessages());
							}
						}
						else
						{
							Clipboard.SendOperationFailedMessage(selectedControl.GetFieldNameForMessages());
						}
					}
					break;
				case "Reset":
					if(selectedControl != null)
					{
						GUI.changed = true;
						DrawGUI.Use(e);

						// Reset exceptions should be handled internally by drawer but making sure.
						try
						{
							selectedControl.Reset(true);
							ExitGUIUtility.ExitGUI();
						}
						catch(Exception exception)
						{
							#if DEV_MODE
							Debug.LogWarning(exception.ToString());
							#endif

							if(ExitGUIUtility.ShouldRethrowException(exception))
							{
								throw;
							}
						}
					}
					break;
				case "Duplicate":
					if(!DrawGUI.EditingTextField && selectedControl != null)
					{
						var collection = selectedControl.Parent as ICollectionDrawer;
						if(collection != null)
						{
							int index = collection.GetMemberIndexInCollection(selectedControl);
							if(index != -1)
							{
								var fieldDrawer = selectedControl as IFieldDrawer;
								if(fieldDrawer != null)
								{
									collection.DuplicateMember(fieldDrawer);
								}
							}

							//don't try to duplicate collection resize fields
							return;
						}

						#if DEV_MODE
						Debug.Log("Calling duplicate on selectedControl " + selectedControl);
						#endif
						GUI.changed = true;
						DrawGUI.Use(e);

						try
						{
							selectedControl.Duplicate();
							ExitGUIUtility.ExitGUI();
						}
						catch(Exception exception)
						{
							#if DEV_MODE
							Debug.LogWarning(exception.ToString());
							#endif

							if(ExitGUIUtility.ShouldRethrowException(exception))
							{
								throw;
							}
						}
					}
					break;
			}
		}
		
		public void OnExecuteCommand(Event e)
		{
			var selectedControl = FocusedDrawer;
			if(selectedControl != null)
			{
				selectedControl.OnExecuteCommand(e);
			}
		}

		public void ResetNextUpdateCachedValues()
		{
			state.nextUpdateCachedValues = UpdateCachedValuesInterval;
		}

		public void UpdateCachedValuesFromFields()
		{
			state.nextUpdateCachedValues = UpdateCachedValuesInterval;
			state.drawers.UpdateCachedValuesFromFieldsRecursively();
		}

		/// <inheritdoc/>
		public void BroadcastOnFilterChanged()
		{
			ApplyForEachControl(BroadcastOnFilterChanged);
		}

		/// <summary>
		/// Calls OnFilterChanged for the drawers.
		/// </summary>
		private void BroadcastOnFilterChanged(IDrawer control)
		{
			control.OnFilterChanged(state.filter);
		}
		
		private void OnFindCommandGiven()
		{
			var toolbar = Toolbar;
			if(toolbar != null)
			{
				if(toolbar.OnFindCommandGiven())
				{
					if(SelectedPart != InspectorPart.Toolbar)
					{
						Manager.Select(this, InspectorPart.Toolbar, ReasonSelectionChanged.KeyPressShortcut);
					}

					GUIUtility.ExitGUI();
				}
			}
		}

		public void SetFilter(string setFilterValue)
		{
			state.filter.SetFilter(setFilterValue, this);
		}

		/// <inheritdoc/>
		public virtual void OnProjectOrHierarchyChanged(OnChangedEventSubject changed)
		{
			#if UNITY_EDITOR
			if(UnityEditor.EditorApplication.isCompiling)
			{
				#if DEV_MODE
				Debug.LogWarning("Cancelling OnProjectOrHierarchyChanged because EditorApplication.isCompiling");
				#endif
				return;
			}
			#endif

			state.OnProjectOrHierarchyChanged(changed);
			
			if(!state.TryRecoverAnyNullUnityObjects())
			{
				#if DEV_MODE && DEBUG_PROJECT_OR_HIERARCHY_CHANGED
				Debug.Log("OnProjectOrHierarchyChanged("+changed+") - Rebuilding drawers because had null targets.");
				#endif
				
				RebuildDrawers(true);
			}
			else if(!RebuildDrawers(false))
			{
				#if DEV_MODE && DEBUG_PROJECT_OR_HIERARCHY_CHANGED
				Debug.Log("Drawer not rebuilt after OnProjectOrHierarchyChanged");
				#endif

				var inspected = state.drawers;

				bool drawerHaveInvalidState = false;
				for(int n = inspected.Length - 1; n >= 0; n--)
				{
					var go = inspected[n] as IOnProjectOrHierarchyChanged;
					if(go != null)
					{
						go.OnProjectOrHierarchyChanged(changed, ref drawerHaveInvalidState);
						if(drawerHaveInvalidState)
						{
							RebuildDrawers(true);
							break;
						}
					}
				}
				return;
			}
			else
			{
				#if DEV_MODE && DEBUG_PROJECT_OR_HIERARCHY_CHANGED
				Debug.Log("OnProjectOrHierarchyChanged("+changed+") - Drawer rebuilt");
				#endif
			}

			if(State.ViewIsLocked)
			{
				State.ViewIsLocked = false;
			}
		}

		public bool RequiresConstantRepaint()
		{
			try
			{
				var drawers = state.drawers;
				for(int n = drawers.Length - 1; n >= 0; n--)
				{
					if(drawers[n].RequiresConstantRepaint)
					{
						return true;
					}				
				}
			}
			#if DEV_MODE
			// this can happen for a few frames when the inspected asset is destroyed
			catch(NullReferenceException e)
			{
				Debug.LogError(e);
			#else
			catch
			{
			#endif
				ForceRebuildDrawers();
				return false;
			}

			var previewDrawer = PreviewDrawer;
			if(previewDrawer != null)
			{
				return previewDrawer.RequiresConstantRepaint();
			}

			return false;
		}

		/// <inheritdoc/>
		public void EnableDebugMode()
		{
			SetDebugMode(true);
		}

		/// <inheritdoc/>
		public void DisableDebugMode()
		{
			SetDebugMode(false);
		}

		private void SetDebugMode(bool value)
		{
			var inspected = state.inspected;
			RebuildDrawers(ArrayPool<Object>.ZeroSizeArray, true);
			state.DebugMode = value;
			RebuildDrawers(inspected, true);
		}

		protected void UpdateDrawPreviewArea()
		{
			SetDrawPreviewArea(ShouldDrawPreviewArea());
		}

		protected void SetDrawPreviewArea(bool setDrawPreviewArea)
		{
			#if DEV_MODE && DEBUG_PREVIEW_AREA
			Debug.Log(ToString()+".drawPreviewArea = "+StringUtils.ToColorizedString(setDrawPreviewArea));
			#endif

			if(drawPreviewArea != setDrawPreviewArea)
			{
				drawPreviewArea = setDrawPreviewArea;
				GUI.changed = true;
			}
		}

		public void ReloadPreviewInstances()
		{
			if(PreviewDrawer != null)
			{
				PreviewDrawer.ReloadPreviewInstances();
			}
			inspectorDrawer.RefreshView();
		}

		/// <summary> Determine if we should draw preview area for current targets with current preferences. </summary>
		/// <returns> True if should draw preview area, false if shouldn't. </returns>
		protected abstract bool ShouldDrawPreviewArea();

		/// <summary> This should be called by the IInspectorDrawer of the inspector during every OnGUI event. </summary>
		/// <param name="inspectorDimensions"> The position and bounds for where the inspecto should be drawn. </param>
		/// <param name="anyInspectorPartMouseovered"> True if any inspector part is currently mouseovered. </param>
		public abstract void OnGUI(Rect inspectorDimensions, bool anyInspectorPartMouseovered);

		#if !POWER_INSPECTOR_LITE
		/// <summary>
		/// Creates the toolbar that should appear at the top of the inspector.
		/// </summary>
		/// <returns>New instance of Inspector toolbar or null if inspector should have no toolbar. </returns>
		[CanBeNull]
		protected virtual IInspectorToolbar CreateToolbar()
		{
			return null;
		}

		/// <summary>
		/// Setups inspector toolbar so it's ready to be used.
		/// </summary>
		/// <param name="setupToolbar">Toolbar to setup. Can not be null.</param>
		protected virtual void SetupToolbar([NotNull]IInspectorToolbar setupToolbar)
		{
			setupToolbar.Setup(this);
		}
#endif
	}
}