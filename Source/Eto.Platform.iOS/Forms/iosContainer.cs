using System;
using Eto.Platform.iOS.Forms.Controls;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosContainer
	{
		UIView ContainerControl { get; }

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

		#region IContainer implementation
		
		public virtual void SetLayout (Layout layout)
		{
		}

		public virtual UIView ContainerControl
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
				return this.Control;
			}
		}
		#endregion

	}
}

