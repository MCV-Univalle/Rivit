//#define DEBUG_SCROLL_TO_SHOW
//#define DEBUG_VIEW_IS_LOCKED
//#define DEBUG_CAN_HAVE_SCROLLBAR
//#define DEBUG_HAS_SCROLLBAR
//#define DEBUG_UPDATE_DIMENSIONS
//#define DEBUG_UPDATE_CONTENT_RECT

using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Sisus
{
	/// <summary>
	/// Contains the state data of an inspector instance.
	/// </summary>
	public class InspectorState
	{
		private static readonly Rect ZeroSizeRect = new Rect();

		public DrawerGroup drawers;
		public Object[] inspected = new Object[0];
		public readonly SearchFilter filter = new SearchFilter();
		
		public bool assetMode;
		public Rect contentRect = new Rect(0f, 0f, 0f, 0f);
		
		#if !POWER_INSPECTOR_LITE
		public readonly SelectionHistory selectionHistory = new SelectionHistory();
		#endif
		
		public Rect previousKeyboardControlRect;
		public int previousKeyboardControl;
		public Rect keyboardRectLastFrame;
		public int keyboardControlLastFrame;
		public int nextUpdateCachedValues;

		public Action<bool> OnDebugModeChanged;
		public Action<float> OnHeightChanged;
		public Action OnWidthChanged;
		public Action OnLockedStateChanged;
		public Action OnScrollPosChanged;

		/// <summary>
		/// Callback that is invoked every time the inspected targets have changed.
		/// </summary>
		public Action<Object[], DrawerGroup> OnInspectedTargetsChanged;

		private bool hasScrollBar;
		private Rect windowRect = new Rect(0f, 0f, 0f, 0f);
		private readonly List<DrawerActionWithId> onNextLayoutForAllVisibleDrawers = new List<DrawerActionWithId>(2);
		private bool debugMode;
		private bool viewIsLocked;
		private Vector2 scrollPos;

		public bool usingLocalSpace = true;

		/// <summary> Gets a value indicating whether the current drawers are requesting that any search boxes that might exist on the toolbar are disabled. </summary>
		/// <value> True if toolbar search box should be disabled, false if not. </value>
		public bool WantsSearchBoxDisabled
		{
			get
			{
				return drawers.WantsSearchBoxDisabled;
			}
		}

		public SearchFilter SearchFilter
		{
			get
			{
				return filter;
			}
		}

		public bool ViewIsLocked
		{
			get
			{
				return viewIsLocked || filter.DeterminesInspectedTarget();
			}

			set
			{
				if(viewIsLocked != value)
				{
					#if DEV_MODE && DEBUG_VIEW_IS_LOCKED
					Debug.Log("viewIsLocked = "+StringUtils.ToColorizedString(value));
					#endif

					viewIsLocked = value;
					
					if(OnLockedStateChanged != null)
					{
						OnLockedStateChanged();
					}
				}
			}
		}

		public Vector2 ScrollPos
		{
			get
			{
				return scrollPos;
			}

			set
			{
				if(scrollPos != value)
				{
					scrollPos = value;

					if(OnScrollPosChanged != null)
					{
						OnScrollPosChanged();
					}
				}
			}
		}

		public bool DebugMode
		{
			get
			{
				return debugMode;
			}

			set
			{
				if(debugMode != value)
				{
					#if DEV_MODE && PI_ASSERTATIONS
					Debug.Assert(drawers.Length == 0, "You should clear the drawer before changing debug mode!");
					#endif

					debugMode = value;

					if(OnDebugModeChanged != null)
					{
						OnDebugModeChanged(debugMode);
					}
				}
			}
		}
		
		public bool HasScrollBar
		{
			get
			{
				return hasScrollBar;
			}

			private set
			{
				#if DEV_MODE && DEBUG_HAS_SCROLLBAR
				if(value != hasScrollBar) { Debug.Log("HasScrollBar ="+StringUtils.ToColorizedString(value)); }
				#endif

				hasScrollBar = value;
			}
		}

		/// <summary> Gets rectangle describing the dimensions of the inspector window, with x and y values always be at zero. </summary>
		/// <value> The size of the inspector window. </value>
		public Rect WindowRect
		{
			get
			{
				return windowRect;
			}
		}

		/// <summary> Gets rectangle describing the screen-space position and size of the inspector window. </summary>
		/// <value> The size and position of the inspector window. </value>
		public Rect ScreenSpaceWindowRect
		{
			get
			{
				var color = windowRect;
				color.x += scrollPos.x;
				color.y += scrollPos.y;
				return color;
			}
		}

		public Rect ScrollBarRect
		{
			get
			{
				var scrollBarRect = windowRect;
				scrollBarRect.width = DrawGUI.ScrollBarWidth;
				scrollBarRect.x = windowRect.x + windowRect.width - DrawGUI.ScrollBarWidth;
				return scrollBarRect;
			}
		}

		public void Setup(IInspector inspector, Vector2 setScrollPos, bool setViewIsLocked, Action doOnLockedStateChanged = null)
		{
			#if DEV_MODE
			Debug.Assert(Screen.width > 0f, "!  InspectorState.Setup called with Screen.width==0f! This should only be called from OnGUI! \nEvent was "+StringUtils.ToString(Event.current)+", contentRect.width was "+ contentRect.width);
			#endif

			if(drawers == null)
			{
				drawers = DrawerGroup.Create(inspector, null, new GUIContent("Inspected"));
			}

			if(!TryRecoverAnyNullUnityObjects())
			{
				#if DEV_MODE
				if(setViewIsLocked) { Debug.LogWarning("InspectorState.Setup - changing setViewIsLocked to "+StringUtils.False+" because TryRecoverNullUnityObjects returned "+StringUtils.False); }
				#endif
				setViewIsLocked = false;
			}

			scrollPos = setScrollPos;

			#if DEV_MODE && DEBUG_VIEW_IS_LOCKED
			Debug.Log("viewIsLocked = "+StringUtils.ToColorizedString(setViewIsLocked));
			#endif

			viewIsLocked = setViewIsLocked;

			// new test
			if(OnLockedStateChanged != null)
			{
				OnLockedStateChanged -= doOnLockedStateChanged;
				OnLockedStateChanged += doOnLockedStateChanged;
			}

			if(contentRect.width <= 0f)
			{
				if(Screen.width > 0f)
				{
					windowRect.width = Screen.width - 4f;
					contentRect.width = windowRect.width;
				}
				else
				{
					InspectorUtility.ActiveInspector.OnNextLayout(()=>
					{
						if(contentRect.width <= 0f)
						{
							windowRect.width = Screen.width - 4f;
							contentRect.width = windowRect.width;
						}
					});
				}
			}
		}

		/// <summary>
		/// Checks if inspected contains any targets that equal null, and if so,
		/// attempts to recover references to them. Any targets that remain null
		/// after these attempts get removed from the inspected array.
		/// </summary>
		/// <returns> True if there were no null targets, or any null targets could be recovered. Returns false if there were unrecoverable null targets. </returns>
		public bool TryRecoverAnyNullUnityObjects()
		{
			bool noUnfixableNullsFound = true;
			for(int n = inspected.Length - 1; n >= 0; n--)
			{
				var target = inspected[n];
				if(target == null)
				{
					if(UnityObjectExtensions.TryToFixNull(ref target))
					{
						#if DEV_MODE
						Debug.LogWarning("InspectorState.inspected["+n+"] (\""+target.name+"\") was null, but was able to recover it using InstanceID.");
						#endif
						continue;
					}

					#if DEV_MODE
					Debug.LogWarning("InspectorState.inspected["+n+"] (\""+(ReferenceEquals(target, null) ? "null" : target.name)+"\") was null and unrecoverable. Removing from array");
					#endif
			
					// if could not recover target, remove null entry from array
					inspected = inspected.RemoveAt(n);
					noUnfixableNullsFound = false;
				}
			}
			return noUnfixableNullsFound;
		}

		public void Dispose()
		{
			if(drawers != null && drawers.Length > 0)
			{
				drawers.Clear();
			}
			scrollPos = default(Vector2); 
			ArrayExtensions.ArrayToZeroSize(ref inspected);
			filter.SetFilter("", null, null, null);
			
			contentRect.y = 0f;
			contentRect.height = 0f;
			
			windowRect.y = 0f;
			windowRect.height = 0f;
			
			#if !POWER_INSPECTOR_LITE
			selectionHistory.Clear();
			#endif

			viewIsLocked = false;
			assetMode = false;
			HasScrollBar = false;
			usingLocalSpace = true;

			onNextLayoutForAllVisibleDrawers.Clear();
			
			previousKeyboardControlRect = default(Rect);
			previousKeyboardControl = 0;
			keyboardRectLastFrame = default(Rect);
			keyboardControlLastFrame = 0;

			OnScrollPosChanged = null;
			OnLockedStateChanged = null;
			OnDebugModeChanged = null;
			OnWidthChanged = null;
			OnHeightChanged = null;
		}

		/// <summary>
		/// Useful for example for detecting mouseover in the right
		/// place (in the gui layout structure) and at the right time
		/// (during the right event)
		/// </summary>
		public void OnNextLayoutForVisibleDrawers(int id, Action<IDrawer> action)
		{
			for(int n = onNextLayoutForAllVisibleDrawers.Count - 1; n >= 0; n--)
			{
				if(onNextLayoutForAllVisibleDrawers[n].id == id)
				{
					onNextLayoutForAllVisibleDrawers[n].action = action;
					return;
				}
			}
			onNextLayoutForAllVisibleDrawers.Add(new DrawerActionWithId(id, action));
		}

		public void ClearOnNextLayoutForVisibleDrawers(int id)
		{
			for(int n = onNextLayoutForAllVisibleDrawers.Count - 1; n >= 0; n--)
			{
				if(onNextLayoutForAllVisibleDrawers[n].id == id)
				{
					onNextLayoutForAllVisibleDrawers.RemoveAt(n);
					return;
				}
			}
		}

		public void ClearAllOnNextLayoutForVisibleDrawers()
		{
			onNextLayoutForAllVisibleDrawers.Clear();
		}

		public void HandleOnNextLayoutForVisibleDrawers(IDrawer drawer)
		{
			for(int n = onNextLayoutForAllVisibleDrawers.Count - 1; n >= 0; n--)
			{
				try
				{
					onNextLayoutForAllVisibleDrawers[n].action(drawer);
				}
				catch(Exception e)
				{
					Debug.LogError(e);
				}
			}
		}

		/// <summary>
		/// instantly scrolls the view upwards or downwards the minimum amount
		/// necessary to show the whole area. If the area is larger than the current
		/// view, then position it so it starts from the very top of the view.
		/// </summary>
		/// <param name="area"></param>
		public void ScrollToShow(Rect area)
		{
			float windowHeight = windowRect.height;
			float targetStart = area.y;

			if(area.height > windowHeight)
			{
				//if target area is larger than current view
				//scroll the view so that the top of the target
				//area is at the top of the view window
				SetScrollPosY(targetStart);

				#if DEV_MODE && DEBUG_SCROLL_TO_SHOW
				Debug.Log("(too large) scroll up until top is positioned at top of view: "+§.y);
				#endif
				return;
			}

			float currentPos = scrollPos.y;
			
			float targetEnd = area.y + area.height;
			float windowEnd = currentPos + windowHeight;

			bool dirIsDown = targetStart > currentPos;
			if(dirIsDown)
			{
				if(targetEnd < windowEnd)
				{
					//area already inside view, no need to scroll
					#if DEV_MODE && DEBUG_SCROLL_TO_SHOW
					Debug.Log("(inside) no need to scroll)");
					#endif
					return;
				}
				
				//position end of area at bottom of window
				//append by single line height so that it's easier for the user
				//to make out that this is the very end of the component
				SetScrollPosY(targetEnd - windowHeight + DrawGUI.SingleLineHeight);

				#if DEV_MODE && DEBUG_SCROLL_TO_SHOW
				Debug.Log("(below) position end of area at bottom of window: "+scrollPos.y+" (targetEnd="+targetEnd+", windowHeight="+windowHeight+ ", scrollPosWas="+ currentPos+", windowEnd = "+ windowEnd+")");
				#endif
				return;
			}
			
			//if target area is above view, scroll up until the top
			//of the area is positioned at the top of the view window
			SetScrollPosY(targetStart);

			#if DEV_MODE && DEBUG_SCROLL_TO_SHOW
			Debug.Log("(above) scroll up until top is positioned at top of view: "+scrollPos.y);
			#endif
		}
		
		public void SetScrollPosY(float value)
		{
			if(!scrollPos.y.Equals(value))
			{
				scrollPos.y = value;
				if(OnScrollPosChanged != null)
				{
					OnScrollPosChanged();
				}
			}
		}

		/// <summary>
		/// Called every time the inspected targets are changed.
		/// </summary>
		/// <param name="inspector"> The inspector. </param>
		public void OnInspectedChanged(IInspector inspector)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!inspected.ContainsNullObjects());
			Debug.Assert(!inspected.ContainsObjectsOfType(typeof(Transform)));
			#endif

			#if !POWER_INSPECTOR_LITE
			selectionHistory.RecordNewSelection(inspected);
			#endif

			HasScrollBar = false;
			Setup(inspector, Vector2.zero, viewIsLocked, null);

			if(OnInspectedTargetsChanged != null)
			{
				OnInspectedTargetsChanged(inspected, drawers);
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(OnInspectedTargetsChanged.GetInvocationList().Length < 5, "OnInspectedTargetsChanged.GetInvocationList().Length was "+ OnInspectedTargetsChanged.GetInvocationList().Length+". Seems suspicious. Intentional?");
			#endif
		}

		public void UpdateDimensions(bool drawOnScreen, bool drawOffscreen, Rect inspectorWindowRect, float toolbarHeight, float previewAreaHeight)
		{
			#if DEV_MODE && DEBUG_UPDATE_DIMENSIONS
			bool windowRectChanged = windowRect != inspectorWindowRect;
			#endif

			windowRect = inspectorWindowRect;
			
			#if DEV_MODE && DEBUG_UPDATE_CONTENT_RECT
			var componentHeights = "";
			#endif

			float contentHeight = 0f;
			int goCount = drawers.Length;
			for(var i = 0; i < goCount; i++)
			{
				var drawer = drawers[i];
				if(drawer != null)
				{
					if(drawOnScreen)
					{
						//this is needed because some Drawer can only calculate their heights during the OnGUI phase
						GUI.changed = false;
						drawers.Draw(inspectorWindowRect);
					}
					else if(drawOffscreen)
					{
						//this is needed because some Drawer can only calculate their heights during the OnGUI phase
						GUI.changed = false;
						drawers.Draw(ZeroSizeRect);
					}

					float drawerHeight = drawer.Height;
					contentHeight += drawerHeight;

					#if DEV_MODE && DEBUG_UPDATE_CONTENT_RECT
					var members = (drawer as IParentDrawer).VisibleMembers;
					componentHeights = "\n"+drawer.ToString() + " "+members.Length+ " VisibleMembers:";
					foreach(var member in (drawer as IParentDrawer).VisibleMembers)
					{
						var parent = member as IParentDrawer;
						if(parent != null)
						{
							componentHeights += "\n" + member.Type.Name + " : " + member.Height + " (Unfolded=" + parent.Unfolded + ", Unfoldedness="+parent.Unfoldedness+")";
						}
						else
						{
							componentHeights += "\n"+ member.Type.Name + " : " + member.Height;
						}
					}
					#endif
				}
			}

			#if DEV_MODE && DEBUG_UPDATE_DIMENSIONS			
			if(windowRectChanged) { Debug.Log("windowRect = "+windowRect); }
			#endif

			bool heightChanged = false;
			if(!Mathf.Approximately(contentRect.height, contentHeight))
			{
				#if DEV_MODE && DEBUG_UPDATE_CONTENT_RECT
				Debug.Log("contentHeight = "+ contentHeight+" ( was "+ contentRect.height+"). Event="+StringUtils.ToString(Event.current)+ componentHeights);
				#endif

				contentRect.height = contentHeight;
				heightChanged = true;

				#if DEV_MODE && DEBUG_CAN_HAVE_SCROLLBAR
				if(drawer.Length >= 1) { Debug.Log("CanHaveScrollBar: "+StringUtils.ToColorizedString(CanHaveScrollBar())+" with drawer[0] "+drawer[0]+" is IScrollable: "+StringUtils.ToColorizedString(drawer[0] is IScrollable)); }
				#endif
			}
			
			float setWidth;
			if(CanHaveScrollBar() && toolbarHeight + contentHeight + previewAreaHeight > inspectorWindowRect.height)
			{
				setWidth = inspectorWindowRect.width - DrawGUI.ScrollBarWidth;
				HasScrollBar = true;
			}
			else
			{
				setWidth = inspectorWindowRect.width;
				HasScrollBar = false;
			}

			bool widthChanged = false;
			if(!Mathf.Approximately(contentRect.width, setWidth))
			{
				contentRect.width = setWidth;
				widthChanged = true;
			}

			if(widthChanged && OnWidthChanged != null)
			{
				OnWidthChanged();
			}

			if(heightChanged && OnHeightChanged != null)
			{
				OnHeightChanged(contentHeight);
			}
		}

		public bool CanHaveScrollBar()
		{
			return drawers.Length != -1 || !(drawers[0] is IScrollable);
		}
		public void OnProjectOrHierarchyChanged(OnChangedEventSubject changed)
		{
			#if !POWER_INSPECTOR_LITE
			selectionHistory.RemoveUnloadedAndInvalidTargets();
			#endif
		}
	}
}