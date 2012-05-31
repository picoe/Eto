using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IButton : ITextControl
	{
	}
	
	public class Button : TextControl
	{
		public static Size DefaultSize = new Size(80, 26);

		private EventHandler<EventArgs> click;
		public event EventHandler<EventArgs> Click
		{
			add { click += value; }
			remove { click -= value; }
		}

		public virtual void OnClick(EventArgs e)
		{
			if (click != null) click(this, e);
		}

		public Button() : this (Generator.Current)
		{
		}
		
		public Button(Generator g) : this (g, typeof(IButton))
		{
			//this.Width = DefaultSize.Width;
		}
		
		protected Button(Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}
	}
}
