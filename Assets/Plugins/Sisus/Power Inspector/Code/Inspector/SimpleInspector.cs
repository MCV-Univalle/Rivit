using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sisus
{
	public sealed class SimpleInspector : Inspector
	{
		/// <inheritdoc/>
		protected override PreviewDrawer PreviewDrawer
		{
			get
			{
				return null;
			}
		}
		
		public static void Setup([NotNull]SimpleInspector inspector, [NotNull]InspectorPreferences preferences, [NotNull]IInspectorDrawer drawer, [NotNull]Object[] inspected, Vector2 scrollPos, bool viewIsLocked)
		{
			inspector.Setup(drawer, preferences, inspected, scrollPos, viewIsLocked);
		}

		/// <summary> This should be called by the IInspectorDrawer of the inspector during every OnGUI event. </summary>
		/// <param name="position"> Rect describing the size and position of the inspector. </param>
		public void OnGUI(Rect position)
		{
			OnGUI(position, InspectorDrawer.MouseIsOver, true);
		}

		/// <inheritdoc/>
		public override void OnGUI(Rect position, bool anyInspectorPartMouseovered)
		{
			OnGUI(position, anyInspectorPartMouseovered, true);
		}

		/// <summary> This should be called by the IInspectorDrawer of the inspector during every OnGUI event. </summary>
		/// <param name="position"> Rect describing the size and position of the inspector. </param>
		/// <summary> This should be called by the IInspectorDrawer of the inspector during every OnGUI event. </summary>
		/// <param name="anyInspectorPartMouseovered"> True if any inspector part is currently mouseovered. </param>
		/// <param name="scrollable"> True if inspector should be drawn as a scrollable view. This might be false e.g. if the inspector drawer handles creating the scroll view. </param>
		public void OnGUI(Rect position, bool anyInspectorPartMouseovered, bool scrollable)
		{
			UnityEngine.Profiling.Profiler.BeginSample("SimpleInspector.OnGUI");

			#if DEV_MODE
			Debug.Assert(InspectorDrawer.Manager.ActiveInstances.Contains(this));
			#endif
			
			InspectorUtility.BeginInspector(this, ref anyInspectorPartMouseovered);
		
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
						break;
					}
					
					OnCursorPositionOrLayoutChanged();
					
					InspectorDrawer.RefreshView();
					break;
			}

			bool dirty = false;

			if(scrollable)
			{
				BeginScrollView(position);
			}
			
			if(DrawViewport())
			{
				dirty = true;
			}
			
			if(scrollable)
			{
				GUI.EndScrollView();
			}
			
			if(dirty)
			{
				InspectorDrawer.RefreshView();
			}

			InspectorUtility.EndInspector(this);

			UnityEngine.Profiling.Profiler.EndSample();
		}

		/// <inheritdoc/>
		public override void Select(Object target)
		{
			RebuildDrawers(ArrayPool<Object>.CreateWithContent(target), false);
		}

		/// <inheritdoc/>
		public override void Select(Object[] targets)
		{
			RebuildDrawers(targets, false);
		}

		/// <inheritdoc/>
		public override void Message(GUIContent message, Object context = null, MessageType messageType = MessageType.Info, bool alsoLogToConsole = true)
		{
			switch(messageType)
			{
				case MessageType.Error:
					Debug.LogError(message, context);
					return;
				case MessageType.Warning:
					Debug.LogWarning(message, context);
					return;
				default:
					Debug.Log(message, context);
					return;
			}
		}

		public override string ToString()
		{
			return "SimpleInspector";
		}

		/// <inheritdoc/>
		protected override bool ShouldDrawPreviewArea()
		{
			return false;
		}
	}
}