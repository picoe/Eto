using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IDrawableCell : ICell
	{
	}

	/// <summary>
	/// A cell that is rendered by custom code.
	/// </summary>
	public class DrawableCell : Cell
	{
		public Action<Graphics, RectangleF, object> PaintHandler { get; set; }

		public DrawableCell ()
			: this(Generator.Current)
		{
		}

		public DrawableCell(Generator g)
			: base(g, typeof(IDrawableCell), true)
		{
		}
	}
}
