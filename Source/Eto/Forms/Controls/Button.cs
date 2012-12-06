using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IButton : ITextControl
	{
        Image Image { get; set; }
	}
	
	public class Button : TextControl
	{
        IButton inner;

		public static Size DefaultSize = new Size(80, 26);

		EventHandler<EventArgs> click;
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
            inner = (IButton)base.Handler;

			//this.Width = DefaultSize.Width;
		}
		
		protected Button(Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}

        public Image Image
        {
            get { return inner.Image; }
            set { inner.Image = value; }
        }
	}
}
