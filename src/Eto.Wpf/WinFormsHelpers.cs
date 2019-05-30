using Eto.Forms;
using Eto.Wpf.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swf = System.Windows.Forms;

namespace Eto.Wpf
{
	public static class WinFormsHelpers
	{
		/// <summary>
		/// Wraps a Windows Forms control into an Eto control so it can be added to an Eto container.
		/// </summary>
		/// <param name="control">Windows Forms control to wrap</param>
		/// <returns>An Eto control wrapper</returns>
		public static Control ToEto(this swf.Control control)
		{
			if (control == null)
				return null;
			return new Control(new WindowsFormsHostHandler<swf.Control, Control, Control.ICallback>(control));
		}

		/// <summary>
		/// Wraps a native win32 window in an Eto control so it can be used as a parent when showing dialogs, etc.
		/// </summary>
		/// <remarks>
		/// This is useful when your application is fully native and does not use WinForms or Wpf.
		/// </remarks>
		/// <returns>The eto window wrapper around the win32 window with the specified handle.</returns>
		/// <param name="windowHandle">Handle of the win32 window.</param>
		public static Window ToEtoWindow(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
				return null;
			return new Form(new HwndFormHandler(windowHandle));
		}

		static readonly object Form_Key = new object();

		/// <summary>
		/// Wraps a Windows Forms Form to an Eto window to use for parenting of Eto dialogs and forms
		/// </summary>
		/// <param name="form">Windows Forms Form to wrap</param>
		/// <returns>Wrapped Eto window</returns>
		public static Window ToEto(swf.Form form)
		{
			if (form == null)
				return null;
			var etoForm = new Form(new HwndFormHandler(form.Handle));
			etoForm.Properties[Form_Key] = form;
			return etoForm;
		}
	}
}
