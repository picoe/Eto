using Eto.Mac.Forms.Controls;



namespace Eto.Mac.Forms
{
	public class PixelLayoutHandler : MacContainer<NSView, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		MacEventView container;
		public override NSView ContainerControl => container;

		public class PixelLayoutView : NSView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public PixelLayoutHandler Handler
			{
				get { return (PixelLayoutHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}

			public PixelLayoutView()
			{
				AutoresizesSubviews = false;
			}

			public override bool IsFlipped => true;

			public override void Layout()
			{
				if (MacView.NewLayout)
					base.Layout();
				Handler?.PerformLayout();
				if (!MacView.NewLayout)
					base.Layout();
			}
		}

		protected override NSView CreateControl()
		{
			container = new MacEventView { Handler = this };
			var control = new PixelLayoutView { Handler = this };
			container.ContentView = control;
			return control;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var naturalSize = NaturalSize;
			if (naturalSize != null)
				return naturalSize.Value;
			var size = SizeF.Empty;
			foreach (var item in Widget.Controls)
			{
				var frameSize = item.GetPreferredSize(availableSize);
				size = SizeF.Max(size, frameSize + new SizeF(item.Location));
			}
			NaturalSize = size;
			return size;
		}

		void SetPosition(Control control, PointF point)
		{
			var macView = control.GetMacViewHandler();
			var availableSize = Widget.Loaded ? Size.MaxValue : macView.GetAlignmentFrame().Size.ToEtoSize();
			var preferredSize = control.GetPreferredSize(availableSize);

			var origin = point.ToNS();
			if (!Control.IsFlipped)
			{
				origin.Y = Control.Frame.Height - origin.Y - preferredSize.Height;
			}

			macView.SetAlignmentFrame(new CGRect(origin, preferredSize.ToNS()));
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			Control.NeedsLayout = true;
		}

		void PerformLayout()
		{
			// set sizes of controls when resizing since available size changes, 
			// it may change the preferred size of the children.
			foreach (var control in Widget.Controls)
			{
				var macView = control.GetMacViewHandler();
				if (macView == null)
					continue;

				var preferredSize = control.GetPreferredSize(SizeF.PositiveInfinity);
				macView.SetAlignmentFrameSize(preferredSize.ToNS());
			}
		}

		public void Add(Control child, int x, int y)
		{
			var location = new Point(x, y);
			var childView = child.GetContainerView();
			childView.AutoresizingMask = NSViewResizingMask.NotSizable;
			SetPosition(child, location);
			Control.AddSubview(childView);
			InvalidateMeasure();
		}

		public void Move(Control child, int x, int y)
		{
			var location = new Point(x, y);
			SetPosition(child, location);
			InvalidateMeasure();
		}

		public void Remove(Control child)
		{
			var childView = child.GetContainerView();
			childView.RemoveFromSuperview();
			InvalidateMeasure();
		}
	}
}
