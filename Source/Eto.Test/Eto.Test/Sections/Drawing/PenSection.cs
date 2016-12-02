using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// Tests various aspects of pens
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Section("Drawing", "Pen")]
	public class PenSection : Scrollable
	{
		Drawable drawable;

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap { get; set; }

		public DashStyle DashStyle { get; set; }

		public float PenThickness { get; set; }

		public PenSection()
		{
			PenThickness = 4;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, PenJoinControl(), PenCapControl(), DashStyleControl(), null);
			if (Platform.Supports<NumericStepper>())
				layout.AddSeparateRow(null, PenThicknessControl(), null);
			layout.AddCentered(GetDrawable());

			Content = layout;
		}

		Control PenJoinControl()
		{
			var control = new EnumDropDown<PenLineJoin>();
			control.Bind(c => c.SelectedValue, this, r => r.LineJoin);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control PenCapControl()
		{
			var control = new EnumDropDown<PenLineCap>();
			control.Bind(c => c.SelectedValue, this, r => r.LineCap);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control PenThicknessControl()
		{
			var control = new NumericStepper { MinValue = 1, MaxValue = 10 };
			control.Bind(c => c.Value, this, r => r.PenThickness);
			control.ValueChanged += Refresh;

			var layout = new DynamicLayout { Padding = Padding.Empty };
			layout.AddRow(new Label { Text = "Thickness Step:", VerticalAlignment = VerticalAlignment.Center }, control);
			return layout;
		}

		class DashStyleItem : ListItem
		{
			public DashStyle Style { get; set; }
		}

		Control DashStyleControl()
		{
			var control = new DropDown();
			control.Items.Add(new DashStyleItem { Text = "Solid", Style = DashStyles.Solid });
			control.Items.Add(new DashStyleItem { Text = "Dash", Style = DashStyles.Dash });
			control.Items.Add(new DashStyleItem { Text = "Dot", Style = DashStyles.Dot });
			control.Items.Add(new DashStyleItem { Text = "Dash Dot", Style = DashStyles.DashDot });
			control.Items.Add(new DashStyleItem { Text = "Dash Dot Dot", Style = DashStyles.DashDotDot });
			control.SelectedIndex = 0;
			control.SelectedIndexChanged += (sender, e) =>
			{
				if (control.SelectedValue != null)
					DashStyle = ((DashStyleItem)control.SelectedValue).Style;
				Refresh(sender, e);
			};
			return control;
		}

		void Refresh(object sender, EventArgs e)
		{
			drawable.Invalidate();
		}

		Drawable GetDrawable()
		{
			drawable = new Drawable
			{
				Size = new Size(560, 300)
			};
			drawable.Paint += (sender, pe) => Draw(pe.Graphics, null);
			return drawable;
		}

		void Draw(Graphics g, Action<Pen> action)
		{
			var path = new GraphicsPath();
			path.AddLines(new PointF(0, 0), new PointF(100, 40), new PointF(0, 30), new PointF(50, 70));

			for (int i = 0; i < 4; i++)
			{
				float thickness = 1f + i * PenThickness;
				var pen = new Pen(Colors.Black, thickness);
				pen.LineCap = LineCap;
				pen.LineJoin = LineJoin;
				pen.DashStyle = DashStyle;
				if (action != null)
					action(pen);
				var y = i * 20;
				g.DrawLine(pen, 10, y, 110, y);

				y = 80 + i * 50;
				g.DrawRectangle(pen, 10, y, 100, 30);

				y = i * 70;
				g.DrawArc(pen, 140, y, 100, 80, 160, 160);

				y = i * 70;
				g.DrawEllipse(pen, 260, y, 100, 50);

				g.SaveTransform();
				y = i * 70;
				g.TranslateTransform(400, y);
				g.DrawPath(pen, path);
				g.RestoreTransform();
			}
		}
	}
}
