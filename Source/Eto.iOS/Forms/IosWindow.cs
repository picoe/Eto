using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.iOS.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Forms;
using sd = System.Drawing;

namespace Eto.iOS.Forms
{
	public abstract class IosWindow<TControl, TWidget, TCallback> : MacPanel<TControl, TWidget, TCallback>, Window.IHandler
		where TControl: UIView
		where TWidget: Window
		where TCallback: Window.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }

		public new Point Location
		{
			get
			{
				return Control.Frame.Location.ToEtoPoint();
			}
			set
			{
				var frame = this.Control.Frame;
				frame.Location = value.ToSDPointF();
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
				var control = (UIToolbar)value.ControlObject;
				control.SizeToFit();
				var height = control.Frame.Height;
				sd.SizeF screenSize;
				if (UIDevice.CurrentDevice.CheckSystemVersion(7,0))
					screenSize = UIScreen.MainScreen.Bounds.Size;
				else
					screenSize = UIScreen.MainScreen.ApplicationFrame.Size;

				var bottom = toolBar.Dock == ToolBarDock.Bottom;
				var frame = new sd.RectangleF(0, 0, screenSize.Width, height);
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

				this.AddChild(value);
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

		public Rectangle? RestoreBounds
		{
			get { return null; }
		}

		public WindowStyle WindowStyle { get { return WindowStyle.Default; } set { } }

		public virtual void BringToFront()
		{
		}

		public virtual void SendToBack()
		{
		}

		public ContextMenu ContextMenu { get { return null; } set { } }
	}
}

