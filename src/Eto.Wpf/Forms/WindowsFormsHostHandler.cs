using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Wpf.Forms
{
	public class WindowsFormsHostHandler<TControl, TWidget, TCallback> : WpfFrameworkElement<WindowsFormsHost, TWidget, TCallback>
		where TControl : swf.Control
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		public override bool UseKeyPreview => true;
		public override bool UseMousePreview => true;

		public override Color BackgroundColor
		{
			get { return WinFormsControl.BackColor.ToEto(); }
			set { WinFormsControl.BackColor = value.ToSD(); }
		}

		public TControl WinFormsControl
		{
			get { return (TControl)Control.Child; }
			set { Control.Child = value; }
		}

		public WindowsFormsHostHandler(TControl control)
			: this()
		{
			Control.Child = control;
		}

		public WindowsFormsHostHandler()
		{
			Control = new WindowsFormsHost();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.MouseMoveEvent:
					WinFormsControl.MouseMove += WinFormsControl_MouseMove;
					break;
				case Eto.Forms.Control.MouseUpEvent:
					WinFormsControl.MouseUp += WinFormsControl_MouseUp;
					break;
				case Eto.Forms.Control.MouseDownEvent:
					WinFormsControl.MouseDown += WinFormsControl_MouseDown;
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					WinFormsControl.MouseDoubleClick += WinFormsControl_MouseDoubleClick;
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					WinFormsControl.MouseEnter += WinFormsControl_MouseEnter;
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					WinFormsControl.MouseLeave += WinFormsControl_MouseLeave;
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					WinFormsControl.MouseWheel += WinFormsControl_MouseWheel;
					break;
				case Eto.Forms.Control.KeyDownEvent:
					WinFormsControl.KeyDown += WinFormsControl_KeyDown;
					WinFormsControl.KeyPress += WinFormsControl_KeyPress;
					break;
				case Eto.Forms.Control.KeyUpEvent:
					WinFormsControl.KeyUp += WinFormsControl_KeyUp;
					break;
				case TextControl.TextChangedEvent:
					WinFormsControl.TextChanged += WinFormsControl_TextChanged;
					break;
				case Eto.Forms.Control.TextInputEvent:
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
					break;
				case Eto.Forms.Control.GotFocusEvent:
					WinFormsControl.GotFocus += WinFormsControl_GotFocus;
					break;
				case Eto.Forms.Control.LostFocusEvent:
					WinFormsControl.LostFocus += WinFormsControl_LostFocus;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		Keys key;
		bool handled;
		char keyChar;
		bool charPressed;
		public Keys? LastKeyDown { get; set; }

		void WinFormsControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			charPressed = false;
			handled = true;
			key = e.KeyData.ToEto();

			if (key != Keys.None && LastKeyDown != key)
			{
				var kpea = new KeyEventArgs(key, KeyEventType.KeyDown);
				Callback.OnKeyDown(Widget, kpea);

				handled = e.SuppressKeyPress = e.Handled = kpea.Handled;
			}
			else
				handled = false;

			if (!handled && charPressed)
			{
				// this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
				// we want the char event to come after the dialog is closed, and handled is set to true!
				var kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
				Callback.OnKeyDown(Widget, kpea);
				e.SuppressKeyPress = e.Handled = kpea.Handled;
			}

			LastKeyDown = null;
		}

		void WinFormsControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			charPressed = true;
			keyChar = e.KeyChar;
			if (!handled)
			{
				if (!char.IsControl(e.KeyChar))
				{
					var tia = new TextInputEventArgs(keyChar.ToString());
					Callback.OnTextInput(Widget, tia);
					e.Handled = tia.Cancel;
				}

				if (!e.Handled)
				{
					var kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
					Callback.OnKeyDown(Widget, kpea);
					e.Handled = kpea.Handled;
				}
			}
			else
				e.Handled = true;
		}


		void WinFormsControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			key = e.KeyData.ToEto();

			var kpea = new KeyEventArgs(key, KeyEventType.KeyUp);
			Callback.OnKeyUp(Widget, kpea);
			e.Handled = kpea.Handled;
		}

		void WinFormsControl_TextChanged(object sender, EventArgs e)
		{
			var widget = Widget as TextControl;
			if (widget != null)
			{
				var callback = (TextControl.ICallback)((ICallbackSource)widget).Callback;
				callback.OnTextChanged(widget, e);
			}
		}

		void WinFormsControl_MouseWheel(object sender, swf.MouseEventArgs e) => Callback.OnMouseWheel(Widget, e.ToEto(WinFormsControl));

		void WinFormsControl_MouseLeave(object sender, EventArgs e) => Callback.OnMouseLeave(Widget, new MouseEventArgs(Mouse.Buttons, Keyboard.Modifiers, PointFromScreen(Mouse.Position)));

		void WinFormsControl_MouseEnter(object sender, EventArgs e) => Callback.OnMouseEnter(Widget, new MouseEventArgs(Mouse.Buttons, Keyboard.Modifiers, PointFromScreen(Mouse.Position)));

		void WinFormsControl_MouseDoubleClick(object sender, swf.MouseEventArgs e) => Callback.OnMouseDoubleClick(Widget, e.ToEto(WinFormsControl));

		void WinFormsControl_MouseDown(object sender, swf.MouseEventArgs e) => Callback.OnMouseDown(Widget, e.ToEto(WinFormsControl));

		void WinFormsControl_MouseUp(object sender, swf.MouseEventArgs e) => Callback.OnMouseUp(Widget, e.ToEto(WinFormsControl));

		void WinFormsControl_MouseMove(object sender, swf.MouseEventArgs e) => Callback.OnMouseMove(Widget, e.ToEto(WinFormsControl));

		void WinFormsControl_LostFocus(object sender, EventArgs e) => Callback.OnLostFocus(Widget, EventArgs.Empty);

		void WinFormsControl_GotFocus(object sender, EventArgs e) => Callback.OnGotFocus(Widget, EventArgs.Empty);

		public override void Focus()
		{
			if (Widget.Loaded && WinFormsControl.IsHandleCreated)
				WinFormsControl.Focus();
			else
				Widget.LoadComplete += Widget_LoadComplete;
		}

		void Widget_LoadComplete(object sender, EventArgs e)
		{
			Widget.LoadComplete -= Widget_LoadComplete;
			WinFormsControl.Focus();
		}

		public override bool HasFocus => WinFormsControl.Focused;

		public override bool AllowDrop
		{
			get { return WinFormsControl.AllowDrop; }
			set { WinFormsControl.AllowDrop = value; }
		}

		public override void SuspendLayout()
		{
			base.SuspendLayout();
			WinFormsControl.SuspendLayout();
		}

		public override void ResumeLayout()
		{
			base.ResumeLayout();
			WinFormsControl.ResumeLayout();
		}

		public override void Invalidate(bool invalidateChildren)
		{
			WinFormsControl.Invalidate(invalidateChildren);
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			WinFormsControl.Invalidate(rect.ToSD(), invalidateChildren);
		}

		public override bool Enabled
		{
			get { return WinFormsControl.Enabled; }
			set { WinFormsControl.Enabled = value; }
		}
	}
}
