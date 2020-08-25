using System;
using UnityEngine;
using Sisus.Attributes;
using JetBrains.Annotations;

namespace Sisus.Demo
{
	[Serializable]
	public abstract class DemoGameObjectDrawer : GameObjectDrawer
	{
		protected static Texture arrow;
		protected static readonly TweenedBool animState = new TweenedBool(3f);

		protected Rect arrowPos;

		/// <inheritdoc/>
		public override void Setup([NotNullOrEmpty] GameObject[] setTargets, [CanBeNull] IParentDrawer setParent, [NotNull] IInspector setInspector)
		{
			if(arrow == null)
			{
				var demoGraphics = FileUtility.LoadAssetByName<DemoGraphics>("Demo Graphics");
				arrow = demoGraphics.arrow;
			}

			base.Setup(setTargets, setParent, setInspector);

			PowerInspectorDocumentation.ShowUrlIfWindowOpen(DocumentationPageUrl);
		}

		protected static Rect GetArrowPos(Rect position, float xPos)
		{
			var arrowPos = position;
			arrowPos.y += 1f + animState * 5f;
			arrowPos.x += xPos;
			arrowPos.width = 24f;
			arrowPos.height = 33f;
			return arrowPos;
		}

		/// <inheritdoc/>
		public override bool DrawPrefix(Rect position)
		{
			bool dirty = base.DrawPrefix(position);

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

		protected virtual void DrawArrow(Rect arrowPos)
		{
			GUI.DrawTexture(arrowPos, arrow, ScaleMode.ScaleToFit);
		}
	}
}