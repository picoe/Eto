using System;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.ObjCRuntime;
using sd = System.Drawing;
using Eto.Platform.Mac.Forms.Actions;

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

	public abstract class ToolBarItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IToolBarItem, IToolBarItemHandler, ICopyFromAction
		where TControl: NSToolbarItem
		where TWidget: ToolBarItem
	{
		Image image;
		NSButton button;
		NSMenuItem menuItem;
		sd.SizeF buttonSize = new sd.SizeF(42, 24);
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
			set
			{
				if (button == null && value)
				{
					button = new NSButton
					{
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
			set
			{
				tint = value;
			}
		}

		public virtual string Identifier { get; set; }

		protected ToolBarItemHandler()
		{
			this.Identifier = Guid.NewGuid().ToString();
		}

		public void UseStandardButton(bool grayscale)
		{
			UseButton = true;
			if (grayscale)
				Tint = Colors.Gray;
		}

		public override TControl CreateControl()
		{
			return (TControl)new NSToolbarItem(Identifier);
		}

		static readonly Selector selAction = new Selector("action");

		protected override void Initialize()
		{
			base.Initialize();
			Control.Target = new ToolBarItemHandlerTarget { Handler = this };
			Control.Action = selAction;
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

		public string Text
		{
			get { return Control.Label; }
			set { Control.Label = menuItem.Title = value ?? string.Empty; }
		}

		public string ToolTip
		{
			get { return Control.ToolTip; }
			set { 
				if (menuItem != null)
					menuItem.ToolTip = value ?? "";
				if (button != null)
					button.ToolTip = value ?? "";
			}
		}

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
			var nsimage = image.ToNS(UseButton ? (int?)20 : null);
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint(tint.Value.ToNS());
			Control.Image = nsimage;
		}

		public virtual bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public virtual bool Selectable { get; set; }

		public void OnClick()
		{
			InvokeButton();
		}

		NSToolbarItem IToolBarBaseItemHandler.Control
		{
			get { return Control; }
		}

		public void CopyFrom(BaseAction action)
		{
			var m = action as MacButtonAction;
			if (m != null)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}
	}
}
