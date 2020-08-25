using System;
using JetBrains.Annotations;

namespace Sisus.Attributes
{
	/// <summary>
	/// Attribute that specifies that its target should be shown in Power Inspector as a button.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method), MeansImplicitUse(ImplicitUseKindFlags.Access | ImplicitUseKindFlags.Assign)]
	public class ButtonAttribute : ShowInInspectorAttribute, IUseDrawer, IDrawerSetupDataProvider
	{
		[NotNull]
		public readonly string prefixLabelText;
		[NotNull]
		public readonly string buttonText;
		[NotNull]
		public readonly string guiStyle;

		public ButtonAttribute()
		{
			buttonText = "";
			prefixLabelText = "";
			guiStyle = "";
		}

		public ButtonAttribute([NotNull]string setButtonText)
		{
			buttonText = setButtonText;
			prefixLabelText = "";
			guiStyle = "";
		}

		public ButtonAttribute([NotNull]string setPrefixLabelText, [NotNull]string setButtonText)
		{
			buttonText = setButtonText;
			prefixLabelText = setPrefixLabelText;
			guiStyle = "";
		}

		public ButtonAttribute([NotNull]string setPrefixLabelText, [NotNull]string setButtonText, [NotNull]string setGuiStyle)
		{
			buttonText = setButtonText;
			prefixLabelText = setPrefixLabelText;
			guiStyle = setGuiStyle;
		}

		/// <inheritdoc />
		public Type GetDrawerType([NotNull]Type attributeHolderType, [NotNull]Type defaultDrawerTypeForAttributeHolder)
		{
			return typeof(MethodButtonDrawer);
		}

		/// <inheritdoc />
		public object[] GetSetupParameters()
		{
			return new[] { prefixLabelText , buttonText, guiStyle };
		}
	}
}