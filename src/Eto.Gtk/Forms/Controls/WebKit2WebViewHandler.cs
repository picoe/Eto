#if GTKCORE || GTK3
using System;
using System.Runtime.InteropServices;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<Gtk.Widget, WebView, WebView.ICallback>, WebView.IHandler
	{
		public bool BrowserContextMenuEnabled { get; set; }

		public string DocumentTitle
		{
			get { return NativeMethods.webkit_web_view_get_title(Control.Handle); }
		}

		public Uri Url
		{
			get { return new Uri(NativeMethods.webkit_web_view_get_uri(Control.Handle)); }
			set
			{
				if (value != null)
					NativeMethods.webkit_web_view_load_uri(Control.Handle, value.AbsoluteUri);
				else
					NativeMethods.webkit_web_view_load_html(Control.Handle, "", "");
			}
		}

		public Gtk.ScrolledWindow Scroll
		{
			get { return scroll; }
		}

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}

		EventHandler<WebViewTitleEventArgs> titleChanged;
		EventHandler<WebViewLoadedEventArgs> navigated;
		EventHandler<WebViewLoadedEventArgs> documentLoaded;
		EventHandler<WebViewLoadingEventArgs> documentLoading;
		EventHandler<WebViewNewWindowEventArgs> openNewWindow;

		readonly Gtk.ScrolledWindow scroll;
		string jsreturn;
		bool jsrunning;

		public WebViewHandler()
		{
			scroll = new Gtk.ScrolledWindow();
			Control = new Gtk.Widget(NativeMethods.webkit_web_view_new());
			scroll.Add(Control);
		}

		protected override void Initialize()
		{
			base.Initialize();

			Control.AddSignalHandler(
				"context-menu",
				(Action<object, GLib.SignalArgs>)WebViewHandler_ContextMenu,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"notify::title",
				(Action<object, GLib.SignalArgs>)WebViewHandler_TitleChanged,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"load-changed",
				(Action<object, GLib.SignalArgs>)WebViewHandler_LoadChanged,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"decide-policy",
				(Action<object, GLib.SignalArgs>)WebViewHandler_DecidePolicy,
				typeof(GLib.SignalArgs)
			);
		}

		private void WebViewHandler_TitleChanged(object o, GLib.SignalArgs args)
		{
			titleChanged?.Invoke(this, new WebViewTitleEventArgs(DocumentTitle));
		}

		private void WebViewHandler_ContextMenu(object o, GLib.SignalArgs args)
		{
			args.RetVal = !BrowserContextMenuEnabled;
		}

		private void WebViewHandler_LoadChanged(object o, GLib.SignalArgs args)
		{
			var loadEvent = (int)args.Args[0];

			switch (loadEvent)
			{
				case 2: // WEBKIT_LOAD_COMMITTED
					navigated?.Invoke(this, new WebViewLoadedEventArgs(Url));
					break;
				case 3: // WEBKIT_LOAD_FINISHED
					documentLoaded?.Invoke(this, new WebViewLoadedEventArgs(Url));
					break;
			}
		}

		[GLib.ConnectBefore]
		private void WebViewHandler_DecidePolicy(object o, GLib.SignalArgs args)
		{
			var decision = (GLib.Object)args.Args[0];
			var type = (int)args.Args[1];

			if (type != 0 && type != 1)
				return;
			
			var request = NativeMethods.webkit_navigation_policy_decision_get_request(decision.Handle);
			var uri = new Uri(NativeMethods.webkit_uri_request_get_uri(request));

			switch (type)
			{
				case 0: // WEBKIT_POLICY_DECISION_TYPE_NAVIGATION_ACTION
					var loadingArgs = new WebViewLoadingEventArgs(uri, true);
					documentLoading?.Invoke(this, loadingArgs);
					args.RetVal = loadingArgs.Cancel;
					break;
				case 1: // WEBKIT_POLICY_DECISION_TYPE_NEW_WINDOW_ACTION
					var newWindowArgs = new WebViewNewWindowEventArgs(uri, "");
					openNewWindow?.Invoke(this, newWindowArgs);
					args.RetVal = newWindowArgs.Cancel;
					break;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case WebView.NavigatedEvent:
					navigated += (sender, e) => Callback.OnNavigated(Widget, e);
					break;
				case WebView.DocumentLoadedEvent:
					documentLoaded += (sender, e) => Callback.OnDocumentLoaded(Widget, e);
					break;
				case WebView.DocumentLoadingEvent:
					documentLoading += (sender, e) => Callback.OnDocumentLoading(Widget, e);
					break;
				case WebView.OpenNewWindowEvent:
					openNewWindow += (sender, e) => Callback.OnOpenNewWindow(Widget, e);
					break;
				case WebView.DocumentTitleChangedEvent:
					titleChanged += (sender, e) => Callback.OnDocumentTitleChanged(Widget, e);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string ExecuteScript(string script)
		{
			jsrunning = true;
			jsreturn = "";

			NativeMethods.webkit_web_view_run_javascript(Control.Handle, "function EtOrEtFuN() {" + script + " } EtOrEtFuN();", IntPtr.Zero, (FinishScriptExecutionDelegate)FinishScriptExecution, IntPtr.Zero);

			while (jsrunning)
				Gtk.Application.RunIteration();

			return jsreturn;
		}

		delegate void FinishScriptExecutionDelegate(IntPtr webview, IntPtr result, IntPtr error);

		private void FinishScriptExecution(IntPtr webview, IntPtr result, IntPtr error)
		{
			var jsresult = NativeMethods.webkit_web_view_run_javascript_finish(Control.Handle, result, IntPtr.Zero);
			if (jsresult != IntPtr.Zero)
			{
				var context = NativeMethods.webkit_javascript_result_get_global_context(jsresult);
				var value = NativeMethods.webkit_javascript_result_get_value(jsresult);

				var strvalue = NativeMethods.JSValueToStringCopy(context, value, IntPtr.Zero);
				var strlen = NativeMethods.JSStringGetMaximumUTF8CStringSize(strvalue);
				var utfvalue = Marshal.AllocHGlobal(strlen);
				NativeMethods.JSStringGetUTF8CString(strvalue, utfvalue, strlen);
				jsreturn = NativeMethods.GetString(utfvalue);

				Marshal.FreeHGlobal(utfvalue);
				NativeMethods.JSStringRelease(strvalue);
			}

			jsrunning = false;
		}

		public void LoadHtml(string html, Uri baseUri)
		{
			NativeMethods.webkit_web_view_load_html(Control.Handle, html, baseUri?.AbsoluteUri ?? "");
		}

		public void Stop()
		{
			NativeMethods.webkit_web_view_stop_loading(Control.Handle);
		}

		public void Reload()
		{
			NativeMethods.webkit_web_view_reload(Control.Handle);
		}

		public void GoBack()
		{
			NativeMethods.webkit_web_view_go_back(Control.Handle);
		}

		public void GoForward()
		{
			NativeMethods.webkit_web_view_go_forward(Control.Handle);
		}

		public bool CanGoBack
		{
			get { return NativeMethods.webkit_web_view_can_go_back(Control.Handle); }
		}

		public bool CanGoForward
		{
			get { return NativeMethods.webkit_web_view_can_go_forward(Control.Handle); }
		}

		public void ShowPrintDialog()
		{
			NativeMethods.webkit_web_view_run_javascript(Control.Handle, "print();", IntPtr.Zero, null, IntPtr.Zero);
		}
	}
}
#endif