using System;
using UIKit;
using Eto.Forms;
using Eto.iOS.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Forms;
using sd = System.Drawing;
using Eto.iOS.Forms.Toolbar;

namespace Eto.iOS.Forms
{
	public abstract class IosWindow<TControl, TWidget, TCallback> : MacPanel<TControl, TWidget, TCallback>, Window.IHandler
		where TControl: UIView
		where TWidget: Window
		where TCallback: Window.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }

		public bool DisableNavigationToolbar { get; set; }

		public new Point Location
		{
			get
			{
				return Control.Frame.Location.ToEtoPoint();
			}
			set
			{
				var frame = this.Control.Frame;
				frame.Location = value.ToNS();
				this.Control.Frame = frame;
			}
		}

		public double Opacity
		{
			get { return Control.Alpha; }
			set { Control.Alpha = (float)value; }
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Window.ClosedEvent:
				case Window.ClosingEvent:
				// TODO
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.BackgroundColor = UIColor.White;
		}

		public virtual void Close()
		{
		}

		ToolBar toolBar;

		public ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				toolBar = value;
				if (toolBar != null)
				{
					var toolbarHandler = toolBar.Handler as ToolBarHandler;
					if (toolbarHandler != null)
					{
						toolbarHandler.UpdateContentSize = () => LayoutAllChildren();
					}
				}
				SetToolbar();
			}
		}

		public override void OnLoad(EventArgs e)
		{
			SetToolbar(true);
			base.OnLoad(e);
		}

		NavigationHandler GetTopNavigation()
		{
			var content = Widget.Content;
			while (content is Panel)
			{
				content = ((Panel)content).Content;
			}
			return content == null ? null : content.Handler as NavigationHandler;
		}

		protected override CoreGraphics.CGRect AdjustContent(CoreGraphics.CGRect rect)
		{
			rect = base.AdjustContent(rect);
			if (ToolBar != null)
			{
				var toolbarHandler = toolBar.Handler as ToolBarHandler;
				if (toolbarHandler != null)
				{
					var tbheight = toolbarHandler.Control.Frame.Height;
					rect.Height -= tbheight;
					if (ToolBar.Dock == ToolBarDock.Top)
						rect.Y += tbheight;
				}
			}
			return rect;
		}

		protected bool UseTopToolBar
		{
			get
			{
				return toolBar != null && toolBar.Dock == ToolBarDock.Top;
			}
		}

		void SetToolbar(bool force = false)
		{
			if (toolBar == null)
				return;
			var control = ToolBarHandler.GetControl(toolBar);
			var topNav = GetTopNavigation();
			if (!DisableNavigationToolbar && topNav != null && toolBar.Dock == ToolBarDock.Bottom)
			{
				topNav.MainToolBar = control.Items;
			}
			else if (Widget.Loaded || force)
			{
				control.SizeToFit();
				var height = control.Frame.Height;
				CoreGraphics.CGSize screenSize;
				if (Platform.IsIos7)
					screenSize = UIScreen.MainScreen.Bounds.Size;
				else
					screenSize = UIScreen.MainScreen.ApplicationFrame.Size;
				var bottom = toolBar.Dock == ToolBarDock.Bottom;
				var frame = new CoreGraphics.CGRect(0, 0, screenSize.Width, height);
				var mask = UIViewAutoresizing.FlexibleWidth;
				if (bottom)
				{
					frame.Y = screenSize.Height - height;
					mask |= UIViewAutoresizing.FlexibleTopMargin;
				}
				else
					frame.Y = ApplicationHandler.Instance.StatusBarAdjustment;
				control.Frame = frame;
				control.AutoresizingMask = mask;
				this.AddChild(toolBar);
				if (Widget.Loaded)
					LayoutChildren();
			}
		}

		public abstract string Title { get; set; }

		public Screen Screen
		{
			get { return Screen.PrimaryScreen; }
		}

		public MenuBar Menu { get { return null; } set { } }

		public Icon Icon { get { return null; } set { } }

		public bool Resizable { get { return false; } set { } }

		public bool Maximizable { get { return false; } set { } }

		public bool Minimizable { get { return false; } set { } }

		public bool ShowInTaskbar { get { return false; } set { } }

		public bool Topmost { get { return false; } set { } }

		public WindowState WindowState { get; set; }

		public Rectangle RestoreBounds
		{
			get { return Widget.Bounds; }
		}

		public WindowStyle WindowStyle { get { return WindowStyle.Default; } set { } }

		public virtual void BringToFront()
		{
		}

		public virtual void SendToBack()
		{
		}

		public override ContextMenu ContextMenu { get { return null; } set { } }

		public virtual void SetOwner(Window owner)
		{
		}

		public float LogicalPixelSize
		{
			get
			{
				return (float)UIScreen.MainScreen.NativeScale;
			}
		}
	}
}

