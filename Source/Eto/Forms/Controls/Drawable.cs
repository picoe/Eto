using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IDrawable : IPanel
	{
		bool SupportsCreateGraphics { get; }

		void Create();

		void Update(Rectangle rect);

		bool CanFocus { get; set; }

		Graphics CreateGraphics();
	}

	public class PaintEventArgs : EventArgs
	{
		readonly Graphics graphics;
		readonly Rectangle clipRectangle;

		public PaintEventArgs(Graphics graphics, Rectangle clipRectangle)
		{
			this.clipRectangle = clipRectangle;
			this.graphics = graphics;
		}

		public Graphics Graphics
		{
			get { return graphics; }
		}

		#pragma warning disable 612,618

		[Obsolete("Use Platform.Instance instead")]
		public Generator Generator
		{
			get { return graphics.Platform; }
		}

		#pragma warning restore 612,618

		public Rectangle ClipRectangle
		{
			get { return clipRectangle; }
		}
	}

	[Handler(typeof(IDrawable))]
	public partial class Drawable : Panel
	{
		new IDrawable Handler { get { return (IDrawable)base.Handler; } }

		public event EventHandler<PaintEventArgs> Paint;

		public Drawable()
		{
			Handler.Create();
			Initialize();
		}

		protected Drawable(IDrawable handler)
			: base(handler)
		{
		}

		[Obsolete("Use default constructor instead")]
		public Drawable(Generator generator)
			: this(generator, typeof(IDrawable))
		{
		}

		[Obsolete("Use default constructor with HandlerAttribute instead")]
		protected Drawable(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create();
			if (initialize) Initialize();
		}


		[Obsolete("Use Drawable(IDrawable) instead")]
		public Drawable(Generator generator, IDrawable handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		public virtual void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
				Paint(this, e);
		}

		public bool SupportsCreateGraphics
		{
			get { return Handler.SupportsCreateGraphics; }
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
