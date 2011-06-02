using System;
using Eto.Platform.iOS.Forms.Controls;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	public class iosContainer<T, W> : iosControl<T, W>, IContainer
		where T: UIView
		where W: Container
	{
		public iosContainer ()
		{
		}

		#region IContainer implementation
		
		public virtual void SetLayout (Layout layout)
		{
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
				return this.Control;
			}
		}
		#endregion

	}
}

