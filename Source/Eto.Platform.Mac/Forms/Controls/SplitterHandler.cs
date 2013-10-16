using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class SplitterHandler : MacView<NSSplitView, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		int? position;
		SplitterFixedPanel fixedPanel;
		bool raiseSplitterMoved;

		public override NSView ContainerControl { get { return Control; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Splitter.PositionChangedEvent:
					this.raiseSplitterMoved = true;
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		static void ResizeSubviews(SplitterHandler handler, System.Drawing.SizeF oldSize)
		{
			var splitView = handler.Control;
			var dividerThickness = splitView.DividerThickness;
			var panel1Rect = splitView.Subviews[0].Frame;
			var panel2Rect = splitView.Subviews[1].Frame;
			var newFrame = splitView.Frame;
				
			if (oldSize.IsEmpty)
				oldSize = newFrame.Size;
	
			if (splitView.IsVertical)
			{
				panel2Rect.Height = panel1Rect.Height = newFrame.Height;
				panel1Rect.Location = new System.Drawing.PointF(0, 0);
				if (handler.position == null)
				{
					panel1Rect.Width = Math.Max(0, newFrame.Width / 2);
					panel2Rect.Width = Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
				}
				else
				{
					var pos = handler.position.Value;
					switch (handler.fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							panel1Rect.Width = Math.Max(0, Math.Min(newFrame.Width - dividerThickness, pos));
							panel2Rect.Width = Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Width = Math.Max(0, Math.Min(newFrame.Width - dividerThickness, oldSize.Width - pos - dividerThickness));
							panel1Rect.Width = Math.Max(0, newFrame.Width - panel2Rect.Width - dividerThickness);
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Width / oldSize.Width;
							panel1Rect.Width = Math.Max(0, Math.Min(newFrame.Width - dividerThickness, pos * oldscale));
							panel2Rect.Width = Math.Max(0, newFrame.Width - panel1Rect.Width - dividerThickness);
							break;
					}
				}
				panel2Rect.X = Math.Min(panel1Rect.Width + dividerThickness, newFrame.Width);
			}
			else
			{
				panel2Rect.X = 0;
				panel2Rect.Width = panel1Rect.Width = newFrame.Width;
				panel1Rect.Location = new System.Drawing.PointF(0, 0);
				if (handler.position == null)
				{
					panel1Rect.Height = Math.Max(0, newFrame.Height / 2);
					panel2Rect.Height = Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
				}
				else
				{
					var pos = handler.position.Value;
					switch (handler.fixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							panel1Rect.Height = Math.Max(0, Math.Min(newFrame.Height - dividerThickness, pos));
							panel2Rect.Height = Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Height = Math.Max(0, Math.Min(newFrame.Height - dividerThickness, oldSize.Height - pos - dividerThickness));
							panel1Rect.Height = Math.Max(0, newFrame.Height - panel2Rect.Height - dividerThickness);
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Height / oldSize.Height;
							panel1Rect.Height = Math.Max(0, Math.Min(newFrame.Height - dividerThickness, pos * oldscale));
							panel2Rect.Height = Math.Max(0, newFrame.Height - panel1Rect.Height - dividerThickness);
							break;
					}
				}
				panel2Rect.Y = Math.Min(panel1Rect.Height + dividerThickness, newFrame.Height);
			}
				
			splitView.Subviews[0].Frame = panel1Rect;
			splitView.Subviews[1].Frame = panel2Rect;
		}

		class SVDelegate : NSSplitViewDelegate
		{
			public SplitterHandler Handler { get; set; }

			public override void Resize(NSSplitView splitView, System.Drawing.SizeF oldSize)
			{
				SplitterHandler.ResizeSubviews(Handler, oldSize);
			}

			public override float ConstrainSplitPosition(NSSplitView splitView, float proposedPosition, int subviewDividerIndex)
			{
				if (Handler.Enabled)
					return proposedPosition;
				else
					return Handler.Position;
			}

			public override void DidResizeSubviews(MonoMac.Foundation.NSNotification notification)
			{
				var subview = Handler.Control.Subviews[0];
				if (subview != null && Handler.Widget.Loaded)
				{
					if (Handler.Control.IsVertical)
					{
						Handler.position = (int)subview.Frame.Width;
					}
					else
					{
						Handler.position = (int)subview.Frame.Height;
					}
				}
				if (Handler.raiseSplitterMoved)
					Handler.Widget.OnPositionChanged(EventArgs.Empty);
			}
		}
		// stupid hack for OSX 10.5 so that mouse down/drag/up events fire in children properly..
		class EtoSplitView : NSSplitView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SplitterHandler Handler
			{ 
				get { return (SplitterHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
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

		public SplitterHandler()
		{
			Enabled = true;
			Control = new EtoSplitView { Handler = this };
			Control.DividerStyle = NSSplitViewDividerStyle.Thin;
			Control.AddSubview(new NSView { AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
			Control.AddSubview(new NSView { AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
			Control.IsVertical = true;
			Control.Delegate = new SVDelegate { Handler = this };
		}

		public int Position
		{
			get { return position ?? 0; }
			set
			{
				position = value;
				if (Widget.Loaded)
					Control.ResizeSubviewsWithOldSize(System.Drawing.SizeF.Empty);
			}
		}

		public SplitterOrientation Orientation
		{
			get
			{
				if (Control.IsVertical)
					return SplitterOrientation.Horizontal;
				else
					return SplitterOrientation.Vertical;
			}
			set
			{
				switch (value)
				{
					default:
					case SplitterOrientation.Horizontal:
						Control.IsVertical = true;
						break;
					case SplitterOrientation.Vertical:
						Control.IsVertical = false;
						break;
				}
				if (Widget.Loaded)
					Control.ResizeSubviewsWithOldSize(System.Drawing.SizeF.Empty);
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
					Control.ResizeSubviewsWithOldSize(System.Drawing.SizeF.Empty);
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

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			Control.ResizeSubviewsWithOldSize(System.Drawing.SizeF.Empty);
		}

		protected override Size GetNaturalSize(Size availableSize)
		{
			Size size = new Size();

			var p1 = panel1.GetMacAutoSizing();
			var p2 = panel2.GetMacAutoSizing();
			var p1size = p1 != null ? p1.GetPreferredSize(availableSize) : Size.Empty;
			var p2size = p2 != null ? p2.GetPreferredSize(availableSize) : Size.Empty;
			if (Control.IsVertical)
			{
				if (position != null)
				{
					switch (FixedPanel)
					{
						case SplitterFixedPanel.None:
						case SplitterFixedPanel.Panel1:
							p1size.Width = Math.Max(p1size.Width, position.Value);
							break;
						case SplitterFixedPanel.Panel2:
							p2size.Width = Math.Max(p2size.Width, this.Size.Width - position.Value);
							break;
					}
				}
				size.Width = p1size.Width + p2size.Width + (int)Control.DividerThickness;
				size.Height = Math.Max(p1size.Height, p2size.Height);
			}
			else
			{
				size.Height = p1size.Height + p2size.Height + (int)Control.DividerThickness;
				size.Width = Math.Max(p1size.Width, p2size.Width);
			}
			return size;
		}
	}
}
