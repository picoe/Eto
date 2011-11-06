using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosLayout {
		object LayoutObject { get; }		
	}
	
	public abstract class iosLayout<T, W> : iosObject<T, W>, ILayout, IiosLayout
		where T: NSObject
		where W: Layout
	{
		public virtual object LayoutObject
		{
			get { return null; }
		}
		
		public virtual void SizeToFit()
		{
		}
		
		public virtual void OnPreLoad ()
		{
		}
		
		public virtual void OnLoad()
		{
		}
		
		public virtual void OnLoadComplete ()
		{
		}
		
		public virtual void Update()
		{
		}
		
		public virtual void SetContainerSize(SD.SizeF size)
		{
			var container = Widget.Container.Handler as IiosContainer;
			if (container != null) container.SetContentSize (size);
			else {
				/*var view = Widget.Container.ContainerObject as NSView;
				if (view != null) view.SetFrameSize (size);*/
			}
		}
		
		protected void AutoSize(Control view)
		{
			var mh = view.Handler as IiosView;
			if (mh != null && !mh.AutoSize) return;
			
			var c = view.ControlObject as UIControl;
			if (c != null) c.SizeToFit ();
		}
		

	}
}
