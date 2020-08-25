#if !POWER_INSPECTOR_LITE
using UnityEngine;
using Sisus.Attributes;

namespace Sisus
{
	[ToolbarItemFor(typeof(PowerInspectorToolbar), 20, ToolbarItemAlignment.Right, true)]
	public class LockViewToolbarItem : ToolbarItem
	{
		private float size = 11f;
		
		private Rect buttonRect = new Rect();
		private GUIContent label;
		private GUIContent enableLabel;
		private GUIContent disableLabel;

		private ISplittableInspectorDrawer splittableDrawer;

		/// <inheritdoc/>
		public override float MinWidth
		{
			get
			{
				return size;
			}
		}

		/// <inheritdoc/>
		public override float MaxWidth
		{
			get
			{
				return size;
			}
		}

		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetTerminologyUrl("lock-mode");
			}
		}

		/// <inheritdoc/>
		protected override void Setup()
		{
			var calcSize = InspectorPreferences.Styles.LockButton.CalcSize(GUIContent.none);
			size = calcSize.x + 4f;

			var labels = inspector.Preferences.labels;
			enableLabel = labels.EnableLockView;
			disableLabel = labels.DisableLockView;
			label = inspector.State.ViewIsLocked ? disableLabel : enableLabel;

			splittableDrawer = inspector.InspectorDrawer as ISplittableInspectorDrawer;
		}

		/// <inheritdoc/>
		public override bool ShouldShow()
		{
			return base.ShouldShow() && splittableDrawer != null;
		}

		/// <inheritdoc/>
		protected override void UpdateDrawPositions(Rect itemPosition)
		{
			buttonRect.x = itemPosition.x + 1f;
			buttonRect.y = itemPosition.y + 2f;
			buttonRect.width = size;
			buttonRect.height = size;
		}

		/// <inheritdoc/>
		protected override void OnRepaint(Rect itemPosition)
		{
			bool isLocked = inspector.State.ViewIsLocked;
			GUI.Toggle(buttonRect, isLocked, label, InspectorPreferences.Styles.LockButton);

			if(isLocked && !IsSelected)
			{
				var color = inspector.Preferences.theme.LockViewHighlight;
				DrawSelectionRect(itemPosition, color);
			}
		}

		/// <inheritdoc/>
		protected override bool OnActivated(Event inputEvent, bool isClick)
		{
			DrawGUI.Use(inputEvent);
			ToggleViewIsLocked();
			return true;
		}

		private void ToggleViewIsLocked()
		{
			var state = inspector.State;
			var setLocked = !state.ViewIsLocked;

			label = setLocked ? disableLabel : enableLabel;

			#if DEV_MODE
			Debug.Log(inspector+".Toolbar.ToggleViewIsLocked - ViewIsLocked = "+StringUtils.ToColorizedString(setLocked)+" with Event="+StringUtils.ToString(Event.current));
			#endif
			
			if(state.SearchFilter.DeterminesInspectedTarget())
			{
				inspector.SetFilter("");
			}

			state.ViewIsLocked = setLocked;
			if(!setLocked)
			{
				inspector.RebuildDrawersIfTargetsChanged();
			}
		}

		protected override void OnCopyCommandGiven()
		{
			var inspected = inspector.State.inspected;
			if(inspected.Length > 0 && inspected[0] != null)
			{
				if(inspected.Length == 1)
				{
					Clipboard.CopyObjectReference(inspected[0], inspected[0].GetType());
					Clipboard.SendCopyToClipboardMessage("Copied{0} reference.", "Inspected");
				}
				else
				{
					Clipboard.CopyObjectReferences(inspected, inspected[0].GetType());
					Clipboard.SendCopyToClipboardMessage("Copied{0} references.", "Inspected");
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnPasteCommandGiven()
		{
			if(Clipboard.HasObjectReference())
			{
				inspector.RebuildDrawers(Clipboard.PasteObjectReferences().ToArray(), false);
			}
		}
	}
}
#endif