using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForComponent(typeof(KeyboardNavigationDemoPt2))]
	public sealed class KeyboardNavigationDemoPt2Drawer : ComponentDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("keyboard-navigation");
			}
		}
	}
}