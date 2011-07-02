using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IPixelLayout : IPositionalLayout
	{
	}
	
	public class PixelLayout : Layout
	{
		IPixelLayout inner;

		public PixelLayout(Container container)
			: base(container.Generator, container, typeof(IPixelLayout))
		{
			inner = (IPixelLayout)Handler;
		}

		public void Add(Control control, int x, int y)
		{
			control.SetParentLayout(this);
			inner.Add(control, x, y);
			Container.InnerControls.Add(control);
			if (Loaded) control.OnLoad (EventArgs.Empty);
		}

		public void Add(Control child, Point p)
		{
			Add(child, p.X, p.Y);
		}
		
		public void Move(Control child, int x, int y)
		{
			inner.Move(child, x, y);
		}
		
		public void Move(Control child, Point p)
		{
			Move(child, p.X, p.Y);
		}
		
		public void Remove(Control child)
		{
			if (Container.InnerControls.Remove(child))
				inner.Remove(child);
		}
	}
}
