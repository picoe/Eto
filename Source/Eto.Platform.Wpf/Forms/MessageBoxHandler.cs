using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class MessageBoxHandler : IMessageBox
	{
		public string Text
		{
			get; set;
		}

		public IWidget Handler
		{
			get; set; 
		}

		public void Initialize ()
		{
		}

		public DialogResult ShowDialog (Control parent)
		{
			var element = parent.ControlObject as System.Windows.FrameworkElement;
			var window = element.GetParent<System.Windows.Window>();
			System.Windows.MessageBoxResult result;
			if (window != null) result = System.Windows.MessageBox.Show (window, Text);
			else result = System.Windows.MessageBox.Show (Text);
			
			return Convert(result);
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			var element = parent.ControlObject as System.Windows.FrameworkElement;
			var window = element.GetParent<System.Windows.Window> ();
			System.Windows.MessageBoxResult result;
			var wpfbuttons = Convert(buttons);
			if (window != null) result = System.Windows.MessageBox.Show (window, Text, string.Empty, wpfbuttons);
			else result = System.Windows.MessageBox.Show (Text, string.Empty, wpfbuttons);
			return Convert (result);
		}

		DialogResult Convert (System.Windows.MessageBoxResult result)
		{
			switch (result) {
				case System.Windows.MessageBoxResult.Cancel: return DialogResult.Cancel;
				case System.Windows.MessageBoxResult.No: return DialogResult.No;
				case System.Windows.MessageBoxResult.None: return DialogResult.None;
				case System.Windows.MessageBoxResult.Yes: return DialogResult.Yes;
				case System.Windows.MessageBoxResult.OK: return DialogResult.Ok;
				default: throw new NotSupportedException ();
			}
		}

		System.Windows.MessageBoxButton Convert (MessageBoxButtons value)
		{
			switch (value) {
				case MessageBoxButtons.YesNo:
					return System.Windows.MessageBoxButton.YesNo;
				case MessageBoxButtons.YesNoCancel:
					return System.Windows.MessageBoxButton.YesNoCancel;
				case MessageBoxButtons.OK:
					return System.Windows.MessageBoxButton.OK;
				case MessageBoxButtons.OKCancel:
					return System.Windows.MessageBoxButton.OKCancel;
				default:
					throw new NotSupportedException ();
			}
		}

	}
}
