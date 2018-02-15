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
	[Section("Drawing", "Brush")]
	public class BrushSection : Scrollable
	{
		readonly Image image = TestIcons.TestImage;
		readonly Drawable drawable;
		Brush brush;
		BrushItem selectedItem;
		readonly DynamicRow matrixRow;
		readonly DynamicRow gradientRow;
		readonly DynamicRow radialRow;
		readonly DynamicRow radiusRow;
		readonly DynamicRow linearRow;
		readonly DynamicRow textureRow;
		readonly DynamicRow colorStartEndRow;
		bool useBackgroundColor;

		public enum DrawMode
		{
			Fill,
			Draw
		}

		public float Rotation { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float OffsetX { get; set; }

		public float OffsetY { get; set; }

		public SizeF Radius { get; set; }

		public PointF Center { get; set; }

		public PointF GradientOrigin { get; set; }

		public PointF StartPoint { get; set; }

		public PointF EndPoint { get; set; }

		public DrawMode Mode { get; set; }

		public float Opacity { get; set; }

		public Color StartColor { get; set; } = Colors.AliceBlue;

		public Color EndColor { get; set; } = Colors.Black;

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
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			// defaults
			ScaleX = 100f;
			ScaleY = 100f;
			Center = new PointF(110, 60);
			GradientOrigin = new PointF(150, 90);
			Radius = new SizeF(100f, 50f);
			StartPoint = new PointF(50, 50);
			EndPoint = new PointF(100, 100);
			Opacity = 1f;

			drawable = new Drawable { Size = new Size(450, 400) };

			drawable.Paint += (sender, pe) => Draw(pe.Graphics);

			if (( Platform.SupportedFeatures & PlatformFeatures.DrawableWithTransparentContent ) == 0)
				layout.AddSeparateRow(null, BrushControl(), UseBackgroundColorControl(), CreateModeControl(), null);
			else
				layout.AddSeparateRow(null, BrushControl(), UseBackgroundColorControl(), WithContent(), CreateModeControl(), null);
			if (Platform.Supports<NumericStepper>())
			{
				matrixRow = layout.AddSeparateRow(null, new Label { Text = "Rot" }, RotationControl(), new Label { Text = "Sx" }, ScaleXControl(), new Label { Text = "Sy" }, ScaleYControl(), new Label { Text = "Ox" }, OffsetXControl(), new Label { Text = "Oy" }, OffsetYControl(), null);
				matrixRow.Table.Visible = false;
			}
			gradientRow = layout.AddSeparateRow(null, GradientWrapControl(), null);
			gradientRow.Table.Visible = false;
			radialRow = layout.AddSeparateRow(null, "Center:", CenterControl(), "GradientOrigin:", GradientOriginControl(), null);
			radiusRow = layout.AddSeparateRow(null, "Radius:", RadiusControl(), null);
			linearRow = layout.AddSeparateRow(null, "Start:", StartPointControl(), "End:", EndPointControl(), null);
			textureRow = layout.AddSeparateRow(null, "Opacity:", CreateOpacityControl(), null);
			colorStartEndRow = layout.AddSeparateRow(null, "Start:", CreateStartColor(), "End:", CreateEndColor(), null);
			layout.AddSeparateRow(null, drawable, null);
			layout.Add(null);

			this.Content = layout;
		}

		class BrushItem : ListItem
		{
			public Func<Brush> CreateBrush { get; set; }

			public bool SupportsMatrix { get; set; }

			public bool SupportsGradient { get; set; }

			public bool SupportsRadial { get; set; }

			public bool SupportsLinear { get; set; }

			public bool SupportsTexture { get; set; }

			public bool SupportsStartEndColor { get; set; }
		}

		Control BrushControl()
		{
			var control = new DropDown();
			control.Items.Add(new BrushItem { Text = "Solid", CreateBrush = () => new SolidBrush(Colors.LightSkyBlue) });
			control.Items.Add(new BrushItem
			{
				Text = "Texture",
				SupportsMatrix = true,
				SupportsTexture = true,
				CreateBrush = () => new TextureBrush(image, Opacity) { Transform = GetTransform() }
			});
			control.Items.Add(new BrushItem
			{
				Text = "Linear Gradient",
				SupportsMatrix = true,
				SupportsGradient = true,
				SupportsLinear = true,
				SupportsStartEndColor = true,
				CreateBrush = () => new LinearGradientBrush(StartColor, EndColor, StartPoint, EndPoint)
				{
					Wrap = GradientWrap,
					Transform = GetTransform()
				}
			});
			control.Items.Add(new BrushItem
			{
				Text = "Radial Gradient",
				SupportsMatrix = true,
				SupportsGradient = true,
				SupportsRadial = true,
				SupportsStartEndColor = true,
				CreateBrush = () => new RadialGradientBrush(StartColor, EndColor, Center, GradientOrigin, Radius)
				{
					Wrap = GradientWrap,
					Transform = GetTransform()
				}
			});
			control.SelectedValue = control.Items.OfType<BrushItem>().First(); //r => r.Text == "Linear Gradient");
			control.SelectedValueChanged += (sender, e) => SetItem(control.SelectedValue as BrushItem);
			Load += (sender, e) => SetItem(control.SelectedValue as BrushItem);
			control.SelectedValueChanged += (sender, e) => Refresh();
			return control;
		}

		void SetItem(BrushItem item)
		{
			selectedItem = item;
			if (item == null)
				item = new BrushItem();

			SuspendLayout(); // for winforms
			if (matrixRow != null)
				matrixRow.Table.Visible = item.SupportsMatrix;
			gradientRow.Table.Visible = item.SupportsGradient;
			radialRow.Table.Visible = radiusRow.Table.Visible = item.SupportsRadial;
			linearRow.Table.Visible = item.SupportsLinear;
			textureRow.Table.Visible = item.SupportsTexture;
			colorStartEndRow.Table.Visible = item.SupportsStartEndColor;
			Refresh();
			ResumeLayout();
		}

		Control ScaleXControl()
		{
			var control = new NumericStepper { MinValue = 1, MaxValue = 1000 };
			control.ValueBinding.Bind(() => ScaleX, v =>
			{
				ScaleX = (float)v;
				Refresh();
			});
			return control;
		}

		Control ScaleYControl()
		{
			var control = new NumericStepper { MinValue = 1, MaxValue = 1000 };
			control.ValueBinding.Bind(() => ScaleY, v =>
			{
				ScaleY = (float)v;
				Refresh();
			});
			return control;
		}

		Control RotationControl()
		{
			var control = new NumericStepper { MinValue = 0, MaxValue = 360 };
			control.ValueBinding.Bind(() => Rotation, v =>
			{
				Rotation = (float)v;
				Refresh();
			});
			return control;
		}

		Control OffsetXControl()
		{
			var control = new NumericStepper();
			control.ValueBinding.Bind(() => OffsetX, v =>
			{
				OffsetX = (float)v;
				Refresh();
			});
			return control;
		}

		Control OffsetYControl()
		{
			var control = new NumericStepper();
			control.ValueBinding.Bind(() => OffsetY, v =>
			{
				OffsetY = (float)v;
				Refresh();
			});
			return control;
		}

		Control CreateModeControl()
		{
			var control = new EnumRadioButtonList<DrawMode>();
			control.SelectedValueBinding.Bind(() => Mode, v =>
			{
				Mode = v;
				Refresh();
			});
			return control;
		}

		Control CenterControl()
		{
			return PointControl(() => Center, v => Center = v);
		}

		Control GradientOriginControl()
		{
			return PointControl(() => GradientOrigin, v => GradientOrigin = v);
		}

		Control RadiusControl()
		{
			return SizeControl(() => Radius, v => Radius = v);
		}

		Control StartPointControl()
		{
			return PointControl(() => StartPoint, v => StartPoint = v);
		}

		Control EndPointControl()
		{
			return PointControl(() => EndPoint, v => EndPoint = v);
		}

		Control PointControl(Func<PointF> getValue, Action<PointF> setValue)
		{
			var xpoint = new NumericStepper();
			xpoint.ValueBinding.Bind(() => getValue().X, v =>
			{
				var p = getValue();
				p.X = (float)v;
				setValue(p);
				Refresh();
			});

			var ypoint = new NumericStepper();
			ypoint.ValueBinding.Bind(() => getValue().Y, v =>
			{
				var p = getValue();
				p.Y = (float)v;
				setValue(p);
				Refresh();
			});

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = { "X:", xpoint, "Y:", ypoint }
			};
		}

		Control SizeControl(Func<SizeF> getValue, Action<SizeF> setValue)
		{
			var xpoint = new NumericStepper();
			xpoint.ValueBinding.Bind(() => getValue().Width, v =>
			{
				var p = getValue();
				p.Width = (float)v;
				setValue(p);
				Refresh();
			});

			var ypoint = new NumericStepper();
			ypoint.ValueBinding.Bind(() => getValue().Height, v =>
			{
				var p = getValue();
				p.Height = (float)v;
				setValue(p);
				Refresh();
			});

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = { "W:", xpoint, "H:", ypoint }
			};
		}

		Control GradientWrapControl()
		{
			var control = new EnumDropDown<GradientWrapMode>();
			control.SelectedValueBinding.Bind(() => GradientWrap, v =>
			{
				GradientWrap = v;
				Refresh();
			});
			return control;
		}

		Control CreateStartColor()
		{
			var control = new ColorPicker { AllowAlpha = true };
			control.ValueBinding.Bind(() => StartColor, v =>
			{
				StartColor = v;
				Refresh();
			});
			return control;
		}

		Control CreateEndColor()
		{
			var control = new ColorPicker { AllowAlpha = true };
			control.ValueBinding.Bind(() => EndColor, v =>
			{
				EndColor = v;
				Refresh();
			});
			return control;
		}

		Control CreateOpacityControl()
		{
			var control = new NumericStepper { Value = 1, MinValue = 0, MaxValue = 1, DecimalPlaces = 2, Increment = 0.1f };
			control.ValueBinding.Bind(() => Opacity, v =>
			{
				Opacity = (float)v;
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

		Control WithContent()
		{
			var control = new CheckBox() { Text = "With Content" };
			control.CheckedChanged += (s, e) =>
			{
				drawable.Content = ( (CheckBox)s ).Checked == true ?
				TableLayout.AutoSized(
					new Label() {
						Text = "Some text",
						BackgroundColor = Colors.Transparent },
					centered: true
				).With(tl => tl.BackgroundColor = Color.FromArgb(0x40000000)) : null;
			};
			return control;
		}

		void Refresh()
		{
			if (selectedItem != null)
			{
				if (brush != null)
					brush.Dispose();
				brush = selectedItem.CreateBrush();
			}
			drawable.Invalidate();
		}

		IMatrix GetTransform()
		{
			var matrix = Matrix.Create();
			matrix.Translate(OffsetX, OffsetY);
			matrix.Scale(Math.Max(ScaleX / 100f, 0.01f), Math.Max(ScaleY / 100f, 0.01f));
			matrix.Rotate(Rotation);
			return matrix;
		}

		void Draw(Graphics g)
		{
			if (brush == null)
				return;
			var rect = new RectangleF(10, 10, 200, 100);

			if (Mode == DrawMode.Fill)
			{
				var pen = new Pen(Colors.Black);
				/**/
				g.FillEllipse(brush, rect);
				g.DrawEllipse(pen, rect);
				/**/
				rect = new RectangleF(10, 120, 200, 80);
				g.FillRectangle(brush, rect);
				g.DrawRectangle(pen, rect);
				/**/
				rect = new RectangleF(10, 210, 200, 80);
				g.FillPie(brush, rect, 100, 240);
				g.DrawArc(pen, rect, 100, 240);
				/**/
				var points = new[] { new PointF(300, 10), new PointF(350, 30), new PointF(400, 90), new PointF(320, 100) };
				g.FillPolygon(brush, points);
				g.DrawPolygon(pen, points);
				/**/
				pen.Dispose();
			}
			else 
			{
				var pen = new Pen(brush, 10);
				/**/
				g.DrawEllipse(pen, rect);
				/**/
				rect = new RectangleF(10, 120, 200, 80);
				g.DrawRectangle(pen, rect);
				/**/
				rect = new RectangleF(10, 210, 200, 80);
				g.DrawArc(pen, rect, 100, 240);
				/**/
				var points = new[] { new PointF(300, 10), new PointF(350, 30), new PointF(400, 90), new PointF(320, 100) };
				g.DrawPolygon(pen, points);
				/**/
				pen.Dispose();
			}
		}
	}
}

