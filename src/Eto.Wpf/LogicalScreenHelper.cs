using System;
using System.Collections.Generic;
using System.Linq;
using sd = System.Drawing;
#if WPF
#elif WINFORMS
using Eto.WinForms.Forms;
#endif

namespace Eto
{
	public abstract class LogicalScreenHelper<T>
	{
		public abstract IEnumerable<T> AllScreens { get; }

		public abstract T PrimaryScreen { get; }

		public abstract sd.Rectangle GetBounds(T screen);

		public abstract Eto.Drawing.SizeF GetLogicalSize(T screen);

		public abstract float GetLogicalPixelSize(T screen);

		public virtual float GetMaxLogicalPixelSize() => AllScreens.Max((Func<T, float>)GetLogicalPixelSize);

		public Eto.Drawing.PointF GetLogicalLocation(T screen)
		{
			/**/
			var bounds = GetBounds(screen);
			var primaryScreen = PrimaryScreen;
			if (screen.Equals(primaryScreen))
				return bounds.Location.ToEto();
			var primaryBounds = GetBounds(primaryScreen);
			Eto.Drawing.PointF location = primaryBounds.Location.ToEto();

			// this finds all adjacent screens between the primary screen and the specified screen
			// to calculate it's logical position

			// if it is not adjacent, we use the maximum pixel size to figure out its position.
			var allScreens = AllScreens.ToList();

			var maxLogicalPixelSize = GetMaxLogicalPixelSize();

			if (bounds.X < primaryBounds.X)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in allScreens.OrderByDescending(s => GetBounds(s).X))
				{
					var scnBounds = GetBounds(scn);
					if (scnBounds.X > primaryBounds.X || (!scn.Equals(screen) && bounds.Right > scnBounds.X))
						continue;
					if (scnBounds.X < bounds.X)
						break;
					if (scnBounds.Right == GetBounds(adjacentScreen).X)
					{
						var logicalSize = GetLogicalSize(scn);
						location.X -= logicalSize.Width;
						adjacentScreen = scn;
					}
					if (scn.Equals(screen))
						break;
				}
				if (!adjacentScreen.Equals(screen))
				{
					location.X = bounds.X / maxLogicalPixelSize;
				}
			}
			else if (bounds.X > primaryBounds.X)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in allScreens.OrderBy(s => GetBounds(s).X))
				{
					var scnBounds = GetBounds(scn);
					if (scnBounds.X < primaryBounds.X || (!scn.Equals(screen) && bounds.X < scnBounds.Right))
						continue;
					if (scnBounds.X > bounds.X)
						break;
					if (scnBounds.X == GetBounds(adjacentScreen).Right)
					{
						var logicalSize = GetLogicalSize(adjacentScreen);
						location.X += logicalSize.Width;
						adjacentScreen = scn;
					}
					if (scn.Equals(screen))
						break;
				}
				if (!adjacentScreen.Equals(screen))
				{
					location.X = bounds.X / maxLogicalPixelSize;
				}
			}

			if (bounds.Y < primaryBounds.Y)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in allScreens.OrderByDescending(s => GetBounds(s).Y))
				{
					var scnBounds = GetBounds(scn);
					if (scnBounds.Y > primaryBounds.Y || (!scn.Equals(screen) && bounds.Bottom > scnBounds.Y))
						continue;
					if (scnBounds.Y < bounds.Y)
						break;
					if (scnBounds.Bottom == GetBounds(adjacentScreen).Y)
					{
						var logicalSize = GetLogicalSize(scn);
						location.Y -= logicalSize.Height;
						adjacentScreen = scn;
					}
					if (scn.Equals(screen))
						break;
				}
				if (!adjacentScreen.Equals(screen))
				{
					location.Y = bounds.Y / maxLogicalPixelSize;
				}
			}
			else if (bounds.Y > primaryBounds.Y)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in allScreens.OrderBy(s => GetBounds(s).Y))
				{
					var scnBounds = GetBounds(scn);
					if (scnBounds.Y < primaryBounds.Y || (!scn.Equals(screen) && bounds.Y < scnBounds.Bottom))
						continue;
					if (scnBounds.Y > bounds.Y)
						break;
					if (scnBounds.Y == GetBounds(adjacentScreen).Bottom)
					{
						var logicalSize = GetLogicalSize(adjacentScreen);
						location.Y += logicalSize.Height;
						adjacentScreen = scn;
					}
					if (scn.Equals(screen))
						break;
				}
				if (!adjacentScreen.Equals(screen))
				{
					location.Y = bounds.Y / maxLogicalPixelSize;
				}
			}

			return location;
		}
	}
}
