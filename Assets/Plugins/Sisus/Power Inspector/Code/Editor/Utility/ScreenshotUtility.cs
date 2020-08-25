using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;

namespace Sisus
{
	public static class ScreenshotUtility
	{
		[NotNull]
		public static Texture2D ScreenshotEditorWindow(EditorWindow window)
		{
			int width = (int)window.position.width;
			int height = (int)window.position.height;
			var screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
			screenshot.alphaIsTransparency = false;
			screenshot.filterMode = FilterMode.Point;
			screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			screenshot.Apply();
 
			return screenshot;
		}
	}
}