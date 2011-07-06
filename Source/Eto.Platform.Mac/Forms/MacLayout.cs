using System;
using Eto.Forms;
using SD = System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public interface IMacLayout {
		object LayoutObject { get; }
		void SizeToFit();
		void SetContainerSize(SD.SizeF size);
	}
	
	public abstract class MacLayout<T, W> : MacObject<T, W>, ILayout, IMacLayout
		where T: NSObject
		where W: Layout
	{
		
		
		public virtual object LayoutObject
		{
			get { return null; }
		}
		
		public abstract void SizeToFit();
		
		public virtual void OnLoad()
		{
			//Widget.OnLoad (EventArgs.E)	
			
		}
		
		public void SetContainerSize(SD.SizeF size)
		{
			var container = Widget.Container.Handler as IMacContainer;
			if (container != null) container.SetContentSize (size);
			else {
				var view = Widget.Container.ContainerObject as NSView;
				if (view != null) view.SetFrameSize (size);
			}
		}

		protected void AutoSize(Control view)
		{
			var c = view.ControlObject as NSControl;
			var mh = view.Handler as IMacView;
			if (mh != null && !mh.AutoSize) return;
			//Console.WriteLine ("OLD view: {0} size: {1}", view, view.Size);
			
			/* taken care of by MacContainer.OnLoad
			var container = view as Container;
			if (container != null && container.Layout != null)
			{
				var layout = container.Layout.Handler as IMacLayout;
				if (layout != null) layout.SizeToFit();
			}*/

			if (c != null) c.SizeToFit();
			//Console.WriteLine ("view: {0} size: {1}", view, view.Size);
		}
		
	}
}
