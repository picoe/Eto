using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using NSToolbar = MonoTouch.UIKit.UIToolbar;
using NSToolbarItem = MonoTouch.UIKit.UIBarButtonItem;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public interface IToolBarBaseItemHandler
	{
		NSToolbarItem Control { get; }
		bool Selectable { get; }
		void ControlAdded(ToolBarHandler toolbar);
	}

	public interface IToolBarItemHandler : IToolBarBaseItemHandler
	{
		void OnClick();
		bool Enabled { get; }
		UIBarButtonItem Button { get; }
	}

	public class ToolBarHandler : WidgetHandler<NSToolbar, ToolBar>, IToolBar
	{
		readonly List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		public ToolBarHandler()
		{
			Control = new NSToolbar();
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			Control.BarStyle = UIBarStyle.Default;
		}

		public void AddButton(ToolItem button, int index)
		{
			var handler = (IToolBarBaseItemHandler)button.Handler;
			items.Add(handler);
			var list = GetItems();
			list.Insert(index, (NSToolbarItem)button.ControlObject);
			Control.Items = list.ToArray();
			if (handler != null)
				handler.ControlAdded(this);
		}

		private List<NSToolbarItem> GetItems()
		{
			var list = Control.Items;
			return list != null ? list.ToList() : new List<NSToolbarItem>();
		}

		public void RemoveButton(ToolItem item)
		{
			var handler = item.Handler as IToolBarBaseItemHandler;
			var index = items.IndexOf(handler);
			items.Remove(handler);
			var list = GetItems();
			list.RemoveAt(index);
			Control.Items = list.ToArray();
		}

		public void Clear()
		{
			Control.Items = null;
		}

		public ToolBarTextAlign TextAlign { get; set; } // TODO
		public ToolBarDock Dock { get; set; } // TODO
	}

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IToolItem, IToolBarItemHandler
		where TControl : NSToolbarItem
		where TWidget : ToolItem
	{
		Image image;
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetImage();
			}
		}

		void SetImage()
		{
			var nsimage = image.ToUI();
#if TODO
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint(tint.Value.ToNSUI());
#endif
			button.SetImage(nsimage, UIControlState.Normal);
		}

		Color? tint;
		public Color? Tint
		{
			get { return tint; }
			set
			{
				tint = value;
			}
		}

		UIButton button;
		public override TControl CreateControl()
		{
			// Create a button so that any image can be used.
			// (A standard toolbar item uses only the alpha channel of the image.)
			button = new UIButton(new sd.RectangleF(0, 0, 40, 40));
			button.TouchUpInside += (s, e) => OnClick();
			var result = new NSToolbarItem(button);
			return (TControl)result;
		}

		public void CreateFromCommand(Command command)
		{
		}
		
		static readonly Selector selAction = new Selector("action");

		protected override void Initialize()
		{
			base.Initialize();
			Control.Enabled = true;
		}

		public string Text
		{
			get { return Control.Title; }
			set { Control.Title = value; button.SetTitle(value, UIControlState.Normal); }
		}

		public string ToolTip { get; set; } // iOS does not support ToolTip

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public virtual bool Selectable { get; set; }

		public virtual void ControlAdded(ToolBarHandler toolbar)
		{
		}

		public virtual void InvokeButton()
		{
		}

		public void OnClick()
		{
			InvokeButton();
		}

		NSToolbarItem IToolBarBaseItemHandler.Control
		{
			get { return Control; }
		}

		public NSToolbarItem Button
		{
			get { return Control; }
		}
	}

	public class ButtonToolItemHandler : ToolItemHandler<NSToolbarItem, ButtonToolItem>, IButtonToolItem
	{
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}

	public class CheckToolItemHandler : ToolItemHandler<NSToolbarItem, CheckToolItem>, ICheckToolItem
	{
		bool isChecked;
		ToolBarHandler toolbarHandler;

		public bool Checked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
#if TODO				
				if (isChecked && Control != null && toolbarHandler != null && toolbarHandler.Control != null)
					toolbarHandler.Control.SelectedItemIdentifier = Identifier;
#endif
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Selectable = true;
		}

		public override void ControlAdded(ToolBarHandler toolbar)
		{
			base.ControlAdded(toolbar);
			toolbarHandler = toolbar;
#if TODO
			if (isChecked)
				toolbar.Control.SelectedItemIdentifier = Identifier;
#endif
		}

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}

	public class SeparatorToolItemHandler : WidgetHandler<NSToolbarItem, SeparatorToolItem>, ISeparatorToolItem, IToolBarBaseItemHandler
	{
		public SeparatorToolItemHandler()
		{
			Type = SeparatorToolItemType.Divider;
		}

		public bool Selectable
		{
			get { return false; }
		}

		public SeparatorToolItemType Type { get; set; }

		public override NSToolbarItem CreateControl()
		{
			var result = new NSToolbarItem(UIBarButtonSystemItem.FixedSpace);
			result.Width = 10;
			return result;
		}

		public void ControlAdded(ToolBarHandler toolbar)
		{
		}

		public void CreateFromCommand(Command command)
		{
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Eto.Drawing.Image Image
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}
	}
}

