using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Texture that dynamically changes to match the active Editor Skin
	/// (Default light skin or Pro dark skin).
	/// </summary>
	[Serializable]
	public class SkinnedTexture
	{
		// Warning "Field x is never assigned to, and will always have its default value null"
		// does not apply here because values are assigned through the Unity inspector
		#pragma warning disable 0649

		// values assigned through the Unity inspector
		[SerializeField, UsedImplicitly]
		private Texture imageLightSkin;

		// values assigned through the Unity inspector
		[SerializeField, UsedImplicitly]
		private Texture imageDarkSkin;
		
		#pragma warning restore 0649

		public Texture Get()
		{
			return DrawGUI.IsProSkin ? imageDarkSkin : imageLightSkin;
		}

		public static implicit operator Texture(SkinnedTexture skinnedTexture)
		{
			return skinnedTexture.Get();
		}
	}
}