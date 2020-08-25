using System;
using UnityEngine;
using Sisus.Attributes;

namespace Sisus.Demo
{
	[Serializable, DrawerForComponent(typeof(TransformDrawerDemo))]
	public sealed class TransformDrawerDemoDrawer : ComponentDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetDrawerInfoUrl("transform-drawer");
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