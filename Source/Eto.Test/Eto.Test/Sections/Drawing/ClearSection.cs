using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Drawing
{
	public class ClearSection : Scrollable, INotifyPropertyChanged
	{
		bool useClearColor;
		bool useGraphicsPathClip;

		public bool UseClearColor
		{
			get { return useClearColor; }
			set
			{
				useClearColor = value;
				OnPropertyChanged(new PropertyChangedEventArgs("UseClearColor"));
			}
		}

		public bool UseGraphicsPathClip
		{
			get { return useGraphicsPathClip; }
			set
			{
				useGraphicsPathClip = value;
				OnPropertyChanged(new PropertyChangedEventArgs("UseGraphicsPathClip"));
			}
		}

		public ClearSection()
		{
			var layout = new DynamicLayout();
			layout.AddSeparateRow(null, UseClearColorControl(), UseGraphicsPathClipControl(), null);
			layout.BeginVertical();
			layout.AddRow(new Label { Text = "Drawable" }, ClearGraphicsTest(), null);
			layout.AddRow(new Label { Text = "Bitmap (with yellow background)" }, ClearBitmapTest(), null);
			layout.EndVertical();
			layout.Add(null);

			Content = layout;
		}

		Control UseClearColorControl()
		{
			var control = new CheckBox { Text = "Use Red Clear Color at 0.5 alpha" };
			control.Bind(r => r.Checked, this, r => r.UseClearColor);
			return control;
		}

		Control UseGraphicsPathClipControl()
		{
			var control = new CheckBox { Text = "Use graphics path clip" };
			control.Bind(r => r.Checked, this, r => r.UseGraphicsPathClip);
			return control;
		}

		Control ClearGraphicsTest()
		{
			var control = new Drawable
			{
				Size = new Size (200, 200),
				BackgroundColor = Colors.Yellow
			};
			control.Paint += (sender, e) => DrawSample(e.Graphics);
			PropertyChanged += (sender, e) => control.Invalidate();
			return control;
		}

		void DrawSample(Graphics graphics)
		{
			using (graphics.Platform.Context)
			{
				graphics.FillRectangle(Brushes.Green, 0, 0, 200, 200);
				if (UseGraphicsPathClip)
				{
					var path = GraphicsPath.GetRoundRect(new RectangleF(10, 10, 180, 180), 20);
					graphics.SetClip(path);
				}
				else
					graphics.SetClip(new RectangleF(10, 10, 180, 180));

				if (UseClearColor)
					graphics.Clear(new SolidBrush(new Color(Colors.Red, 0.5f)));
				else
					graphics.Clear();
				graphics.FillEllipse(Brushes.Blue, 25, 25, 150, 150);
			}
		}

		Image CreateImage()
		{
			var image = new Bitmap(200, 200, PixelFormat.Format32bppRgba);
			using (var graphics = new Graphics(image))
			{
				DrawSample(graphics);
			}
			return image;
		}

		Control ClearBitmapTest()
		{
			var control = new DrawableImageView
			{
				Image = CreateImage (),
				Size = new Size (200, 200),
				BackgroundColor = Colors.Yellow
			};

			PropertyChanged += (sender, e) => control.Image = CreateImage();

			return control;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
