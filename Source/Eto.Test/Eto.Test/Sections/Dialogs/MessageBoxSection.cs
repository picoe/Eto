using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Dialogs
{
	public class MessageBoxSection : Scrollable
	{
		public string MessageBoxText { get; set; }

		public string MessageBoxCaption { get; set; }

		public MessageBoxType MessageBoxType { get; set; }

		public MessageBoxButtons MessageBoxButtons { get; set; }

		public MessageBoxDefaultButton MessageBoxDefaultButton { get; set; }

		public bool AttachToParent { get; set; }

		public MessageBoxSection()
		{
			MessageBoxText = "Some message";
			MessageBoxCaption = "Some caption";
			AttachToParent = true;

			var layout = new DynamicLayout();

			layout.AddSeparateRow(null, new Label { Text = "Caption" }, CaptionBox(), null);
			layout.AddSeparateRow(null, new Label { Text = "Text" }, TitleBox(), null);

			layout.BeginVertical(Padding.Empty);

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(new Label { Text = "Type", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Right });
			layout.Add(MessageBoxTypeCombo());
			layout.Add(AttachToParentCheckBox());
			layout.Add(null);
			layout.EndHorizontal();

			layout.EndBeginVertical();
			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(new Label { Text = "Buttons", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Right });
			layout.Add(MessageBoxButtonsCombo());
			layout.Add(new Label { Text = "Default Button", VerticalAlign = VerticalAlign.Middle, HorizontalAlign = HorizontalAlign.Right });
			layout.Add(MessageBoxDefaultButtonCombo());
			layout.Add(null);
			layout.EndHorizontal();

			layout.EndVertical();

			layout.AddSeparateRow(null, ShowDialogButton(), null);
			layout.Add(null);

			Content = layout;
		}

		Control CaptionBox()
		{
			var control = new TextBox { Size = new Size(300, -1) };
			var binding = new ObjectBinding<MessageBoxSection, string>(this, r => r.MessageBoxCaption, (r, val) => r.MessageBoxCaption = val);
			control.TextBinding.Bind(binding);
			return control;
		}

		Control TitleBox()
		{
			var control = new TextArea { Size = new Size(300, -1) };
			var binding = new ObjectBinding<MessageBoxSection, string>(this, r => r.MessageBoxText, (r, val) => r.MessageBoxText = val);
			control.TextBinding.Bind(binding);
			return control;
		}

		Control MessageBoxTypeCombo()
		{
			var control = new EnumComboBox<MessageBoxType>();
			var binding = new ObjectBinding<MessageBoxSection, MessageBoxType>(this, r => r.MessageBoxType, (r, val) => r.MessageBoxType = val);
			control.SelectedValueBinding.Bind(binding);
			return control;
		}

		Control MessageBoxButtonsCombo()
		{
			var control = new EnumComboBox<MessageBoxButtons>();
			var binding = new ObjectBinding<MessageBoxSection, MessageBoxButtons>(this, r => r.MessageBoxButtons, (r, val) => r.MessageBoxButtons = val);
			control.SelectedValueBinding.Bind(binding);
			return control;
		}

		Control MessageBoxDefaultButtonCombo()
		{
			var control = new EnumComboBox<MessageBoxDefaultButton>();
			var binding = new ObjectBinding<MessageBoxSection, MessageBoxDefaultButton>(this, r => r.MessageBoxDefaultButton, (r, val) => r.MessageBoxDefaultButton = val);
			control.SelectedValueBinding.Bind(binding);
			return control;
		}

		Control AttachToParentCheckBox()
		{
			var control = new CheckBox { Text = "Attach to Parent Window" };
			var binding = new ObjectBinding<MessageBoxSection, bool>(this, r => r.AttachToParent, (r, val) => r.AttachToParent = val);
			control.CheckedBinding.Bind(binding);
			return control;
		}

		Control ShowDialogButton()
		{
			var control = new Button { Text = "Show Dialog" };
			control.Click += (sender, e) =>
			{
				var caption = string.IsNullOrEmpty(MessageBoxCaption) ? null : MessageBoxCaption;
				DialogResult result;
				if (AttachToParent)
					result = MessageBox.Show(this, text: MessageBoxText, caption: caption, type: MessageBoxType, buttons: MessageBoxButtons, defaultButton: MessageBoxDefaultButton);
				else
					result = MessageBox.Show(text: MessageBoxText, caption: caption, type: MessageBoxType, buttons: MessageBoxButtons, defaultButton: MessageBoxDefaultButton);
				Log.Write(this, "MessageBox Result: {0}", result);
			};
			return control;
		}
	}
}

