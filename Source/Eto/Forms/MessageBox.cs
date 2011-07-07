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

	public interface IMessageBox : IWidget
	{
		string Text { get; set; }
		string Caption { get; set; }
		DialogResult ShowDialog(Control parent);
		DialogResult ShowDialog(Control parent, MessageBoxButtons buttons);
	}
	
	public static class MessageBox
	{
		public static DialogResult Show (string text, string caption = null)
		{
			return Show (null, text, caption);
		}

		public static DialogResult Show(Control parent, string text, string caption = null)
		{
			return Show(Generator.Current, parent, text, caption);
		}
		
		public static DialogResult Show(Generator g, Control parent, string text, string caption = null)
		{
			var mb = g.CreateControl<IMessageBox>();
			mb.Text = text;
			mb.Caption = caption;
			return mb.ShowDialog(parent);
		}

		public static DialogResult Show (string text, MessageBoxButtons buttons)
		{
			return Show ((Control)null, text, buttons);
		}

		public static DialogResult Show (string text, string caption, MessageBoxButtons buttons)
		{
			return Show ((Control)null, text, caption, buttons);
		}

		public static DialogResult Show (Control parent, string text, MessageBoxButtons buttons)
		{
			return Show (Generator.Current, parent, text, null, buttons);
		}

		public static DialogResult Show (Control parent, string text, string caption, MessageBoxButtons buttons)
		{
			return Show (Generator.Current, parent, text, caption, buttons);
		}

		public static DialogResult Show (Generator g, Control parent, string text, MessageBoxButtons buttons)
		{
			return Show (g, parent, text, null, buttons);
		}

		public static DialogResult Show (Generator g, Control parent, string text, string caption, MessageBoxButtons buttons)
		{
			var mb = g.CreateControl<IMessageBox> ();
			mb.Text = text;
			mb.Caption = caption;
			return mb.ShowDialog (parent, buttons);
		}
	}
}
