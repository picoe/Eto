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
		new IColorDialog Handler { get { return (IColorDialog)base.Handler; } }
		
		public event EventHandler<EventArgs> ColorChanged;
		
		public virtual void OnColorChanged (EventArgs e)
		{
			if (ColorChanged != null)
				ColorChanged (this, e);
		}
		
		public ColorDialog ()
			: this (Generator.Current)
		{
		}
		
		public ColorDialog (Generator g)
			: this (g, typeof(IColorDialog))
		{
		}
		
		protected ColorDialog (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public Color Color {
			get { return Handler.Color; }
			set { Handler.Color = value; }
		}
	}
}

