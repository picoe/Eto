using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Platform.iOS.Forms.Controls;

namespace Eto.Platform.iOS.Forms
{
	public abstract class iosWindow<T, W> : iosContainer<T, W>, IWindow
		where T: UIView
		where W: Window
	{

		public iosWindow ()
		{
		}
		
		#region IWindow implementation
		
		public virtual void Close ()
		{
		}

		public ToolBar ToolBar {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		#endregion

		#region ITextControl implementation
		
		public abstract string Text { get; set; }

		#endregion


	}
}

