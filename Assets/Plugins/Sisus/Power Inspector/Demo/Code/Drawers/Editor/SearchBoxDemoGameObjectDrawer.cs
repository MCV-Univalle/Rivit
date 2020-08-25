using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForGameObject(typeof(SearchBoxDemo))]
	public sealed class SearchBoxDemoGameObjectDrawer : DemoGameObjectDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("search-box");
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			var toolbarItem = Inspector.Toolbar == null ? null : Inspector.Toolbar.GetItem<SearchBoxToolbarItem>();
			if(toolbarItem != null)
			{
				arrowPos = GetArrowPos(position, toolbarItem.Bounds.center.x - 12f);
			}
			else
			{
				arrowPos = GetArrowPos(position, 160f);
			}
		}
	}
}