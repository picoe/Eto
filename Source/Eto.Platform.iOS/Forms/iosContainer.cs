using System;
using Eto.Platform.iOS.Forms.Controls;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosContainer
	{
		UIView ContentControl { get; }

		void SetContentSize (System.Drawing.SizeF size);
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

		public override Eto.Drawing.Size? PreferredSize {
			get {
				var layout = Widget.Layout.Handler as IiosLayout;
				if (layout != null)
					return layout.GetPreferredSize ();
				else
					return base.PreferredSize;
			}
			set {
				base.PreferredSize = value;
			}
		}

		#region IContainer implementation
		
		public virtual void SetLayout (Layout layout)
		{
		}

		public virtual UIView ContentControl
		{
			get { return Control; }
		}

		public virtual Eto.Drawing.Size ClientSize {
			get {
				return this.Size;
			}
			set {
				this.Size = value;
			}
		}

		public virtual object ContainerObject {
			get {
				return this.ContentControl;
			}
		}
		#endregion

		bool disposed;

		protected override void Dispose (bool disposing)
		{
			if (!disposed && Widget.Layout != null) {
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

