using System;
using System.Collections.Generic;
using System.Text;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed message box handler to allow more customization and theming
	/// </summary>
	public class ThemedMessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
	{
		/// <inheritdoc />
		public string Text { get; set; }
		/// <inheritdoc />
		public string Caption { get; set; }
		/// <inheritdoc />
		public MessageBoxType Type { get; set; }
		/// <inheritdoc />
		public MessageBoxButtons Buttons { get; set; }
		/// <inheritdoc />
		public MessageBoxDefaultButton DefaultButton { get; set; }

		/// <inheritdoc />
		public DialogResult ShowDialog(Control parent)
		{
			var dlg = new ThemedMessageBox();
			dlg.Title = Caption;
			dlg.Text = Text;
			// todo: add ability to get icons needed from SystemIcons
			//dlg.Image = GetImage();

			var app = Application.Instance;
			switch (Buttons)
			{
				case MessageBoxButtons.OK:
					dlg.AddButton(app.Localize(Widget, "OK"), DialogResult.Ok, DefaultButton == MessageBoxDefaultButton.OK);
					break;
				case MessageBoxButtons.OKCancel:
					dlg.AddButton(app.Localize(Widget, "OK"), DialogResult.Ok, DefaultButton == MessageBoxDefaultButton.OK);
					dlg.AddButton(app.Localize(Widget, "Cancel"), DialogResult.Cancel, DefaultButton == MessageBoxDefaultButton.Cancel, true);
					break;
				case MessageBoxButtons.YesNo:
					dlg.AddButton(app.Localize(Widget, "&Yes"), DialogResult.Yes, DefaultButton == MessageBoxDefaultButton.Yes);
					dlg.AddButton(app.Localize(Widget, "&No"), DialogResult.No, DefaultButton == MessageBoxDefaultButton.No);
					break;
				case MessageBoxButtons.YesNoCancel:
					dlg.AddButton(app.Localize(Widget, "Cancel"), DialogResult.Cancel, DefaultButton == MessageBoxDefaultButton.Cancel, true);
					dlg.AddButton(app.Localize(Widget, "&No"), DialogResult.No, DefaultButton == MessageBoxDefaultButton.No);
					dlg.AddButton(app.Localize(Widget, "&Yes"), DialogResult.Yes, DefaultButton == MessageBoxDefaultButton.Yes);
					break;
			}

			dlg.ShowModal(parent);

			return dlg.Result as DialogResult? ?? DialogResult.Cancel;
		}
	}


	/// <summary>
	/// Message box implementation 
	/// </summary>
	public class ThemedMessageBox : Dialog
	{
		readonly Label textLabel = new Label();
		readonly ImageView image = new ImageView();


		/// <summary>
		/// Gets or sets the result of the dialog
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the ThemedMessageBox class
		/// </summary>
		public ThemedMessageBox()
		{
			Closeable = false;
			ShowInTaskbar = false;
			Resizable = false;

			var layout = new DynamicLayout();
			layout.Padding = new Padding(22, 28);
			layout.DefaultSpacing = new Size(8, 8);

			layout.AddRow(TableLayout.AutoSized(image, centered: true), textLabel);

			Content = layout;

			HandleEvent(KeyDownEvent);
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.KeyData == (Application.Instance.CommonModifier | Keys.C))
			{
				var clipboard = new Clipboard();
				var divider = "---------------------------\n";
				var buttons = string.Join("   ", NegativeButtons.Concat(PositiveButtons).Select(r => r.Text.Replace("&", "")));
				clipboard.Text = $"{divider}{Title}\n{divider}{Text}\n{divider}{buttons}\n{divider}";
				e.Handled = true;
			}
		}

		/// <summary>
		/// Adds a button to the message box
		/// </summary>
		/// <param name="text"></param>
		/// <param name="result"></param>
		/// <param name="isDefault"></param>
		/// <param name="isAbort"></param>
		public void AddButton(string text, object result, bool isDefault = false, bool isAbort = false)
		{
			var button = new Button { Text = text };
			button.Click += (sender, e) =>
			{
				Result = result;
				Close();
			};
			PositiveButtons.Add(button);

			if (isDefault)
			{
				DefaultButton = button;
				button.Focus();
			}
			if (isAbort)
			{
				AbortButton = button;
				Closeable = true;
				Result = result;
			}
		}

		/// <summary>
		/// Gets or sets the text of the message box
		/// </summary>
		public string Text
		{
			get => textLabel.Text;
			set => textLabel.Text = value;
		}

		/// <summary>
		/// Gets or sets the alignment of the text
		/// </summary>
		public TextAlignment TextAlignment
		{
			get => textLabel.TextAlignment;
			set => textLabel.TextAlignment = value;
		}

		/// <summary>
		/// Gets or sets the image to show
		/// </summary>
		public Image Image
		{
			get => image.Image;
			set => image.Image = value;
		}
	}
}
