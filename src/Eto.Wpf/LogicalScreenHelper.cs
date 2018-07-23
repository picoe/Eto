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

		T FindLeftScreen(T screen) =>
			AllScreens
			.Where(r => GetBounds(r).X >= 0 && GetBounds(r).Right == GetBounds(screen).X)
			.OrderBy(r => GetLogicalPixelSize(r))
			.FirstOrDefault();

		T FindRightScreen(T screen) =>
			AllScreens
			.Where(r => GetBounds(r).X < 0 && GetBounds(r).X == GetBounds(screen).Right)
			.OrderBy(r => GetLogicalPixelSize(r))
			.FirstOrDefault();

		T FindTopScreen(T screen) =>
			AllScreens
			.Where(r => GetBounds(r).Y >= 0 && GetBounds(r).Bottom == GetBounds(screen).Y)
			.OrderBy(r => GetLogicalPixelSize(r))
			.FirstOrDefault();

		T FindBottomScreen(T screen) =>
			AllScreens
			.Where(r => GetBounds(r).Y < 0 && GetBounds(r).Y == GetBounds(screen).Bottom)
			.OrderBy(r => GetLogicalPixelSize(r))
			.FirstOrDefault();

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

			if (bounds.X < primaryBounds.X)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in AllScreens.OrderByDescending(s => GetBounds(s).X))
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
					location.X = bounds.X / GetMaxLogicalPixelSize();
				}
			}
			else if (bounds.X > primaryBounds.X)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in AllScreens.OrderBy(s => GetBounds(s).X))
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
					location.X = bounds.X / GetMaxLogicalPixelSize();
				}
			}

			if (bounds.Y < primaryBounds.Y)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in AllScreens.OrderByDescending(s => GetBounds(s).Y))
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
					location.Y = bounds.Y / GetMaxLogicalPixelSize();
				}
			}
			else if (bounds.Y > primaryBounds.Y)
			{
				var adjacentScreen = primaryScreen;
				foreach (var scn in AllScreens.OrderBy(s => GetBounds(s).Y))
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
					location.Y = bounds.Y / GetMaxLogicalPixelSize();
				}
			}

			return location;
			/**

			var bounds = GetBounds(screen);
			float x, y;
			if (bounds.X > 0)
			{
				var left = FindLeftScreen(screen);
				if (left != null)
					x = GetLogicalLocation(left).X + GetLogicalSize(left).Width;
				else
					x = bounds.X / GetMaxLogicalPixelSize();
			}
			else if (bounds.X < 0)
			{
				var right = FindRightScreen(screen);
				if (right != null)
					x = GetLogicalLocation(right).X - GetLogicalSize(screen).Width;
				else
					x = bounds.X / GetLogicalPixelSize(screen);
			}
			else x = bounds.X;
			if (bounds.Y > 0)
			{
				var top = FindTopScreen(screen);
				if (top != null)
					y = GetLogicalLocation(top).Y + GetLogicalSize(top).Height;
				else
					y = bounds.Y / GetMaxLogicalPixelSize();
			}
			else if (bounds.Y < 0)
			{
				var bottom = FindBottomScreen(screen);
				if (bottom != null)
					y = GetLogicalLocation(bottom).Y - GetLogicalSize(screen).Height;
				else
					y = bounds.Y / GetLogicalPixelSize(screen);
			}
			else y = bounds.Y;
			return new Eto.Drawing.PointF(x, y);
			/**/
		}
	}
}
