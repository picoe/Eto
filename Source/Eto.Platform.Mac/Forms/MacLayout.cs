using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Linq;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms
{
	public interface IMacLayout : IMacAutoSizing
	{
		object LayoutObject { get; }

		void SetContainerSize (SD.SizeF size);

		SD.RectangleF GetPosition (Control control);
		
		void LayoutChildren ();

		void UpdateParentLayout (bool updateSize);
		
		Layout Widget { get; }
		
		bool InitialLayout { get; }
	}
	
	public abstract class MacLayout<T, W> : MacObject<T, W>, ILayout, IMacLayout
		where T: NSObject
		where W: Layout
	{
		public bool InitialLayout { get; private set; }
		
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
			// make sure LayoutChildren is called only once per layout, and in parent to child order
			var parentLayoutHandler = Widget.ParentLayout != null ? Widget.ParentLayout.Handler as IMacLayout : null;
			if (parentLayoutHandler == null || parentLayoutHandler.InitialLayout) {
				this.InitialLayout = true;
				LayoutChildren ();
				foreach (var childContainer in Widget.Container.Children.Select (r => r.Handler).OfType<IMacContainer>()) {
					childContainer.LayoutChildren();
				}
			}
			
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
				if (view != null && size != view.Frame.Size)
					view.SetFrameSize (size);
			}
		}
		
		protected Size GetPreferredSize (Control view)
		{
			var mh = view.Handler as IMacAutoSizing;
			if (mh != null) {
				return mh.GetPreferredSize ();
			}
			
			var c = view.ControlObject as NSControl;
			if (c != null) {
				c.SizeToFit ();
				return Generator.ConvertF (c.Frame.Size);
			}
			return Size.Empty;
		}
		
		public virtual void Update ()
		{
			LayoutChildren ();	
		}

		public virtual void AttachedToContainer ()
		{
		}

		public abstract Size GetPreferredSize ();
		
		public abstract void LayoutChildren ();
		
		public void UpdateParentLayout (bool updateSize = true)
		{
			var layout = this as IMacLayout;
			if (layout.Widget.ParentLayout != null) {
				// traverse up the tree to update everything we own
				if (updateSize) {
					if (!AutoSize) {
						foreach (var child in Widget.Controls.Select (r => r.Handler).OfType<IMacContainer>()) {
							var size = child.GetPreferredSize ();
							child.SetContentSize (Generator.ConvertF (size));
						}
						updateSize = false;
					}
				}
				layout = layout.Widget.ParentLayout.InnerLayout.Handler as IMacLayout;
				layout.UpdateParentLayout (updateSize);
			} else {
				if (updateSize) {
					if (AutoSize) {
						var size = GetPreferredSize ();
						SetContainerSize (Generator.ConvertF (size));
					} else {
						foreach (var child in Widget.Controls.Select (r => r.Handler).OfType<IMacContainer>()) {
							var size = child.GetPreferredSize ();
							child.SetContentSize (Generator.ConvertF (size));
						}
					}
				}
				
				// layout everything!
				LayoutChildren ();
				foreach (var childContainer in Widget.Container.Children.Select (r => r.Handler).OfType<IMacContainer>()) {
					childContainer.LayoutChildren();
				}
				
			}
			
		}
	}
}
