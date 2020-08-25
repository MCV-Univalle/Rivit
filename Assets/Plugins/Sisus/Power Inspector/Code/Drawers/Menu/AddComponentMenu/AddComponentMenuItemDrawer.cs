using System;
using UnityEngine;

namespace Sisus
{
	/// <summary>
	/// Drawer for an item representing a Component in an open Add Component popup menu.
	/// </summary>
	public sealed class AddComponentMenuItemDrawer : BaseDrawer
	{
		public static IDrawer activeItem;
		
		private AddComponentMenuItem item;

		private bool wasJustClicked;

		/// <inheritdoc/>
		public override Type Type
		{
			get
			{
				return item.type;
			}
		}

		public AddComponentMenuItem Item
		{
			get
			{
				return item;
			}
		}

		/// <summary> Creates a new instance of the drawer or returns a reusable instance from the pool. </summary>
		/// <param name="item"> Information about the menu item. </param>
		/// <returns> The drawer instance, ready to be used. </returns>
		public static AddComponentMenuItemDrawer Create(AddComponentMenuItem item)
		{
			AddComponentMenuItemDrawer result;
			if(!DrawerPool.TryGet(out result))
			{
				result = new AddComponentMenuItemDrawer();
			}
			result.Setup(item);
			result.LateSetup();
			return result;
		}

		/// <summary>
		/// Prevents a default instance of the drawers from being created.
		/// The Create method should be used instead of this constructor.
		/// </summary>
		private AddComponentMenuItemDrawer() { }

		/// <inheritdoc/>
		protected sealed override void Setup(IParentDrawer setParent, GUIContent setLabel)
		{
			throw new NotSupportedException("Please use the other Setup method of AddComponentMenuItemDrawer.");
		}

		/// <summary>
		/// Sets up the drawer so that it is ready to be used.
		/// LateSetup should be called right after this.
		/// </summary>
		/// <param name="setItem"> Information about the menu item. </param>
		private void Setup(AddComponentMenuItem setItem)
		{
			item = setItem;
			label = GUIContentPool.Create(setItem.label);
		}

		/// <inheritdoc />
		public override object GetValue(int index)
		{
			return item.label;
		}

		/// <inheritdoc />
		public override bool SetValue(object newValue)
		{
			if(parent != null)
			{
				return parent.SetValue(newValue.ToString());
			}
			return false;
		}

		/// <inheritdoc />
		public override bool Draw(Rect position)
		{
			GenerateControlId();

			if(Event.current.type == EventType.Layout)
			{
				OnLayoutEvent(position);
			}

			float previewSize = 17f;

			var buttonRect = position;
			buttonRect.x += previewSize;
			buttonRect.width -= previewSize;
			if(GUI.Button(buttonRect, label, activeItem == this ? DrawGUI.prefixLabelWhite : DrawGUI.prefixLabel))
			{
				Debug.Log(GetType().Name +" - GUI.Button clicked");
				wasJustClicked = true; //why was this commented out? because it didn't work?
				DrawGUI.Use(Event.current);
			}

			var preview = item.Preview;
			if(preview != null)
			{
				var iconRect = buttonRect;
				iconRect.width = previewSize;
				iconRect.height = previewSize;
				iconRect.x -= previewSize;

				GUI.DrawTexture(iconRect, preview, ScaleMode.ScaleToFit);
			}

			if(item.IsGroup)
			{
				var arrowRect = buttonRect;
				arrowRect.x += buttonRect.width - previewSize;
				arrowRect.y += 2f;
				arrowRect.width = 13f;
				arrowRect.height = 13f;
				
				GUI.Label(arrowRect, GUIContent.none, "AC RightArrow");
			}
			
			if(wasJustClicked)
			{
				wasJustClicked = false;
				GUI.changed = true;
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		protected override void BuildContextMenu(ref Menu menu, bool extendedMenu)
		{
			#if !POWER_INSPECTOR_LITE
			menu.Add("Copy", CopyToClipboard);
			#endif

			#if UNITY_EDITOR
			menu.Add("Ping", PingAsset);
			#endif
		}

		#if UNITY_EDITOR
		private void PingAsset()
		{
			var instance = Activator.CreateInstance(Type);
			var mono = instance as MonoBehaviour;
			if(mono != null)
			{
				GUI.changed = true;
				DrawGUI.Active.PingObject(UnityEditor.MonoScript.FromMonoBehaviour(mono));
			}
		}
		#endif
		
		/// <inheritdoc />
		public override void Dispose()
		{
			if(activeItem == this)
			{
				activeItem = null;
			}
			base.Dispose();
		}

		/// <inheritdoc />
		public override void OnMouseover()
		{
			//do nothing
		}
	}
}