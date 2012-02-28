﻿using System;
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

		public void Initialize ()
		{
		}

		public DialogResult ShowDialog (Control parent)
		{
			var element = parent == null ? null : parent.ControlObject as System.Windows.FrameworkElement;
			var window = element == null ? null : element.GetParent<System.Windows.Window>();
			System.Windows.MessageBoxResult result;
			var icon = Convert (Type);
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Text : null);
            if (window != null) result = System.Windows.MessageBox.Show(window, Text, caption, System.Windows.MessageBoxButton.OK, icon);
            else result = System.Windows.MessageBox.Show(Text, caption, System.Windows.MessageBoxButton.OK, icon);
			
			return Convert(result);
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			var element = parent == null ? null: parent.ControlObject as System.Windows.FrameworkElement;
			var window = element == null ? null: element.GetParent<System.Windows.Window>();
			System.Windows.MessageBoxResult result;
			var wpfbuttons = Convert(buttons);
			var icon = Convert(Type);
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Text : null);
            if (window != null) result = System.Windows.MessageBox.Show(window, Text, caption, wpfbuttons, icon);
            else result = System.Windows.MessageBox.Show(Text, caption, wpfbuttons, icon);
			return Convert (result);
		}

		System.Windows.MessageBoxImage Convert (MessageBoxType type)
		{
			switch (type) {
				default:
				case MessageBoxType.Information:
					return System.Windows.MessageBoxImage.Information;
				case MessageBoxType.Error:
					return System.Windows.MessageBoxImage.Error;
				case MessageBoxType.Question:
					return System.Windows.MessageBoxImage.Question;
				case MessageBoxType.Warning:
					return System.Windows.MessageBoxImage.Warning;
			}
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
