using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForGameObject(typeof(SplitViewDemo))]
	public sealed class SplitViewDemoGameObjectDrawer : DemoGameObjectDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("split-view");
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			arrowPos = GetArrowPos(position, position.xMax - 23f);
		}
	}
}