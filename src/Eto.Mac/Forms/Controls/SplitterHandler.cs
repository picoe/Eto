using System;
using Eto.Forms;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class SplitterHandler : MacContainer<NSSplitView, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		double relative = double.NaN;

		double GetRelativePosition()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				return pos;
			var size = Orientation == Orientation.Horizontal ? Control.Bounds.Width : Control.Bounds.Height;
			size -= SplitterWidth;
			if (fixedPanel == SplitterFixedPanel.Panel2)
				return size - pos;
			return pos / (double)size;
		}

		public double RelativePosition
		{
			get
			{
				if (!Widget.Loaded)
					return relative;
				
				return GetRelativePosition();
			}
			set
			{
				relative = value;
				if (Widget.Loaded)
				{
					SetRelative();
					UpdatePosition();
				}
			}
		}

		void SetRelative()
		{
			if (double.IsNaN(relative))
				return;

			if (fixedPanel == SplitterFixedPanel.Panel1)
				position = (int)Math.Round(relative);
			else
			{
				var size = Orientation == Orientation.Horizontal ? Control.Bounds.Width : Control.Bounds.Height;
				size -= SplitterWidth;
				if (size <= 1)
					return;
				if (fixedPanel == SplitterFixedPanel.Panel2)
					position = (int)Math.Round(size - relative);
				else
					position = (int)Math.Round(size * relative);
			}
			relative = double.NaN;
		}

		public int SplitterWidth
		{
			get { return (int)Math.Round(Control.DividerThickness); }
			set {
				if (value <= 2)
					Control.DividerStyle = NSSplitViewDividerStyle.Thin;
				else
					Control.DividerStyle = NSSplitViewDividerStyle.Thick;
			}
		}


		Control panel1;
		Control panel2;
		int? position;
		SplitterFixedPanel fixedPanel;
		bool initialPositionSet;
		int panel1MinimumSize;
		int panel2MinimumSize;
		static readonly object WasLoaded_Key = new object();

		bool WasLoaded
		{ 
			get { return Widget.Properties.Get<bool>(WasLoaded_Key); } 
			set { Widget.Properties.Set(WasLoaded_Key, value); }
		}

		public override NSView ContainerControl { get { return Control; } }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					// handled by delegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		CGSize? oldSizeCache;
		void ResizeSubviews(CGSize oldSize2)
		{
			var splitView = Control;
			var dividerThickness = splitView.DividerThickness;
			var newFrame = splitView.Frame;

			var panel1Rect = splitView.Subviews[0].Frame;
			var panel2Rect = splitView.Subviews[1].Frame;

			bool isSmall = newFrame.Height <= 1 || newFrame.Width <= 1;
			if (!isSmall && initialPositionSet)
				SetRelative();

			var oldSize = oldSizeCache ?? newFrame.Size;
			oldSizeCache = newFrame.Size;

			if (oldSize.Height <= 0 && oldSize.Width <= 0)
				oldSize = newFrame.Size;

			if (panel1?.Visible != true)
			{
				if (splitView.IsVertical)
				{
					panel1Rect = new CGRect(0, 0, 0, newFrame.Height);
					panel2Rect = new CGRect(dividerThickness, 0, newFrame.Width - dividerThickness, newFrame.Height);
				}
				else
				{
					panel1Rect = new CGRect(0, 0, newFrame.Width, 0);
					panel2Rect = new CGRect(0, dividerThickness, newFrame.Width, newFrame.Height - dividerThickness);
				}
			}
			else if (panel2?.Visible != true)
			{
				if (splitView.IsVertical)
				{
					panel1Rect = new CGRect(0, 0, Math.Max(0, newFrame.Width - dividerThickness), newFrame.Height);
					panel2Rect = new CGRect(newFrame.Width, 0, 0, newFrame.Height);
				}
				else
				{
					panel1Rect = new CGRect(0, 0, newFrame.Width, Math.Max(0, newFrame.Height - dividerThickness));
					panel2Rect = new CGRect(0, newFrame.Height, newFrame.Width, 0);
				}
			}
			else
			{
				nfloat totalSize;
				nfloat panel1Size;
				nfloat panel2Size;
				nfloat old;
				if (splitView.IsVertical)
				{
					panel2Rect.Y = 0;
					panel2Rect.Height = panel1Rect.Height = newFrame.Height;
					totalSize = newFrame.Width;
					old = oldSize.Width;
				}
				else
				{
					panel2Rect.X = 0;
					panel2Rect.Width = panel1Rect.Width = newFrame.Width;
					totalSize = newFrame.Height;
					old = oldSize.Height;
				}
				panel1Rect = new CGRect(CGPoint.Empty, panel1Rect.Size);

				if (position == null)
				{
					panel1Size = (nfloat)Math.Max(0, totalSize / 2);
					panel2Size = (nfloat)Math.Max(0, totalSize - panel1Size - dividerThickness);
				}
				else {
					var pos = position.Value;
					switch (fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							panel1Size = (nfloat)Math.Max(0, Math.Min(totalSize - dividerThickness, pos));
							panel2Size = (nfloat)Math.Max(0, totalSize - panel1Size - dividerThickness);
							break;
						case SplitterFixedPanel.Panel2:
							panel2Size = (nfloat)Math.Max(0, Math.Min(totalSize - dividerThickness, old - pos - dividerThickness));
							panel1Size = (nfloat)Math.Max(0, totalSize - panel2Size - dividerThickness);
							break;
						default:
						case SplitterFixedPanel.None:
							var oldscale = old > 0 ? totalSize / old : 1;
							panel1Size = (nfloat)Math.Round(Math.Max(0, Math.Min(totalSize - dividerThickness, pos * oldscale)));
							panel2Size = (nfloat)Math.Max(0, totalSize - panel1Size - dividerThickness);
							break;
					}
				}

				// ensure we don't shrink panels beyond minimum sizes
				if (panel2Size < Panel2MinimumSize)
				{
					panel2Size = (nfloat)Math.Min(Panel2MinimumSize, totalSize);
					panel1Size = (nfloat)Math.Max(0, totalSize - panel2Size - dividerThickness);
				}
				if (panel1Size < Panel1MinimumSize)
				{
					panel1Size = (nfloat)Math.Min(Panel1MinimumSize, totalSize);
					panel2Size = (nfloat)Math.Max(0, totalSize - panel1Size - dividerThickness);
				}

				if (splitView.IsVertical)
				{
					panel2Rect.X = (nfloat)Math.Min(panel1Size + dividerThickness, totalSize);
					panel1Rect.Width = panel1Size;
					panel2Rect.Width = panel2Size;
				}
				else
				{
					panel2Rect.Y = (nfloat)Math.Min(panel1Size + dividerThickness, totalSize);
					panel1Rect.Height = panel1Size;
					panel2Rect.Height = panel2Size;
				}
			}

			splitView.Subviews[0].Frame = panel1Rect;
			splitView.Subviews[1].Frame = panel2Rect;
			//Console.WriteLine($"Splitter resize: frame: {splitView.Frame.Size}, position: {position}, panel1({panel1?.Visible}): {panel1Rect}, panel2({panel2?.Visible}): {panel2Rect}");
		}

		public class EtoSplitViewDelegate : NSSplitViewDelegate
		{
			WeakReference handler;

			public SplitterHandler Handler { get { return (SplitterHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void Resize(NSSplitView splitView, CGSize oldSize)
			{
				Handler?.ResizeSubviews(oldSize);
			}

			public override nfloat ConstrainSplitPosition(NSSplitView splitView, nfloat proposedPosition, nint subviewDividerIndex)
			{
				var h = Handler;
				if (h == null)
					return proposedPosition;
				var totalSize = splitView.IsVertical ? splitView.Bounds.Width : splitView.Bounds.Height;

				if (h.Panel1?.Visible != true)
					return 0;
				
				if (h.Panel2?.Visible != true)
					return (nfloat)Math.Max(0, totalSize - splitView.DividerThickness);

				if (!h.Enabled)
					return h.Position;

				if ((h.Panel1MinimumSize > 0 || h.Panel2MinimumSize > 0))
				{
					// constrain to panel 2 minimum size
					proposedPosition = (nfloat)Math.Min(totalSize - h.panel2MinimumSize - splitView.DividerThickness, proposedPosition);
					// constrain to panel 1 minimum size
					proposedPosition = (nfloat)Math.Max(proposedPosition, h.Panel1MinimumSize);
					// constrain to size of control
					proposedPosition = (nfloat)Math.Min(totalSize, proposedPosition);
				}

				return (nfloat)Math.Round(proposedPosition);
			}
			
			public override void DidResizeSubviews(NSNotification notification)
			{
				var h = Handler;
				if (h == null)
					return;
				var subview = h.Control.Subviews[0];
				if (subview != null && h.position != null && h.initialPositionSet && h.Widget.Loaded && h.Control.Window != null) // && h.Widget.ParentWindow != null && h.Widget.ParentWindow.Loaded)
				{
					if (h.panel1 == null || !h.panel1.Visible || h.panel2 == null || !h.panel2.Visible)
					{
						// remember relative position if either panel is not visible
						if (double.IsNaN(h.relative))
							h.relative = h.RelativePosition;
						return;
					}
					var mainFrame = h.Control.Frame;
					if (mainFrame.Width <= 1 || mainFrame.Height <= 1)
						return;
					h.position = h.Control.IsVertical ? (int)subview.Frame.Width : (int)subview.Frame.Height;
					h.Callback.OnPositionChanged(h.Widget, EventArgs.Empty);
				}
			}
		}
		// stupid hack for OSX 10.5 so that mouse down/drag/up events fire in children properly..
		public class EtoSplitView : NSSplitView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SplitterHandler Handler
			{ 
				get { return (SplitterHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoSplitView(SplitterHandler handler)
			{
				DividerStyle = NSSplitViewDividerStyle.Thin;
				IsVertical = true;
				AddSubview(new NSView { AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
				AddSubview(new NSView { AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
				Delegate = new EtoSplitViewDelegate { Handler = handler };
			}

			public override void MouseDown(NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				    || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseDown(theEvent);
			}

			public override void MouseDragged(NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				    || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseDragged(theEvent);
			}

			public override void MouseUp(NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				    || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseUp(theEvent);
			}

			public override void Layout()
			{
				if (MacView.NewLayout)
					base.Layout();
				Handler?.PerformLayout();
				if (!MacView.NewLayout)
					base.Layout();
			}
		}

		private void PerformLayout()
		{
			if (!initialPositionSet && Widget.Loaded)
			{
				SetInitialSplitPosition();
				UpdatePosition();
				initialPositionSet = true;
			}
		}

		protected override NSSplitView CreateControl() => new EtoSplitView(this);

		protected override void Initialize()
		{
			Enabled = true;
			base.Initialize();
		}

		public int Position
		{
			get { return position ?? (int)(Control.IsVertical ? Control.Subviews[0].Frame.Width : Control.Subviews[0].Frame.Height); }
			set
			{
				position = value;
				relative = double.NaN;
				if (Widget.Loaded)
					UpdatePosition();
			}
		}

		public Orientation Orientation
		{
			get
			{
				return Control.IsVertical ? Orientation.Horizontal : Orientation.Vertical;
			}
			set
			{
				Control.IsVertical = value == Orientation.Horizontal;
				if (Widget.Loaded)
					UpdatePosition();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				fixedPanel = value;
				if (Widget.Loaded)
				{
					if (double.IsNaN(relative))
						relative = RelativePosition;
					UpdatePosition();
				}
				else if (WasLoaded)
				{
					relative = GetRelativePosition();
				}
			}
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				if (panel1 != value)
				{
					var view = value.GetContainerView() ?? new NSView();
					view.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					Control.ReplaceSubviewWith(Control.Subviews[0], view);
					panel1 = value;
					if (Widget.Loaded)
						UpdatePosition();
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				if (panel2 != value)
				{
					var view = value.GetContainerView() ?? new NSView();
					view.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					Control.ReplaceSubviewWith(Control.Subviews[1], view);
					panel2 = value;
					if (Widget.Loaded)
						UpdatePosition();
				}
			}
		}

		public int Panel1MinimumSize
		{
			get { return panel1MinimumSize; }
			set
			{
				panel1MinimumSize = value;
				if (Widget.Loaded)
					UpdatePosition();
			}
		}

		public int Panel2MinimumSize
		{
			get { return panel2MinimumSize; }
			set
			{
				panel2MinimumSize = value;
				if (Widget.Loaded)
					UpdatePosition();
			}
		}

		void SetInitialSplitPosition()
		{
			if (!double.IsNaN(relative))
				SetRelative();
			else if (position == null)
			{
				switch (fixedPanel)
				{
					case SplitterFixedPanel.None:
					case SplitterFixedPanel.Panel1:
						var size1 = panel1.GetPreferredSize(SizeF.PositiveInfinity);
						position = (int)(Orientation == Orientation.Horizontal ? size1.Width : size1.Height);
						break;
					case SplitterFixedPanel.Panel2:
						var size2 = panel2.GetPreferredSize(SizeF.PositiveInfinity);
						if (Orientation == Orientation.Horizontal)
							position = (int)(Control.Frame.Width - size2.Width - Control.DividerThickness);
						else
							position = (int)(Control.Frame.Height - size2.Height - Control.DividerThickness);
						break;
				}
			}
			else
			{
				var preferredSize = Orientation == Orientation.Horizontal ? UserPreferredSize.Width : UserPreferredSize.Height;
				if (preferredSize == -1)
					return;

				var size = Orientation == Orientation.Horizontal ? Size.Width : Size.Height;
				switch (fixedPanel)
				{
					case SplitterFixedPanel.Panel2:
						position = size - (preferredSize - position.Value);
						break;
					case SplitterFixedPanel.None:
						if (preferredSize > 0)
							position = position.Value * size / preferredSize;
						break;
				}
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			WasLoaded = false;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			WasLoaded = true;
			// remember relative position if we're shown again at a different size
			if (double.IsNaN(relative))
				relative = GetRelativePosition();
			initialPositionSet = false;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = new SizeF();

			var size1 = panel1.GetPreferredSize(availableSize);
			var size2 = panel2.GetPreferredSize(availableSize);
			if (Control.IsVertical)
			{
				if (!double.IsNaN(relative))
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							size1.Width = (float)Math.Round(relative);
							break;
						case SplitterFixedPanel.Panel2:
							size2.Width = (float)Math.Round(relative);
							break;
						case SplitterFixedPanel.None:
							size1.Width = (float)Math.Round(Math.Max(size1.Width/relative, size2.Width/(1-relative)));
							size2.Width = 0;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else if (position != null)
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.None:
						case SplitterFixedPanel.Panel1:
							size1.Width = Math.Max(size1.Width, position.Value);
							break;
						case SplitterFixedPanel.Panel2:
							size2.Width = Math.Max(size2.Width, Size.Width - position.Value);
							break;
					}
				}
				if (position != null)
					size1.Width = position.Value;
				size.Width = (float)(size1.Width + size2.Width + Control.DividerThickness);
				size.Height = Math.Max(size1.Height, size2.Height);
			}
			else
			{
				if (!double.IsNaN(relative))
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							size1.Height = (float)Math.Round(relative);
							break;
						case SplitterFixedPanel.Panel2:
							size2.Height = (float)Math.Round(relative);
							break;
						case SplitterFixedPanel.None:
							size1.Height = (float)Math.Round(Math.Max(size1.Height/relative, size2.Height/(1-relative)));
							size2.Height = 0;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				if (position != null)
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.None:
						case SplitterFixedPanel.Panel1:
							size1.Height = Math.Max(size1.Height, position.Value);
							break;
						case SplitterFixedPanel.Panel2:
							size2.Height = Math.Max(size2.Height, Size.Height - position.Value);
							break;
					}
				}
				if (position != null)
					size1.Height = position.Value;
				size.Height = (float)(size1.Height + size2.Height + Control.DividerThickness);
				size.Width = Math.Max(size1.Width, size2.Width);
			}
			return size;
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			Control.NeedsLayout = true;
		}

		void UpdatePosition()
		{
			if (!Control.InLiveResize)
				Control.ResizeSubviewsWithOldSize(CGSize.Empty);
		}
	}
}
