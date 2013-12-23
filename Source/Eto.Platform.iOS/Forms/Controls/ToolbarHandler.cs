using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;
using Eto.Platform.iOS.Drawing;
using NSToolbar = MonoTouch.UIKit.UIToolbar;
using NSToolbarItem = MonoTouch.UIKit.UIBarButtonItem;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using sd = System.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public interface IToolBarBaseItemHandler
	{
		string Identifier { get; }

		NSToolbarItem Control { get; }

		bool Selectable { get; }

		void ControlAdded(ToolBarHandler toolbar);
	}

	public interface IToolBarItemHandler : IToolBarBaseItemHandler
	{
		void OnClick();

		bool Enabled { get; }

		UIBarButtonItem Button { get; }

		MacToolBarItemStyle ToolBarItemStyle { get; set; }
	}

	/// <summary>
	/// A toolbar item can be displayed in three ways.
	/// To set a non-default style, create a custom style handler
	/// for the Mac platform that sets the style to one of these types.
	/// </summary>
	public enum MacToolBarItemStyle
	{
		/// <summary>
		/// The default appearance, with 32x32 icons.
		/// Does not have a View.
		/// </summary>
		Default,
		/// <summary>
		/// A small button with a rounded bezel.
		/// </summary>
		StandardButton,
		/// <summary>
		/// A large button. Similar in appearance to Default,
		/// but uses a Button as the View.
		/// </summary>
		LargeButton
	}

	public class ToolBarHandler : WidgetHandler<NSToolbar, ToolBar>, IToolBar
	{
		readonly List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		public ToolBarHandler()
		{
			Control = new NSToolbar();
			//Control.SizeMode = NSToolbarSizeMode.Default;
			//Control.Visible = true;
			//Control.ShowsBaselineSeparator = true;
			//Control.AllowsUserCustomization = true;
			//Control.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
			//Control.Delegate = new TBDelegate { Handler = this };
		}

		public void AddButton(ToolItem item)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			items.Add(handler);
			var list = GetItems();
			list.Add((NSToolbarItem)item.ControlObject);
			Control.Items = list.ToArray();
			if (handler != null)
				handler.ControlAdded(this);
			//Control.ValidateVisibleItems();
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
			button = new UIButton(new sd.RectangleF(0, 0, 61, 30));
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
		
#if TODO
			Control.Autovalidates = false;

			menuItem = new NSMenuItem(string.Empty);
			menuItem.Action = Control.Action;
			menuItem.Target = Control.Target;
			Control.MenuFormRepresentation = menuItem;
#endif
			Control.Enabled = true;

			this.ToolBarItemStyle = MacToolBarItemStyle.Default;
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

		public MacToolBarItemStyle ToolBarItemStyle { get; set; }

		public string Identifier
		{
			get { throw new NotImplementedException(); }
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

		public virtual string Identifier
		{
			get
			{
				switch (Type)
				{
#if TODO
					default:
						return NSToolbar.NSToolbarSeparatorItemIdentifier;
					case SeparatorToolItemType.Space:
						return NSToolbar.NSToolbarSpaceItemIdentifier;
					case SeparatorToolItemType.FlexibleSpace:
						return NSToolbar.NSToolbarFlexibleSpaceItemIdentifier;
#endif					
				}
				throw new NotImplementedException();
			}
		}

		public bool Selectable
		{
			get { return false; }
		}

		public SeparatorToolItemType Type { get; set; }

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

