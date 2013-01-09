using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.ObjCRuntime;
using Eto.Platform.Mac.Drawing;
using sd = System.Drawing;

namespace Eto.Platform.Mac
{
	interface IToolBarBaseItemHandler
	{
		string Identifier { get; }
		NSToolbarItem Control { get; }
		bool Selectable { get; }
		void ControlAdded(ToolBarHandler toolbar);
	}
	
	interface IToolBarItemHandler : IToolBarBaseItemHandler
	{
		void OnClick();
		bool Enabled { get; }
	}
	
	class ToolBarItemHandlerTarget : NSObject
	{
		public IToolBarItemHandler Handler { get; set; }
		
		[Export("validateToolbarItem:")]
		public bool ValidateToolbarItem(NSToolbarItem item)
		{
			return Handler.Enabled;
		}
		
		[Export("action")]
		public bool action()
		{
			Handler.OnClick();
			return true;
		}
	}

	public abstract class ToolBarItemHandler<T, W> : WidgetHandler<T, W>, IToolBarItem, IToolBarItemHandler
		where T: NSToolbarItem
		where W: ToolBarItem
	{
		Image image;
		NSButton button;
		NSMenuItem menuItem;
		sd.SizeF buttonSize = new sd.SizeF (42, 24);
		Color? tint;

		public sd.SizeF ButtonSize
		{
			get { return buttonSize; }
			set
			{
				buttonSize = value;
				button.Frame = new sd.RectangleF(sd.PointF.Empty, buttonSize);
			}
		}

		public bool UseButton
		{
			get { return Control.View != null; }
			set {
				if (button == null && value) {
					button = new NSButton {
						BezelStyle = NSBezelStyle.TexturedRounded,
						Frame = new sd.RectangleF(sd.PointF.Empty, buttonSize),
						Target = Control.Target,
						Action = Control.Action
					};
				}
				Control.View = value ? button : null;
			}
		}

		public NSButton Button
		{
			get { return button; }
		}

		public Color? Tint
		{
			get { return tint; }
			set {
				tint = value;
			}
		}

		public virtual string Identifier { get; set; }
		
		public ToolBarItemHandler()
		{
			this.Identifier = Guid.NewGuid().ToString();
		}

		public void UseStandardButton (bool grayscale)
		{
			UseButton = true;
			if (grayscale)
				Tint = Colors.Gray;
		}

		public override T CreateControl ()
		{
			return (T)new NSToolbarItem(this.Identifier);
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Control.Target = new ToolBarItemHandlerTarget{ Handler = this };
			Control.Action = new Selector("action");
			Control.Autovalidates = false;

			menuItem = new NSMenuItem(string.Empty);
			menuItem.Action = Control.Action;
			menuItem.Target = Control.Target;
			Control.MenuFormRepresentation = menuItem;
			Control.Enabled = true;
		}
		
		public virtual void ControlAdded(ToolBarHandler toolbar)
		{
		}
		
		public virtual void InvokeButton()
		{
		}
		
		public string Text {
			get { return Control.Label; }
			set { Control.Label = menuItem.Title = value ?? string.Empty; }
		}

		public string ToolTip {
			get { return Control.ToolTip; }
			set { menuItem.ToolTip = button.ToolTip = value ?? string.Empty; }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				this.image = value;
				SetImage ();
			}
		}

		void SetImage ()
		{
			var nsimage = this.image.ToNS (UseButton ? (int?)20 : null);
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint (tint.Value.ToNS ());
			Control.Image =  nsimage;
		}

		public virtual bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
	
		public virtual bool Selectable { get; set; }
		
		public void OnClick()
		{
			this.InvokeButton();
		}

		NSToolbarItem IToolBarBaseItemHandler.Control
		{
			get { return (NSToolbarItem)this.Control; }
		}
	}


}
