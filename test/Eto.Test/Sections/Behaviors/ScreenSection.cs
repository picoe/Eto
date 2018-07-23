using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Screen")]
	public class ScreenSection : Scrollable
	{
		readonly RectangleF displayBounds = Screen.DisplayBounds;
		readonly Screen[] screens;
		Window parentWindow;
		Label windowPositionLabel;
		Label mousePositionLabel;

		public ScreenSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			screens = Screen.Screens.ToArray();

			layout.BeginCentered();
			layout.Add($"Display Bounds: {displayBounds}");
			layout.BeginVertical(Padding.Empty);
			var num = 0;
			foreach (var screen in screens)
			{
				layout.Add($"Screen {num++}, BitsPerPixel: {screen.BitsPerPixel}, IsPrimary: {screen.IsPrimary}");
				layout.BeginVertical(new Padding(10, 0));
				layout.Add($"Bounds: {screen.Bounds}");
				layout.Add($"WorkingArea: {screen.WorkingArea}");
				layout.Add($"DPI: {screen.DPI}, Scale: {screen.Scale}, LogicalPixelSize: {screen.LogicalPixelSize}");
				layout.EndVertical();
			}
			layout.EndVertical();
			layout.Add(windowPositionLabel = new Label());
			layout.Add(mousePositionLabel = new Label());
			layout.EndCentered();

			layout.Add(ScreenLayout());

			Content = layout;
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			parentWindow = ParentWindow;
			parentWindow.LocationChanged += HandleLocationChanged;
		}

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			parentWindow.LocationChanged -= HandleLocationChanged;
		}

		void HandleLocationChanged(object sender, EventArgs e)
		{
			windowPositionLabel.Text = $"Window Bounds: {parentWindow.Bounds}";
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mousePositionLabel.Text = $"Mouse Position: {Mouse.Position}";
			Invalidate();
		}

		Control ScreenLayout()
		{
			var drawable = new Drawable();
			drawable.Paint += (sender, pe) =>
			{
				var scaleSize = (SizeF)drawable.Size / displayBounds.Size;
				var scale = Math.Min(scaleSize.Width, scaleSize.Height);
				var offset = (drawable.Size - (displayBounds.Size * scale)) / 2;
				offset.Height -= displayBounds.Y * scale;
				offset.Width -= displayBounds.X * scale;
				offset = Size.Round(offset);
				foreach (var screen in screens)
				{
					var screenBounds = (screen.Bounds * scale) + offset;
                    screenBounds.Size -= 1;

                    pe.Graphics.FillRectangle(Colors.White, screenBounds);

					var workingArea = (screen.WorkingArea * scale) + offset;
					pe.Graphics.FillRectangle(Colors.Blue, workingArea);

					pe.Graphics.DrawRectangle(Colors.Black, screenBounds);
				}

                var windowBounds = ((RectangleF)ParentWindow.Bounds * scale) + offset;
                windowBounds.Size -= 1;
                pe.Graphics.FillRectangle(new Color(Colors.LightSkyBlue, 0.8f), windowBounds);
                pe.Graphics.DrawRectangle(Colors.White, windowBounds);

				var mousePosition = Mouse.Position * scale + offset;
				var mouseRect = new RectangleF(mousePosition, SizeF.Empty);
				mouseRect.Inflate(2, 2);
				pe.Graphics.FillEllipse(Colors.Red, mouseRect);
			};
			return drawable;
		}
	}
}

