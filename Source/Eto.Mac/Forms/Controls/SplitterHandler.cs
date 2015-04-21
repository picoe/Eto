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

		public override void LayoutParent(bool updateSize = true)
		{
			UpdatePosition();
			base.LayoutParent(updateSize);
		}

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

		static void ResizeSubviews(SplitterHandler handler, CGSize oldSize)
		{
			var splitView = handler.Control;
			var dividerThickness = splitView.DividerThickness;
			var newFrame = splitView.Frame;

			if (handler.panel1 == null || !handler.panel1.Visible)
			{
				splitView.Subviews[0].SetFrameSize(SizeF.Empty.ToNS());
				splitView.Subviews[1].Frame = newFrame;
				return;
			}

			if (handler.panel2 == null || !handler.panel2.Visible)
			{
				splitView.Subviews[0].Frame = newFrame;
				splitView.Subviews[1].SetFrameSize(SizeF.Empty.ToNS());
				return;
			}
			if (handler.initialPositionSet)
				handler.SetRelative();

			var panel1Rect = splitView.Subviews[0].Frame;
			var panel2Rect = splitView.Subviews[1].Frame;
				
			if (oldSize.Height <= 0 && oldSize.Width <= 0)
				oldSize = newFrame.Size;
	
			if (splitView.IsVertical)
			{
				panel2Rect.Y = 0;
				panel2Rect.Height = panel1Rect.Height = newFrame.Height;
				panel1Rect = new CGRect(CGPoint.Empty, panel1Rect.Size);
				if (handler.position == null)
				{
					panel1Rect.Width = (nfloat)Math.Max(0, newFrame.Width / 2);
					panel2Rect.Width = (nfloat)Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
				}
				else
				{
					var pos = handler.position.Value;
					switch (handler.fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							panel1Rect.Width = (nfloat)Math.Max(0, Math.Min(newFrame.Width - dividerThickness, pos));
							panel2Rect.Width = (nfloat)Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Width = (nfloat)Math.Max(0, Math.Min(newFrame.Width - dividerThickness, oldSize.Width - pos - dividerThickness));
							panel1Rect.Width = (nfloat)Math.Max(0, newFrame.Width - panel2Rect.Width - dividerThickness);
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Width / oldSize.Width;
							panel1Rect.Width = (nfloat)Math.Round(Math.Max(0, Math.Min(newFrame.Width - dividerThickness, pos * oldscale)));
							panel2Rect.Width = (nfloat)Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
							break;
					}
				}
				panel2Rect.X = (nfloat)Math.Min(panel1Rect.Width + dividerThickness, newFrame.Width);
			}
			else
			{
				panel2Rect.X = 0;
				panel2Rect.Width = panel1Rect.Width = newFrame.Width;
				panel1Rect = new CGRect(CGPoint.Empty, panel1Rect.Size);
				if (handler.position == null)
				{
					panel1Rect.Height = (nfloat)Math.Max(0, newFrame.Height / 2);
					panel2Rect.Height = (nfloat)Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
				}
				else
				{
					var pos = handler.position.Value;
					switch (handler.fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							panel1Rect.Height = (nfloat)Math.Max(0, Math.Min(newFrame.Height - dividerThickness, pos));
							panel2Rect.Height = (nfloat)Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Height = (nfloat)Math.Max(0, Math.Min(newFrame.Height - dividerThickness, oldSize.Height - pos - dividerThickness));
							panel1Rect.Height = (nfloat)Math.Max(0, newFrame.Height - panel2Rect.Height - dividerThickness);
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Height / oldSize.Height;
							panel1Rect.Height = (nfloat)Math.Round(Math.Max(0, Math.Min(newFrame.Height - dividerThickness, pos * oldscale)));
							panel2Rect.Height = (nfloat)Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
							break;
					}
				}
				panel2Rect.Y = (nfloat)Math.Min(panel1Rect.Height + dividerThickness, newFrame.Height);
			}

			splitView.Subviews[0].Frame = panel1Rect;
			splitView.Subviews[1].Frame = panel2Rect;
		}

		public class EtoSplitViewDelegate : NSSplitViewDelegate
		{
			WeakReference handler;

			public SplitterHandler Handler { get { return (SplitterHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void Resize(NSSplitView splitView, CGSize oldSize)
			{
				SplitterHandler.ResizeSubviews(Handler, oldSize);
			}

			public override nfloat ConstrainSplitPosition(NSSplitView splitView, nfloat proposedPosition, nint subviewDividerIndex)
			{
				return Handler.Enabled ? (nfloat)Math.Round(proposedPosition) : Handler.Position;
			}

			public override void DidResizeSubviews(NSNotification notification)
			{
				var h = Handler;
				var subview = h.Control.Subviews[0];
				if (subview != null && h.position != null && h.initialPositionSet && h.Widget.Loaded && h.Widget.ParentWindow != null && h.Widget.ParentWindow.Loaded)
				{
					if (h.panel1 == null || !h.panel1.Visible || h.panel2 == null || !h.panel2.Visible)
					{
						// remember relative position if either panel is not visible
						if (double.IsNaN(h.relative))
							h.relative = h.RelativePosition;
						return;
					}
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
		}


		protected override NSSplitView CreateControl()
		{
			return new EtoSplitView(this);
		}

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

		public override bool Enabled { get; set; }

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
					var view = value.GetContainerView();
					Control.ReplaceSubviewWith(Control.Subviews[0], view ?? new NSView());
					panel1 = value;
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
					var view = value.GetContainerView();
					Control.ReplaceSubviewWith(Control.Subviews[1], view ?? new NSView());
					panel2 = value;
				}
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
						var size1 = panel1.GetPreferredSize(SizeF.MaxValue);
						position = (int)(Orientation == Orientation.Horizontal ? size1.Width : size1.Height);
						break;
					case SplitterFixedPanel.Panel2:
						var size2 = panel2.GetPreferredSize(SizeF.MaxValue);
						if (Orientation == Orientation.Horizontal)
							position = (int)(Control.Frame.Width - size2.Width - Control.DividerThickness);
						else
							position = (int)(Control.Frame.Height - size2.Height - Control.DividerThickness);
						break;
				}
			}
			else if (PreferredSize != null)
			{
				var preferredSize = Orientation == Orientation.Horizontal ? PreferredSize.Value.Width : PreferredSize.Value.Height;
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
			SetInitialSplitPosition();
			UpdatePosition();
			initialPositionSet = true;
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

		void UpdatePosition()
		{
			Control.ResizeSubviewsWithOldSize(CGSize.Empty);
		}
	}
}
