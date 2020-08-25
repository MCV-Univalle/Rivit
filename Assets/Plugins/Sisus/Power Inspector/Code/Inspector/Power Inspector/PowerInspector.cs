#define SAFE_MODE

//#define DEBUG_CLICK
//#define DEBUG_MOUSEOVERED_PART

using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	[Serializable]
	public sealed class PowerInspector : Inspector
	{
		private static InspectorPreferences preferencesCached;
		
		[SerializeField]
		private readonly PreviewDrawer previewDrawer;
		
		/// <inheritdoc />
		protected override PreviewDrawer PreviewDrawer
		{
			get
			{
				return previewDrawer;
			}
		}
		
		/// <summary> Initializes the PowerInspector instance. </summary>
		/// <param name="inspector"> The inspector. </param>
		/// <param name="preferences"> inspector preferences. </param>
		/// <param name="drawer"> The drawer. </param>
		/// <param name="inspected"> The inspected targets. </param>
		/// <param name="scrollPos"> The viewport scroll position. </param>
		/// <param name="viewIsLocked"> True if view is locked. </param>
		public static void Setup(PowerInspector inspector, InspectorPreferences preferences, IInspectorDrawer drawer, Object[] inspected, Vector2 scrollPos, bool viewIsLocked)
		{
			inspector.Setup(drawer, preferences, inspected, scrollPos, viewIsLocked);
		}

		/// <inheritdoc/>
		public override void Setup(IInspectorDrawer drawer, InspectorPreferences setPreferences, Object[] inspected, Vector2 scrollPos, bool viewIsLocked)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(setPreferences != null);
			#endif

			// Call base.Setup before calling Setup for the toolbar, so that the Preferences field gets assigned first
			base.Setup(drawer, setPreferences, inspected, scrollPos, viewIsLocked);
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Preferences != null);
			Debug.Assert(Preferences == setPreferences);
			#endif
		}

		/// <summary>
		/// Gets the preferences asset for 
		/// </summary>
		/// <returns> The preferences for  </returns>
		public static InspectorPreferences GetPreferences()
		{
			return InspectorPreferences.GetSettingsCached(ref preferencesCached, true);
		}

		/// <summary> This should be called by the IInspectorDrawer of the inspector during every OnGUI event. </summary>
		/// <param name="inspectorDimensions"> The position and bounds for where the inspecto should be drawn. </param>
		/// <param name="anyInspectorPartMouseovered"> True if any inspector part is currently mouseovered. </param>
		public override void OnGUI(Rect inspectorDimensions, bool anyInspectorPartMouseovered)
		{
			UnityEngine.Profiling.Profiler.BeginSample("OnGUI");
			
			#if DEV_MODE && DEBUG_CLICK
			var ev = Event.current;
			if(ev.rawType == EventType.MouseDown) { Debug.Log(StringUtils.ToColorizedString(ToString()+" Event=", ev, ", e.type=", ev.type, ", button=", ev.button, ", mousePos=", ev.mousePosition, ", GUIUtility.hotControl=", GUIUtility.hotControl)); }
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			if(inspectorDimensions.width <= 0f) { Debug.LogError(GetType().Name+ ".OnGUI inspectorDimensions.width <= 0f: " + inspectorDimensions);	}
			#endif

			//this can happen e.g. if the preferences file gets reimported due to being altered outside of Unity
			if(Preferences == null)
			{
				Preferences = GetPreferences();
			}

			#if DEV_MODE && DEBUG_MOUSEOVERED_PART
			if(State.drawer.VisibleMembers.Length > 0 && DrawGUI.IsUnityObjectDrag)
			{
				Debug.Log(StringUtils.ToColorizedString(ToString(), ".OnGUI with mouseoveredPart=", MouseoveredPart, ", Event="+StringUtils.ToString(Event.current), ", ignoreAllMouseInputs=", InspectorDrawer.Manager.IgnoreAllMouseInputs, "´, ObjectPickerIsOpen=", ObjectPicker.IsOpen, ", anyInspectorPartMouseovered=", anyInspectorPartMouseovered, ", InspectorDrawer.MouseIsOver=", InspectorDrawer.MouseIsOver, ", DrawGUI.CanRequestMousePosition=", Cursor.CanRequestLocalPosition));
			}
			#endif

			InspectorUtility.BeginInspector(this, ref anyInspectorPartMouseovered);
			
			Rect toolbarRect;
			Rect viewportRect;
			Rect previewAreaRect;
			GetDrawPositions(inspectorDimensions, out toolbarRect, out viewportRect, out previewAreaRect);

			// trying to fix a bug where the default inspector layout gets wacky if both it and this window are open
			// by making sure all values that could affect it are restored back to normal
			// var indentLevelWas = EditorGUI.indentLevel;
			#if UNITY_EDITOR
			var labelWidthWas = EditorGUIUtility.labelWidth;
			#endif
			var matrixWas = GUI.matrix;
	
			var currentEvent = Event.current;
			switch(currentEvent.type)
			{
				case EventType.Layout:
					State.nextUpdateCachedValues--;
					if(State.nextUpdateCachedValues <= 0)
					{
						UpdateCachedValuesFromFields();
					}
					OnCursorPositionOrLayoutChanged();
					break;
                case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.DragUpdated:
					if(IgnoreViewportMouseInputs())
					{
						#if DEV_MODE
						//Debug.Log("ignoring "+ currentEvent.type+"...");
						#endif
						break;
					}
					
					OnCursorPositionOrLayoutChanged();
					InspectorDrawer.RefreshView();
					break;
			}

			bool dirty;
			try
			{
				dirty = DrawViewport(viewportRect);
			}
			catch(Exception e)
			{
				if(ExitGUIUtility.ShouldRethrowException(e))
				{
					NowDrawingPart = InspectorPart.None;
					DrawGUI.IndentLevel = 0;
					#if UNITY_EDITOR
					EditorGUIUtility.labelWidth = labelWidthWas;
					#endif
					GUI.skin = null;
					GUI.matrix = matrixWas;
					throw;
				}
				#if DEV_MODE
				Debug.LogWarning(ToString()+" "+e);
				#endif

				// Always throw ExitGUI exception if exceptions were caught to avoid GUI Layout warnings.
				ExitGUIUtility.ExitGUI();
				return;
			}

			#if !POWER_INSPECTOR_LITE
			NowDrawingPart = InspectorPart.Toolbar;
			{
				Toolbar.Draw(toolbarRect);
			}
			#endif
			NowDrawingPart = InspectorPart.Other;
			
			#if UNITY_EDITOR
			try
			{
				if(DrawPreviewArea)
				{
					NowDrawingPart = InspectorPart.PreviewArea;
					{
						previewDrawer.Draw(previewAreaRect);
					}
					NowDrawingPart = InspectorPart.Other;
				}
			}
			#if DEV_MODE
			catch(ArgumentException e) // GUILayout: Mismatched LayoutGroup.repaint
			{
				Debug.LogError(StringUtils.ToString(Event.current)+" "+e+"\nEvent="+StringUtils.ToString(Event.current));
			#else
			catch(ArgumentException)
			{
			#endif
				//NowDrawingPart = InspectorPart.Other;

				// new test to avoid GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced.
				NowDrawingPart =  InspectorPart.None;
				ExitGUIUtility.ExitGUI();
			}
			#endif
			
			//TO DO: Move to EndInspector if these are needed?
			//trying to fix a bug where the default inspector layout gets wacky if both it and this window are open
			//by making sure all values that could affect it are restored back to normal
			DrawGUI.IndentLevel = 0;
			#if UNITY_EDITOR
			EditorGUIUtility.labelWidth = labelWidthWas;
			#endif
			GUI.skin = null;
			GUI.matrix = matrixWas;
			
			if(dirty)
			{
				InspectorDrawer.RefreshView();
			}

			InspectorUtility.EndInspector(this);

			UnityEngine.Profiling.Profiler.EndSample();
		}
		
		/// <inheritdoc/>
		public override void Message(GUIContent message, Object context = null, MessageType messageType = MessageType.Info, bool alsoLogToConsole = true)
		{
			InspectorDrawer.Message(message, context, messageType, alsoLogToConsole);
		}

		public override string ToString()
		{
			if(InspectorDrawer != null)
			{
				if(InspectorDrawer.MainView == this)
				{
					return "PowerInspector";
				}
				var splittable = InspectorDrawer as ISplittableInspectorDrawer;
				if(splittable != null && splittable.SplitView == this)
				{
					return "PowerInspector/SplitView";
				}
			}
			return "PowerInspector(???)";
		}

		public PowerInspector()
		{
			previewDrawer = new PreviewDrawer(this);
		}

		/// <inheritdoc/>
		protected override bool ShouldDrawPreviewArea()
		{
			try
			{
				return State.drawers.Length > 0 && Platform.EditorMode && (State.assetMode || previewDrawer.HasPreviews);
			}
			// NullReferenceException was thrown at one point when Dispose was called. Should be fixed now.
			#if DEV_MODE
			catch(NullReferenceException e)
			{
				if(State == null)
				{
					Debug.Log(ToString()+".ShouldDrawPreviewArea was called with State "+StringUtils.Null +"\n\n"+e);
				}
				else if(State.drawers == null)
				{
					Debug.Log(ToString()+".ShouldDrawPreviewArea was called with State.drawer "+StringUtils.Null +"\n\n"+e);
				}
				else if(previewDrawer == null)
				{
					Debug.Log(ToString()+".ShouldDrawPreviewArea was called with previewDrawer "+StringUtils.Null +"\n\n"+e);
				}
				else
				{
					Debug.LogError(ToString()+".ShouldDrawPreviewArea "+e);
				}
			#else
			catch(NullReferenceException)
			{
			#endif
				return false;
			}
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			base.Dispose();
			previewDrawer.ResetState();
			SetDrawPreviewArea(false);
		}

		/// <inheritdoc/>
		public override void OnProjectOrHierarchyChanged(OnChangedEventSubject changed)
		{
			if(Preferences == null)
			{
				Preferences = GetPreferences();
			}

			base.OnProjectOrHierarchyChanged(changed);

			previewDrawer.OnProjectOrHierarchyChanged();
			UpdateDrawPreviewArea();
		}

		#if !POWER_INSPECTOR_LITE
		/// <inheritdoc/>
		protected override IInspectorToolbar CreateToolbar()
		{
			return new PowerInspectorToolbar();
		}
		#endif
	}
}