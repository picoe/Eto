using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms.Actions;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.ToolBar
{
	public interface IToolBarBaseItemHandler
	{
		string Identifier { get; }

		ToolItem Widget { get; }

		NSToolbarItem Control { get; }

		bool Selectable { get; }

		bool Visible { get; }

		void ControlAdded(ToolBarHandler toolbar);

		void SetVisible(bool visible);
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

		public IToolBarItemHandler Handler { get => (IToolBarItemHandler)handler?.Target; set => handler = new WeakReference(value); }

		[Export("validateToolbarItem:")]
		public bool ValidateToolbarItem(NSToolbarItem item)
		{
			return Handler?.Enabled == true;
		}

		[Export("action")]
		public bool Action()
		{
			Handler?.OnClick();
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

	static class ToolItemHandler
	{
		internal static readonly Selector selAction = new Selector("action");
		internal static readonly object Visible_Key = new object();
	}

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler, IToolBarItemHandler
		where TControl: NSToolbarItem
		where TWidget: ToolItem
	{
		Image image;
		NSButton button;
		NSMenuItem menuItem;
		Color? tint;

		CGSize ButtonSize
		{
			get { 
				if (toolBarItemStyle == MacToolBarItemStyle.Default)
					return new CGSize (40, 32);
				else if (toolBarItemStyle == MacToolBarItemStyle.StandardButton)
					return new CGSize (40, 24);
				else // large button
					return new CGSize (40, 32); 
			}
		}

		public int ImageSize => (toolBarItemStyle == MacToolBarItemStyle.StandardButton) ? 20 : 32;

		MacToolBarItemStyle toolBarItemStyle;
		public MacToolBarItemStyle ToolBarItemStyle
		{
			get { return toolBarItemStyle; }
			set {
				toolBarItemStyle = value; // set the value first because ButtonSize and ImageSize depend on it.
				button = null;
				if (value == MacToolBarItemStyle.StandardButton || value == MacToolBarItemStyle.LargeButton) {
					button = new NSButton {
						Title = string.Empty,
						BezelStyle = NSBezelStyle.TexturedRounded,
						Bordered = toolBarItemStyle == MacToolBarItemStyle.StandardButton, // no border or bezel in the large button style
						Frame = new CGRect(CGPoint.Empty, ButtonSize),
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

		public NSButton Button => button;

		public Color? Tint
		{
			get { return tint; }
			set
			{
				tint = value;
			}
		}

		public virtual string Identifier { get; set; }

		protected override TControl CreateControl() => (TControl)new NSToolbarItem(Identifier);

		protected virtual MacToolBarItemStyle DefaultStyle { get { return MacToolBarItemStyle.StandardButton; } }

		protected virtual bool IsButton => true;

		protected override void Initialize()
		{
			this.Identifier = Guid.NewGuid().ToString();
			if (IsButton)
			{
				Control.Target = new ToolBarItemHandlerTarget { Handler = this };
				Control.Action = ToolItemHandler.selAction;
				Control.Autovalidates = false;
				Control.Label = string.Empty;

				menuItem = new NSMenuItem(string.Empty)
				{
					Action = Control.Action,
					Target = Control.Target
				};
				Control.MenuFormRepresentation = menuItem;
				Control.Enabled = true;
				this.ToolBarItemStyle = DefaultStyle;
			}
			base.Initialize();
		}

		[Obsolete("Use ToolBarItemStyle and Tint properties instead")]
		public void UseStandardButton(bool grayscale)
		{
			this.ToolBarItemStyle = MacToolBarItemStyle.StandardButton;
			if (grayscale)
				Tint = Colors.Gray;
		}

		ToolBarHandler toolbar;

		public virtual void ControlAdded(ToolBarHandler toolbar)
		{
			this.toolbar = toolbar;
		}

		public virtual void InvokeButton()
		{
		}

		public string Text
		{
			get => Control?.Label;
			set
			{
				if (Control == null)
					return;

				Control.Label = menuItem.Title = value ?? string.Empty;
			}
		}

		public string ToolTip
		{
			get => Control?.ToolTip;
			set
			{
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
			if (Control == null)
				return;
			var nsimage = image.ToNS(ImageSize);
			if (tint != null && nsimage != null)
				nsimage = nsimage.Tint(tint.Value.ToNSUI());
			Control.Image = nsimage;
		}

		public virtual bool Enabled
		{
			get => Control?.Enabled != false;
			set
			{
				if (Control == null)
					return;
				Control.Enabled = value;
			}
		}

		public virtual bool Selectable { get; }

		public void OnClick()
		{
			InvokeButton();
		}

		NSToolbarItem IToolBarBaseItemHandler.Control => Control;

		public bool Visible
		{
			get
			{
				if (menuItem != null)
					return !menuItem.Hidden;
				return Widget.Properties.Get<bool>(ToolItemHandler.Visible_Key, true);
			}
			set
			{
				if (value != Visible)
				{
					toolbar?.ChangeVisibility(Widget, value);

					if (menuItem != null)
						menuItem.Hidden = !value;

					Widget.Properties.Set(ToolItemHandler.Visible_Key, value, true);
				}
			}
		}

		ToolItem IToolBarBaseItemHandler.Widget => Widget;

		public void CreateFromCommand(Command command)
		{
			if (command is MacCommand m)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public void SetVisible(bool visible)
		{
			Widget.Properties.Set<bool?>(ToolItemHandler.Visible_Key, visible, true);
			if (menuItem != null)
				menuItem.Hidden = !visible;
		}
	}
}
