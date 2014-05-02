using System;
using Eto.Forms;
using MonoTouch.UIKit;
using sd = System.Drawing;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ScrollableHandler : MacPanel<UIScrollView, Scrollable>, IScrollable
	{
		bool expandContentWidth = true;
		bool expandContentHeight = true;

		UIView Child { get; set; }

		public override UIView ContainerControl { get { return Control; } }

		BorderType border;

		public BorderType Border
		{
			get { return border; }
			set
			{
				border = value;
				switch (border)
				{
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
			WeakReference handler;

			public ScrollableHandler Handler { get { return (ScrollableHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override UIView ViewForZoomingInScrollView(UIScrollView scrollView)
			{
				return Handler.Child;
			}

			public override void DidZoom(UIScrollView scrollView)
			{
				Handler.Adjust();
			}
		}

		void Adjust()
		{
			var scrollBounds = Control.Bounds;
			var contentSize = Control.ContentSize;
			var insets = Control.ContentInset;
			scrollBounds.Width -= insets.Left + insets.Right;
			scrollBounds.Height -= insets.Top + insets.Bottom;

			// keep content in the center of the scroll area, if smaller
			var location = new sd.PointF(Math.Max(0, (scrollBounds.Width - contentSize.Width) / 2f), Math.Max(0, (scrollBounds.Height - contentSize.Height) / 2f));

			IsResizing = true;
			Child.SetFrameOrigin(location);
			IsResizing = false;
		}

		public override UIView ContentControl
		{
			get { return Child; }
		}

		public override bool Enabled
		{
			get { return Control.ScrollEnabled; }
			set { Control.ScrollEnabled = value; }
		}

		public float MinimumZoom
		{
			get { return Control.MinimumZoomScale; }
			set { Control.MinimumZoomScale = value; }
		}

		public float MaximumZoom
		{
			get { return Control.MaximumZoomScale; }
			set { Control.MaximumZoomScale = value; }
		}

		public float Zoom
		{
			get { return Control.ZoomScale; }
			set { Control.SetZoomScale(value, false); }
		}

		public ScrollableHandler()
		{
			Child = new UIView();

			Control = new UIScrollView();
			Control.BackgroundColor = UIColor.White;
			Control.ContentMode = UIViewContentMode.TopLeft;
			Control.ScrollEnabled = true;
			Control.Delegate = new Delegate { Handler = this };
			Control.AddSubview(Child);
			ExpandContentHeight = ExpandContentWidth = true;

			/*
			foreach (var gestureRecognizer in Control.GestureRecognizers.OfType<UIPanGestureRecognizer>()) {
				gestureRecognizer.MinimumNumberOfTouches = 2;
				gestureRecognizer.MaximumNumberOfTouches = 2;
			}*/			
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			return SizeF.Min(availableSize, base.GetPreferredSize(availableSize));
		}

		public void UpdateScrollSizes()
		{
			if (Widget.Loaded)
				SetContentSize(Content.GetPreferredSize(Size.MaxValue).ToSD());
		}

		public Point ScrollPosition
		{
			get
			{
				var inset = Control.ContentInset;
				var offset = Control.ContentOffset;
				return new Point((int)Math.Round((offset.X + inset.Left) / Control.ZoomScale), (int)Math.Round((offset.Y + inset.Top) / Control.ZoomScale));
			}
			set
			{
				Control.SetContentOffset(new sd.PointF(value.X * Control.ZoomScale, value.Y * Control.ZoomScale), false);
			}
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();
			UpdateScrollSizes();
		}

		public Size ScrollSize
		{
			get
			{
				return new Size((int)Math.Round(Control.ContentSize.Width / Control.ZoomScale), (int)Math.Round(Control.ContentSize.Height / Control.ZoomScale));
			}
			set
			{
				var size = value.ToSDSizeF();
				size = new sd.SizeF(size.Width * Control.ZoomScale, size.Height * Control.ZoomScale);
				Child.SetFrameSize(size);
				Control.ContentSize = size;
				Adjust();
			}
		}

		public override void SetContentSize(sd.SizeF contentSize)
		{
			if (!Widget.Loaded)
				return;
			contentSize = new sd.SizeF(contentSize.Width * Control.ZoomScale, contentSize.Height * Control.ZoomScale);
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = Math.Max(contentSize.Height, MinimumSize.Height);
			}
			if (ExpandContentWidth)
				contentSize.Width = Math.Max(ClientSize.Width, contentSize.Width);
			if (ExpandContentHeight)
				contentSize.Height = Math.Max(ClientSize.Height, contentSize.Height);
			Control.ContentSize = contentSize;
			Child.SetFrameSize(contentSize);
			Adjust();
		}

		public Rectangle VisibleRect
		{
			get
			{
				var size = new Size((int)Math.Min(ClientSize.Width / Control.ZoomScale, ScrollSize.Width), (int)Math.Min(ClientSize.Height / Control.ZoomScale, ScrollSize.Height));
				return new Rectangle(ScrollPosition, size);
			}
		}

		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value)
				{
					expandContentWidth = value;
					UpdateScrollSizes();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set
			{
				if (expandContentHeight != value)
				{
					expandContentHeight = value;
					UpdateScrollSizes();
				}
			}
		}
	}
}

