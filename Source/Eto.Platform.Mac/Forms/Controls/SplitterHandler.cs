using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using sd = System.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class SplitterHandler : MacView<NSSplitView, Splitter>, ISplitter
	{
		Control panel1;
		Control panel2;
		int? position;
		SplitterFixedPanel fixedPanel;

		public bool RecurseToChildren { get { return true; } }

		public override NSView ContainerControl { get { return Control; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

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

		static void ResizeSubviews(SplitterHandler handler, sd.SizeF oldSize)
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
				panel1Rect.Location = new sd.PointF(0, 0);
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
				panel1Rect.Location = new sd.PointF(0, 0);
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
			WeakReference handler;

			public SplitterHandler Handler { get { return (SplitterHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void Resize(NSSplitView splitView, sd.SizeF oldSize)
			{
				SplitterHandler.ResizeSubviews(Handler, oldSize);
			}

			public override float ConstrainSplitPosition(NSSplitView splitView, float proposedPosition, int subviewDividerIndex)
			{
				return Handler.Enabled ? proposedPosition : Handler.Position;
			}

			public override void DidResizeSubviews(MonoMac.Foundation.NSNotification notification)
			{
				var subview = Handler.Control.Subviews[0];
				if (subview != null && Handler.Widget.Loaded && Handler.Widget.ParentWindow != null && Handler.Widget.ParentWindow.Loaded)
				{
					Handler.position = Handler.Control.IsVertical ? (int)subview.Frame.Width : (int)subview.Frame.Height;
					Handler.Widget.OnPositionChanged(EventArgs.Empty);
				}
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
					Control.ResizeSubviewsWithOldSize(sd.SizeF.Empty);
			}
		}

		public SplitterOrientation Orientation
		{
			get
			{
				return Control.IsVertical ? SplitterOrientation.Horizontal : SplitterOrientation.Vertical;
			}
			set
			{
				Control.IsVertical = value == SplitterOrientation.Horizontal;
				if (Widget.Loaded)
					Control.ResizeSubviewsWithOldSize(sd.SizeF.Empty);
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
					Control.ResizeSubviewsWithOldSize(sd.SizeF.Empty);
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
			if (position == null)
			{
				switch (fixedPanel)
				{
					case SplitterFixedPanel.Panel1:
						var size1 = panel1.GetPreferredSize(SizeF.MaxValue);
						position = (int)(Orientation == SplitterOrientation.Horizontal ? size1.Width : size1.Height);
						break;
					case SplitterFixedPanel.Panel2:
						var size2 = panel2.GetPreferredSize(SizeF.MaxValue);
						if (Orientation == SplitterOrientation.Horizontal)
							position = (int)(Control.Frame.Width - size2.Width - Control.DividerThickness);
						else
							position = (int)(Control.Frame.Height - size2.Height - Control.DividerThickness);
						break;
				}
			}
			Control.ResizeSubviewsWithOldSize(sd.SizeF.Empty);
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = new SizeF();

			var p1size = panel1.GetPreferredSize(availableSize);
			var p2size = panel2.GetPreferredSize(availableSize);
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
							p2size.Width = Math.Max(p2size.Width, Size.Width - position.Value);
							break;
					}
				}
				if (position != null)
					p1size.Width = position.Value;
				size.Width = p1size.Width + p2size.Width + Control.DividerThickness;
				size.Height = Math.Max(p1size.Height, p2size.Height);
			}
			else
			{
				if (position != null)
					p1size.Height = position.Value;
				size.Height = p1size.Height + p2size.Height + Control.DividerThickness;
				size.Width = Math.Max(p1size.Width, p2size.Width);
			}
			return size;
		}
	}
}
