using System.Collections.Generic;
using Eto.Drawing;
using System;

namespace Eto.Forms
{
	[Handler(typeof(Screen.IHandler))]
	public class Screen : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public Screen()
		{
		}

		public Screen(IHandler handler)
			: base(handler)
		{
		}

		#pragma warning disable 612,618

		[Obsolete("Use default constructor instead")]
		public Screen (Generator generator)
			: base (generator, typeof(IHandler))
		{
		}

		[Obsolete("Use Screen(IScreen) instead")]
		public Screen (Generator generator, IHandler handler)
			: base (generator, handler)
		{
		}

		#pragma warning restore 612,618

		public static IEnumerable<Screen> Screens
		{
			get
			{
				var handler = Platform.Instance.CreateShared <IScreensHandler>();
				return handler.Screens;
			}
		}

		public static RectangleF DisplayBounds
		{
			get
			{
				var handler = Platform.Instance.CreateShared <IScreensHandler>();
				var bounds = RectangleF.Empty;
				foreach (var screen in handler.Screens)
				{
					bounds.Union(screen.Bounds);
				}
				return bounds;
			}
		}

		public static Screen PrimaryScreen
		{
			get
			{
				var handler = Platform.Instance.CreateShared<IScreensHandler>();
				return handler.PrimaryScreen;
			}
		}

		public float DPI { get { return Handler.Scale * 72f; } }

		public float Scale { get { return Handler.Scale; } }

		public float RealDPI { get { return Handler.RealScale * 72f; } }

		public float RealScale { get { return Handler.RealScale; } }

		public RectangleF Bounds { get { return Handler.Bounds; } }

		public RectangleF WorkingArea { get { return Handler.WorkingArea; } }

		public int BitsPerPixel { get { return Handler.BitsPerPixel; } }

		public bool IsPrimary { get { return Handler.IsPrimary; } }

		public new interface IHandler : Widget.IHandler
		{
			float Scale { get; }

			float RealScale { get; }

			int BitsPerPixel { get; }

			RectangleF Bounds { get; }

			RectangleF WorkingArea { get; }

			bool IsPrimary { get; }
		}

		public interface IScreensHandler
		{
			IEnumerable<Screen> Screens { get; }

			Screen PrimaryScreen { get; }
		}

	}
}

