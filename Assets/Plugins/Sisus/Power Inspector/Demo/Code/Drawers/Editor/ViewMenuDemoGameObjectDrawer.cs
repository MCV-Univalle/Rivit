using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForGameObject(typeof(ViewMenuDemo))]
	public sealed class ViewMenuDemoGameObjectDrawer : DemoGameObjectDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("view-menu");
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			arrowPos = GetArrowPos(position, 65f);
		}
	}
}