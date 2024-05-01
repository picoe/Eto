using Eto.WinForms.CustomControls;
namespace Eto.WinForms.Forms.Controls
{
	public class EtoSearchTextBox : EtoTextBox
	{
		private readonly swf.PictureBox searchImage;

		private readonly swf.Button clearSearchButton;

		public EtoSearchTextBox()
		{
			clearSearchButton = new swf.Button
			{
				Dock = swf.DockStyle.Right,
				Size = new sd.Size(16, 16),
				TabStop = false,
				FlatStyle = swf.FlatStyle.Flat,
				Cursor = swf.Cursors.Arrow,
				ImageAlign = sd.ContentAlignment.MiddleCenter,
				Image = Resources.Clear
			};
			clearSearchButton.FlatAppearance.BorderSize = 0;
			clearSearchButton.Click += Clear_Click;

			searchImage = new swf.PictureBox
			{
				Dock = swf.DockStyle.Left,
				Size = new sd.Size(16, 16),
				TabIndex = 0,
				SizeMode = swf.PictureBoxSizeMode.CenterImage,
				Image = Resources.Search
			};


			Controls.Add(clearSearchButton);
			Controls.Add(searchImage);

			UpdateClearButton();
		}


		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetRounded();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			SetRounded();
		}

		private void SetRounded()
		{
			Region = sd.Region.FromHrgn(Win32.CreateRoundRectRgn(1, 1, Width, Height, Height * 2 / 3, Height * 2 / 3));
			Win32.SendMessage(Handle, Win32.WM.EM_SETMARGINS, (IntPtr)3, (IntPtr)((16 << 16) + 16));
		}

		private void Clear_Click(object sender, EventArgs e)
		{
			Text = string.Empty;
			Focus();
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			UpdateClearButton();
		}

		private void UpdateClearButton()
		{
			var showClearButton = !string.IsNullOrEmpty(Text);
			if (clearSearchButton.Visible != showClearButton)
			{
				clearSearchButton.Visible = showClearButton;
			}
		}

		public sd.Image SearchImage
		{
			set => searchImage.Image = value;
			get => searchImage.Image;
		}

		public sd.Image CancelSearchImage
		{
			set => clearSearchButton.Image = value;
			get => clearSearchButton.Image;
		}
	}

	public class SearchBoxHandler : TextBoxHandler<EtoSearchTextBox, TextBox, TextBox.ICallback>, SearchBox.IHandler
	{
		public override swf.TextBox SwfTextBox => Control;

		public override EtoTextBox EtoTextBox => Control;

		public SearchBoxHandler()
		{
			Control = new EtoSearchTextBox();
		}
	}
}
