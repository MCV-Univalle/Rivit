using System;
using JetBrains.Annotations;

namespace Sisus.Attributes
{
	/// <summary>
	/// Attribute that specifies that targeted string type class member should be drawn in the inspector using the specified GUIStyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property), MeansImplicitUse(ImplicitUseKindFlags.Access | ImplicitUseKindFlags.Assign)]
	public class StyleAttribute : ShowInInspectorAttribute, IUseDrawer, IDrawerSetupDataProvider
	{
		[NotNull]
		public readonly string guiStyle;

		public StyleAttribute([NotNull]string setGuiStyle)
		{
			guiStyle = setGuiStyle;
		}

		/// <inheritdoc />
		public Type GetDrawerType([NotNull] Type attributeHolderType, [NotNull] Type defaultDrawerTypeForAttributeHolder)
		{
			return typeof(StyledTextDrawer);
		}

		/// <inheritdoc />
		public object[] GetSetupParameters()
		{
			return new[] { guiStyle };
		}
	}
}