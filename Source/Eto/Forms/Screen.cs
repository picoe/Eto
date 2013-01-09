using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IScreen : IInstanceWidget
	{
		float Scale { get; }

		float RealScale { get; }
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

		public static Screen PrimaryScreen (Generator generator = null)
		{
			var handler = generator.CreateShared <IScreens>();
			return handler.PrimaryScreen;
		}

		public float DPI { get { return Handler.Scale * 72f; } }

		public float Scale { get { return Handler.Scale; } }

		public float RealDPI { get { return Handler.RealScale * 72f; } }

		public float RealScale { get { return Handler.RealScale; } }
	}
}

