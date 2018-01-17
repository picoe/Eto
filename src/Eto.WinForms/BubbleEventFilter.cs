using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using swf = System.Windows.Forms;
using Eto.WinForms.Forms;

namespace Eto.WinForms
{
	class BubbleEvent
	{
		public Func<BubbleEventArgs, bool> HandleEvent { get; set; }

		public Win32.WM Message { get; set; }
	}

	class BubbleEventArgs : EventArgs
	{
		public swf.Message Message { get; private set; }

		public Control Control { get; private set; }

		public swf.Control WinControl { get; private set; }

		public IWindowsControl WindowsControl
		{
			get { return Control != null ? Control.Handler as IWindowsControl : null; }
		}

		public IEnumerable<Control> Controls
		{
			get
			{
				var control = Control;
				while (control != null)
				{
					yield return control;
					control = control.VisualParent;
				}
			}
		}

		public IEnumerable<Control> Parents
		{
			get
			{
				if (Control != null)
				{
					var control = Control.VisualParent;
					while (control != null)
					{
						yield return control;
						control = control.VisualParent;
					}
				}
			}
		}

		public BubbleEventArgs(swf.Message message, swf.Control winControl)
		{
			this.Message = message;
			this.Control = ToEto(winControl);
			this.WinControl = winControl;
		}

		static Control ToEto(swf.Control child)
		{
			var handler = child.Tag as Control.IHandler;
			return handler != null ? handler.Widget as Control : null;
		}
	}

	class BubbleEventFilter : swf.IMessageFilter
	{
		readonly Dictionary<int, BubbleEvent> messages = new Dictionary<int, BubbleEvent>();

		public void AddBubbleEvents(Func<BubbleEventArgs, bool> handleEvent, params Win32.WM[] messages)
		{
			foreach (var message in messages)
			{
				AddBubbleEvent(handleEvent, message);
			}
		}

		public void AddBubbleEvent(Func<BubbleEventArgs, bool> handleEvent, Win32.WM message)
		{
			messages.Add((int)message, new BubbleEvent
			{
				Message = message,
				HandleEvent = handleEvent
			});
		}

		public bool PreFilterMessage(ref swf.Message message)
		{
			BubbleEvent bubble;
			if (messages.TryGetValue(message.Msg, out bubble))
			{
				var child = swf.Control.FromHandle(message.HWnd);

				if (child != null)
				{
					var args = new BubbleEventArgs(message, child);
					if (bubble.HandleEvent(args))
						return true;
				}
			}
			return false;
		}

		public void AddBubbleKeyEvent(Action<Control, Control.ICallback, KeyEventArgs> action, Win32.WM message, KeyEventType keyEventType)
		{
			AddBubbleEvent(be => KeyEvent(be, action, keyEventType), message);
		}

		public void AddBubbleKeyCharEvent(Action<Control, Control.ICallback, KeyEventArgs> action, Win32.WM message, KeyEventType keyEventType)
		{
			AddBubbleEvent(be => KeyCharEvent(be, action, keyEventType), message);
		}

		public void AddBubbleMouseEvent(Action<Control, Control.ICallback, MouseEventArgs> action, bool? capture, Win32.WM message, Func<MouseButtons, MouseButtons> modifyButtons = null)
		{
			AddBubbleEvent(be => MouseEvent(be, action, capture, modifyButtons), message);
		}

		public void AddBubbleMouseEvents(Action<Control, Control.ICallback, MouseEventArgs> action, bool? capture, params Win32.WM[] messages)
		{
			foreach (var message in messages)
			{
				AddBubbleEvent(be => MouseEvent(be, action, capture), message);
			}
		}

		static bool MouseEvent(BubbleEventArgs be, Action<Control, Control.ICallback, MouseEventArgs> action, bool? capture, Func<MouseButtons, MouseButtons> modifyButtons = null)
		{
			var mainControl = be.Control;
			if (mainControl == null)
				return false;

			var modifiers = swf.Control.ModifierKeys.ToEto();
			var delta = new SizeF(0, Win32.GetWheelDeltaWParam(be.Message.WParam) / WinConversions.WheelDelta);
			var buttons = Win32.GetMouseButtonWParam(be.Message.WParam).ToEto();
			if (modifyButtons != null)
				buttons = modifyButtons(buttons);
			var handler = be.WindowsControl;
			var mousePosition = swf.Control.MousePosition.ToEto();

			var msg = be.Message;
			var me = new MouseEventArgs(buttons, modifiers, mainControl.PointFromScreen(mousePosition), delta);
			action(mainControl, handler.Callback, me);

			if (!me.Handled && handler != null && handler.ShouldBubbleEvent(msg))
			{
				foreach (var control in be.Parents)
				{
					me = new MouseEventArgs(buttons, modifiers, control.PointFromScreen(mousePosition), delta);
					action(control, handler.Callback, me);
					if (me.Handled)
						return true;
				}
			}
			return me.Handled;
		}

		static bool KeyEvent(BubbleEventArgs be, Action<Control, Control.ICallback, KeyEventArgs> action, KeyEventType keyEventType)
		{
			Keys keyData = ((swf.Keys)(long)be.Message.WParam | swf.Control.ModifierKeys).ToEto();
			
			char? keyChar = null;
			var kevt = new KeyEventArgs(keyData, keyEventType, keyChar);
			if (be.Control != null)
				action(be.Control, (Control.ICallback)((ICallbackSource)be.Control).Callback, kevt);
			if (!kevt.Handled && (keyEventType != KeyEventType.KeyDown || !IsInputKey(be.Message.HWnd, keyData)))
			{
				foreach (var control in be.Parents)
				{
					var callback = (Control.ICallback)((ICallbackSource)control).Callback;
					action(control, callback, kevt);
					if (kevt.Handled)
						break;
				}
			}
			return kevt.Handled;
		}

		static bool IsInputChar(IntPtr hwnd, char charCode)
		{
			int num = charCode == '\t' ? 134 : 132;
			return ((int)((long)Win32.SendMessage(hwnd, Win32.WM.GETDLGCODE, IntPtr.Zero, IntPtr.Zero)) & num) != 0;
		}

		static bool IsInputKey(IntPtr hwnd, Keys keyData)
		{
			if (keyData.HasFlag(Keys.Alt))
			{
				return false;
			}
			int num = 4;
			switch (keyData & Keys.KeyMask)
			{
				case Keys.Tab:
					num = 6;
					break;
				case Keys.Left:
				case Keys.Up:
				case Keys.Right:
				case Keys.Down:
					num = 5;
					break;
			}
			if (!EtoEnvironment.Platform.IsWindows)
				return false;
			return ((int)((long)Win32.SendMessage(hwnd, Win32.WM.GETDLGCODE, IntPtr.Zero, IntPtr.Zero)) & num) != 0;
		}

		static bool KeyCharEvent(BubbleEventArgs be, Action<Control, Control.ICallback, KeyEventArgs> action, KeyEventType keyEventType)
		{
			char keyChar = (char)((long)be.Message.WParam);
			var kevt = new KeyEventArgs(Keys.None, keyEventType, keyChar);
			if (be.Control != null)
				action(be.Control, (Control.ICallback)((ICallbackSource)be.Control).Callback, kevt);
			if (!kevt.Handled && !IsInputChar(be.Message.HWnd, keyChar))
			{
				foreach (var control in be.Parents)
				{
					var callback = (Control.ICallback)((ICallbackSource)control).Callback;
					action(control, callback, kevt);
					if (kevt.Handled)
						break;
				}
			}
			return kevt.Handled;
		}
	}
}
