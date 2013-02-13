using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using sw = System.Windows;
//using WpfMessageBox = Xceed.Wpf.Toolkit.MessageBox;
using WpfMessageBox = System.Windows.MessageBox;

namespace Eto.Platform.Wpf.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, IMessageBox
	{
		public string Text
		{
			get; set;
		}

		public string Caption
		{
			get;
			set;
		}

		public MessageBoxType Type
		{
			get;
			set;
		}

		public DialogResult ShowDialog (Control parent)
		{
			var element = parent == null ? null : parent.GetContainerControl();
			var window = element == null ? null : element.GetParent<sw.Window>();
			sw.MessageBoxResult result;
			var icon = Convert (Type);
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (window != null) result = WpfMessageBox.Show (window, Text, caption, sw.MessageBoxButton.OK, icon);
			else result = WpfMessageBox.Show (Text, caption, sw.MessageBoxButton.OK, icon);
			
			return Convert(result);
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			var element = parent == null ? null: parent.GetContainerControl ();
			var window = element == null ? null : element.GetParent<sw.Window> ();
			sw.MessageBoxResult result;
			var wpfbuttons = Convert(buttons);
			var icon = Convert(Type);
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (window != null) result = WpfMessageBox.Show (window, Text, caption, wpfbuttons, icon);
			else result = WpfMessageBox.Show (Text, caption, wpfbuttons, icon);
			return Convert (result);
		}

		sw.MessageBoxImage Convert (MessageBoxType type)
		{
			switch (type) {
			default:
			case MessageBoxType.Information:
				return sw.MessageBoxImage.Information;
			case MessageBoxType.Error:
				return sw.MessageBoxImage.Error;
			case MessageBoxType.Question:
				return sw.MessageBoxImage.Question;
			case MessageBoxType.Warning:
				return sw.MessageBoxImage.Warning;
			}
		}

		DialogResult Convert (sw.MessageBoxResult result)
		{
			switch (result) {
			case sw.MessageBoxResult.Cancel: return DialogResult.Cancel;
			case sw.MessageBoxResult.No: return DialogResult.No;
			case sw.MessageBoxResult.None: return DialogResult.None;
			case sw.MessageBoxResult.Yes: return DialogResult.Yes;
			case sw.MessageBoxResult.OK: return DialogResult.Ok;
			default: throw new NotSupportedException ();
			}
		}

		sw.MessageBoxButton Convert (MessageBoxButtons value)
		{
			switch (value) {
			case MessageBoxButtons.YesNo:
				return sw.MessageBoxButton.YesNo;
			case MessageBoxButtons.YesNoCancel:
				return sw.MessageBoxButton.YesNoCancel;
			case MessageBoxButtons.OK:
				return sw.MessageBoxButton.OK;
			case MessageBoxButtons.OKCancel:
				return sw.MessageBoxButton.OKCancel;
			default:
				throw new NotSupportedException ();
			}
		}

	}
}
