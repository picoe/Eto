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

		//NSButton Button { get; }

		MacToolBarItemStyle ToolBarItemStyle { get; set; }
	}

	class ToolBarItemHandlerTarget : NSObject
	{
		WeakReference handler;

		public IToolBarItemHandler Handler { get { return (IToolBarItemHandler)handler.Target; } set { handler = new WeakReference(value); } }

		[Export("validateToolbarItem:")]
		public bool ValidateToolbarItem(NSToolbarItem item)
		{
			return Handler.Enabled;
		}

		[Export("action")]
		public bool Action()
		{
			Handler.OnClick();
			return true;
		}
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

		public void AddButton(ToolItem item)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			items.Add(handler);
			var list = Control.Items.ToList();
			list.Add((NSToolbarItem)item.ControlObject);
			Control.Items = list.ToArray();
			if (handler != null)
				handler.ControlAdded(this);
			//Control.ValidateVisibleItems();
		}

		public void RemoveButton(ToolItem item)
		{
			var handler = item.Handler as IToolBarBaseItemHandler;
			var index = items.IndexOf(handler);
			items.Remove(handler);
			var list = Control.Items.ToList();
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
			Control.Image = nsimage;
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

		public void CreateFromCommand(Command command)
		{
			throw new NotImplementedException();
		}
		
		static readonly Selector selAction = new Selector("action");

		protected override void Initialize()
		{
			base.Initialize();
			Control.Target = new ToolBarItemHandlerTarget { Handler = this };
			Control.Action = selAction;

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
			set { Control.Title = value; }
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

