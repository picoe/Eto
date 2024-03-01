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
			dlg.Title = Caption ?? parent?.ParentWindow?.Title ?? Application.Instance.Localize(Widget, "Error");
			dlg.Text = Text;
			// todo: add ability to get icons needed from SystemIcons
			dlg.Image = GetImage();

			var defaultButton = DefaultButton;
			if (defaultButton == MessageBoxDefaultButton.Default)
			{
				switch (Buttons)
				{
					case MessageBoxButtons.OK:
						defaultButton = MessageBoxDefaultButton.OK;
						break;
					case MessageBoxButtons.OKCancel:
						defaultButton = MessageBoxDefaultButton.Cancel;
						break;
					case MessageBoxButtons.YesNo:
						defaultButton = MessageBoxDefaultButton.No;
						break;
					case MessageBoxButtons.YesNoCancel:
						defaultButton = MessageBoxDefaultButton.Cancel;
						break;
					default:
						throw new NotSupportedException();
				}
			}


			var app = Application.Instance;
			switch (Buttons)
			{
				case MessageBoxButtons.OK:
					dlg.AddButton(app.Localize(Widget, "OK"), DialogResult.Ok, defaultButton == MessageBoxDefaultButton.OK, defaultButton == MessageBoxDefaultButton.OK);
					break;
				case MessageBoxButtons.OKCancel:
					dlg.AddButton(app.Localize(Widget, "OK"), DialogResult.Ok, defaultButton == MessageBoxDefaultButton.OK);
					dlg.AddButton(app.Localize(Widget, "Cancel"), DialogResult.Cancel, defaultButton == MessageBoxDefaultButton.Cancel, true);
					break;
				case MessageBoxButtons.YesNo:
					dlg.AddButton(app.Localize(Widget, "&Yes"), DialogResult.Yes, defaultButton == MessageBoxDefaultButton.Yes);
					dlg.AddButton(app.Localize(Widget, "&No"), DialogResult.No, defaultButton == MessageBoxDefaultButton.No);
					break;
				case MessageBoxButtons.YesNoCancel:
					dlg.AddButton(app.Localize(Widget, "Cancel"), DialogResult.Cancel, defaultButton == MessageBoxDefaultButton.Cancel, true);
					dlg.AddButton(app.Localize(Widget, "&No"), DialogResult.No, defaultButton == MessageBoxDefaultButton.No);
					dlg.AddButton(app.Localize(Widget, "&Yes"), DialogResult.Yes, defaultButton == MessageBoxDefaultButton.Yes);
					break;
			}

			dlg.ShowModal(parent);

			return dlg.Result as DialogResult? ?? DialogResult.Cancel;
		}

		private Image GetImage()
		{
			SystemIconType icon = SystemIconType.Error;
			switch (Type)
			{
				case MessageBoxType.Information:
					icon = SystemIconType.Information;
					break;
				case MessageBoxType.Warning:
					icon = SystemIconType.Warning;
					break;
				case MessageBoxType.Error:
					icon = SystemIconType.Error;
					break;
				case MessageBoxType.Question:
					icon = SystemIconType.Question;
					break;

			}
			return SystemIcons.Get(icon, SystemIconSize.Large)?.WithSize(32, 32);
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
			ShowInTaskbar = false;
			Resizable = false;
			Closeable = false;

			textLabel.VerticalAlignment = VerticalAlignment.Center;
			var layout = new DynamicLayout();
			layout.Padding = new Padding(22, 28);
			layout.DefaultSpacing = new Size(8, 8);

			layout.AddRow(new TableLayout(image, null), textLabel);

			Content = layout;


			HandleEvent(KeyDownEvent);
		}

		/// <inheritdoc/>
		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			CalculateSize();
		}

		private void CalculateSize()
		{
			const int minWidth = 340;
			const float maxRatio = 1000f;
			const float idealRatio = 2f;
			const int increment = 2;
			var screen = Screen ?? Screen.PrimaryScreen;
			var workingArea = screen.WorkingArea;
			SizeF? lastGoodSize = null;
			SizeF available = new SizeF(workingArea.Width * .7f, float.PositiveInfinity);
			SizeF size;
			do
			{
				size = textLabel.GetPreferredSize(available);
				var ratio = size.Width / size.Height;
				if (ratio <= maxRatio && size.Width >= minWidth)
				{
					lastGoodSize = size;
					// we're at an ideal ratio of width to height, let's use it.
					if (ratio <= idealRatio)
						break;
				}
				available.Width = (int)Math.Floor(Math.Min(available.Width, size.Width) - increment);
			} while (available.Width >= minWidth + increment);

			if (lastGoodSize != null)
				textLabel.Size = (Size)lastGoodSize.Value;
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
			if (isAbort && !isDefault)
				NegativeButtons.Add(button);
			else
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
			set
			{
				textLabel.Text = value;
			}
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
