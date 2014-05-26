using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// Test for different brushes and options
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BrushSection : Scrollable
	{
		readonly Image image = TestIcons.TestImage;
		readonly Drawable drawable;
		Brush brush;
		readonly LinearGradientBrush gradientBrush;
		readonly Brush textureBrush;
		readonly Brush solidBrush;
		readonly DynamicRow matrixRow;
		readonly DynamicRow gradientRow;
		bool useBackgroundColor;

		public float Rotation { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float OffsetX { get; set; }

		public float OffsetY { get; set; }

		public bool UseBackgroundColor
		{
			get { return useBackgroundColor; }
			set
			{
				useBackgroundColor = value;
				drawable.BackgroundColor = value ? Colors.Blue : Colors.Transparent;
			}
		}

		public GradientWrapMode GradientWrap { get; set; }

		public BrushSection()
		{
			var layout = new DynamicLayout();
			brush = solidBrush = Brushes.LightSkyBlue;
			gradientBrush = new LinearGradientBrush(Colors.AliceBlue, Colors.Black, new PointF(0, 0), new PointF(100f, 100f));
			//gradientBrush = new LinearGradientBrush (new RectangleF (0, 0, 50, 50), Colors.AliceBlue, Colors.Black, 10);
			gradientBrush.Wrap = GradientWrapMode.Repeat;
			textureBrush = new TextureBrush(image, 0.5f);
			brush = textureBrush;

			ScaleX = 100f;
			ScaleY = 100f;

			drawable = new Drawable { Size = new Size(300, 200) };

			drawable.Paint += (sender, pe) => Draw(pe.Graphics);

			layout.AddSeparateRow(null, BrushControl(), UseBackgroundColorControl(), null);
			if (Platform.Supports<NumericUpDown>())
			{
				matrixRow = layout.AddSeparateRow(null, new Label { Text = "Rot" }, RotationControl(), new Label { Text = "Sx" }, ScaleXControl(), new Label { Text = "Sy" }, ScaleYControl(), new Label { Text = "Ox" }, OffsetXControl(), new Label { Text = "Oy" }, OffsetYControl(), null);
				matrixRow.Table.Visible = false;
			}
			gradientRow = layout.AddSeparateRow(null, GradientWrapControl(), null);
			gradientRow.Table.Visible = false;
			layout.AddSeparateRow(null, drawable, null);
			layout.Add(null);

			this.Content = layout;
		}

		class BrushItem : ListItem
		{
			public Brush Brush { get; set; }

			public bool SupportsMatrix { get; set; }

			public bool SupportsGradient { get; set; }
		}

		Control BrushControl()
		{
			var control = new ComboBox();
			control.Items.Add(new BrushItem { Text = "Solid", Brush = solidBrush });
			control.Items.Add(new BrushItem { Text = "Texture", Brush = textureBrush, SupportsMatrix = true });
			control.Items.Add(new BrushItem { Text = "Gradient", Brush = gradientBrush, SupportsMatrix = true, SupportsGradient = true });
			control.SelectedValue = control.Items.OfType<BrushItem>().First(r => r.Brush == brush);
			control.SelectedValueChanged += (sender, e) =>
			{
				var item = (BrushItem)control.SelectedValue;
				if (item != null)
					SetItem(item);
			};
			LoadComplete += (sender, e) => SetItem(control.SelectedValue as BrushItem);
			control.SelectedValueChanged += (sender, e) => Refresh();
			return control;
		}

		void SetItem(BrushItem item)
		{
			brush = item.Brush;
			if (matrixRow != null)
				matrixRow.Table.Visible = item.SupportsMatrix;
			gradientRow.Table.Visible = item.SupportsGradient;
		}

		Control ScaleXControl()
		{
			var control = new NumericUpDown { MinValue = 1, MaxValue = 1000 };
			control.ValueBinding.Bind(() => ScaleX, v =>
			{
				ScaleX = (float)v;
				Refresh();
			});
			return control;
		}

		Control ScaleYControl()
		{
			var control = new NumericUpDown { MinValue = 1, MaxValue = 1000 };
			control.ValueBinding.Bind(() => ScaleY, v =>
			{
				ScaleY = (float)v;
				Refresh();
			});
			return control;
		}

		Control RotationControl()
		{
			var control = new NumericUpDown { MinValue = 0, MaxValue = 360 };
			control.ValueBinding.Bind(() => Rotation, v =>
			{
				Rotation = (float)v;
				Refresh();
			});
			return control;
		}

		Control OffsetXControl()
		{
			var control = new NumericUpDown();
			control.ValueBinding.Bind(() => OffsetX, v =>
			{
				OffsetX = (float)v;
				Refresh();
			});
			return control;
		}

		Control OffsetYControl()
		{
			var control = new NumericUpDown();
			control.ValueBinding.Bind(() => OffsetY, v =>
			{
				OffsetY = (float)v;
				Refresh();
			});
			return control;
		}

		Control GradientWrapControl()
		{
			var control = new EnumComboBox<GradientWrapMode>();
			control.SelectedValueBinding.Bind(() => GradientWrap, v =>
			{
				GradientWrap = v;
				Refresh();
			});
			return control;
		}

		Control UseBackgroundColorControl()
		{
			var control = new CheckBox { Text = "Use Background Color" };
			control.CheckedBinding.Bind(() => UseBackgroundColor, v => UseBackgroundColor = v ?? false);
			return control;
		}

		void Refresh()
		{
			drawable.Invalidate();
		}

		void Draw(Graphics g)
		{
			var matrix = Matrix.Create();
			matrix.Translate(OffsetX, OffsetY);
			matrix.Scale(Math.Max(ScaleX / 100f, 0.1f), Math.Max(ScaleY / 100f, 0.1f));
			matrix.Rotate(Rotation);
			var tb = brush as ITransformBrush;
			if (tb != null)
			{
				tb.Transform = matrix;
			}
			var gb = brush as LinearGradientBrush;
			if (gb != null)
			{
				gb.Wrap = GradientWrap;
			}

			var rect = new RectangleF(0, 0, 200, 100);
			g.FillEllipse(brush, rect);
			g.DrawEllipse(Colors.Black, rect);
			
			rect = new RectangleF(0, 110, 200, 80);
			g.FillRectangle(brush, rect);
			g.DrawRectangle(Colors.Black, rect);
		}
	}
}

