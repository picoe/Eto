using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IScrollable : IContainer
	{
		void UpdateScrollSizes();
		Point ScrollPosition { get; set; }
		Size ScrollSize { get; set; }
		BorderType Border { get; set; }
	}
	
	public class ScrollEventArgs : EventArgs
	{
		public Point ScrollPosition { get; private set; }
		
		public ScrollEventArgs(Point scrollPosition)
		{
			this.ScrollPosition = scrollPosition;
		}
	}
	
	public enum BorderType
	{
		Bezel,
		Line,
		None
	}
	
	public class Scrollable : Container
	{
		IScrollable inner;
		
		public event EventHandler<ScrollEventArgs> Scroll;
		
		public virtual void OnScroll(ScrollEventArgs e)
		{
			if (Scroll != null) Scroll(this, e);
		}
		
		public Scrollable() : this(Generator.Current) {}

		public Scrollable(Generator g) : base(g, typeof(IScrollable))
		{
			inner = (IScrollable)Handler;
		}

		public void UpdateScrollSizes()
		{
			inner.UpdateScrollSizes();
		}

		public Point ScrollPosition
		{
			get { return inner.ScrollPosition; }
			set { inner.ScrollPosition = value; }
		}

		public Size ScrollSize
		{
			get { return inner.ScrollSize; }
			set { inner.ScrollSize = value; }
		}
		
		public BorderType Border
		{
			get { return inner.Border; }
			set { inner.Border = value; }
		}

	}
}
