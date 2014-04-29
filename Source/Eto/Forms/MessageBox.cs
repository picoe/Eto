
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

	public enum MessageBoxDefaultButton
	{
		Default,
		OK,
		Yes = OK,
		No,
		Cancel
	}

	public interface IMessageBox : IWidget
	{
		string Text { get; set; }
		string Caption { get; set; }
		MessageBoxType Type { get; set; }
		MessageBoxButtons Buttons { get; set; }
		MessageBoxDefaultButton DefaultButton { get; set; }
		DialogResult ShowDialog(Control parent);
	}

	public static class MessageBox
	{
		public static DialogResult Show(string text, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(null, text, null, type);
		}

		public static DialogResult Show(string text, string caption, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(null, text, caption, type);
		}

		public static DialogResult Show(Control parent, string text, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(parent != null ? parent.Platform : null, parent, text, null, type);
		}

		public static DialogResult Show(Control parent, string text, string caption, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(parent != null ? parent.Platform : null, parent, text, caption, type);
		}

		public static DialogResult Show(Generator generator, Control parent, string text, string caption = null, MessageBoxType type = MessageBoxType.Information)
		{
			return Show(generator, parent, text, caption, MessageBoxButtons.OK, type);
		}

		public static DialogResult Show(string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			return Show((Control)null, text, buttons, type, defaultButton);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			return Show((Control)null, text, caption, buttons, type, defaultButton);
		}

		public static DialogResult Show(Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			return Show(parent != null ? parent.Platform : null, parent, text, null, buttons, type, defaultButton);
		}

		public static DialogResult Show(Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			return Show(parent != null ? parent.Platform : null, parent, text, caption, buttons, type, defaultButton);
		}

		public static DialogResult Show(Generator generator, Control parent, string text, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			return Show(generator, parent, text, null, buttons, type, defaultButton);
		}

		public static DialogResult Show(Generator generator, Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxType type = MessageBoxType.Information, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Default)
		{
			var mb = (generator ?? Platform.Instance).Create<IMessageBox>();
			mb.Text = text;
			mb.Caption = caption;
			mb.Type = type;
			mb.Buttons = buttons;
			mb.DefaultButton = defaultButton;
			return mb.ShowDialog(parent);
		}
	}
}
