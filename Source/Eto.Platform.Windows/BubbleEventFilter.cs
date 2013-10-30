using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using swf = System.Windows.Forms;

namespace Eto.Platform.Windows
{
	public class BubbleEvent
	{
		public Func<BubbleEventArgs, bool> HandleEvent { get; set; }

		public int Message { get; set; }
	}

	public class BubbleEventArgs : EventArgs
	{
		public swf.Message Message { get; private set; }

		public Control Control { get; private set; }

		public swf.Control WinControl { get; private set; }

		public IWindowsControl WindowsControl
		{
			get { return Control != null ? Control.Handler as IWindowsControl : null; }
		}

		public IEnumerable<Control> Parents
		{
			get
			{
				var control = WinControl;
				while (control != null) {
					var etochild = ToEto (control);
					if (etochild != null)
						yield return etochild;
					control = control.Parent;
				}
			}
		}

		public BubbleEventArgs (swf.Message message, swf.Control winControl)
		{
			this.Message = message;
			this.Control = ToEto(winControl);
			this.WinControl = winControl;
		}

		static Control ToEto (swf.Control child)
		{
			var handler = child.Tag as IControl;
			if (handler != null) {
				return handler.Widget as Control;
			}
			return null;
		}
	}

	public class BubbleEventFilter : swf.IMessageFilter
	{
		Dictionary<int, BubbleEvent> messages = new Dictionary<int, BubbleEvent> ();

		public void AddBubbleEvents (Func<BubbleEventArgs, bool> handleEvent, params int[] messages)
		{
			foreach (var message in messages) {
				AddBubbleEvent (handleEvent, message);
			}
		}

		public void AddBubbleEvent (Func<BubbleEventArgs, bool> handleEvent, int message)
		{
			messages.Add (message, new BubbleEvent {
				Message = message,
				HandleEvent = handleEvent
			});
		}

		public bool PreFilterMessage (ref swf.Message message)
		{
			BubbleEvent bubble;
			if (messages.TryGetValue (message.Msg, out bubble)) {
				var child = swf.Control.FromHandle (message.HWnd);

				if (child != null) {
					var args = new BubbleEventArgs (message, child);
					if (bubble.HandleEvent (args))
						return true;
				}
			}
			return false;
		}

		public void AddBubbleMouseEvent (Action<Control, MouseEventArgs> action, bool? capture, int message, Func<MouseButtons, MouseButtons> modifyButtons = null)
		{
			AddBubbleEvent (be => MouseEvent (be, action, capture, modifyButtons), message);
		}

		public void AddBubbleMouseEvents (Action<Control, MouseEventArgs> action, bool? capture, params int[] messages)
		{
			foreach (var message in messages) {
				AddBubbleEvent (be => MouseEvent (be, action, capture), message);
			}
		}

		static bool MouseEvent (BubbleEventArgs be, Action<Control, MouseEventArgs> action, bool? capture, Func<MouseButtons, MouseButtons> modifyButtons = null)
		{
			var modifiers = swf.Control.ModifierKeys.ToEto ();
			var delta = new SizeF (0, Win32.GetWheelDeltaWParam (be.Message.WParam) / Conversions.WheelDelta);
			var buttons = Win32.GetMouseButtonWParam (be.Message.WParam).ToEto ();
			if (modifyButtons != null)
				buttons = modifyButtons (buttons);
			var handler = be.WindowsControl;
			var mousePosition = swf.Control.MousePosition.ToEto ();
			var ret = false;
			foreach (var control in be.Parents) {
				var me = new MouseEventArgs(buttons, modifiers, control.PointFromScreen(mousePosition), delta);
				action (control, me);
				if (me.Handled) {
					ret = true;
					break;
				}
			}
			if (capture != null && ret || (handler != null && handler.ShouldCaptureMouse)) {
				//be.WinControl.Capture = capture.Value;
			}
			return ret;
		}

	}
}
