using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Behaviors
{
	public class ScreenSection : Scrollable
	{
		readonly RectangleF displayBounds = Screen.DisplayBounds();
		readonly Screen[] screens;
		Window parentWindow;

		public ScreenSection()
		{
			var layout = new DynamicLayout();

			screens = Screen.Screens().ToArray();
			layout.AddSeparateRow(null, new Label { Text = string.Format ("Display Bounds: {0}", displayBounds) }, null);
			layout.BeginVertical(Padding.Empty);
			var num = 0;
			foreach (var screen in screens)
			{
				layout.AddRow(null, new Label { Text = string.Format ("Screen {0}", num++) }, new Label { Text = string.Format ("BitsPerPixel: {0}, IsPrimary: {1}", screen.BitsPerPixel, screen.IsPrimary) });
				layout.AddRow(null, new Label { Text = "Bounds:", HorizontalAlign = HorizontalAlign.Right }, new Label { Text = screen.Bounds.ToString () }, null);
				layout.AddRow(null, new Label { Text = "Working Area:", HorizontalAlign = HorizontalAlign.Right }, new Label { Text = screen.WorkingArea.ToString () }, null);
			}
			layout.EndVertical();
			layout.Add(ScreenLayout());

			Content = layout;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			parentWindow = ParentWindow;
			parentWindow.LocationChanged += HandleLocationChanged;
		}

		public override void OnUnLoad(EventArgs e)
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
			drawable.Paint += (sender, pe) => {
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

