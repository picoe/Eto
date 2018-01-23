using System;
using Eto.Forms;
using sw = System.Windows;
//using WpfMessageBox = Xceed.Wpf.Toolkit.MessageBox;
using WpfMessageBox = System.Windows.MessageBox;

namespace Eto.Wpf.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
	{
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			using (var visualStyles = new EnableThemingInScope(ApplicationHandler.EnableVisualStyles))
			{
				var element = parent == null ? null : parent.GetContainerControl();
				var window = element == null ? null : element.GetVisualParent<sw.Window>();
				sw.MessageBoxResult result;
				var buttons = Convert(Buttons);
				var defaultButton = Convert(DefaultButton, Buttons);
				var icon = Convert(Type);
				var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
				if (window != null) result = WpfMessageBox.Show(window, Text, caption, buttons, icon, defaultButton);
				else result = WpfMessageBox.Show(Text, caption, buttons, icon, defaultButton);
				return Convert(result);
			}
		}

		public static sw.MessageBoxResult Convert(MessageBoxDefaultButton defaultButton, MessageBoxButtons buttons)
		{
			switch (defaultButton)
			{
				case MessageBoxDefaultButton.OK:
					return sw.MessageBoxResult.OK;
				case MessageBoxDefaultButton.No:
					return sw.MessageBoxResult.No;
				case MessageBoxDefaultButton.Cancel:
					return sw.MessageBoxResult.Cancel;
				case MessageBoxDefaultButton.Default:
					switch (buttons)
					{
						case MessageBoxButtons.OK:
							return sw.MessageBoxResult.OK;
						case MessageBoxButtons.OKCancel:
							return sw.MessageBoxResult.Cancel;
						case MessageBoxButtons.YesNo:
							return sw.MessageBoxResult.No;
						case MessageBoxButtons.YesNoCancel:
							return sw.MessageBoxResult.Cancel;
						default:
							throw new NotSupportedException();
					}
				default:
					throw new NotSupportedException();
			}
		}


		static sw.MessageBoxImage Convert(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Information:
					return sw.MessageBoxImage.Information;
				case MessageBoxType.Error:
					return sw.MessageBoxImage.Error;
				case MessageBoxType.Question:
					return sw.MessageBoxImage.Question;
				case MessageBoxType.Warning:
					return sw.MessageBoxImage.Warning;
				default:
					throw new NotSupportedException();
			}
		}

		static DialogResult Convert(sw.MessageBoxResult result)
		{
			switch (result)
			{
				case sw.MessageBoxResult.Cancel: return DialogResult.Cancel;
				case sw.MessageBoxResult.No: return DialogResult.No;
				case sw.MessageBoxResult.None: return DialogResult.None;
				case sw.MessageBoxResult.Yes: return DialogResult.Yes;
				case sw.MessageBoxResult.OK: return DialogResult.Ok;
				default: throw new NotSupportedException();
			}
		}

		static sw.MessageBoxButton Convert(MessageBoxButtons value)
		{
			switch (value)
			{
				case MessageBoxButtons.YesNo:
					return sw.MessageBoxButton.YesNo;
				case MessageBoxButtons.YesNoCancel:
					return sw.MessageBoxButton.YesNoCancel;
				case MessageBoxButtons.OK:
					return sw.MessageBoxButton.OK;
				case MessageBoxButtons.OKCancel:
					return sw.MessageBoxButton.OKCancel;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
