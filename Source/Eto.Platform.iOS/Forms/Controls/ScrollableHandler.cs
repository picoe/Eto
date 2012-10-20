using System;
using Eto.Forms;
using MonoTouch.UIKit;
using SD = System.Drawing;
using System.Linq;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ScrollableHandler : iosContainer<UIScrollView, Scrollable>, IScrollable
	{
		UIView Child { get; set; }
		BorderType border;
		
		public BorderType Border {
			get { return border; }
			set {
				border = value;
				switch (border) {
				case BorderType.Bezel:
					Control.Layer.BorderWidth = 2.0f;
					Control.Layer.BorderColor = UIColor.LightGray.CGColor;
					Control.Layer.CornerRadius = 4f;
					break;
				case BorderType.Line:
					Control.Layer.BorderWidth = 1.0f;
					Control.Layer.BorderColor = UIColor.DarkGray.CGColor;
					Control.Layer.CornerRadius = 0f;
					break;
				case BorderType.None:
					Control.Layer.BorderWidth = 0f;
					Control.Layer.BorderColor = UIColor.DarkGray.CGColor;
					Control.Layer.CornerRadius = 0f;
					break;
				}
			}
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
				Control.ScrollEnabled = value;
				;
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
			Control.BackgroundColor = UIColor.White;
			Control.ContentMode = UIViewContentMode.Center;
			Control.ScrollEnabled = true;
			Control.Delegate = new Delegate { Handler = this };
			Control.AddSubview (Child);
			this.ExpandContentHeight = ExpandContentWidth = true;

			/*
			foreach (var gestureRecognizer in Control.GestureRecognizers.OfType<UIPanGestureRecognizer>()) {
				gestureRecognizer.MinimumNumberOfTouches = 2;
				gestureRecognizer.MaximumNumberOfTouches = 2;
			}*/			
		}

		public void UpdateScrollSizes ()
		{
			var contentSize = this.GetNaturalSize ();
			
			if (ExpandContentWidth)
				contentSize.Width = Math.Max (this.ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max (this.ClientSize.Height, contentSize.Height);
			Control.ContentSize = Generator.Convert (contentSize);
			Child.SetFrameSize (Control.ContentSize);
			Adjust ();
		}

		public Eto.Drawing.Point ScrollPosition {
			get {
				var inset = Control.ContentInset;
				var offset = Control.ContentOffset;
				return new Eto.Drawing.Point ((int)Math.Round ((offset.X + inset.Left) / Control.ZoomScale), (int)Math.Round ((offset.Y + inset.Top) / Control.ZoomScale));
			}
			set {
				Control.SetContentOffset (Generator.ConvertF ((PointF)value * Control.ZoomScale), false);
			}
		}

		public override void LayoutStarted ()
		{
			base.LayoutStarted ();
			UpdateScrollSizes ();
		}
		
		public override void LayoutComplete ()
		{
			base.LayoutComplete ();
			UpdateScrollSizes ();
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			/*
			Widget.SizeChanged += (sender, ee) => {
				UpdateScrollSizes ();
				Widget.Layout.Update ();
			};*/
		}

		public Eto.Drawing.Size ScrollSize {
			get {
				return new Eto.Drawing.Size((int)Math.Round (Control.ContentSize.Width / Control.ZoomScale), (int)Math.Round (Control.ContentSize.Height / Control.ZoomScale));
			}
			set {
				var size = Generator.ConvertF (value);
				size = new System.Drawing.SizeF (size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Child.SetFrameSize (size);
				Control.ContentSize = size;//new System.Drawing.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Adjust ();
			}
		}
		
		public Rectangle VisibleRect {
			get {
				Size size = new Size((int)Math.Min (ClientSize.Width / Control.ZoomScale, ScrollSize.Width), (int)Math.Min(ClientSize.Height / Control.ZoomScale, ScrollSize.Height));
				return new Rectangle(ScrollPosition, size);
			}
		}

		public bool AutoScrollToControl {
			get {
				return false;
			}
			set {
			}
		}

		#region IScrollable implementation
		public bool ExpandContentWidth {
			get;
			set;
		}
		public bool ExpandContentHeight {
			get;
			set;
		}
		#endregion
	}
}

