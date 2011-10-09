using System;
using Eto.Forms;
using MonoTouch.UIKit;
using SD = System.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ScrollableHandler : iosContainer<UIScrollView, Scrollable>, IScrollable
	{
		UIView Child { get; set; }
		
		public BorderType Border {
			get;
			set;
		}
		
		class Delegate : UIScrollViewDelegate
		{
			public ScrollableHandler Handler { get; set; }
			
			public override UIView ViewForZoomingInScrollView (UIScrollView scrollView)
			{
				return Handler.Child;
			}
			
			public override void DidZoom (UIScrollView scrollView)
			{
				Handler.Adjust ();
			}
		}
		
		void Adjust ()
		{
			var innerView = Child;
			var innerFrame = innerView.Frame;
			var scrollerBounds = Control.Bounds;
			
			var inset = new UIEdgeInsets (0, 0, 0, 0);
			if (scrollerBounds.Size.Width > innerFrame.Size.Width) {
				inset.Left = (scrollerBounds.Size.Width - innerFrame.Size.Width) / 2;
				inset.Right = -inset.Left;  // I don't know why this needs to be negative, but that's what works
			}
			if (scrollerBounds.Size.Height > innerFrame.Size.Height) {
				inset.Top = (scrollerBounds.Size.Height - innerFrame.Size.Height) / 2;
				inset.Bottom = -inset.Top;  // I don't know why this needs to be negative, but that's what works
			}
			Control.ContentInset = inset;				
		}
		
		public override object ContainerObject {
			get {
				return Child;
			}
		}
		
		public override bool Enabled {
			get {
				return Control.ScrollEnabled;
			}
			set {
				Control.ScrollEnabled = value;;
			}
		}
		
		public float MinimumZoom {
			get {
				return Control.MinimumZoomScale;
			}
			set {
				Control.MinimumZoomScale = value;
			}
		}
		
		public float MaximumZoom {
			get {
				return Control.MaximumZoomScale;
			}
			set {
				Control.MaximumZoomScale = value;
			}
		}
		
		public float Zoom {
			get {
				return Control.ZoomScale;
			}
			set {
				Control.SetZoomScale (value, false);
			}
		}
		
		public ScrollableHandler ()
		{
			Child = new UIView ();

			Control = new UIScrollView ();
			Control.ContentMode = UIViewContentMode.Center;
			Control.ScrollEnabled = true;
			Control.Delegate = new Delegate { Handler = this };
			Control.AddSubview (Child);
		}

		public void UpdateScrollSizes ()
		{
			SD.SizeF size = SD.SizeF.Empty;
			foreach (var c in Widget.Controls) {
				var view = c.ControlObject as UIView;
				if (view != null) {
					var frame = view.Frame;
					if (size.Width < frame.Right)
						size.Width = frame.Right;
					if (size.Height < frame.Bottom)
						size.Height = frame.Bottom;
				}
			}
			size = new System.Drawing.SizeF (size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
			Control.ContentSize = size;
			Child.SetFrameSize (size);
			Adjust ();
		}

		public Eto.Drawing.Point ScrollPosition {
			get {
				var inset = Control.ContentInset;
				var offset = Control.ContentOffset;
				return new Eto.Drawing.Point((int)Math.Round(offset.X + inset.Left), (int)Math.Round (offset.Y + inset.Top));
			}
			set {
				Control.SetContentOffset (Generator.ConvertF (value), false);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			UpdateScrollSizes ();
		}

		public Eto.Drawing.Size ScrollSize {
			get {
				return Generator.ConvertF (Control.ContentSize);
			}
			set {
				var size = Generator.ConvertF (value);
				size = new System.Drawing.SizeF (size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Child.SetFrameSize (size);
				Control.ContentSize = size;//new System.Drawing.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Adjust ();
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

