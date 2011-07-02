using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class SplitterHandler : MacView<NSSplitView, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		NSSplitView control;
		int? position;
		
		class SVDelegate : NSSplitViewDelegate
		{
			public SplitterHandler Handler { get; set; }
			
			public override void Resize (NSSplitView splitView, System.Drawing.SizeF oldSize)
			{
				var dividerThickness = splitView.DividerThickness;
				var leftRect = splitView.Subviews[0].Frame;
				var rightRect = splitView.Subviews[1].Frame;
				var newFrame = splitView.Frame;
	
				leftRect.Height = newFrame.Height;
				leftRect.Location = new System.Drawing.PointF(0, 0);
				if (Handler.position == null) leftRect.Width = newFrame.Width / 2;
				else leftRect.Width = Handler.position.Value;
				rightRect.Width = newFrame.Width - leftRect.Width - dividerThickness;
				rightRect.Height = newFrame.Height;
				rightRect.X = leftRect.Width + dividerThickness;
				
				splitView.Subviews[0].Frame = leftRect;
				splitView.Subviews[1].Frame = rightRect;
			} 
			
			public override void DidResizeSubviews (MonoMac.Foundation.NSNotification notification)
			{
				var subview = Handler.control.Subviews[0];
				if (subview != null) Handler.position = (int)subview.Frame.Width;
			}
		}
		
		// stupid hack for OSX 10.5 so that mouse down/drag/up events fire in children properly..
		class MySplitView: NSSplitView
		{
			public override void MouseDown (NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				 || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseDown (theEvent);
			}
			
			public override void MouseDragged (NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				 || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseDragged (theEvent);
			}
			
			public override void MouseUp (NSEvent theEvent)
			{
				var cursor = NSCursor.CurrentCursor;
				if (cursor == NSCursor.ResizeLeftCursor || cursor == NSCursor.ResizeRightCursor || cursor == NSCursor.ResizeLeftRightCursor
				 || cursor == NSCursor.ResizeUpCursor || cursor == NSCursor.ResizeDownCursor || cursor == NSCursor.ResizeUpDownCursor)
					base.MouseUp(theEvent);
			}
		}
		
		public SplitterHandler()
		{
			
			control = new MySplitView();
			control.DividerStyle = NSSplitViewDividerStyle.PaneSplitter;
			control.AddSubview(new NSView{ AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
			control.AddSubview(new NSView{ AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable });
			control.IsVertical = true;
			control.Delegate = new SVDelegate{ Handler = this };
			Control = control;
		}
		
		#region ISplitter Members

		public int Position
		{
			get { return position ?? 0; }
			set {
				position = value;
				var adjview = control.Subviews[0];
				adjview.SetFrameSize(new System.Drawing.SizeF(adjview.Frame.Height, (float)position));
				//control.AdjustSubviews();
			}
		}
		
		public SplitterOrientation Orientation
		{
			get
			{
				if (control.IsVertical) return SplitterOrientation.Vertical;
				else return SplitterOrientation.Horizontal;
			}
			set
			{
				switch (value)
				{
					default:
					case SplitterOrientation.Horizontal: control.IsVertical = false; break;
					case SplitterOrientation.Vertical: control.IsVertical = true; break;
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
					NSView view = (value != null) ? value.ControlObject as NSView : null;
					control.ReplaceSubviewWith(control.Subviews[0], view ?? new NSView());
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
					NSView view = (value != null) ? value.ControlObject as NSView : null;
					control.ReplaceSubviewWith(control.Subviews[1], view ?? new NSView());
					panel2 = value;
				}
			}
		}
		
		#endregion
	}
}
