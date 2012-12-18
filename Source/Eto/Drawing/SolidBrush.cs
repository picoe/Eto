using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines a brush with a solid color for use with <see cref="Graphics"/> fill operations
	/// </summary>
	public interface ISolidBrush : IBrush
	{
		/// <summary>
		/// Gets or sets the fill color of this brush
		/// </summary>
		Color Color { get; set; }
	}
	
	/// <summary>
	/// Handler interface for the <see cref="ISolidBrush"/>
	/// </summary>
	public interface ISolidBrushHandler : ISolidBrush
	{
		/// <summary>
		/// Creates a solid brush with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Initial color for the brush</param>
		void Create (Color color);
	}

	/// <summary>
	/// Methods to create a <see cref="ISolidBrush"/>
	/// </summary>
	public static class SolidBrush
	{
		/// <summary>
		/// Creates a new solid brush with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the brush</param>
		/// <param name="generator">Generator to create the brush for</param>
		/// <returns>A new instance of a solid brush with the specified color</returns>
		public static ISolidBrush Create (Color color, Generator generator = null)
		{
			var handler = (generator ?? Generator.Current).Create<ISolidBrushHandler> ();
			handler.Create (color);
			return handler;
		}
	}
}

