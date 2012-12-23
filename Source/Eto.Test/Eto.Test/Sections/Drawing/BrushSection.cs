using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class BrushSection : Scrollable
	{
		Image image = Bitmap.FromResource ("Eto.Test.TestImage.png");
		Drawable drawable;
		IBrush brush;
		DynamicRow matrixRow;

		public float Rotation { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float OffsetX { get; set; }

		public float OffsetY { get; set; }

		public BrushSection ()
		{
			var layout = new DynamicLayout (this);
			brush = Brushes.Black ();
			ScaleX = 1f;
			ScaleY = 1f;

			drawable = new Drawable { Size = new Size (600, 200) };

			drawable.Paint += (sender, pe) => {
				Draw (pe.Graphics);
			};

			layout.AddSeparateRow (null, BrushControl (), null);
			matrixRow = layout.AddSeparateRow (null, new Label { Text = "Rot" }, RotationControl (), new Label { Text = "Sx"}, ScaleXControl (), new Label { Text = "Sy"}, ScaleYControl (), new Label { Text = "Ox"}, OffsetXControl (), new Label { Text = "Oy"}, OffsetYControl (), null);
			matrixRow.Table.Visible = false;
			layout.AddRow (drawable);
			layout.Add (null);
		}

		class BrushItem : ListItem
		{
			public IBrush Brush { get; set; }

			public bool SupportsMatrix { get; set; }
		}

		Control BrushControl ()
		{
			var control = new ComboBox ();
			control.Items.Add (new BrushItem { Text = "Solid", Brush = Brushes.Black () });
			control.Items.Add (new BrushItem { Text = "Texture", Brush = TextureBrush.Create (image), SupportsMatrix = true });
			control.SelectedIndex = 0;
			control.SelectedValueChanged += (sender, e) => {
				var item = (BrushItem)control.SelectedValue;
				this.brush = item.Brush;
				matrixRow.Table.Visible = item.SupportsMatrix;

			};
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control ScaleXControl ()
		{
			var control = new NumericUpDown { MinValue = 0, MaxValue = 20 };
			control.Bind (c => c.Value, this, c => c.ScaleX);
			control.ValueChanged += Refresh;
			return control;
		}
		
		Control ScaleYControl ()
		{
			var control = new NumericUpDown { MinValue = 0, MaxValue = 20 };
			control.Bind (c => c.Value, this, c => c.ScaleY);
			control.ValueChanged += Refresh;
			return control;
		}
		
		Control RotationControl ()
		{
			var control = new NumericUpDown { MinValue = 0, MaxValue = 360 };
			control.Bind (c => c.Value, this, c => c.Rotation);
			control.ValueChanged += Refresh;
			return control;
		}
		
		Control OffsetXControl ()
		{
			var control = new NumericUpDown { };
			control.Bind (c => c.Value, this, c => c.OffsetX);
			control.ValueChanged += Refresh;
			return control;
		}
		
		Control OffsetYControl ()
		{
			var control = new NumericUpDown { };
			control.Bind (c => c.Value, this, c => c.OffsetY);
			control.ValueChanged += Refresh;
			return control;
		}
		
		void Refresh (object sender, EventArgs e)
		{
			drawable.Invalidate ();
		}

		void Draw (Graphics g)
		{
			var matrix = Matrix.Create ();
			matrix.Translate (OffsetX, OffsetY);
			matrix.Scale (ScaleX, ScaleY);
			matrix.Rotate (Rotation);
			var textureBrush = brush as ITextureBrush;
			if (textureBrush != null) {
				textureBrush.Transform = matrix;
			}

			var rect = new RectangleF (0, 0, 200, 100);
			g.FillEllipse (brush, rect);
			g.DrawEllipse (Colors.Black, rect);
			
			rect = new RectangleF(0, 110, 200, 80);
			g.FillRectangle (brush, rect);
			g.DrawRectangle (Colors.Black, rect);
		}
	}
}

