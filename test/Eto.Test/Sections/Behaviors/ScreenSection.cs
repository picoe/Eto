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
		Drawable screenLayoutDrawable;
		Window parentWindow;
		Label windowPositionLabel;
		Label mousePositionLabel;
		bool showScreenContent;

		protected bool ShowScreenContent
		{
			get => showScreenContent;
			set
			{
				showScreenContent = value;
				screenLayoutDrawable.Invalidate(false);
			}
		}

		public ScreenSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var showScreenContent = new CheckBox { Text = "Show screen content" };
			showScreenContent.CheckedBinding.Bind(this, c => c.ShowScreenContent);

			screens = Screen.Screens.ToArray();

			layout.BeginCentered();
			layout.Add(showScreenContent);
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
			screenLayoutDrawable = new Drawable();
			screenLayoutDrawable.Paint += (sender, pe) =>
			{
				var scaleSize = (SizeF)screenLayoutDrawable.Size / displayBounds.Size;
				var scale = Math.Min(scaleSize.Width, scaleSize.Height);
				var offset = (screenLayoutDrawable.Size - (displayBounds.Size * scale)) / 2;
				offset.Height -= displayBounds.Y * scale;
				offset.Width -= displayBounds.X * scale;
				offset = Size.Round(offset);
				bool hasScreenImages = false;
				foreach (var screen in screens)
				{
					var screenBounds = (screen.Bounds * scale) + offset;
                    screenBounds.Size -= 1;

					var workingArea = (screen.WorkingArea * scale) + offset;
					var screenImage = showScreenContent ? screen.GetImage(new RectangleF(screen.Bounds.Size)) : null;
					if (screenImage != null)
					{
						pe.Graphics.DrawImage(screenImage, screenBounds);
						pe.Graphics.DrawRectangle(Colors.Blue, workingArea);
						hasScreenImages = true;
					}
					else
					{
						pe.Graphics.FillRectangle(Colors.White, screenBounds);
						pe.Graphics.FillRectangle(Colors.Blue, workingArea);
					}

					pe.Graphics.DrawRectangle(Colors.Black, screenBounds);
				}

                var windowBounds = ((RectangleF)ParentWindow.Bounds * scale) + offset;
                windowBounds.Size -= 1;

				if (!hasScreenImages)
				{
					pe.Graphics.FillRectangle(new Color(Colors.LightSkyBlue, 0.8f), windowBounds);
				}
                pe.Graphics.DrawRectangle(Colors.White, windowBounds);

				var mousePosition = Mouse.Position * scale + offset;
				var mouseRect = new RectangleF(mousePosition, SizeF.Empty);
				mouseRect.Inflate(2, 2);
				pe.Graphics.FillEllipse(Colors.Red, mouseRect);
			};
			return screenLayoutDrawable;
		}
	}
}

