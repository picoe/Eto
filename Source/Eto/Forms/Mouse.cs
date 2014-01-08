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
		public static bool IsSupported(Generator generator = null)
		{
			return (generator ?? Generator.Current).Supports<IMouse>();
		}

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

		/// <summary>
		/// Returns true if any of the specified mouse buttons is pressed.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static bool IsAnyButtonPressed(MouseButtons buttons)
		{
			return (GetButtons() & buttons) != MouseButtons.None;
		}
	}
}
