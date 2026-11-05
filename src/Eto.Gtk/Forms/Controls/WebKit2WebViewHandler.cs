#if GTKCORE || GTK3
using GLib;

namespace Eto.GtkSharp.Forms.Controls
{
	public class WebKitJavascriptResult : Opaque
	{
		public static GType GType => new GType(NativeMethods.webkit_javascript_result_get_type());

		public WebKitJavascriptResult(IntPtr raw)
			: base(raw)
		{
		}

		public string Value
		{
			get
			{
				IntPtr jsvalue = NativeMethods.webkit_javascript_result_get_js_value(Handle);
				return NativeMethods.jsc_value_to_string(jsvalue);
			}
		}
	}

	class WebKitUserContentManager : GLib.InitiallyUnowned
	{
		public WebKitUserContentManager(IntPtr raw)
			: base(raw)
		{
		}
		
	}

	public class WebViewHandler : GtkControl<Gtk.Widget, WebView, WebView.ICallback>, WebView.IHandler
	{
		static WebViewHandler()
		{
			GType.Register(WebKitJavascriptResult.GType, typeof(WebKitJavascriptResult));
		}

		// WebKitGTK won't intercept http or https, so hosts are served from eto://host/ instead.
		const string ResourceScheme = "eto";
		const int SOUP_MESSAGE_HEADERS_RESPONSE = 1;

		delegate void UriSchemeRequestDelegate(IntPtr request, IntPtr userData);

		// Static so the native callback can never outlive it.
		static readonly UriSchemeRequestDelegate s_uriSchemeRequest = HandleUriSchemeRequest;
		static readonly HashSet<IntPtr> s_registeredContexts = new HashSet<IntPtr>();
		// Schemes are registered per context, which all web views share, so find the handler by its view.
		static readonly Dictionary<IntPtr, WeakReference> s_resourceViews = new Dictionary<IntPtr, WeakReference>();
		static bool s_supportsSchemeResponses = true;

		Dictionary<string, Func<WebViewResourceRequest, WebViewResourceResponse>> resourceHandlers;

		public bool SupportsResourceHandlers => true;

		public Uri AddResourceHandler(string host, Func<WebViewResourceRequest, WebViewResourceResponse> handler)
		{
			resourceHandlers ??= new Dictionary<string, Func<WebViewResourceRequest, WebViewResourceResponse>>(StringComparer.OrdinalIgnoreCase);
			resourceHandlers[host] = handler;
			s_resourceViews[Control.Handle] = new WeakReference(this);

			var context = NativeMethods.webkit_web_view_get_context(Control.Handle);
			if (s_registeredContexts.Add(context))
			{
				NativeMethods.webkit_web_context_register_uri_scheme(context, ResourceScheme, s_uriSchemeRequest, IntPtr.Zero, IntPtr.Zero);
				// Behave like the https pages other platforms serve, e.g. allowing fetch() and secure-only APIs.
				var securityManager = NativeMethods.webkit_web_context_get_security_manager(context);
				NativeMethods.webkit_security_manager_register_uri_scheme_as_secure(securityManager, ResourceScheme);
				NativeMethods.webkit_security_manager_register_uri_scheme_as_cors_enabled(securityManager, ResourceScheme);
			}
			return new Uri(ResourceScheme + "://" + host.ToLowerInvariant() + "/");
		}

		public void RemoveResourceHandler(string host) => resourceHandlers?.Remove(host);

		protected override void Dispose(bool disposing)
		{
			if (disposing && resourceHandlers != null && Control != null)
				s_resourceViews.Remove(Control.Handle);
			base.Dispose(disposing);
		}

		static void HandleUriSchemeRequest(IntPtr request, IntPtr userData)
		{
			// Letting an exception escape a native callback would crash the application.
			try
			{
				var view = NativeMethods.webkit_uri_scheme_request_get_web_view(request);
				var h = s_resourceViews.TryGetValue(view, out var weakHandler) ? weakHandler.Target as WebViewHandler : null;
				var uri = new Uri(NativeMethods.webkit_uri_scheme_request_get_uri(request));

				var statusCode = 404;
				WebViewResourceResponse response = null;
				byte[] content = null;
				if (h?.resourceHandlers != null && h.resourceHandlers.TryGetValue(uri.Host, out var handler))
				{
					try
					{
						response = handler(new WebViewResourceRequest(uri, GetHttpMethod(request)));
						if (response != null)
						{
							statusCode = response.StatusCode;
							if (response.Content != null)
							{
								using var stream = new MemoryStream();
								response.Content.CopyTo(stream);
								response.Content.Dispose();
								content = stream.ToArray();
							}
						}
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine("WebView resource handler failed for " + uri + ":\n" + ex);
						statusCode = 500;
						response = null;
						content = null;
					}
				}
				FinishUriSchemeRequest(request, statusCode, response, content ?? Array.Empty<byte>());
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("WebView resource request failed:\n" + ex);
			}
		}

		static string GetHttpMethod(IntPtr request)
		{
			try
			{
				var method = NativeMethods.webkit_uri_scheme_request_get_http_method(request);
				// the native wrapper turns a missing method into an empty string
				return string.IsNullOrEmpty(method) ? "GET" : method;
			}
			catch (EntryPointNotFoundException)
			{
				// WebKitGTK before 2.36 only reports the address.
				return "GET";
			}
		}

		static void SetHttpHeaders(IntPtr schemeResponse, WebViewResourceResponse response)
		{
			try
			{
				var soupHeaders = NativeMethods.soup_message_headers_new(SOUP_MESSAGE_HEADERS_RESPONSE);
				// The content type alone only sets the MIME type, so scripts wouldn't see it as a header.
				if (!string.IsNullOrEmpty(response.ContentType) && !response.Headers.ContainsKey("Content-Type"))
					NativeMethods.soup_message_headers_append(soupHeaders, "Content-Type", response.ContentType);
				foreach (var header in response.Headers)
					NativeMethods.soup_message_headers_append(soupHeaders, header.Key, header.Value);
				// takes ownership of the headers
				NativeMethods.webkit_uri_scheme_response_set_http_headers(schemeResponse, soupHeaders);
			}
			catch (EntryPointNotFoundException)
			{
				// libsoup isn't reachable through webkit here, so the extra headers are dropped.
			}
		}

		static void FinishUriSchemeRequest(IntPtr request, int statusCode, WebViewResourceResponse response, byte[] content)
		{
			var bytes = NativeMethods.g_bytes_new(content, (IntPtr)content.Length);
			var stream = NativeMethods.g_memory_input_stream_new_from_bytes(bytes);
			NativeMethods.g_bytes_unref(bytes);
			try
			{
				if (s_supportsSchemeResponses)
				{
					try
					{
						var schemeResponse = NativeMethods.webkit_uri_scheme_response_new(stream, content.Length);
						NativeMethods.webkit_uri_scheme_response_set_status(schemeResponse, (uint)statusCode, null);
						if (!string.IsNullOrEmpty(response?.ContentType))
							NativeMethods.webkit_uri_scheme_response_set_content_type(schemeResponse, response.ContentType);
						if (response != null && (response.Headers.Count > 0 || !string.IsNullOrEmpty(response.ContentType)))
							SetHttpHeaders(schemeResponse, response);
						NativeMethods.webkit_uri_scheme_request_finish_with_response(request, schemeResponse);
						NativeMethods.g_object_unref(schemeResponse);
						return;
					}
					catch (EntryPointNotFoundException)
					{
						s_supportsSchemeResponses = false;
					}
				}

				// WebKitGTK before 2.36 can't send a status, so failures become load errors.
				if (statusCode >= 400)
				{
					var error = NativeMethods.g_error_new_literal(NativeMethods.g_quark_from_string("eto-webview-resource"), statusCode, statusCode == 404 ? "Not Found" : "Request Failed");
					NativeMethods.webkit_uri_scheme_request_finish_error(request, error);
					NativeMethods.g_error_free(error);
				}
				else
				{
					NativeMethods.webkit_uri_scheme_request_finish(request, stream, content.Length, response?.ContentType);
				}
			}
			finally
			{
				NativeMethods.g_object_unref(stream);
			}
		}

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
		Queue<TaskCompletionSource<string>> jscs;
		private readonly FinishScriptExecutionDelegate _finishScriptExecutionDelegate;

		public WebViewHandler()
		{
			scroll = new Gtk.ScrolledWindow();
			Control = new Gtk.Widget(NativeMethods.webkit_web_view_new());
			scroll.Add(Control);
			_finishScriptExecutionDelegate = FinishScriptExecution;
		}

		protected override void Initialize()
		{
			base.Initialize();

			// note: these must go through the (weakly referencing) connector, otherwise the native
			// signal keeps a strong reference to this handler and the control can never be collected.
			Control.AddSignalHandler(
				"context-menu",
				(Action<object, GLib.SignalArgs>)Connector.HandleContextMenu,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"notify::title",
				(Action<object, GLib.SignalArgs>)Connector.HandleTitleChanged,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"load-changed",
				(Action<object, GLib.SignalArgs>)Connector.HandleLoadChanged,
				typeof(GLib.SignalArgs)
			);

			Control.AddSignalHandler(
				"decide-policy",
				(Action<object, GLib.SignalArgs>)Connector.HandleDecidePolicy,
				typeof(GLib.SignalArgs)
			);

			var scriptSource = "window.eto = { postMessage: function(message) { window.webkit.messageHandlers.__eto__.postMessage(message); } };";
			IntPtr script = NativeMethods.webkit_user_script_new(scriptSource, NativeMethods.WebKitUserContentInjectedFrames.WEBKIT_USER_CONTENT_INJECT_ALL_FRAMES, NativeMethods.WebKitUserScriptInjectionTime.WEBKIT_USER_SCRIPT_INJECT_AT_DOCUMENT_START, null, null);
			NativeMethods.webkit_user_content_manager_add_script(NativeMethods.webkit_web_view_get_user_content_manager(Control.Handle), script);
		}

		protected new WebViewConnector Connector => (WebViewConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new WebViewConnector();

		protected class WebViewConnector : GtkControlConnector
		{
			public new WebViewHandler Handler => (WebViewHandler)base.Handler;

			public void HandleContextMenu(object o, GLib.SignalArgs args) => Handler?.WebViewHandler_ContextMenu(o, args);

			public void HandleTitleChanged(object o, GLib.SignalArgs args) => Handler?.WebViewHandler_TitleChanged(o, args);

			public void HandleLoadChanged(object o, GLib.SignalArgs args) => Handler?.WebViewHandler_LoadChanged(o, args);

			[GLib.ConnectBefore]
			public void HandleDecidePolicy(object o, GLib.SignalArgs args) => Handler?.WebViewHandler_DecidePolicy(o, args);

			public void HandleMessageReceived(object o, GLib.SignalArgs args) => Handler?.WebViewHandler_MessageReceived(o, args);
		}

		private void WebViewHandler_MessageReceived(object arg1, SignalArgs args)
		{
			if (args.Args[0] is WebKitJavascriptResult result)
			{
				Callback.OnMessageReceived(Widget, new WebViewMessageEventArgs(result.Value));
			}
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
				case WebView.MessageReceivedEvent:
					var managerPtr = NativeMethods.webkit_web_view_get_user_content_manager(Control.Handle);
					var manager = new WebKitUserContentManager(managerPtr);

					manager.AddSignalHandler(
						"script-message-received::__eto__",
						(Action<object, GLib.SignalArgs>)Connector.HandleMessageReceived,
						typeof(SignalArgs)
					);

					NativeMethods.webkit_user_content_manager_register_script_message_handler(managerPtr, "__eto__");
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string ExecuteScript(string script)
		{
			var task = ExecuteScriptAsync(script);

			while (!task.IsCompleted)
			{
				Gtk.Application.RunIteration();
			}

			return task.Result;
		}

		public Task<string> ExecuteScriptAsync(string script)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
			if (jscs == null)
				jscs = new Queue<TaskCompletionSource<string>>();
			jscs.Enqueue(tcs);

			NativeMethods.webkit_web_view_run_javascript(Control.Handle, $"function _fn() {{{script}}} _fn();", IntPtr.Zero, _finishScriptExecutionDelegate, IntPtr.Zero);

			return tcs.Task;
		}

		delegate void FinishScriptExecutionDelegate(IntPtr webview, IntPtr result, IntPtr error);

		private void FinishScriptExecution(IntPtr webview, IntPtr result, IntPtr error)
		{
			var jsresult = NativeMethods.webkit_web_view_run_javascript_finish(Control.Handle, result, IntPtr.Zero);
			var tcs = jscs?.Count > 0 ? jscs?.Dequeue() : null;
			if (jsresult != IntPtr.Zero)
			{
				var context = NativeMethods.webkit_javascript_result_get_global_context(jsresult);
				var value = NativeMethods.webkit_javascript_result_get_value(jsresult);

				var strvalue = NativeMethods.JSValueToStringCopy(context, value, IntPtr.Zero);
				var strlen = NativeMethods.JSStringGetMaximumUTF8CStringSize(strvalue);
				var utfvalue = Marshal.AllocHGlobal(strlen);
				NativeMethods.JSStringGetUTF8CString(strvalue, utfvalue, strlen);
				var jsreturn = NativeMethods.GetString(utfvalue);

				Marshal.FreeHGlobal(utfvalue);
				NativeMethods.JSStringRelease(strvalue);
				tcs?.SetResult(jsreturn);
			}
			else
			{
				tcs?.SetResult(null);
			}
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