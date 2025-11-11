namespace Eto.Wpf.Forms
{
	public class NativeFormHandler : WpfWindow<sw.Window, Form, Form.ICallback>, Form.IHandler
	{
		public static Window Create(sw.Window window)
		{
			foreach (var w in Application.Instance.Windows)
			{
				if (w.ControlObject is sw.Window etoWindow && etoWindow == window)
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

		public NativeFormHandler(sw.Window window)
		{
			Control = window;
		}

		protected override bool IsAttached => true;

		public bool ShowActivated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CanFocus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Show()
		{
			throw new NotImplementedException();
		}
	}
}
