using System;
using System.Collections.Generic;

namespace Sisus
{
	public class Buttons
	{
		public const float ButtonDrawOffset = 3f;
		
		private List<Button> buttons;

		public Action<Buttons> onButtonRectsChanged;

		public Button this[int index]
		{
			get
			{
				return buttons[index];
			}
		}

		public int Count
		{
			get
			{
				return buttons.Count;
			}
		}

		public Buttons(int capacity)
		{
			buttons = new List<Button>(capacity);
		}

		public void Add(Button button)
		{
			buttons.Add(button);
			button.onRectChanged += OnButtonRectChanged;
			OnButtonRectChanged(button);
		}
		
		public void Clear()
		{
			for(int n = buttons.Count - 1; n >= 0; n--)
			{
				var button = buttons[n];
				button.onRectChanged -= OnButtonRectChanged;
				button.Dispose();
			}
			buttons.Clear();

			if(onButtonRectsChanged != null)
			{
				onButtonRectsChanged(this);
			}
		}

		public float Width()
		{
			int count = buttons.Count;
			if(count == 0)
			{
				return 0f;
			}

			float width = buttons[0].Rect.width;
			for(int n = buttons.Count - 1; n >= 1; n--)
			{
				width += buttons[n].Rect.width + ButtonDrawOffset;
			}
			return width;
		}

		private void OnButtonRectChanged(Button button)
		{
			if(onButtonRectsChanged != null)
			{
				onButtonRectsChanged(this);
			}
		}
	}
}