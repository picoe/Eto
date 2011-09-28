using System;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IPixelLayout : IPositionalLayout
	{
	}
	
	public class PixelLayout : Layout
	{
		IPixelLayout inner;
		List<Control> controls = new List<Control>();
		
		public override IEnumerable<Control> Controls {
			get {
				return controls;
			}
		}

		public PixelLayout(Container container)
			: base(container.Generator, container, typeof(IPixelLayout))
		{
			inner = (IPixelLayout)Handler;
		}

		public void Add(Control control, int x, int y)
		{
			control.SetParentLayout(this);
			inner.Add(control, x, y);
			if (Loaded) {
				control.OnLoad (EventArgs.Empty);
				control.OnLoadComplete (EventArgs.Empty);
			}
			controls.Add(control);
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
		
		public void Remove(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove (control);
		}
		
		public void Remove(Control child)
		{
			if (controls.Remove(child))
				inner.Remove(child);
		}
	}
}
