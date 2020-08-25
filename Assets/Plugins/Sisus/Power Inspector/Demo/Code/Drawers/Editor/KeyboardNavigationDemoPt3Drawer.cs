using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForComponent(typeof(KeyboardNavigationDemoPt3))]
	public sealed class KeyboardNavigationDemoPt3Drawer : ComponentDrawer
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