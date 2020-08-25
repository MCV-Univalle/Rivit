using System;
using UnityEngine;
using Sisus.Attributes;
using JetBrains.Annotations;

namespace Sisus.Demo
{
	[Serializable, DrawerForGameObject(typeof(BackAndForwardButtonDemo))]
	public sealed class BackAndForwardButtonDemoGameObjectDrawer : DemoGameObjectDrawer
	{
		private Rect arrow2Pos;

		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("back-and-forward-buttons");
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			arrowPos = GetArrowPos(position, 1f);
			arrow2Pos = GetArrowPos(position, 31f);
		}

		/// <inheritdoc/>
		public override bool DrawPrefix(Rect position)
		{
			bool dirty = base.DrawPrefix(position);
			GUI.DrawTexture(arrow2Pos, arrow, ScaleMode.ScaleToFit);
			return dirty;
		}
	}
}