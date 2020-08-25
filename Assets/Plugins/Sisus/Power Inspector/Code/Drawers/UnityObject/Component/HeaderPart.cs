namespace Sisus
{
	/// <summary> Parts of UnityObjectDrawer header. </summary>
	public enum HeaderPart
	{
		None = 0,

		Base = 1,

		FoldoutArrow = 10,
		EnabledFlag = 11,
		
		ContextMenuIcon = 20,
		ReferenceIcon = 21,
		
		#if UNITY_2018_1_OR_NEWER
		PresetIcon = 22,
		#endif

		DebugModePlusButton = 30,
		QuickInvokeMenuButton = 31,

		CustomHeaderButton1 = 51,
		CustomHeaderButton2 = 52,
		CustomHeaderButton3 = 53,
		CustomHeaderButton4 = 54,
		CustomHeaderButton5 = 55,
	}
}