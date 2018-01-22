using Eto.Drawing;
using Eto.Forms;
using System;
using System.ComponentModel;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Clear")]
	public class ClearSection : Scrollable, INotifyPropertyChanged
	{
		bool useClearColor;
		ClearClipMode clipMode;

		public enum ClearClipMode
		{
			None,
			GraphicsPath,
			Rectangle
		}

		public bool UseClearColor
		{
			get { return useClearColor; }
			set
			{
				useClearColor = value;
				OnPropertyChanged(new PropertyChangedEventArgs("UseClearColor"));
			}
		}

		public ClearClipMode ClipMode
		{
			get { return clipMode; }
			set
			{
				clipMode = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ClipMode"));
			}
		}

		public ClearSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
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
			control.CheckedBinding.Bind(() => UseClearColor, v => UseClearColor = v ?? false);
			return control;
		}

		Control UseGraphicsPathClipControl()
		{
			var control = new EnumDropDown<ClearClipMode>();
			control.SelectedValueBinding.Bind(this, r => r.ClipMode);
			return new StackLayout { Orientation = Orientation.Horizontal, Items = { "Clip Mode:", control } };
		}

		Control ClearGraphicsTest()
		{
			var control = new Drawable
			{
				Size = new Size(200, 200),
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
				switch (clipMode)
				{
					case ClearClipMode.GraphicsPath:
						var path = GraphicsPath.GetRoundRect(new RectangleF(10, 10, 180, 180), 20);
						graphics.SetClip(path);
						break;
					case ClearClipMode.Rectangle:
						graphics.SetClip(new RectangleF(10, 10, 180, 180));
						break;
				}

				if (UseClearColor)
					graphics.Clear(new SolidBrush(new Color(Colors.Red, 0.5f)));
				else
					graphics.Clear();
				var rnd = new Random();
				graphics.FillEllipse(Brushes.Blue, rnd.Next(50), rnd.Next(50), 150, 150);
			}
		}

		Bitmap image;

		Image CreateImage()
		{
			if (image == null)
				image = new Bitmap(200, 200, PixelFormat.Format32bppRgba);
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
				Image = CreateImage(),
				Size = new Size(200, 200),
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
