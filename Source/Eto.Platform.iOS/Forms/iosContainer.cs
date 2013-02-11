using System;
using Eto.Platform.iOS.Forms.Controls;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosContainer
	{
		UIView ContentControl { get; }

		void SetContentSize (System.Drawing.SizeF size);

		void LayoutComplete ();
		void LayoutStarted ();
	}
	
	public class iosContainer<T, W> : iosControl<T, W>, IContainer, IiosContainer
		where T: UIView
		where W: Container
	{
		public iosContainer ()
		{
		}
		
		public virtual void SetContentSize (System.Drawing.SizeF size)
		{
			
		}
		protected override Eto.Drawing.Size GetNaturalSize ()
		{
			var layout = 
				Widget != null && Widget.Layout != null && Widget.Layout.InnerLayout != null
				? Widget.Layout.InnerLayout.Handler as IiosLayout : null;
			if (layout != null)
				return layout.GetPreferredSize (Size.MaxValue);
			else
				return base.GetNaturalSize ();
		}

		public virtual void LayoutComplete ()
		{
		}

		public virtual void LayoutStarted ()
		{
		}

		public virtual void SetLayout (Layout layout)
		{
		}

		public virtual UIView ContentControl
		{
			get { return Control; }
		}

		public virtual Eto.Drawing.Size ClientSize {
			get { return this.Size; }
			set { this.Size = value; }
		}

		public object ContainerObject {
			get { return this.ContentControl; }
		}

		bool disposed;

		protected override void Dispose (bool disposing)
		{
			if (!disposed && Widget != null && Widget.Layout != null && Widget.Layout.InnerLayout != null) {
				foreach (var control in Widget.Controls.OfType <IDisposable>().Reverse ()) {
					control.Dispose ();
				}

				var layout = Widget.Layout.InnerLayout.Handler as IDisposable;
				if (layout != null)
					layout.Dispose ();
				disposed = true;
			}
			base.Dispose (disposing);
		}
	}
}

