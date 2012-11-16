using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.iOS.Forms
{
	public class FormHandler : iosWindow<UIWindow, Form>, IForm
	{
		public FormHandler ()
		{
			Control = new UIWindow(UIScreen.MainScreen.Bounds);
			Control.AutosizesSubviews = true;
		}
		
		public override string Title {
			get { return null; }
			set { }
		}
		
		public override void Close ()
		{
			Control.RemoveFromSuperview();
			/*
			var viewControllers = Controller.NavigationController.ViewControllers.ToList();
			int index = viewControllers.IndexOf(Controller);
			if (index > 1) Controller.NavigationController.PopToViewController(viewControllers[index-1], true);
			*/
		}
		
		#region IForm implementation
		
		public void Show ()
		{
			Control.MakeKeyAndVisible();
			//ApplicationHandler.Instance.Navigation.PushViewController(Controller, true);
		}
		
		#endregion

		#region IControl implementation

		public Eto.Drawing.Point ScreenToWorld (Eto.Drawing.Point p)
		{
			throw new NotImplementedException ();
		}

		public Eto.Drawing.Point WorldToScreen (Eto.Drawing.Point p)
		{
			throw new NotImplementedException ();
		}

		public DragDropEffects DoDragDrop (object data, DragDropEffects allowedEffects)
		{
			throw new NotImplementedException ();
		}

		public void SetControl (object control)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IForm implementation

		public Eto.Drawing.Color TransparencyKey {
			get;
			set;
		}

		public bool KeyPreview {
			get;
			set;
		}

		#endregion
	}
}

