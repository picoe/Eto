using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Linq;

namespace Eto.Platform.Mac
{
	public interface IMacLayout : IMacAutoSizing
	{
		object LayoutObject { get; }

		void SetContainerSize (SD.SizeF size);

		SD.RectangleF GetPosition (Control control);
		
		void LayoutChildren ();

		void UpdateParentLayout ();
		
		Layout Widget { get; }
	}
	
	public abstract class MacLayout<T, W> : MacObject<T, W>, ILayout, IMacLayout
		where T: NSObject
		where W: Layout
	{
		Layout IMacLayout.Widget {
			get { return Widget; }
		}
		
		public bool AutoSize {
			get { 
				var container = Widget.Container != null ? Widget.Container.Handler as IMacAutoSizing : null;
				if (container != null)
					return container.AutoSize;
				else
					return true;
			}
		}
		
		public virtual object LayoutObject {
			get { return null; }
		}
		
		public virtual void OnPreLoad ()
		{
		}
		
		public virtual void OnLoad ()
		{
		}

		public virtual void OnLoadComplete ()
		{
		}
		
		public virtual SD.RectangleF GetPosition (Control control)
		{
			return ((NSView)control.ControlObject).Frame;
		}
		
		public virtual void SetContainerSize (SD.SizeF size)
		{
			var container = Widget.Container.Handler as IMacContainer;
			if (container != null) {
				if (container.AutoSize)
					container.SetContentSize (size);
			} else {
				var view = Widget.Container.ContainerObject as NSView;
				if (view != null)
					view.SetFrameSize (size);
			}
		}
		
		protected void SizeToFit (Control view)
		{
			var mh = view.Handler as IMacAutoSizing;
			if (mh != null) {
				mh.SizeToFit ();
				return;
			}
			
			var c = view.ControlObject as NSControl;
			if (c != null)
				c.SizeToFit ();
		}
		
		public virtual void Update ()
		{
			LayoutChildren ();	
		}
		
		public abstract void SizeToFit ();
		
		public abstract void LayoutChildren ();
		
		public virtual void UpdateParentLayout ()
		{
			var layout = this as IMacLayout;
			if (layout.Widget.ParentLayout != null) {
				// traverse up the tree to update everything we own
				layout = layout.Widget.ParentLayout.InnerLayout.Handler as IMacLayout;
				layout.UpdateParentLayout ();
			} else {
				if (AutoSize)
					SizeToFit ();
				else {
					foreach (var child in Widget.Controls.Select (r => r.Handler).OfType<IMacContainer>()) {
						child.SizeToFit ();
					}
				}
				
				// layout everything!
				LayoutChildren ();
				foreach (var childContainer in Widget.Container.Children.OfType<Container>()) {
					if (childContainer != null && childContainer.Layout != null) {
						var childLayout = childContainer.Layout.InnerLayout.Handler as IMacLayout;
						if (childLayout != null) {
	#if LOG
							Console.WriteLine ("Laying out {0} with {1} layout", childContainer, childLayout);
	#endif
							childLayout.LayoutChildren ();
						}
					}
				}
				
			}
		}
	}
}
