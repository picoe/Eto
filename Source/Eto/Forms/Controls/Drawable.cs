using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IDrawable : IControl
	{
		void Create ();
		
		void Update (Rectangle rect);

		bool CanFocus { get; set; }

	}

	public class PaintEventArgs : EventArgs
	{
		Graphics graphics;
		Rectangle clipRectangle;

		public PaintEventArgs (Graphics graphics,Rectangle clipRectangle)
		{
			this.clipRectangle = clipRectangle;
			this.graphics = graphics;
		}

		public Graphics Graphics {
			get { return graphics; }
		}

		public Rectangle ClipRectangle {
			get { return clipRectangle; }
		}
	}

	public delegate void PaintEventHandler (object sender, PaintEventArgs pe);

	public partial class Drawable : Control
	{
		IDrawable inner;

		public event PaintEventHandler Paint;

		public Drawable () : this(Generator.Current)
		{
		}

		public Drawable (Generator g) : base(g, typeof(IDrawable), false)
		{
			inner = (IDrawable)Handler;
			inner.Create ();
			Initialize ();
		}

		public virtual void OnPaint (PaintEventArgs pe)
		{
			if (Paint != null)
				Paint (this, pe);
		}

		public bool CanFocus {
			get {
				return inner.CanFocus;
			}
			set {
				inner.CanFocus = value;
			}
		}

		public void Update (Rectangle rect)
		{
			inner.Update (rect);
		}

	}
}
