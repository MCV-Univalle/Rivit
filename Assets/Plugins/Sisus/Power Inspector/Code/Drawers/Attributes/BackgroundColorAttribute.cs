using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus.Attributes
{
	/// <summary>
	/// Attribute that can be added to a Component class to have it be drawn with a specified background color.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class BackgroundColorAttribute : Attribute, IUseDrawer, IDrawerSetupDataProvider
	{
		private static readonly object[] parameterWrapper = new object[1];

		public readonly Color color;

		/// <summary>
		/// Specifies that the attribute holder should be drawn with the specified background color.
		/// </summary>
		/// <param name="colorHtml">
		/// Case insensitive html string to be converted into a color.
		/// 
		/// Supported formats are #RRGGBB, #RRGGBBAA and literal colors with the following supported:
		/// red, cyan, blue, darkblue, lightblue, purple, yellow, lime, fuchsia, white, silver, grey, black, orange, brown, maroon, green, olive, navy, teal, aqua, magenta.
		/// 
		/// When not specified alpha will default to FF (fully opaque).
		/// </param>
		/// <example>
		/// <code>
		/// [BackgroundColor("#00FF00")]
		/// public class ComponentWithGreenBackground : MonoBehaviour { }
		/// 
		/// [BackgroundColor("yellow")]
		/// public class ComponentWithYellowBackground : MonoBehaviour { }
		/// </code>
		/// </example>
		public BackgroundColorAttribute([NotNull]string colorHtml)
		{
			if(colorHtml[0] != '#')
			{
				colorHtml = "#" + colorHtml;
			}

			if(!ColorUtility.TryParseHtmlString(colorHtml, out color))
			{
				Debug.LogError("BackgroundColorAttribute failed to parse color in format rrggbbaa");
			}
		}

		/// <summary>
		/// Specifies that the attribute holder should be drawn with the specified background color.
		/// </summary>
		/// <param name="r"> Color32 red component intensity. </param>
		/// <param name="g"> Color32 green component intensity. </param>
		/// <param name="b"> Color32 alpha component intensity. 0 is fully transparent and 255 is fully opaque. </param>
		/// <example>
		/// <code>
		/// [BackgroundColor(0, 255, 0)]
		/// public class ComponentWithGreenBackground : MonoBehaviour { }
		/// </code>
		/// </example>
		public BackgroundColorAttribute(byte r, byte g, byte b) : base()
		{
			color = new Color32(r, g, b, 255);
		}

		/// <summary>
		/// Specifies that the attribute holder should be drawn with the specified background color.
		/// </summary>
		/// <param name="r"> Color red component intensity between 0 (min) and 255 (max). </param>
		/// <param name="g"> Color green component intensity between 0 (min) and 255 (max). </param>
		/// <param name="b"> Color blue component intensity between 0 (min) and 255 (max). </param>
		/// <param name="a"> Color32 alpha component intensity between 0 (fully transparent) and 255 (fully opaque). </param>
		/// <example>
		/// <code>
		/// [BackgroundColor(0, 255, 0, 255)]
		/// public class ComponentWithGreenBackground : MonoBehaviour { }
		/// </code>
		/// </example>
		public BackgroundColorAttribute(byte r, byte g, byte b, byte a) : base()
		{
			color = new Color32(r, g, b, a);
		}

		/// <summary>
		/// Specifies that the attribute holder should be drawn with the specified background color.
		/// </summary>
		/// <param name="r"> Color red component intensity between 0f (min) and 1f (max). </param>
		/// <param name="g"> Color green component intensity between 0f (min) and 1f (max). </param>
		/// <param name="b"> Color blue component intensity between 0f (min) and 1f (max). </param>
		/// <example>
		/// <code>
		/// [BackgroundColor(0f, 1f, 0f)]
		/// public class ComponentWithGreenBackground : MonoBehaviour { }
		/// </code>
		/// </example>
		public BackgroundColorAttribute(float r, float g, float b) : base()
		{
			color = new Color(r, g, b, 1f);
		}

		/// <summary>
		/// Specifies that the attribute holder should be drawn with the specified background color.
		/// </summary>
		/// <param name="r"> Color red component intensity between 0f (min) and 1f (max). </param>
		/// <param name="g"> Color green component intensity between 0f (min) and 1f (max). </param>
		/// <param name="b"> Color blue component intensity between 0f (min) and 1f (max). </param>
		/// <param name="a"> Color alpha component intensity betwen 0f (fully transparent) and 1f (fully opaque). </param>
		/// <example>
		/// <code>
		/// [BackgroundColor(0f, 1f, 0f, 1f)]
		/// public class ComponentWithGreenBackground : MonoBehaviour { }
		/// </code>
		/// </example>
		public BackgroundColorAttribute(float r, float g, float b, float a) : base()
		{
			color = new Color(r, g, b, a);
		}

		/// <inheritdoc />
		public object[] GetSetupParameters()
		{
			parameterWrapper[0] = color;
			return parameterWrapper;
		}

		/// <inheritdoc/>
		public Type GetDrawerType(Type attributeHolderType, Type defaultDrawerTypeForAttributeHolder)
		{
			#if POWER_INSPECTOR // if power inspector is installed

			#if UNITY_EDITOR
			if(typeof(ICustomEditorComponentDrawer).IsAssignableFrom(defaultDrawerTypeForAttributeHolder))
			{
				return typeof(ColoredCustomEditorComponentDrawer);
			}
			#endif

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(typeof(IEditorlessComponentDrawer).IsAssignableFrom(defaultDrawerTypeForAttributeHolder));
			#endif

			return typeof(ColoredComponentDrawer);

			#else  // if power inspector is not installed
			throw new NotSupportedException("BackgroundColorAttribute.GetDrawerType is not supported because Power Inspector is not installed.");
			#endif
		}
	}
}