using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Test.Sections.Behaviors
{
	/// <summary>
	/// Tests screen information and behaviour of window and cursor positions.
	/// </summary>
	/// <remarks>
	/// Ensure you test this with Per-monitor and System DPI settings in app.manifest
	/// </remarks>
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
			TestApplication.Settings.LastFormPosition = null;
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var showScreenContent = new CheckBox { Text = "Show screen content" };
			showScreenContent.CheckedBinding.Bind(this, c => c.ShowScreenContent);

			var showWindowsAtCorners = new CheckBox { Text = "Create windows at screen corners" };
			showWindowsAtCorners.CheckedChanged += showWindowsAtCorners_CheckedChanged;

			screens = Screen.Screens.ToArray();

			layout.BeginCentered();
			layout.AddSeparateRow(showScreenContent, showWindowsAtCorners, null);
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

		List<Form> cornerWindows = new List<Form>();

		bool RestorePosition(Window window)
		{
			if (TestApplication.Settings.LastFormPosition == null)
				return false;
			var etoRectangle = TestApplication.Settings.LastFormPosition.Value;
			// If the font face or size is different or the save rectangle is empty then bail
			// Returns a Screen for the display that contains the point. In multiple
			// display environments where no display contains the point, the display
			// closest to the specified point is returned.
			var screen_bounds = Screen.FromRectangle(etoRectangle).Bounds;
			// Inflate the bounding rectangle by 1 pixel on all sides to make sure
			// edge cases work
			screen_bounds.Inflate(1, 1);
			// Force the rectangle onto the nearest screen
			var bounds = ForceToNearestScreen(etoRectangle);
			// If the restore rectangle is not completely within the screen bounding
			// rectangle then don't do anything
			if (!screen_bounds.Contains(bounds)) return false;

			var restore_bounds = window.RestoreBounds;
			// Set the window location
			window.Location = bounds.Location;
			// Only restore the size for a manually sized window.
			//if (window.SizeToContent == SizeToContent.Manual)
			if (window.Resizable)
			{
				// Set the window width and height
				window.Size = bounds.Size;
			}
			// Return true if the location or size changed
			return (window.RestoreBounds != restore_bounds);
		}

		static Rectangle ForceToNearestScreen(Rectangle rectangle)
		{
			var result = rectangle;
			// Returns a Screen for the display that contains the point. In multiple
			// display environments where no display contains the point, the display
			// closest to the specified point is returned.
			var screen_bounds = Rectangle.Round(Screen.FromRectangle(rectangle).Bounds);
			// Position the rectangle in the appropriate monitor display space
			if (result.Right > screen_bounds.Right)
				result.Offset(screen_bounds.Right - result.Right, 0);
			if (result.Bottom > screen_bounds.Bottom)
				result.Offset(0, screen_bounds.Bottom - result.Bottom);
			if (result.Left < screen_bounds.Left)
				result.Offset((screen_bounds.Left - result.Left), 0);
			if (result.Top < screen_bounds.Top)
				result.Offset(0, (screen_bounds.Top - result.Top));
			return result;
		}

		public void SavePosition(Window window)
		{
			TestApplication.Settings.LastFormPosition = window.RestoreBounds;
		}

		private void showWindowsAtCorners_CheckedChanged(object sender, EventArgs e)
		{
			CloseCornerWindows();
			Form CreateWindow()
			{
				var form = new Form
				{
					WindowStyle = WindowStyle.None,
					Resizable = false,
					Size = new Size(100, 100),
					ShowActivated = false,
					CanFocus = false,
					ShowInTaskbar = false,
					Padding = 10,
					Content = new Panel { BackgroundColor = Colors.Blue }
				};
				// use anonymous method to prevent event reference
				form.LocationChanged += (_, __) => Invalidate();
				form.Shown += (_, __) => Invalidate();
				form.Closed += (_, __) => Invalidate();
				return form;
			};

			var showWindowsAtCorners = (CheckBox)sender;
			if (showWindowsAtCorners.Checked != true)
				return;

			var restoreForm = new Form {
				Content = "This form should restore its size and position", 
				Resizable = true,
				ClientSize = new Size(450, 300),
				ShowActivated = false
			};
			restoreForm.LocationChanged += (_, __) => Invalidate();
			restoreForm.SizeChanged += (_, __) => Invalidate();
			RestorePosition(restoreForm);
			restoreForm.Closing += (_, __) => SavePosition(restoreForm);
			restoreForm.Show();

			cornerWindows.Add(restoreForm);


			foreach (var screen in Screen.Screens)
			{
				var topLeft = CreateWindow();
				topLeft.Location = Point.Truncate(screen.Bounds.TopLeft);
				topLeft.Show();
				cornerWindows.Add(topLeft);

				var topRight = CreateWindow();
				topRight.Location = Point.Truncate(screen.Bounds.TopRight - new Size(topRight.Width, 0));
				topRight.Show();
				cornerWindows.Add(topRight);

				var bottomLeft = CreateWindow();
				bottomLeft.Location = Point.Truncate(screen.Bounds.BottomLeft - new Size(0, bottomLeft.Height));
				bottomLeft.Show();
				cornerWindows.Add(bottomLeft);

				var bottomRight = CreateWindow();
				bottomRight.Location = Point.Truncate(screen.Bounds.BottomRight - bottomLeft.Size);
				bottomRight.Show();
				cornerWindows.Add(bottomRight);
			}
		}

		private void CloseCornerWindows()
		{
			foreach (var window in cornerWindows.ToArray())
			{
				window.Close();
			}
			cornerWindows.Clear();
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
			CloseCornerWindows();
		}

		void HandleLocationChanged(object sender, EventArgs e)
		{
			windowPositionLabel.Text = $"Window Bounds: {parentWindow.Bounds}, RestoreBounds: {parentWindow.RestoreBounds}";
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

				void DrawWindow(Window window)
				{
					var windowBounds = ((RectangleF)window.Bounds * scale) + offset;
					windowBounds.Size -= 1;

					if (!hasScreenImages)
					{
						pe.Graphics.FillRectangle(new Color(Colors.LightSkyBlue, 0.8f), windowBounds);
					}
					pe.Graphics.DrawRectangle(Colors.White, windowBounds);
				}

				DrawWindow(ParentWindow);

				if (cornerWindows != null)
				{
					foreach (var window in cornerWindows)
					{
						DrawWindow(window);
					}
				}


				var mousePosition = Mouse.Position * scale + offset;
				var mouseRect = new RectangleF(mousePosition, SizeF.Empty);
				mouseRect.Inflate(2, 2);
				pe.Graphics.FillEllipse(Colors.Red, mouseRect);
			};
			return screenLayoutDrawable;
		}
	}
}

