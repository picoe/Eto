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

		public ScreenSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			screens = Screen.Screens.ToArray();
			layout.AddSeparateRow(null, new Label { Text = string.Format("Display Bounds: {0}", displayBounds) }, null);
			layout.BeginVertical(Padding.Empty);
			var num = 0;
			foreach (var screen in screens)
			{
				layout.AddRow(null, new Label { Text = string.Format("Screen {0}, BitsPerPixel: {1}, IsPrimary: {2}", num++, screen.BitsPerPixel, screen.IsPrimary) }, null);
				layout.AddRow(null, new Label { Text = string.Format("Bounds: {0}", screen.Bounds) }, null);
				layout.AddRow(null, new Label { Text = string.Format("Working Area: {0}", screen.WorkingArea) }, null);
				layout.AddRow(null, new Label { Text = string.Format("DPI: {0}, Scale: {1}", screen.DPI, screen.Scale) }, null);

				if (Math.Abs(screen.RealDPI - screen.DPI) > 0.1f)
					layout.AddRow(null, new Label { Text = string.Format("Real DPI: {0}, Real Scale: {1}", screen.RealDPI, screen.RealScale) }, null);
			}
			layout.EndVertical();
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
				foreach (var screen in screens)
				{
					var screenBounds = (screen.Bounds * scale) + offset;
					pe.Graphics.FillRectangle(Colors.White, screenBounds);

					var workingArea = (screen.WorkingArea * scale) + offset;
					pe.Graphics.FillRectangle(Colors.Blue, workingArea);

					screenBounds.Width -= 1;
					screenBounds.Height -= 1;
					pe.Graphics.DrawRectangle(Colors.Black, screenBounds);
				}

				var windowBounds = ((RectangleF)ParentWindow.Bounds * scale) + offset;
				pe.Graphics.FillRectangle(new Color(Colors.LightSkyBlue, 0.8f), windowBounds);
				windowBounds.Width -= 1;
				windowBounds.Height -= 1;
				pe.Graphics.DrawRectangle(Colors.White, windowBounds);
			};
			return drawable;
		}
	}
}

