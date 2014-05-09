using System;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Eto.Drawing;
using MonoMac.ObjCRuntime;
using sd = System.Drawing;
using Eto.Mac.Forms.Actions;

namespace Eto.Mac
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

		NSButton Button { get; }

		MacToolBarItemStyle ToolBarItemStyle {get; set;}
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

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler, IToolBarItemHandler
		where TControl: NSToolbarItem
		where TWidget: ToolItem
	{
		Image image;
		NSButton button;
		NSMenuItem menuItem;
		Color? tint;

		private sd.SizeF ButtonSize
		{
			get { 
				if (toolBarItemStyle == MacToolBarItemStyle.Default)
					return new sd.SizeF (42, 32);
				else if (toolBarItemStyle == MacToolBarItemStyle.StandardButton)
					return new sd.SizeF (42, 24);
				else // large button
					return new sd.SizeF (42, 32); 
			}
		}

		public int ImageSize { get { return (toolBarItemStyle == MacToolBarItemStyle.StandardButton) ? 20 : 32; } }

		MacToolBarItemStyle toolBarItemStyle;
		public MacToolBarItemStyle ToolBarItemStyle
		{
			get { return toolBarItemStyle; }
			set {
				toolBarItemStyle = value; // set the value first because ButtonSize and ImageSize depend on it.
				button = null;
				if (value == MacToolBarItemStyle.StandardButton || value == MacToolBarItemStyle.LargeButton) {
					button = new NSButton {
						BezelStyle = NSBezelStyle.TexturedRounded,
						Bordered = toolBarItemStyle == MacToolBarItemStyle.StandardButton, // no border or bezel in the large button style
						Frame = new sd.RectangleF(sd.PointF.Empty, ButtonSize),
						Target = Control.Target,
						Action = Control.Action,
					};
					if (value == MacToolBarItemStyle.LargeButton)
						button.SetButtonType (NSButtonType.MomentaryChange); // prevents a flash in the large button view. See the comment at the bottom of http://yellowfieldtechnologies.wordpress.com/2011/11/18/nspopover-from-nstoolbaritem/#comments
					Control.View = button;
				}
				SetImage ();
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

		public string Identifier { get; set; }

		protected ToolItemHandler()
		{
			this.Identifier = Guid.NewGuid().ToString();
			Control = (TControl)new NSToolbarItem(Identifier);
			Control.Target = new ToolBarItemHandlerTarget { Handler = this };
			Control.Action = selAction;
			Control.Autovalidates = false;

			menuItem = new NSMenuItem(string.Empty);
			menuItem.Action = Control.Action;
			menuItem.Target = Control.Target;
			Control.MenuFormRepresentation = menuItem;
			Control.Enabled = true;

			this.ToolBarItemStyle = MacToolBarItemStyle.Default;
		}

		public void UseStandardButton(bool grayscale)
		{
			this.ToolBarItemStyle = MacToolBarItemStyle.StandardButton;
			if (grayscale)
				Tint = Colors.Gray;
		}

		static readonly Selector selAction = new Selector("action");

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
					menuItem.ToolTip = value ?? string.Empty;
				if (button != null)
					button.ToolTip = value ?? string.Empty;
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
			var nsimage = image.ToNS(ImageSize);
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint(tint.Value.ToNSUI());
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

		public void CreateFromCommand(Command command)
		{
			var m = command as MacCommand;
			if (m != null)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}
	}
}
