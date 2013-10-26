using Eto.Drawing;

namespace Eto.Forms
{
	public interface IMouse : IWidget
	{
		PointF Position { get; }

		MouseButtons Buttons { get; }
	}

	public static class Mouse
	{
		static IMouse Handler (Generator generator)
		{
			return generator.CreateShared<IMouse> ();
		}

		public static PointF GetPosition (Generator generator = null)
		{
			return Handler (generator).Position;
		}

		public static MouseButtons GetButtons (Generator generator = null)
		{
			return Handler (generator).Buttons;
		}

	}
}
