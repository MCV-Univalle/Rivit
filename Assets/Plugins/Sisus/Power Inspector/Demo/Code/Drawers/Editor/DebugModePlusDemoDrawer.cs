using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForComponent(typeof(DebugModePlusDemo))]
	public sealed class DebugModePlusDemoDrawer : ComponentDrawer
	{
		private static Texture arrow;
		private static readonly TweenedBool animState = new TweenedBool(3f);

		private Rect arrowPos;

		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("debug-mode");
			}
		}

		/// <inheritdoc/>
		protected override void Setup(Component[] setTargets, IParentDrawer setParent, GUIContent setLabel, IInspector setInspector)
		{
			if(arrow == null)
			{
				var demoGraphics = FileUtility.LoadAssetByName<DemoGraphics>("Demo Graphics");
				arrow = demoGraphics.arrow;
			}

			PowerInspectorDocumentation.ShowUrlIfWindowOpen(DocumentationPageUrl);

			base.Setup(setTargets, setParent, setLabel, setInspector);
		}

		/// <inheritdoc/>
		public override bool Draw(Rect position)
		{
			bool dirty = base.Draw(position);

			if(Event.current.type == EventType.Repaint)
			{
				if(!animState.NowTweening)
				{
					animState.SetTarget(inspector.InspectorDrawer, !animState);
				}
				DrawArrow(arrowPos);
			}

			return dirty;
		}

		private void DrawArrow(Rect arrowPos)
		{
			if(arrowPos.width > 0f && !DebugMode)
			{
				GUI.DrawTexture(arrowPos, arrow, ScaleMode.ScaleToFit);
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			arrowPos = GetArrowPos(position);
		}

		private Rect GetArrowPos(Rect position)
		{
			var debugModePlusButton = headerParts[HeaderPart.DebugModePlusButton];
			if(debugModePlusButton == null)
			{
				return default(Rect);
			}

			var arrowPos = position;
			arrowPos.y += HeaderHeight + 1f + animState * 5f;
			arrowPos.x += debugModePlusButton.Rect.x - 3f;
			arrowPos.width = 24f;
			arrowPos.height = 33f;
			return arrowPos;
		}
	}
}