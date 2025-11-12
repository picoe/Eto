namespace Eto.WinForms.Forms
{
	public class NativeFormHandler : WindowHandler<swf.Form, Form, Form.ICallback>, Form.IHandler
	{
		public static Window Create(swf.Form window)
		{
			foreach (var w in Application.Instance.Windows)
			{
				if (w.ControlObject is swf.Form etoWindow && etoWindow == window)
					return w;
			}

			for (int i = 0; i < g_windows.Count; i++)
			{
				WeakReference w = g_windows[i];
				if (w.Target is NativeFormHandler handler && handler.Control == window)
				{
					return handler.Widget;
				}
				if (w.Target == null)
				{
					g_windows.RemoveAt(i);
					i--;
				}
			}
			var form = new Form(new NativeFormHandler(window));
			g_windows.Add(new WeakReference(form.Handler));
			return form;
		}

		private static readonly List<WeakReference> g_windows = new();
		
		public override bool IsAttached => true;

		public bool ShowActivated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CanFocus { get => Control.CanFocus; set => throw new NotImplementedException(); }

		public NativeFormHandler(swf.Form form)
		{
			Control = form;
		}
		public void Show() => Control.Show();
	}
}
