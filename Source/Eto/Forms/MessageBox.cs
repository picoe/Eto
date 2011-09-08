using System;

namespace Eto.Forms
{
	public enum MessageBoxButtons
	{
		OK,
		OKCancel,
		YesNo,
		YesNoCancel
	}
	
	public enum MessageBoxType
	{
		Information,
		Warning,
		Error,
		Question
	}

	public interface IMessageBox : IWidget
	{
		string Text { get; set; }
		string Caption { get; set; }
		MessageBoxType Type { get; set; }
		DialogResult ShowDialog(Control parent);
		DialogResult ShowDialog(Control parent, MessageBoxButtons buttons);
	}
	
	public static class MessageBox
	{
		public static DialogResult Show (string text, MessageBoxType type = MessageBoxType.Information)
		{
			return Show (null, text, null, type);
		}
		
		public static DialogResult Show (string text, string caption, MessageBoxType type = MessageBoxType.Information)
		{
			return Show (null, text, caption, type);
		}

		public static DialogResult Show(Control parent, string text, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(parent != null ? parent.Generator : Generator.Current, parent, text, null, type);
		}
		
		public static DialogResult Show(Control parent, string text, string caption, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(parent != null ? parent.Generator : Generator.Current, parent, text, caption, type);
		}
		
		public static DialogResult Show(Generator g, Control parent, string text, string caption = null, MessageBoxType type = MessageBoxType.Information)
		{
			var mb = g.CreateControl<IMessageBox>();
			mb.Text = text;
			mb.Caption = caption;
			mb.Type = type;
			return mb.ShowDialog(parent);
		}

		public static DialogResult Show (string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			return Show ((Control)null, text, buttons, type);
		}

		public static DialogResult Show (string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			return Show ((Control)null, text, caption, buttons, type);
		}

		public static DialogResult Show (Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			return Show (parent != null ? parent.Generator : Generator.Current, parent, text, null, buttons, type);
		}

		public static DialogResult Show (Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			return Show (parent != null ? parent.Generator : Generator.Current, parent, text, caption, buttons, type);
		}

		public static DialogResult Show (Generator g, Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			return Show (g, parent, text, null, buttons, type);
		}

		public static DialogResult Show (Generator g, Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information)
		{
			var mb = g.CreateControl<IMessageBox> ();
			mb.Text = text;
			mb.Caption = caption;
			mb.Type = type;
			return mb.ShowDialog (parent, buttons);
		}
	}
}
