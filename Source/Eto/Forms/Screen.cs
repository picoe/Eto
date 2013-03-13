using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IScreen : IInstanceWidget
	{
		float Scale { get; }

		float RealScale { get; }

		int BitsPerPixel { get; }

		RectangleF Bounds { get; }

		RectangleF WorkingArea { get; }

		bool IsPrimary { get; }
	}

	public interface IScreens : IWidget
	{
		IEnumerable<Screen> Screens { get; }

		Screen PrimaryScreen { get; }
	}

	public class Screen : InstanceWidget
	{
		new IScreen Handler { get { return (IScreen)base.Handler; } }

		public Screen (Generator generator = null)
			: base (generator, typeof(IScreen))
		{
		}

		public Screen (Generator generator, IScreen handler)
			: base (generator, handler)
		{
		}

		public static IEnumerable<Screen> Screens (Generator generator = null)
		{
			var handler = generator.CreateShared <IScreens>();
			return handler.Screens;
		}

		public static RectangleF DisplayBounds (Generator generator = null)
		{
			var handler = generator.CreateShared <IScreens> ();
			var bounds = RectangleF.Empty;
			foreach (var screen in handler.Screens) {
				bounds.Union (screen.Bounds);
			}
			return bounds;
		}

		public static Screen PrimaryScreen (Generator generator = null)
		{
			var handler = generator.CreateShared <IScreens>();
			return handler.PrimaryScreen;
		}

		public float DPI { get { return Handler.Scale * 72f; } }

		public float Scale { get { return Handler.Scale; } }

		public float RealDPI { get { return Handler.RealScale * 72f; } }

		public float RealScale { get { return Handler.RealScale; } }

		public RectangleF Bounds { get { return Handler.Bounds; } }

		public RectangleF WorkingArea { get { return Handler.WorkingArea; } }

		public int BitsPerPixel { get { return Handler.BitsPerPixel; } }

		public bool IsPrimary { get { return Handler.IsPrimary; } }
	}
}

