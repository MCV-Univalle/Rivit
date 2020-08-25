#if !POWER_INSPECTOR_LITE
using Sisus.Attributes;

namespace Sisus
{
	[ToolbarFor(typeof(PreferencesInspector))]
	public sealed class PreferencesToolbar : InspectorToolbar
	{
		public const float DefaultToolbarHeight = PowerInspectorToolbar.DefaultToolbarHeight;
		
		public readonly float ToolbarHeight = DefaultToolbarHeight;
		
		/// <inheritdoc/>
		public override float Height
		{
			get
			{
				return ToolbarHeight;
			}
		}

		public PreferencesToolbar(float setHeight = DefaultToolbarHeight) : base()
		{
			ToolbarHeight = setHeight;
		}
	}
}
#endif