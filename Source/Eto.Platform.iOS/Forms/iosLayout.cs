using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosLayout {
		object LayoutObject { get; }	
		Size GetPreferredSize (Size availableSize);
	}
	
	public abstract class iosLayout<T, W> : iosContainer<T, W>, ILayout, IiosLayout
		where T: UIView
		where W: Layout
	{
		public virtual object LayoutObject
		{
			get { return null; }
		}
		
		public virtual void SizeToFit()
		{
		}
		
		public virtual void Update()
		{
			Layout ();
		}

		public virtual void UpdateParentLayout()
		{
		}

		public virtual void Layout()
		{
			var container = this.Widget.Container.Handler as IiosContainer;
			container.LayoutStarted ();
			LayoutChildren ();
			container.LayoutComplete ();
		}

		public abstract void LayoutChildren ();


		public virtual void SetContainerSize(SD.SizeF size)
		{
			var container = Widget.Container.Handler as IiosContainer;
			if (container != null) container.SetContentSize (size);
			else {
				/*var view = Widget.Container.ContainerObject as NSView;
				if (view != null) view.SetFrameSize (size);*/
			}
		}

	}
}
