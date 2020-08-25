//#define DEBUG_ENABLED

using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	public enum FontSize
	{
		Normal = 0,
		Small = 1,
		Big = 2,
		Bold = 3
	}

	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public static class Fonts
	{
		public static Font Normal
		{
			get;
			private set;
		}

		public static Font Small
		{
			get;
			private set;
		}

		public static Font Big
		{
			get;
			private set;
		}

		public static Font Bold
		{
			get;
			private set;
		}
		
		public static FontCharSizes NormalSizes
		{
			get;
			private set;
		}

		public static FontCharSizes SmallSizes
		{
			get;
			private set;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// This is called on editor load because of usage of the InitializeOnLoad attribute.
		/// </summary>
		[UsedImplicitly]
		static Fonts()
		{
			EditorApplication.delayCall += DelayedSetup;
		}
		#endif
		
		private static void DelayedSetup()
		{
			#if UNITY_EDITOR
			if(!ApplicationUtility.IsReady())
			{
				EditorApplication.delayCall += DelayedSetup;
				return;
			}
			#endif

			DrawGUI.OnNextBeginOnGUI(Setup, false);
		}

		/// <summary>
		/// This is called when entering play mode or when the game is loaded.
		/// </summary>
		[RuntimeInitializeOnLoadMethod, UsedImplicitly]
		private static void RuntimeInitializeOnLoad()
		{
			DrawGUI.OnNextBeginOnGUI(Setup, false);
		}

		/// <summary>
		/// Setups this class. Called during OnGUI.
		/// </summary>
		private static void Setup()
		{
			#if UNITY_EDITOR
			Normal = EditorStyles.standardFont;
			Bold = EditorStyles.boldFont;
			Small = EditorStyles.miniFont;
			Big = (Font)EditorGUIUtility.LoadRequired("Fonts/Lucida Grande Big.ttf");

			if(Big == null)
			{
				Big = Normal;
			}
			#else
			Normal = GUI.skin.font;
			Bold = Normal;
			Small = Normal;
			Big = Normal;
			#endif
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Normal != null);
			Debug.Assert(Small != null);
			Debug.Assert(Big != null);
			Debug.Assert(Bold != null);
			#endif

			#if UNITY_EDITOR
			NormalSizes = new FontCharSizes(Normal, EditorStyles.label.fontSize);
			SmallSizes = new FontCharSizes(Small, EditorStyles.miniButton.fontSize);
			#else
			NormalSizes = new FontCharSizes(Normal, NormalSizes.FontSize);
			SmallSizes = Fonts.NormalSizes;
			#endif
			
			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Normal="+Normal.name+", Small="+Small.name+", Big="+Big.name+", Bold="+Bold.name);
			#endif
		}

		public static Font GetFont(FontSize fontStyle)
		{
			switch(fontStyle)
			{
				case FontSize.Normal:
					return Normal;
				case FontSize.Small:
					return Small;
				case FontSize.Big:
					return Big;
				case FontSize.Bold:
					return Bold;
				default:
					throw new System.IndexOutOfRangeException(fontStyle.ToString());
			}
		}
	}
}