using System;
using System.Linq;
using UnityEngine;
using Sisus.Attributes;
using Object = UnityEngine.Object;

namespace Sisus.Demo
{
	[Serializable, DrawerForGameObject(typeof(MultiEditingModesDemo))]
	public sealed class MultiEditingModesDemoGameObjectDrawer : DemoGameObjectDrawer
	{
		/// <inheritdoc/>
		public override string DocumentationPageUrl
		{
			get
			{
				return PowerInspectorDocumentation.GetFeatureUrl("multi-editing-modes");
			}
		}

		/// <inheritdoc/>
		public override void LateSetup()
		{
			base.LateSetup();

			if(targets.Length < 2)
			{
				var multiEditingModesDemos = Object.FindObjectsOfType<MultiEditingModesDemo>();
				if(multiEditingModesDemos.Length > 1)
				{
					Inspector.OnNextLayout(() => Inspector.Select(multiEditingModesDemos.ToList().Select((component) => component.gameObject).ToArray()));
				}
			}
		}

		/// <inheritdoc/>
		protected override void GetDrawPositions(Rect position)
		{
			base.GetDrawPositions(position);
			arrowPos = GetArrowPos(position, position.xMax - 57f);
		}

		/// <inheritdoc/>
		protected override void DrawArrow(Rect arrowPos)
		{
			if(targets.Length > 1)
			{
				base.DrawArrow(arrowPos);
			}
		}
	}
}