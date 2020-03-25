using System.Collections.Generic;
using Eto.Drawing;
using System;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a display on the system.
	/// </summary>
	[Handler(typeof(Screen.IHandler))]
	public class Screen : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Screen"/> class.
		/// </summary>
		public Screen()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Screen"/> class with the specified <paramref name="handler"/>.
		/// </summary>
		/// <remarks>
		/// Used by platform implementations to create instances of a screen with a particular handler.
		/// </remarks>
		/// <param name="handler">Handler for the screen.</param>
		public Screen(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets an enumerable of display screens available on the current system.
		/// </summary>
		/// <value>The screens of the current system.</value>
		public static IEnumerable<Screen> Screens
		{
			get
			{
				var handler = Platform.Instance.CreateShared<IScreensHandler>();
				return handler.Screens;
			}
		}

		/// <summary>
		/// Gets the display bounds of all screens on the current system.
		/// </summary>
		/// <value>The display bounds of all screens.</value>
		public static RectangleF DisplayBounds
		{
			get
			{
				var handler = Platform.Instance.CreateShared<IScreensHandler>();
				var bounds = RectangleF.Empty;
				foreach (var screen in handler.Screens)
				{
					bounds.Union(screen.Bounds);
				}
				return bounds;
			}
		}

		/// <summary>
		/// Gets the primary screen of the current system.
		/// </summary>
		/// <remarks>
		/// This is typically the user's main screen.
		/// </remarks>
		/// <value>The primary screen.</value>
		public static Screen PrimaryScreen
		{
			get
			{
				var handler = Platform.Instance.CreateShared<IScreensHandler>();
				return handler.PrimaryScreen;
			}
		}

		/// <summary>
		/// Gets the logical Dots/Pixels Per Inch of the screen.
		/// </summary>
		/// <value>The Dots Per Inch</value>
		public float DPI { get { return Handler.Scale * 72f; } }

		/// <summary>
		/// Gets the logical scale of the pixels of the screen vs. points.
		/// </summary>
		/// <remarks>
		/// The scale can be used to translate points to pixels.  E.g.
		/// <code>
		/// var pixels = points * screen.Scale;
		/// </code>
		/// This is useful when creating fonts that need to be a certain pixel size.
		/// 
		/// Since this is a logical scale, this will give you the 'recommended' pixel size that will appear to be the same
		/// physical size, even on retina displays.
		/// </remarks>
		/// <value>The logical scale of pixels per point.</value>
		public float Scale { get { return Handler.Scale; } }

		/// <summary>
		/// Gets the real Dots/Pixels Per Inch of the screen, accounting for retina displays.
		/// </summary>
		/// <remarks>
		/// This is similar to <see cref="DPI"/>, however will give you the 'real' DPI of the screen.
		/// For example, a Retina display on OS X will have the RealDPI twice the DPI reported.
		/// </remarks>
		/// <value>The real DP.</value>
		public float RealDPI { get { return Handler.RealScale * 72f; } }

		/// <summary>
		/// Gets the real scale of the pixels of the screen vs. points.
		/// </summary>
		/// <remarks>
		/// The scale can be used to translate points to 'real' pixels.  E.g.
		/// <code>
		/// var pixels = points * screen.Scale;
		/// </code>
		/// This is useful when creating fonts that need to be a certain pixel size.
		/// 
		/// Since this is a real scale, this will give you the actual pixel size. 
		/// This means on retina displays on OS X will appear to be half the physical size as regular displays.
		/// </remarks>
		/// <value>The real scale of pixels per point.</value>
		public float RealScale { get { return Handler.RealScale; } }

		/// <summary>
		/// Gets the bounds of the display in the <see cref="DisplayBounds"/> area.
		/// </summary>
		/// <remarks>
		/// The primary screen's upper left corner is always located at 0,0.
		/// A negative X/Y indicates that the screen location is to the left or top of the primary screen.
		/// A positive X/Y indicates that the screen location is to the right or bottom of the primary screen.
		/// </remarks>
		/// <value>The display's bounds.</value>
		public RectangleF Bounds { get { return Handler.Bounds; } }

		/// <summary>
		/// Gets the working area of the display, excluding any menu/task bars, docks, etc.
		/// </summary>
		/// <remarks>
		/// This is useful to position your window in the usable area of the screen.
		/// </remarks>
		/// <value>The working area of the screen.</value>
		public RectangleF WorkingArea { get { return Handler.WorkingArea; } }

		/// <summary>
		/// Gets the number of bits each pixel uses to represent its color value.
		/// </summary>
		/// <value>The screen's bits per pixel.</value>
		public int BitsPerPixel { get { return Handler.BitsPerPixel; } }

		/// <summary>
		/// Gets a value indicating whether this screen is the primary/main screen.
		/// </summary>
		/// <value><c>true</c> if this is the primary screen; otherwise, <c>false</c>.</value>
		public bool IsPrimary { get { return Handler.IsPrimary; } }

		/// <summary>
		/// Gets the number of physical pixels per logical pixel of this display.
		/// </summary>
		/// <remarks>
		/// On Retina/HighDPI displays, this will usually return 2.0, but can also be a fraction of pixels.  
		/// Non-retina will return 1.0.
		/// 
		/// This essentially returns the value of <see cref="RealScale"/> divided by <see cref="Scale"/>.
		/// </remarks>
		/// <value>The number of physical pixels for each logical pixel.</value>
		public float LogicalPixelSize { get { return RealScale / Scale; } }

		/// <summary>
		/// Gets a copy of a portion of the screen as an image
		/// </summary>
		/// <param name="rect">Rectangle to get the screen contents</param>
		/// <returns>A new image (Icon or Bitmap) representing the specified rectangle</returns>
		public Image GetImage(RectangleF rect) => Handler.GetImage(rect);

		/// <summary>
		/// Handler interface for the <see cref="Screen"/>.
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets the logical scale of the pixels of the screen vs. points.
			/// </summary>
			/// <remarks>
			/// This scale should be based on a standard 72dpi screen.
			/// </remarks>
			/// <value>The logical scale of pixels per point.</value>
			float Scale { get; }

			/// <summary>
			/// Gets the real scale of the pixels of the screen vs. points.
			/// </summary>
			/// <remarks>
			/// This should be based on a standard 72dpi screen, and is useful for retina displays when the real DPI
			/// is double the logical DPI.  E.g. 1440x900 logical screen size is actually 2880x1800 pixels.
			/// </remarks>
			/// <value>The real scale of pixels per point.</value>
			float RealScale { get; }

			/// <summary>
			/// Gets the number of bits each pixel uses to represent its color value.
			/// </summary>
			/// <value>The screen's bits per pixel.</value>
			int BitsPerPixel { get; }

			/// <summary>
			/// Gets the bounds of the display.
			/// </summary>
			/// <remarks>
			/// The primary screen's upper left corner is always located at 0,0.
			/// A negative X/Y indicates that the screen location is to the left or top of the primary screen.
			/// A positive X/Y indicates that the screen location is to the right or bottom of the primary screen.
			/// </remarks>
			/// <value>The display's bounds.</value>
			RectangleF Bounds { get; }

			/// <summary>
			/// Gets the working area of the display, excluding any menu/task bars, docks, etc.
			/// </summary>
			/// <remarks>
			/// This is useful to position your window in the usable area of the screen.
			/// </remarks>
			/// <value>The working area of the screen.</value>
			RectangleF WorkingArea { get; }

			/// <summary>
			/// Gets a value indicating whether this screen is the primary/main screen.
			/// </summary>
			/// <value><c>true</c> if this is the primary screen; otherwise, <c>false</c>.</value>
			bool IsPrimary { get; }

			/// <summary>
			/// Gets a copy of a portion of the screen as an image
			/// </summary>
			/// <param name="rect">Rectangle to get the screen contents</param>
			/// <returns>A new image (Icon or Bitmap) representing the specified rectangle</returns>
			Image GetImage(RectangleF rect);
		}

		/// <summary>
		/// Handler interface for static methods of the <see cref="Screen"/>.
		/// </summary>
		public interface IScreensHandler
		{
			/// <summary>
			/// Gets an enumerable of display screens available on the current system.
			/// </summary>
			/// <value>The screens of the current system.</value>
			IEnumerable<Screen> Screens { get; }

			/// <summary>
			/// Gets the primary screen of the current system.
			/// </summary>
			/// <remarks>
			/// This is typically the user's main screen.
			/// </remarks>
			/// <value>The primary screen.</value>
			Screen PrimaryScreen { get; }
		}

		/// <summary>
		/// Gets the screen that contains the specified <paramref name="point"/>,
		/// or the closest screen to the point if it is outside the bounds of all screens.
		/// </summary>
		/// <returns>The screen encompassing or closest the specified point.</returns>
		/// <param name="point">Point to find the screen.</param>
		public static Screen FromPoint(PointF point)
		{
			var screens = Screens.ToArray();
			foreach (var screen in screens)
			{
				if (screen.Bounds.Contains(point))
					return screen;
			}

			Screen foundScreen = null;
			float foundDistance = float.MaxValue;
			foreach (var screen in screens)
			{
				var diff = RectangleF.Distance(screen.Bounds, point);
				var distance = (float)Math.Sqrt(diff.Width * diff.Width + diff.Height * diff.Height);
				if (distance < foundDistance)
				{
					foundScreen = screen;
					foundDistance = distance;
				}
			}
			return foundScreen ?? PrimaryScreen;
		}

		/// <summary>
		/// Gets the screen that encompases the biggest part of the specified <paramref name="rectangle"/>,
		/// or the closest screen to the rectangle if it is outside the bounds of all screens..
		/// </summary>
		/// <returns>The screen encompassing or closest to the specified rectangle.</returns>
		/// <param name="rectangle">Rectangle to find the screen.</param>
		public static Screen FromRectangle(RectangleF rectangle)
		{
			Screen foundScreen = null;
			float foundArea = 0;
			var screens = Screens.ToArray();
			foreach (var screen in screens)
			{
				var rect = rectangle;
				rect.Intersect(screen.Bounds);
				var area = rect.Size.Width * rect.Size.Height;
				if (area > foundArea)
				{
					foundScreen = screen;
					foundArea = area;
				}
			}
			if (foundScreen != null)
				return foundScreen;

			// find by distance
			float foundDistance = float.MaxValue;
			foreach (var screen in screens)
			{
				var diff = RectangleF.Distance(rectangle, screen.Bounds);
				var distance = (float)Math.Sqrt(diff.Width * diff.Width + diff.Height * diff.Height);
				if (distance < foundDistance)
				{
					foundScreen = screen;
					foundDistance = distance;
				}
			}
			return foundScreen ?? PrimaryScreen;
		}
	}
}

