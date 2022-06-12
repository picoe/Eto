using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Themed handler for the <see cref="ColorPicker"/> using a Drawable and the ColorDialog.
	/// </summary>
	/// <remarks>
	/// This is useful if for example you have changed out the ColorDialog to a non-standard dialog and want to use that on
	/// all platforms using a consistent interface.
	/// </remarks>
	public class ThemedColorPickerHandler : ThemedControlHandler<Control, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		static Color ActiveBackgroundColor = new Color(Colors.Gray, 0.8f);
		static Color InactiveBackgroundColor = new Color(Colors.Gray, 0.2f);
		static Color EnabledBorderColor = new Color(Colors.Gray, 0.6f);
		static Color DisabledBorderColor = new Color(Colors.Gray, 0.3f);
		static Color InnerColorBorder = new Color(Colors.Gray, 0.8f);

		Color _color;
		bool _isActive;

		/// <summary>
		/// Initializes a new instance of ThemedColorPickerHandler
		/// </summary>
		public ThemedColorPickerHandler()
		{
			var drawable = new Drawable { Size = new Size(44, 23) };
			drawable.Paint += Drawable_Paint;
			Control = drawable;
			Control.MouseDown += Control_MouseDown;
			Control.MouseUp += Control_MouseUp;
			Control.EnabledChanged += Control_EnabledChanged;
		}

		private void Control_EnabledChanged(object sender, EventArgs e)
		{
			Invalidate(false);
		}

		private void Drawable_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			g.PixelOffsetMode = PixelOffsetMode.None;
			var rect = new Rectangle(Size);

			g.FillRectangle(_isActive ? ActiveBackgroundColor : InactiveBackgroundColor, rect);

			var outlineRect = rect;
			outlineRect.Size -= 1;
			g.DrawRectangle(Enabled ? EnabledBorderColor : DisabledBorderColor, outlineRect);


			var rectInner = rect;
			var innerSpacing = Math.Max(0, Math.Min(6, (Math.Min(rect.Width, rect.Height) - 6) / 2));

			rectInner.Inflate(-innerSpacing, -innerSpacing);

			g.FillRectangle(Color, rectInner);

			if (innerSpacing > 2)
			{
				// draw border around color
				var rectInnerBorder = rectInner;
				rectInnerBorder.Location -= 1;
				rectInnerBorder.Size += 1;
				g.DrawRectangle(InnerColorBorder, rectInnerBorder);
			}

		}

		/// <inheritdoc/>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			if (Enabled && e.Buttons == MouseButtons.Primary && new RectangleF(Size).Contains(e.Location))
			{
				e.Handled = true;

				var dlg = new ColorDialog
				{
					AllowAlpha = AllowAlpha,
					Color = Color
				};
				dlg.ColorChanged += (sender2, e2) =>
				{
					Color = dlg.Color;
				};

				_isActive = true;
				Invalidate(false);

				var lastColor = Color;

				var result = dlg.ShowDialog(Control);
				if (result == DialogResult.Cancel || result == DialogResult.Abort)
					Color = lastColor;

				_isActive = false;
				Invalidate(false);
			}
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (Enabled && e.Buttons == MouseButtons.Primary)
			{
				e.Handled = true;
			}
		}

		/// <inheritdoc/>
		public Color Color
		{
			get => _color;
			set
			{
				if (_color != value)
				{
					_color = value;
					Invalidate(false);
					Callback.OnColorChanged(Widget, EventArgs.Empty);
				}
			}
		}

		/// <inheritdoc/>
		public bool AllowAlpha { get; set; }

		/// <inheritdoc/>
		public bool SupportsAllowAlpha => true;
	}
}
