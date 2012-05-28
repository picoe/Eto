using System;
using System.Runtime.InteropServices;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Diagnostics;

namespace Eto.Platform.Windows
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		public static bool EnableScrollingUnderMouse = true;
		
		public void RunIteration ()
		{
			SWF.Application.DoEvents ();
		}

		public void Restart ()
		{
			SWF.Application.Restart ();
		}
		
		public void Run (string[] args)
		{
			SWF.Application.EnableVisualStyles ();
			if (!Eto.Misc.Platform.IsMono)
				SWF.Application.DoEvents ();
			
			Application app = ((Application)Widget);
			app.OnInitialized (EventArgs.Empty);

			if (EnableScrollingUnderMouse)
				SWF.Application.AddMessageFilter (new ScrollMessageFilter ());
			
			//SWF.Application.AddMessageFilter(new KeyFilter());
			if (app.MainForm != null && app.MainForm.Loaded) 
				SWF.Application.Run ((SWF.Form)app.MainForm.ControlObject);
			else 
				SWF.Application.Run ();
		}

		public void Quit ()
		{
			SWF.Application.Exit ();
		}
		
		public void Open (string url)
		{
			var info = new ProcessStartInfo (url);
			Process.Start (info);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Application.TerminatingEvent:
				// handled by WindowHandler
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public void GetSystemActions (GenerateActionArgs args, bool addStandardItems)
		{
			
		}

		public void Invoke (Action action)
		{
			if (Widget.MainForm != null) {
				var window = this.Widget.MainForm.ControlObject as SWF.Control;
				if (window == null) window = SWF.Form.ActiveForm;

				if (window != null && window.InvokeRequired) {
					window.Invoke (action);
				}
				else action ();
			}
			else action ();
		}
		
		public void AsyncInvoke (Action action)
		{
			if (Widget.MainForm != null) {
				var window = this.Widget.MainForm.ControlObject as SWF.Control;
				if (window == null) window = SWF.Form.ActiveForm;

				if (window != null && window.InvokeRequired) {
					window.BeginInvoke (action);
				}
				else action ();
			}
			else action ();
		}
		

		public Key CommonModifier {
			get {
				return Key.Control;
			}
		}

		public Key AlternateModifier {
			get {
				return Key.Alt;
			}
		}
		
	}
}
