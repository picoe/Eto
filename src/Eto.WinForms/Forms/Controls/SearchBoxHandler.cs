using Eto.WinForms.CustomControls;
namespace Eto.WinForms.Forms.Controls
{
	public class EtoSearchTextBox : EtoTextBox
	{
		private readonly swf.PictureBox searchImage;

		private readonly swf.Button clearSearchButton;

		public EtoSearchTextBox()
		{
			var scaledSize = this.LogicalToDeviceUnits(new sd.Size(16, 16));

			clearSearchButton = new swf.Button
			{
				Dock = swf.DockStyle.Right,
				Size = scaledSize,
				TabStop = false,
				FlatStyle = swf.FlatStyle.Flat,
				Cursor = swf.Cursors.Arrow,
				ImageAlign = sd.ContentAlignment.MiddleCenter,
				Image = CreateClearImage(scaledSize.Width)
			};
			clearSearchButton.FlatAppearance.BorderSize = 0;
			clearSearchButton.Click += Clear_Click;

			searchImage = new swf.PictureBox
			{
				Dock = swf.DockStyle.Left,
				Size = scaledSize,
				TabIndex = 0,
				SizeMode = swf.PictureBoxSizeMode.CenterImage,
				Image = CreateSearchImage(scaledSize.Width)
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
			Win32.SendMessage(Handle, Win32.WM.EM_SETMARGINS, (IntPtr)3, (IntPtr)((this.LogicalToDeviceUnits(16) << 16) + this.LogicalToDeviceUnits(16)));
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

		private static sd.Bitmap CreateSearchImage(int size)
		{
			var bmp = new sd.Bitmap(size, size, sdi.PixelFormat.Format32bppArgb);
			using (var g = sd.Graphics.FromImage(bmp))
			{
				g.SmoothingMode = sd2.SmoothingMode.AntiAlias;

				var scale = size / 16f;
				var penWidth = Math.Max(1f, 1.5f * scale);
				using (var pen = new sd.Pen(sd.Color.FromArgb(136, 136, 136), penWidth))
				{
					// magnifying glass lens
					var circleSize = 7.5f * scale;
					var circleOffset = 2.5f * scale;
					g.DrawEllipse(pen, circleOffset, circleOffset, circleSize, circleSize);

					// handle
					pen.StartCap = sd2.LineCap.Round;
					pen.EndCap = sd2.LineCap.Round;
					g.DrawLine(pen, 9.5f * scale, 9.5f * scale, 13f * scale, 13f * scale);
				}
			}
			return bmp;
		}

		private static sd.Bitmap CreateClearImage(int size)
		{
			var bmp = new sd.Bitmap(size, size, sdi.PixelFormat.Format32bppArgb);
			using (var g = sd.Graphics.FromImage(bmp))
			{
				g.SmoothingMode = sd2.SmoothingMode.AntiAlias;

				var scale = size / 16f;
				var penWidth = Math.Max(1f, 1.5f * scale);
				using (var pen = new sd.Pen(sd.Color.FromArgb(136, 136, 136), penWidth))
				{
					pen.StartCap = sd2.LineCap.Round;
					pen.EndCap = sd2.LineCap.Round;

					// X shape
					var margin = 4f * scale;
					var end = size - margin;
					g.DrawLine(pen, margin, margin, end, end);
					g.DrawLine(pen, end, margin, margin, end);
				}
			}
			return bmp;
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
