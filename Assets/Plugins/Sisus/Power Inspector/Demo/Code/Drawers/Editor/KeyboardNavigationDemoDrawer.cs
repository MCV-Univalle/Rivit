using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForComponent(typeof(KeyboardNavigationDemo))]
	public sealed class KeyboardNavigationDemoDrawer : ComponentDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("keyboard-navigation");
			}
		}

		/// <inheritdoc/>
		protected override void Setup(Component[] setTargets, IParentDrawer setParent, GUIContent setLabel, IInspector setInspector)
		{
			PowerInspectorDocumentation.ShowUrlIfWindowOpen(DocumentationPageUrl);
			base.Setup(setTargets, setParent, setLabel, setInspector);
		}
	}
}