using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IDrawable : IDockContainer
	{
		void Create();

		void Update(Rectangle rect);

		bool CanFocus { get; set; }

		Graphics CreateGraphics();
	}

	public class PaintEventArgs : EventArgs
	{
		Graphics graphics;
		Rectangle clipRectangle;

		public PaintEventArgs(Graphics graphics, Rectangle clipRectangle)
		{
			this.clipRectangle = clipRectangle;
			this.graphics = graphics;
		}

		public Graphics Graphics
		{
			get { return graphics; }
		}

		public Rectangle ClipRectangle
		{
			get { return clipRectangle; }
		}
	}

	public delegate void PaintEventHandler(object sender, PaintEventArgs pe);

	public partial class Drawable : DockContainer
	{
		new IDrawable Handler { get { return (IDrawable)base.Handler; } }

		public event PaintEventHandler Paint;

		public Drawable()
			: this(Generator.Current)
		{
		}

		public Drawable(Generator generator)
			: this(generator, typeof(IDrawable))
		{
		}

		protected Drawable(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create();
			if (initialize) Initialize();
		}


		public Drawable(Generator generator, IDrawable handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		public virtual void OnPaint(PaintEventArgs pe)
		{
			if (Paint != null)
				Paint(this, pe);
		}

		public Graphics CreateGraphics()
		{
			return Handler.CreateGraphics();
		}

		public bool CanFocus
		{
			get { return Handler.CanFocus; }
			set { Handler.CanFocus = value; }
		}

		public void Update(Rectangle rect)
		{
			Handler.Update(rect);
		}

	}
}
