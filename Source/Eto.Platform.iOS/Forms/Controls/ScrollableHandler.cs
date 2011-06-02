using System;
using Eto.Forms;
using MonoTouch.UIKit;
using SD = System.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ScrollableHandler : iosContainer<UIScrollView, Scrollable>, IScrollable
	{
		UIView Child { get; set; }
		
		class Delegate : UIScrollViewDelegate
		{
			public ScrollableHandler Handler { get; set; }
			
			public override UIView ViewForZoomingInScrollView (UIScrollView scrollView)
			{
				return Handler.Child;
			}
			
			public override void DidZoom (UIScrollView scrollView)
			{
				Handler.Adjust();
			}
		}
		
		void Adjust()
		{
			var scrollView = Control;
			var imageView = Child;
			var innerFrame = imageView.Frame;
			var scrollerBounds = scrollView.Bounds;
			
			/*
			if ( ( innerFrame.Size.Width < scrollerBounds.Size.Width ) || ( innerFrame.Size.Height < scrollerBounds.Size.Height ) )
			{
			    var tempx = imageView.Center.X - ( scrollerBounds.Size.Width / 2 );
			    var tempy = imageView.Center.Y - ( scrollerBounds.Size.Height / 2 );
			   // scrollView.ContentOffset = new SD.PointF(tempx, tempy);
			}*/
			
			var anEdgeInset = new UIEdgeInsets(0, 0, 0, 0);
			if ( scrollerBounds.Size.Width > innerFrame.Size.Width )
			{
			    anEdgeInset.Left = (scrollerBounds.Size.Width - innerFrame.Size.Width) / 2;
			    anEdgeInset.Right = -anEdgeInset.Left;  // I don't know why this needs to be negative, but that's what works
			}
			if ( scrollerBounds.Size.Height > innerFrame.Size.Height )
			{
			    anEdgeInset.Top = (scrollerBounds.Size.Height - innerFrame.Size.Height) / 2;
			    anEdgeInset.Bottom = -anEdgeInset.Top;  // I don't know why this needs to be negative, but that's what works
			}
			scrollView.ContentInset = anEdgeInset;				
			//Console.WriteLine("Content inset: {0}", anEdgeInset);
		}
		
		public override object ContainerObject {
			get {
				return Child;
			}
		}
		
		public ScrollableHandler ()
		{
			Child = new UIView();

			Control = new UIScrollView();
			//Control.ZoomScale = 0.5F;
			Control.ContentMode = UIViewContentMode.Center;
			Control.ScrollEnabled = true;
			Control.MinimumZoomScale = 0.25F;
			Control.MaximumZoomScale = 3.0F;
			Control.Delegate = new Delegate { Handler = this };
			Control.AddSubview(Child);
		}

		public void UpdateScrollSizes ()
		{
			SD.SizeF size = new SD.SizeF(0, 0);
			Control.ContentOffset = SD.PointF.Empty;
			foreach (var c in Widget.Controls)
			{
				var view = c.ControlObject as UIView;
				if (view != null)
				{
					var frame = view.Frame;
					if (size.Width < frame.Right) size.Width = frame.Right;
					if (size.Height < frame.Bottom) size.Height = frame.Bottom;
				}
			}
			size = new System.Drawing.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
			Child.SetFrameSize(size);
			Control.ContentSize = size;
			Adjust();
		}

		public Eto.Drawing.Point ScrollPosition {
			get {
				return Generator.ConvertF(Control.ContentOffset);
			}
			set {
				Control.SetContentOffset(Generator.ConvertF(value), false);
			}
		}

		public Eto.Drawing.Size ScrollSize {
			get {
				return Generator.ConvertF(Control.ContentSize);
			}
			set {
				var size = Generator.ConvertF(value);
				size = new System.Drawing.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Child.SetFrameSize(size);
				Control.ContentSize = size;//new System.Drawing.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Adjust();
			}
		}

		public bool AutoScrollToControl {
			get {
				return false;
			}
			set {
			}
		}

	}
}

