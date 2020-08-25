using UnityEngine;

namespace Sisus
{
	public delegate void OnHeaderPartClicked(Event inputEvent);

	/// <summary>
	/// Class responsible for drawing a part of the header of Drawer.
	/// </summary>
	public class HeaderPartDrawer
	{
		private static readonly Pool<HeaderPartDrawer> Pool = new Pool<HeaderPartDrawer>(6);

		private HeaderPart part;
		private Rect rect;
		private OnHeaderPartClicked onClicked;
		private OnHeaderPartClicked onRightClicked;
		private Texture texture;
		private bool drawSelectionRect;
		private bool drawMouseoverRect;
		private bool selectable;
		private readonly GUIContent label = new GUIContent();
		private Color? guiColor;

		public HeaderPart Part
		{
			get
			{
				return part;
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
				#if UNITY_2019_3_OR_NEWER
				float moveUp = value.height - 10f;
				if(moveUp > 0f)
				{
					value.y -= moveUp * 0.25f;
				}
				#endif

				rect = value;
			}
		}

		public Texture Texture
		{
			set
			{
				texture = value;
				label.image = value;
			}
		}

		public bool DrawMouseoverRect
		{
			get
			{
				return drawMouseoverRect;
			}
		}

		public bool Selectable
		{
			get
			{
				return selectable;
			}
		}

		public void SetGUIColor(Color color)
		{
			guiColor = color;
		}

		public static HeaderPartDrawer Create(HeaderPart headerPart, bool drawSelectionRect, bool drawMouseoverRect, string tooltip, OnHeaderPartClicked onPartClicked, OnHeaderPartClicked onPartRightClicked = null, bool selectable = true)
		{
			HeaderPartDrawer result;
			if(!Pool.TryGet(out result))
			{
				result = new HeaderPartDrawer();
			}
			result.Setup(headerPart, drawSelectionRect, drawMouseoverRect, tooltip, onPartClicked, onPartRightClicked, selectable);
			return result;
		}

		public static HeaderPartDrawer Create(HeaderPart headerPart, bool drawSelectionRect, bool drawMouseoverRect, Texture drawTexture, string tooltip, OnHeaderPartClicked onPartClicked, OnHeaderPartClicked onPartRightClicked = null, bool selectable = true)
		{
			HeaderPartDrawer result;
			if(!Pool.TryGet(out result))
			{
				result = new HeaderPartDrawer();
			}
			result.Setup(headerPart, drawSelectionRect, drawMouseoverRect, drawTexture, tooltip, onPartClicked, onPartRightClicked, selectable);
			return result;
		}

		private void Setup(HeaderPart headerPart, bool drawsSelectionRect, bool drawsMouseoverRect, string tooltip, OnHeaderPartClicked onPartClicked, OnHeaderPartClicked onPartRightClicked, bool isSelectable)
		{
			part = headerPart;
			rect = default(Rect);
			drawSelectionRect = drawsSelectionRect;
			drawMouseoverRect = drawsMouseoverRect;
			onClicked = onPartClicked;
			onRightClicked = onPartRightClicked;
			selectable = isSelectable;

			label.image = null;
			label.tooltip = tooltip;
		}
		
		private void Setup(HeaderPart headerPart, bool drawsSelectionRect, bool drawsMouseoverRect, Texture drawTexture, string tooltip, OnHeaderPartClicked onPartClicked, OnHeaderPartClicked onPartRightClicked, bool isSelectable)
		{
			part = headerPart;
			rect = default(Rect);
			drawSelectionRect = drawsSelectionRect;
			drawMouseoverRect = drawsMouseoverRect;
			texture = drawTexture;
			onClicked = onPartClicked;
			onRightClicked = onPartRightClicked;
			selectable = isSelectable;
			
			label.image = drawTexture;
			label.tooltip = tooltip;
		}

		public bool MouseIsOver()
		{
			return rect.Contains(Cursor.LocalPosition);
		}

		public bool OnClicked(Event inputEvent)
		{
			#if DEV_MODE
			Debug.Log("HeaderPart."+part+ ".OnClick with onClicked="+StringUtils.ToString(onClicked));
			#endif

			if(onClicked != null)
			{
				onClicked(inputEvent);
				return true;
			}
			return false;
		}

		public bool OnRightClicked(Event inputEvent)
		{
			if(onRightClicked != null)
			{
				onRightClicked(inputEvent);
				return true;
			}
			return false;
		}

		public void Draw()
		{
			if(texture != null)
			{
				#if UNITY_2019_3_OR_NEWER
				if(MouseIsOver())
				{
					if(DrawGUI.IsProSkin)
					{
						DrawGUI.Active.ColorRect(rect, new Color(1f, 1f, 1f, 0.1f));
					}
					else
					{
						DrawGUI.Active.ColorRect(rect, new Color(1f, 1f, 1f, 0.6f));
					}
				}
				#endif

				var guiColorWas = GUI.color;
				if(guiColor.HasValue)
				{
					GUI.color = guiColor.Value;
				}

				GUI.Label(rect, label, InspectorPreferences.Styles.Centered);

				GUI.color = guiColorWas;
			}
			else if(label.tooltip.Length > 0)
			{
				GUI.Label(rect, label, InspectorPreferences.Styles.Centered);
			}
		}

		public void DrawSelectionRect()
		{
			if(drawSelectionRect)
			{
				DrawGUI.DrawControlSelectionIndicator(rect);
			}
		}
		
		public void Dispose()
		{
			texture = null;
			onClicked = null;
			guiColor = null;

			var disposing = this;
			Pool.Dispose(ref disposing);
		}

		public static implicit operator HeaderPart(HeaderPartDrawer info)
		{
			return info == null ? HeaderPart.None : info.part;
		}

		public override string ToString()
		{
			return part.ToString();
		}
	}
}