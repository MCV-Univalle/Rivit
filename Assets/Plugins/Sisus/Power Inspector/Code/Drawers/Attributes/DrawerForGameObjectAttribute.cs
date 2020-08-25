using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus.Attributes
{
	/// <summary>
	/// Place this attribute on a class which implements IComponentDrawer to inform DrawerProvider
	/// that the drawers are used to represent Components of the given type - and optionally types inheriting from said type.
	/// </summary>
	public sealed class DrawerForGameObjectAttribute : DrawerForBaseAttribute
	{
		[CanBeNull]
		public readonly Type requireComponentOnGameObject;

		/// <inheritdoc/>
		[NotNull]
		public override Type Target
		{
			get
			{
				return typeof(GameObject);
			}
		}

		/// <inheritdoc/>
		public override bool TargetExtendingTypes
		{
			get
			{
				return false;
			}
		}

		public DrawerForGameObjectAttribute() : base(false) { }

		public DrawerForGameObjectAttribute([CanBeNull]Type requireComponent) : base(false)
		{
			requireComponentOnGameObject = requireComponent;
		}

		internal DrawerForGameObjectAttribute(bool setIsFallback) : base(setIsFallback) { }

		internal DrawerForGameObjectAttribute([CanBeNull]Type requireComponent, bool setIsFallback) : base(setIsFallback)
		{
			requireComponentOnGameObject = requireComponent;
		}

		#if DEV_MODE && PI_ASSERTATIONS
		/// <inheritdoc/>
		public override void AssertDataIsValid(Type drawerType) { }
		#endif
	}
}