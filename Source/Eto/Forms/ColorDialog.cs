using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IColorDialog : ICommonDialog
	{
		Color Color { get; set; }
	}
	
	
	public class ColorDialog : CommonDialog
	{
		IColorDialog inner;
		
		
		public event EventHandler<EventArgs> ColorChanged;
		
		public virtual void OnColorChanged(EventArgs e)
		{
			if (ColorChanged != null) ColorChanged(this, e);
		}
		
		public ColorDialog ()
			: this (Generator.Current)
		{
		}
		
		public ColorDialog (Generator g)
			: base(g, typeof(IColorDialog))
		{
			inner = (IColorDialog)Handler;
		}
		
		public Color Color
		{
			get { return inner.Color; }
			set { inner.Color = value; }
		}
		
	}
}

