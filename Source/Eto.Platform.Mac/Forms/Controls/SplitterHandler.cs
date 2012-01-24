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
		SplitterFixedPanel fixedPanel;
		
		class SVDelegate : NSSplitViewDelegate
		{
			bool initialized;
			public SplitterHandler Handler { get; set; }
			
			public override void Resize (NSSplitView splitView, System.Drawing.SizeF oldSize)
			{
				var dividerThickness = splitView.DividerThickness;
				var panel1Rect = splitView.Subviews[0].Frame;
				var panel2Rect = splitView.Subviews[1].Frame;
				var newFrame = splitView.Frame;
				
				if (oldSize.IsEmpty)
					oldSize = newFrame.Size;
	
				if (splitView.IsVertical) {
					panel2Rect.Height = panel1Rect.Height = newFrame.Height;
					panel1Rect.Location = new System.Drawing.PointF(0, 0);
					if (Handler.position == null) {
						panel1Rect.Width = newFrame.Width / 2;
						panel2Rect.Width = newFrame.Width - panel1Rect.Width - dividerThickness;
					}
					else {
						switch (Handler.fixedPanel) {
						case SplitterFixedPanel.Panel1:
							panel1Rect.Width = Handler.position.Value;
							panel2Rect.Width = newFrame.Width - panel1Rect.Width - dividerThickness;
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Width = oldSize.Width - Handler.position.Value - dividerThickness;
							panel1Rect.Width = newFrame.Width - panel2Rect.Width - dividerThickness;
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Width / oldSize.Width;
							panel1Rect.Width = Handler.position.Value * oldscale;
							panel2Rect.Width = newFrame.Width - panel1Rect.Width - dividerThickness;
							break;
						}
					}
					panel2Rect.X = panel1Rect.Width + dividerThickness;
				}
				else {
					panel2Rect.X = 0;
					panel2Rect.Width = panel1Rect.Width = newFrame.Width;
					panel1Rect.Location = new System.Drawing.PointF(0, 0);
					if (Handler.position == null) {
						panel1Rect.Height = newFrame.Height / 2;
						panel2Rect.Height = newFrame.Height - panel1Rect.Height - dividerThickness;
					}
					else {
						switch (Handler.fixedPanel) {
						case SplitterFixedPanel.Panel1:
							panel1Rect.Height = Handler.position.Value;
							panel2Rect.Height = newFrame.Height - panel1Rect.Height - dividerThickness;
							break;
						case SplitterFixedPanel.Panel2:
							panel2Rect.Height = oldSize.Height - Handler.position.Value - dividerThickness;
							panel1Rect.Height = newFrame.Height - panel2Rect.Height - dividerThickness;
							break;
						case SplitterFixedPanel.None:
							var oldscale = newFrame.Height / oldSize.Height;
							panel1Rect.Height = Handler.position.Value * oldscale;
							panel2Rect.Height = newFrame.Height - panel1Rect.Height - dividerThickness;
							break;
						}
					}
					panel2Rect.Y = panel1Rect.Height + dividerThickness;
				}
				initialized = true;
				
				splitView.Subviews[0].Frame = panel1Rect;
				splitView.Subviews[1].Frame = panel2Rect;
			} 


			public override float ConstrainSplitPosition (NSSplitView splitView, float proposedPosition, int subviewDividerIndex)
			{
				if (Handler.Enabled)
					return proposedPosition;
				else 
					return Handler.Position;
			}
			
			public override void DidResizeSubviews (MonoMac.Foundation.NSNotification notification)
			{
				var subview = Handler.control.Subviews[0];
				if (subview != null && initialized) {
					if (Handler.control.IsVertical) {
						Handler.position = (int)subview.Frame.Width;
					}
					else {
						Handler.position = (int)subview.Frame.Height;
					}
				}
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
			Enabled = true;
			control = new MySplitView();
			control.DividerStyle = NSSplitViewDividerStyle.Thin;
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
				control.AdjustSubviews();
			}
		}
		
		public SplitterOrientation Orientation
		{
			get
			{
				if (control.IsVertical) return SplitterOrientation.Horizontal;
				else return SplitterOrientation.Vertical;
			}
			set
			{
				switch (value)
				{
					default:
					case SplitterOrientation.Horizontal: control.IsVertical = true; break;
					case SplitterOrientation.Vertical: control.IsVertical = false; break;
				}
				control.AdjustSubviews();
				control.NeedsDisplay = true;
			}
		}
		
		public override bool Enabled { get; set; }
		
		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set {
				fixedPanel = value;
				control.AdjustSubviews();
				control.NeedsDisplay = true;
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
