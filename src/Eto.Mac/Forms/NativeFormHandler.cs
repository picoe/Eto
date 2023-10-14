namespace Eto.Mac.Forms
{
	public class NativeFormHandler : FormHandler
	{
		public NativeFormHandler(NSWindow window)
			: base(window)
		{
		}
		public NativeFormHandler(NSWindowController windowController)
			: base(windowController)
		{
		}

		public override void AttachEvent(string id)
		{
			// native window, so attach observers instead of using the delegate so we don't clobber existing functionality
			switch (id)
			{
				case Window.ClosedEvent:
					AddObserver(NSWindow.WillCloseNotification, n => Callback.OnClosed(Widget, EventArgs.Empty));
					break;
				case Window.SizeChangedEvent:
					AddObserver(NSWindow.DidResizeNotification, n => Callback.OnSizeChanged(Widget, EventArgs.Empty));
					break;
				case Window.GotFocusEvent:
					AddObserver(NSWindow.DidBecomeKeyNotification, n => Callback.OnGotFocus(Widget, EventArgs.Empty));
					break;
				case Window.LostFocusEvent:
					AddObserver(NSWindow.DidResignKeyNotification, n => Callback.OnLostFocus(Widget, EventArgs.Empty));
					break;
			}
			return;
		}

		protected override void ConfigureWindow()
		{
		}

		internal static Window CreateWrapper(NSWindowController windowController, NSWindow nswindow)
		{
			var handle = windowController?.Handle ?? nswindow.Handle;
			
			s_cachedWindows ??= new Dictionary<NativeHandle, WeakReference>();
			
			if (s_cachedWindows.TryGetValue(handle, out var windowReference) && windowReference.Target is Window window)
				return window;
				
			var handler = windowController != null ? new NativeFormHandler(windowController) : new NativeFormHandler(nswindow);
 			window = new Form(handler);
			s_cachedWindows[handle] = new WeakReference(window);
			List<NativeHandle> toRemove = null;
			foreach (var entry in s_cachedWindows)
			{
				if (!entry.Value.IsAlive)
				{
					toRemove ??= new List<NativeHandle>();
					toRemove.Add(entry.Key);
				}
			}
			if (toRemove != null)
			{
				foreach	(var entry in toRemove)
				{
					s_cachedWindows.Remove(entry);
				}
			}
			return window;
		}

		static Dictionary<NativeHandle, WeakReference> s_cachedWindows;

		public override Size Size
		{
			get => Control.Frame.Size.ToEtoSize();
			set => base.Size = value;
		}
	}
}

