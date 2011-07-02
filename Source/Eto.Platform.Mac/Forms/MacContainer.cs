using System;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using SD = System.Drawing;

namespace Eto.Platform.Mac
{
	public interface IMacContainer
	{
		void SetContentSize(SD.SizeF contentSize);
	}
	
	public abstract class MacContainer<T, W> : MacView<T, W>, IContainer, IMacContainer
		where T: NSView
		where W: Container
	{
	

		public abstract object ContainerObject {
			get;
		}
		
		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}
		
		public virtual void SetLayout(Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null) return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null)
			{
				var container = ContainerObject as NSView;
				control.SetFrameSize(container.Frame.Size);
				container.AddSubview(control);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			
			if (this.AutoSize && Widget.Layout != null) {
				var layout = Widget.Layout.Handler as IMacLayout;
				if (layout != null) layout.SizeToFit ();
			}
		}
		
		
		#region IMacContainer implementation
		public virtual void SetContentSize (SD.SizeF contentSize)
		{
			Control.SetFrameSize (contentSize);
		}
		#endregion
	}
}

