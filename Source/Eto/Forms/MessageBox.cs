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
		DialogResult ShowDialog(Control parent);
		DialogResult ShowDialog(Control parent, MessageBoxButtons buttons);
	}
	
	public static class MessageBox
	{
		public static DialogResult Show(Control parent, string text)
		{
			return Show(Generator.Current, parent, text);
		}
		
		public static DialogResult Show(Generator g, Control parent, string text)
		{
			var mb = g.CreateControl<IMessageBox>();
			mb.Text = text;
			return mb.ShowDialog(parent);
		}

		public static DialogResult Show(Control parent, string text, MessageBoxButtons buttons)
		{
			return Show(Generator.Current, parent, text, buttons);
		}

		public static DialogResult Show(Generator g, Control parent, string text, MessageBoxButtons buttons)
		{
			var mb = g.CreateControl<IMessageBox>();
			mb.Text = text;
			return mb.ShowDialog(parent, buttons);
		}
	}
}
