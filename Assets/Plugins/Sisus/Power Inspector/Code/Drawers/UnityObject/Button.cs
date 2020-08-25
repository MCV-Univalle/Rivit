using System;
using UnityEngine;

namespace Sisus
{
	public class Button
	{
		private static readonly Pool<Button> pool = new Pool<Button>(4);

		private GUIContent label;
		private Rect rect;
		private Action onClicked;
		private Color color;
		private bool hasColor;

		public Action<Button> onRectChanged;

		public GUIContent Label
		{
			get
			{
				return label;
			}
		}

		public Rect Rect
		{
			get
			{
				return rect;
			}

			set
			{
				if(!rect.Equals(value))
				{
					rect = value;
					if(onRectChanged != null)
					{
						onRectChanged(this);
					}
				}
			}
		}

		public Color? Color
		{
			get
			{
				if(hasColor)
				{
					return color;
				}
				return null;
			}

			set
			{
				if(value != null)
				{
					if(!hasColor || color != value)
					{
						GUI.changed = true;
						color = value.Value;
						hasColor = true;
					}
				}
				else if(hasColor)
				{
					GUI.changed = true;
					hasColor = false;
					color = UnityEngine.Color.white;
				}
			}
		}

		public static Button Create(GUIContent buttonLabel, Action onButtonClicked)
		{
			Button result;
			if(!pool.TryGet(out result))
			{
				result = new Button();
			}
			result.Setup(buttonLabel, onButtonClicked);
			return result;
		}

		public static Button Create(GUIContent buttonLabel, Action onButtonClicked, Color guiColor)
		{
			Button result;
			if(!pool.TryGet(out result))
			{
				result = new Button();
			}
			result.Setup(buttonLabel, onButtonClicked, guiColor);
			return result;
		}
		
		private void Setup(GUIContent buttonLabel, Action onButtonClicked)
		{
			label = buttonLabel;
			onClicked = onButtonClicked;
			hasColor = false;
			color = UnityEngine.Color.white;
		}

		private void Setup(GUIContent buttonLabel, Action onButtonClicked, Color guiColor)
		{
			label = buttonLabel;
			onClicked = onButtonClicked;
			hasColor = true;
			color = guiColor;
		}

		public bool MouseIsOver()
		{
			return rect.MouseIsOver();
		}

		public void OnClicked()
		{
			onClicked();
		}

		public void Draw(GUIStyle style)
		{
			if(hasColor)
			{
				var guiColorWas = GUI.color;
				GUI.color = color; //should we blend the alpha values of existing GUI.color and color?
				GUI.Button(rect, label, style);
				GUI.color = guiColorWas;
			}
			else
			{
				GUI.Button(rect, label, style);
			}
		}

		public void Dispose()
		{
			GUIContentPool.Dispose(ref label);
			onClicked = null;
			hasColor = false;

			var disposing = this;
			pool.Dispose(ref disposing);
		}

		public override string ToString()
		{
			return "Button("+label+")";
		}
	}
}