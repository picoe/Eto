using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IScrollable : IContainer
	{
		void UpdateScrollSizes ();

		Point ScrollPosition { get; set; }

		Size ScrollSize { get; set; }

		BorderType Border { get; set; }

		Rectangle VisibleRect { get; }

		bool ExpandContentWidth { get; set; }

		bool ExpandContentHeight { get; set; }
	}
	
	public class ScrollEventArgs : EventArgs
	{
		public Point ScrollPosition { get; private set; }
		
		public ScrollEventArgs (Point scrollPosition)
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
	
	public partial class Scrollable : Container
	{
		IScrollable handler;
		public const string ScrollEvent = "Scrollable.ScrollEvent";
		
		event EventHandler<ScrollEventArgs> scroll;

		public event EventHandler<ScrollEventArgs> Scroll {
			add { 
				HandleEvent (ScrollEvent);
				scroll += value; 
			}
			remove { scroll -= value; }
		}
		
		public virtual void OnScroll (ScrollEventArgs e)
		{
			if (scroll != null)
				scroll (this, e);
		}
		
		public Scrollable () : this (Generator.Current)
		{
		}

		public Scrollable (Generator g) : this (g, typeof(IScrollable))
		{
		}
		
		protected Scrollable (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IScrollable)Handler;
		}

		public void UpdateScrollSizes ()
		{
			handler.UpdateScrollSizes ();
		}

		public Point ScrollPosition {
			get { return handler.ScrollPosition; }
			set { handler.ScrollPosition = value; }
		}

		public Size ScrollSize {
			get { return handler.ScrollSize; }
			set { handler.ScrollSize = value; }
		}
		
		public BorderType Border {
			get { return handler.Border; }
			set { handler.Border = value; }
		}
		
		public Rectangle VisibleRect {
			get { return handler.VisibleRect; }
		}

		public bool ExpandContentWidth {
			get { return handler.ExpandContentWidth; }
			set { handler.ExpandContentWidth = value; }
		}

		public bool ExpandContentHeight {
			get { return handler.ExpandContentHeight; }
			set { handler.ExpandContentHeight = value; }
		}
	}
}
