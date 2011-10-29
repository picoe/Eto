using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{
	public class TabPageHandler : WidgetHandler<NSTabViewItem, TabPage>, ITabPage, IMacContainer
	{
		const int ICON_PADDING = 2;
		Image image;
		bool focus;
		static IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");
		
		class MyTabViewItem : NSTabViewItem
		{
			public TabPageHandler Handler { get; set; }
			
			public override void DrawLabel (bool shouldTruncateLabel, SD.RectangleF labelRect)
			{
				if (Handler.image != null) {
					var nsimage = Handler.image.ControlObject as NSImage;

					if (nsimage.RespondsToSelector(new Selector(selDrawInRectFromRectOperationFractionRespectFlippedHints)))
						nsimage.Draw (new SD.RectangleF (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new SD.RectangleF (SD.PointF.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1, true, null);
					else {
						#pragma warning disable 618
						nsimage.Flipped = this.View.IsFlipped;
						#pragma warning restore 618
						nsimage.Draw (new SD.RectangleF (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new SD.RectangleF (SD.PointF.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1);
					}
					
					labelRect.X += labelRect.Height + ICON_PADDING;
					labelRect.Width -= labelRect.Height + ICON_PADDING;
					base.DrawLabel (shouldTruncateLabel, labelRect);
				}
				base.DrawLabel (shouldTruncateLabel, labelRect);
			}
			
			public override SD.SizeF SizeOfLabel (bool computeMin)
			{
				var size = base.SizeOfLabel (computeMin);
				if (Handler.image != null) {
					size.Width += size.Height + ICON_PADDING;
				}
				return size;
			}
		}
		
		public TabPageHandler ()
		{
			Control = new MyTabViewItem{ Handler = this };
			Control.Identifier = new NSString (Guid.NewGuid ().ToString ());
			//Control.View = new NSView();
			Control.Color = NSColor.Blue;
		}

		// TODO: implement this, or remove from base?
		public Cursor Cursor {
			get; set;
		}
		
		public string ToolTip {
			get; set; 
		}
		
		public string Text {
			get { return Control.Label; }
			set { Control.Label = value; }
		}
		
		public Image Image {
			get { return image; }
			set {
				image = value;
				if (image != null) {
				}
			}
		}

		public Size? MinimumSize {
			get;
			set;
		}
		
		public virtual object ContainerObject {
			get {
				return Control.View;
			}
		}

		public virtual void SetLayout (Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null)
				return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null) {
				var container = ContainerObject as NSView;
				control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
				control.SetFrameSize (container.Frame.Size);
				container.AddSubview (control);
			}
		}
		
		public virtual void SetParentLayout (Layout layout)
		{
		}
		
		public virtual void SetParent (Control parent)
		{
		}

		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (focus) 
				Focus ();
			if (this.AutoSize) {
				if (Widget.Layout != null) {
					var layout = Widget.Layout.Handler as IMacLayout;
					if (layout != null)
						layout.SizeToFit ();
				} else
					SetContentSize (SD.SizeF.Empty);
			}
		}
		
		#region IControl implementation
		public void Invalidate ()
		{
			Control.View.NeedsDisplay = true;
		}

		void IControl.Invalidate (Eto.Drawing.Rectangle rect)
		{
			Control.View.SetNeedsDisplayInRect (Generator.ConvertF (rect));
		}

		public Eto.Drawing.Graphics CreateGraphics ()
		{
			throw new NotImplementedException ();
		}

		public void SuspendLayout ()
		{
		}

		public void ResumeLayout ()
		{
		}

		public void Focus ()
		{
			if (Control.View.Window != null)
				Control.View.Window.MakeFirstResponder (Control.View);
			else
				focus = true;
		}

		public virtual Color BackgroundColor {
			get { 
				if (!Control.View.WantsLayer) {
					Control.View.WantsLayer = true;
				}
				return Generator.Convert (Control.View.Layer.BackgroundColor);
			}
			set {
				if (!Control.View.WantsLayer) {
					Control.View.WantsLayer = true;
				}
				Control.View.Layer.BackgroundColor = Generator.Convert (value);
			}
		}

		public string Id {
			get;
			set;
		}

		public Eto.Drawing.Size Size {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Eto.Drawing.Size ClientSize {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool Enabled {
			get {
				return false;
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool HasFocus {
			get {
				return false;
			}
		}

		public bool Visible {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		#endregion

		#region IMacContainer implementation
		public void SetContentSize (SD.SizeF contentSize)
		{
			
		}

		public bool AutoSize {
			get;
			set;
		}
		#endregion
	}
}
