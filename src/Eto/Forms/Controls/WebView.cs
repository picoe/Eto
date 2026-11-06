namespace Eto.Forms;

/// <summary>
/// Event arguments when the <see cref="WebView"/> has finished loaded a uri
/// </summary>
public class WebViewLoadedEventArgs : EventArgs
{
	/// <summary>
	/// Gets the URI of the page that was loaded.
	/// </summary>
	/// <value>The URI.</value>
	public Uri Uri { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.WebViewLoadedEventArgs"/> class.
	/// </summary>
	/// <param name="uri">URI of the page that was loaded.</param>
	public WebViewLoadedEventArgs(Uri uri)
	{
		this.Uri = uri;
	}
}

/// <summary>
/// Event arguments when the <see cref="WebView"/> is loading a new uri.
/// </summary>
public class WebViewLoadingEventArgs : WebViewLoadedEventArgs
{
	/// <summary>
	/// Gets or sets a value indicating whether to cancel the load.
	/// </summary>
	/// <value><c>true</c> to cancel loading the page; otherwise, <c>false</c>.</value>
	public bool Cancel { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the main frame is loading, or a child frame.
	/// </summary>
	/// <value><c>true</c> if loading for the main frame; otherwise, <c>false</c>.</value>
	public bool IsMainFrame { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.WebViewLoadingEventArgs"/> class.
	/// </summary>
	/// <param name="uri">URI of the page that is loading.</param>
	/// <param name="isMainFrame">If set to <c>true</c> the uri is for the main frame, otherwise <c>false</c>.</param>
	public WebViewLoadingEventArgs(Uri uri, bool isMainFrame)
		: base(uri)
	{
		this.IsMainFrame = isMainFrame;
	}
}

/// <summary>
/// Event arguments for when the <see cref="WebView"/> changes its title
/// </summary>
public class WebViewTitleEventArgs : EventArgs
{
	/// <summary>
	/// Gets the new title for the page.
	/// </summary>
	/// <value>The title for the page.</value>
	public string Title { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.WebViewTitleEventArgs"/> class.
	/// </summary>
	/// <param name="title">New title for the page.</param>
	public WebViewTitleEventArgs(string title)
	{
		this.Title = title;
	}
}

/// <summary>
/// Event arguments for when the <see cref="WebView"/> prompts to open a new window.
/// </summary>
public class WebViewNewWindowEventArgs : WebViewLoadingEventArgs
{
	/// <summary>
	/// Gets the name of the new window.
	/// </summary>
	/// <value>The name of the new window.</value>
	public string NewWindowName { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.WebViewNewWindowEventArgs"/> class.
	/// </summary>
	/// <param name="uri">URI of the new window.</param>
	/// <param name="newWindowName">Name of the new window.</param>
	public WebViewNewWindowEventArgs(Uri uri, string newWindowName)
		: base(uri, false)
	{
		this.NewWindowName = newWindowName;
	}
}

/// <summary>
/// Event arguments for when a message is sent via javascript in the <see cref="WebView"/>
/// </summary>
/// <since>2.10</since>
public class WebViewMessageEventArgs : EventArgs
{
	/// <summary>
	/// Gets the received message.
	/// </summary>
	/// <value>The message.</value>
	public string Message { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.WebViewMessageEventArgs"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public WebViewMessageEventArgs(string message)
	{
		this.Message = message;
	}
}

/// <summary>
/// A request from the <see cref="WebView"/> for content served by a handler added with <see cref="WebView.AddResourceHandler"/>.
/// </summary>
/// <since>2.14</since>
public class WebViewResourceRequest
{
	/// <summary>
	/// Gets the full address requested, e.g. https://myapp.example/index.html.
	/// </summary>
	public Uri Uri { get; }

	/// <summary>
	/// Gets the HTTP method of the request, e.g. GET or POST.
	/// </summary>
	public string Method { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="WebViewResourceRequest"/> class.
	/// </summary>
	/// <param name="uri">Address requested.</param>
	/// <param name="method">HTTP method of the request.</param>
	public WebViewResourceRequest(Uri uri, string method)
	{
		Uri = uri;
		Method = method;
	}
}

/// <summary>
/// Content to return to the <see cref="WebView"/> from a handler added with <see cref="WebView.AddResourceHandler"/>.
/// </summary>
/// <since>2.14</since>
public class WebViewResourceResponse
{
	/// <summary>
	/// Gets the content to send. The web view reads it after the handler returns, so don't dispose it.
	/// </summary>
	public Stream Content { get; }

	/// <summary>
	/// Gets the MIME type of the content, e.g. text/html; charset=utf-8.
	/// </summary>
	public string ContentType { get; }

	/// <summary>
	/// Gets or sets the HTTP status code, 200 by default.
	/// </summary>
	public int StatusCode { get; set; } = 200;

	/// <summary>
	/// Gets extra HTTP headers to send, e.g. Cache-Control.
	/// </summary>
	public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Initializes a new response with the specified content.
	/// </summary>
	/// <param name="content">Content to send.</param>
	/// <param name="contentType">MIME type of the content.</param>
	public WebViewResourceResponse(Stream content, string contentType)
	{
		Content = content;
		ContentType = contentType;
	}

	/// <summary>
	/// Initializes a new response with text content, sent as UTF-8.
	/// </summary>
	/// <param name="content">Text to send.</param>
	/// <param name="contentType">MIME type of the content without a charset, e.g. text/html.</param>
	public WebViewResourceResponse(string content, string contentType)
		: this(new MemoryStream(Encoding.UTF8.GetBytes(content ?? string.Empty)), contentType + "; charset=utf-8")
	{
	}
}

/// <summary>
/// Control to show a browser control that can display html and execute javascript.
/// </summary>
/// <remarks>
/// Most platforms have built-in support for a browser control, which by default this will use.
/// 
/// There are other browser implementations available, such as Chromium, etc. 
/// You can create your own handler for the web view if you want to use a different browser control.
/// </remarks>
[Handler(typeof(WebView.IHandler))]
public class WebView : Control
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	#region Events

	/// <summary>
	/// Identifier for handlers when attaching the <see cref="Navigated"/> event.
	/// </summary>
	public const string NavigatedEvent = "WebView.Navigated";

	/// <summary>
	/// Occurs when the browser has loaded a new uri and has begun loading it.
	/// </summary>
	/// <remarks>
	/// This happens after the <see cref="DocumentLoading"/> event.
	/// Once the document is fully loaded, the <see cref="DocumentLoaded"/> event will be triggered.
	/// </remarks>
	public event EventHandler<WebViewLoadedEventArgs> Navigated
	{
		add { Properties.AddHandlerEvent(NavigatedEvent, value); }
		remove { Properties.RemoveEvent(NavigatedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="Navigated"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnNavigated(WebViewLoadedEventArgs e)
	{
		Properties.TriggerEvent(NavigatedEvent, this, e);
	}

	/// <summary>
	/// Identifier for handlers when attaching the <see cref="DocumentLoaded"/> event.
	/// </summary>
	public const string DocumentLoadedEvent = "WebView.DocumentLoaded";

	/// <summary>
	/// Occurs when the document is fully loaded.
	/// </summary>
	/// <remarks>
	/// This event fires after the entire document has loaded and is fully rendered.
	/// Usually this is a good event to use when determining when you can execute scripts
	/// or interact with the document.
	/// </remarks>
	public event EventHandler<WebViewLoadedEventArgs> DocumentLoaded
	{
		add { Properties.AddHandlerEvent(DocumentLoadedEvent, value); }
		remove { Properties.RemoveEvent(DocumentLoadedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="DocumentLoaded"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnDocumentLoaded(WebViewLoadedEventArgs e)
	{
		Properties.TriggerEvent(DocumentLoadedEvent, this, e);
	}

	/// <summary>
	/// Identifier for handlers when attaching the <see cref="DocumentLoading"/> event.
	/// </summary>
	public const string DocumentLoadingEvent = "WebView.DocumentLoading";

	/// <summary>
	/// Occurs when a document starts loading.
	/// </summary>
	/// <remarks>
	/// This fires when the document shown is changed, and notifies which url is being loaded.
	/// You can cancel the loading of a page through this event, though you should check the <see cref="WebViewLoadingEventArgs.IsMainFrame"/>
	/// to determine if it is for the main frame, or a child frame.
	/// </remarks>
	public event EventHandler<WebViewLoadingEventArgs> DocumentLoading
	{
		add { Properties.AddHandlerEvent(DocumentLoadingEvent, value); }
		remove { Properties.RemoveEvent(DocumentLoadingEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="DocumentLoading"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnDocumentLoading(WebViewLoadingEventArgs e)
	{
		Properties.TriggerEvent(DocumentLoadingEvent, this, e);
	}

	/// <summary>
	/// Identifier for handlers when attaching the <see cref="OpenNewWindow"/> event.
	/// </summary>
	public const string OpenNewWindowEvent = "WebView.OpenNewWindow";

	/// <summary>
	/// Occurs when the page prompts to open a link in a new window
	/// </summary>
	/// <remarks>
	/// This event will occur when a user or script opens a new link in a new window.
	/// 
	/// This is usually when an anchor's target is set to _blank, or a specific window name.
	/// 
	/// You must handle this event to perform an action, otherwise no action will occur.
	/// </remarks>
	public event EventHandler<WebViewNewWindowEventArgs> OpenNewWindow
	{
		add { Properties.AddHandlerEvent(OpenNewWindowEvent, value); }
		remove { Properties.RemoveEvent(OpenNewWindowEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="OpenNewWindow"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnOpenNewWindow(WebViewNewWindowEventArgs e)
	{
		Properties.TriggerEvent(OpenNewWindowEvent, this, e);
	}

	/// <summary>
	/// Identifier for handlers when attaching the <see cref="DocumentTitleChanged"/> event.
	/// </summary>
	public const string DocumentTitleChangedEvent = "WebView.DocumentTitleChanged";

	/// <summary>
	/// Occurs when the title of the page has change either through navigation or a script.
	/// </summary>
	public event EventHandler<WebViewTitleEventArgs> DocumentTitleChanged
	{
		add { Properties.AddHandlerEvent(DocumentTitleChangedEvent, value); }
		remove { Properties.RemoveEvent(DocumentTitleChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="DocumentTitleChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnDocumentTitleChanged(WebViewTitleEventArgs e)
	{
		Properties.TriggerEvent(DocumentTitleChangedEvent, this, e);
	}


	/// <summary>
	/// Identifier for handlers when attaching the <see cref="MessageReceived"/> event.
	/// </summary>
	/// <since>2.10</since>
	public const string MessageReceivedEvent = "WebView.MessageReceived";

	/// <summary>
	/// Occurs when a message is received from a JS call to window.eto.postMessage(string).
	/// </summary>
	/// <since>2.10</since>
	public event EventHandler<WebViewMessageEventArgs> MessageReceived
	{
		add { Properties.AddHandlerEvent(MessageReceivedEvent, value); }
		remove { Properties.RemoveEvent(MessageReceivedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="MessageReceived"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	/// <since>2.10</since>
	protected virtual void OnMessageReceived(WebViewMessageEventArgs e)
	{
		Properties.TriggerEvent(MessageReceivedEvent, this, e);
	}

	#endregion

	static WebView()
	{
		EventLookup.Register<WebView>(c => c.OnNavigated(null), WebView.NavigatedEvent);
		EventLookup.Register<WebView>(c => c.OnDocumentLoaded(null), WebView.DocumentLoadedEvent);
		EventLookup.Register<WebView>(c => c.OnDocumentLoading(null), WebView.DocumentLoadingEvent);
		EventLookup.Register<WebView>(c => c.OnDocumentTitleChanged(null), WebView.DocumentTitleChangedEvent);
		EventLookup.Register<WebView>(c => c.OnOpenNewWindow(null), WebView.OpenNewWindowEvent);
		EventLookup.Register<WebView>(c => c.OnMessageReceived(null), WebView.MessageReceivedEvent);
	}

	/// <summary>
	/// Navigates the browser back to the previous page in history, if there is one.
	/// </summary>
	/// <seealso cref="CanGoBack"/>
	public void GoBack() => Handler.GoBack();

	/// <summary>
	/// Gets a value indicating whether the browser can go back to the previous page in history.
	/// </summary>
	/// <seealso cref="GoBack"/>
	/// <value><c>true</c> if the browser can go back; otherwise, <c>false</c>.</value>
	public bool CanGoBack => Handler.CanGoBack;

	/// <summary>
	/// Navigates the browser forward to the next page in history, if there is one.
	/// </summary>
	/// <seealso cref="CanGoForward"/>
	public void GoForward() => Handler.GoForward();

	/// <summary>
	/// Gets a value indicating whether the browser can go forward to the next page.
	/// </summary>
	/// <seealso cref="GoForward"/>
	/// <value><c>true</c> if the browser can go forward; otherwise, <c>false</c>.</value>
	public bool CanGoForward => Handler.CanGoForward;

	/// <summary>
	/// Gets or sets the URL of the currently navigated page.
	/// </summary>
	/// <remarks>
	/// Setting this will cause the current page to stop loading (if not already loaded), and begin loading another page.
	/// Loading the new page can be cancelled by the <see cref="DocumentLoading"/> event.
	/// </remarks>
	/// <value>The URL of the currently navigated page.</value>
	public Uri Url
	{
		get { return Handler.Url; }
		set { Handler.Url = value; }
	}

	/// <summary>
	/// Stops loading the current page.
	/// </summary>
	/// <remarks>
	/// You can determine if the page is finished loading by the <see cref="DocumentLoaded"/> event.
	/// </remarks>
	public void Stop() => Handler.Stop();

	/// <summary>
	/// Reloads the current page
	/// </summary>
	public void Reload() => Handler.Reload();

	/// <summary>
	/// Executes the specified javascript in the context of the current page, returning its result.
	/// </summary>
	/// <returns>The script to execute.</returns>
	/// <param name="script">Script result.</param>
	public string ExecuteScript(string script) => Handler.ExecuteScript(script);

	/// <summary>
	/// Executes the specified javascript in the context of the current page asynchronously, returning its result.
	/// </summary>
	/// <remarks>
	/// Note that not all platforms support asynchronous execution of javascript and may run synchronously.
	/// </remarks>
	/// <returns>A task with the script to execute.</returns>
	/// <param name="script">Script result.</param>
	public Task<string> ExecuteScriptAsync(string script) => Handler.ExecuteScriptAsync(script);

	/// <summary>
	/// Gets the document title of the current page.
	/// </summary>
	/// <value>The document title.</value>
	public string DocumentTitle
	{
		get { return Handler.DocumentTitle; }
	}

	/// <summary>
	/// Loads the specified stream as html into the control.
	/// </summary>
	/// <param name="stream">Stream containing the html to load.</param>
	/// <param name="baseUri">Base URI to load associated resources. Can be a url or file path.</param>
	public void LoadHtml(Stream stream, Uri baseUri = null)
	{
		using (var reader = new StreamReader(stream))
		{
			Handler.LoadHtml(reader.ReadToEnd(), baseUri);
		}
	}

	/// <summary>
	/// Loads the specified html string.
	/// </summary>
	/// <param name="html">Html string to load.</param>
	/// <param name="baseUri">Base URI to load associated resources. Can be a url or file path.</param>
	public void LoadHtml(string html, Uri baseUri = null)
	{
		Handler.LoadHtml(html, baseUri);
	}

	/// <summary>
	/// Shows the print dialog for the current page.
	/// </summary>
	/// <remarks>
	/// This prompts the browser to print its contents.
	/// </remarks>
	public void ShowPrintDialog()
	{
		Handler.ShowPrintDialog();
	}

	/// <summary>
	/// Gets or sets a value indicating whether the user can click to show the context menu.
	/// </summary>
	/// <remarks>
	/// This is useful when using a browser control with content that should not be changed.
	/// The context menu can show navigation items which may cause the page to reload so setting this
	/// value to false will ensure the user can only interact with the page as is.
	/// </remarks>
	/// <value><c>true</c> if the context menu is enabled; otherwise, <c>false</c>.</value>
	public bool BrowserContextMenuEnabled
	{
		get { return Handler.BrowserContextMenuEnabled; }
		set { Handler.BrowserContextMenuEnabled = value; }
	}

	/// <summary>
	/// Gets a value indicating whether the current platform supports <see cref="AddResourceHandler"/>.
	/// </summary>
	/// <since>2.14</since>
	public bool SupportsResourceHandlers => Handler.SupportsResourceHandlers;

	/// <summary>
	/// Serves every request to <paramref name="host"/> from <paramref name="handler"/> instead of the network.
	/// </summary>
	/// <remarks>
	/// Navigate to the returned address with <see cref="Url"/>, e.g. <c>webView.Url = webView.AddResourceHandler("myapp.example", handler)</c>.
	/// It is eto://myapp.example/, except with WebView2 using an environment the app created without the eto scheme, where it is https://myapp.example/.
	/// The handler runs on the UI thread; return null to respond with 404 Not Found.
	/// Adding a host again replaces its handler. Use a host under the reserved .example domain so it can never be a real site.
	/// </remarks>
	/// <param name="host">Host name to serve, e.g. myapp.example.</param>
	/// <param name="handler">Returns the content for each request.</param>
	/// <returns>The root address of the host, e.g. https://myapp.example/.</returns>
	/// <exception cref="NotSupportedException">The platform doesn't support it. Check <see cref="SupportsResourceHandlers"/> first.</exception>
	/// <since>2.14</since>
	public Uri AddResourceHandler(string host, Func<WebViewResourceRequest, WebViewResourceResponse> handler)
	{
		if (handler == null)
			throw new ArgumentNullException(nameof(handler));
		ValidateResourceHost(host);
		return Handler.AddResourceHandler(host, handler);
	}

	/// <summary>
	/// Stops serving requests to a host added with <see cref="AddResourceHandler"/>.
	/// </summary>
	/// <param name="host">Host name to stop serving.</param>
	/// <since>2.14</since>
	public void RemoveResourceHandler(string host)
	{
		ValidateResourceHost(host);
		Handler.RemoveResourceHandler(host);
	}

	void ValidateResourceHost(string host)
	{
		if (string.IsNullOrEmpty(host))
			throw new ArgumentNullException(nameof(host));
		if (Uri.CheckHostName(host) != UriHostNameType.Dns)
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid host name", host), nameof(host));
		if (!Handler.SupportsResourceHandlers)
			throw new NotSupportedException("This platform does not support serving content to the WebView directly");
	}

	/// <summary>
	/// Callback interface for the <see cref="WebView"/>.
	/// </summary>
	public new interface ICallback : Control.ICallback
	{
		/// <summary>
		/// Raises the navigated event.
		/// </summary>
		void OnNavigated(WebView widget, WebViewLoadedEventArgs e);

		/// <summary>
		/// Raises the document loaded event.
		/// </summary>
		void OnDocumentLoaded(WebView widget, WebViewLoadedEventArgs e);

		/// <summary>
		/// Raises the document loading event.
		/// </summary>
		void OnDocumentLoading(WebView widget, WebViewLoadingEventArgs e);

		/// <summary>
		/// Raises the open new window event.
		/// </summary>
		void OnOpenNewWindow(WebView widget, WebViewNewWindowEventArgs e);

		/// <summary>
		/// Raises the document title changed event.
		/// </summary>
		void OnDocumentTitleChanged(WebView widget, WebViewTitleEventArgs e);

		/// <summary>
		/// Raises the message received event.
		/// </summary>
		/// <since>2.10</since>
		void OnMessageReceived(WebView widget, WebViewMessageEventArgs e);
	}

	static readonly object callback = new Callback();

	/// <summary>
	/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
	/// </summary>
	/// <returns>The callback instance to use for this widget</returns>
	protected override object GetCallback()
	{
		return callback;
	}

	/// <summary>
	/// Callback implementation for handlers of the <see cref="WebView"/>
	/// </summary>
	protected new class Callback : Control.Callback, ICallback
	{
		/// <summary>
		/// Raises the navigated event.
		/// </summary>
		public void OnNavigated(WebView widget, WebViewLoadedEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnNavigated(e);
		}

		/// <summary>
		/// Raises the document loaded event.
		/// </summary>
		public void OnDocumentLoaded(WebView widget, WebViewLoadedEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnDocumentLoaded(e);
		}

		/// <summary>
		/// Raises the document loading event.
		/// </summary>
		public void OnDocumentLoading(WebView widget, WebViewLoadingEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnDocumentLoading(e);
		}

		/// <summary>
		/// Raises the open new window event.
		/// </summary>
		public void OnOpenNewWindow(WebView widget, WebViewNewWindowEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnOpenNewWindow(e);
		}

		/// <summary>
		/// Raises the document title changed event.
		/// </summary>
		public void OnDocumentTitleChanged(WebView widget, WebViewTitleEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnDocumentTitleChanged(e);
		}

		/// <summary>
		/// Raises the message received event.
		/// </summary>
		/// <since>2.10</since>
		public void OnMessageReceived(WebView widget, WebViewMessageEventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnMessageReceived(e);
		}
	}

	/// <summary>
	/// Handler interface for the <see cref="WebView"/>.
	/// </summary>
	public new interface IHandler : Control.IHandler
	{
		/// <summary>
		/// Gets or sets the URL of the currently navigated page.
		/// </summary>
		/// <remarks>
		/// Setting this will cause the current page to stop loading (if not already loaded), and begin loading another page.
		/// Loading the new page can be cancelled by the <see cref="DocumentLoading"/> event.
		/// </remarks>
		/// <value>The URL of the currently navigated page.</value>
		Uri Url { get; set; }

		/// <summary>
		/// Loads the specified html string.
		/// </summary>
		/// <param name="html">Html string to load.</param>
		/// <param name="baseUri">Base URI to load associated resources. Can be a url or file path.</param>
		void LoadHtml(string html, Uri baseUri);

		/// <summary>
		/// Navigates the browser back to the previous page in history, if there is one.
		/// </summary>
		/// <seealso cref="CanGoBack"/>
		void GoBack();

		/// <summary>
		/// Gets a value indicating whether the browser can go back to the previous page in history.
		/// </summary>
		/// <seealso cref="GoBack"/>
		/// <value><c>true</c> if the browser can go back; otherwise, <c>false</c>.</value>
		bool CanGoBack { get; }

		/// <summary>
		/// Navigates the browser forward to the next page in history, if there is one.
		/// </summary>
		/// <seealso cref="CanGoForward"/>
		void GoForward();

		/// <summary>
		/// Gets a value indicating whether the browser can go forward to the next page.
		/// </summary>
		/// <seealso cref="GoForward"/>
		/// <value><c>true</c> if the browser can go forward; otherwise, <c>false</c>.</value>
		bool CanGoForward { get; }

		/// <summary>
		/// Stops loading the current page.
		/// </summary>
		/// <remarks>
		/// You can determine if the page is finished loading by the <see cref="DocumentLoaded"/> event.
		/// </remarks>
		void Stop();

		/// <summary>
		/// Reloads the current page
		/// </summary>
		void Reload();

		/// <summary>
		/// Gets the document title of the current page.
		/// </summary>
		/// <value>The document title.</value>
		string DocumentTitle { get; }

		/// <summary>
		/// Executes the specified javascript in the context of the current page, returning its result.
		/// </summary>
		/// <returns>The script to execute.</returns>
		/// <param name="script">Script result.</param>
		string ExecuteScript(string script);

		/// <summary>
		/// Executes the specified javascript in the context of the current page asynchronously, returning its result.
		/// </summary>
		/// <returns>A task with the script to execute.</returns>
		/// <param name="script">Script result.</param>
		Task<string> ExecuteScriptAsync(string script);

		/// <summary>
		/// Shows the print dialog for the current page.
		/// </summary>
		/// <remarks>
		/// This prompts the browser to print its contents.
		/// </remarks>
		void ShowPrintDialog();

		/// <summary>
		/// Gets or sets a value indicating whether the user can click to show the context menu.
		/// </summary>
		/// <remarks>
		/// This is useful when using a browser control with content that should not be changed.
		/// The context menu can show navigation items which may cause the page to reload so setting this
		/// value to false will ensure the user can only interact with the page as is.
		/// </remarks>
		/// <value><c>true</c> if the context menu is enabled; otherwise, <c>false</c>.</value>
		bool BrowserContextMenuEnabled { get; set; }

#if NET
		// Defaults let existing handlers compile unchanged.

		/// <inheritdoc cref="WebView.SupportsResourceHandlers"/>
		/// <since>2.14</since>
		bool SupportsResourceHandlers => false;

		/// <inheritdoc cref="WebView.AddResourceHandler"/>
		/// <since>2.14</since>
		Uri AddResourceHandler(string host, Func<WebViewResourceRequest, WebViewResourceResponse> handler) => null;

		/// <inheritdoc cref="WebView.RemoveResourceHandler"/>
		/// <since>2.14</since>
		void RemoveResourceHandler(string host) { }
#else
		/// <inheritdoc cref="WebView.SupportsResourceHandlers"/>
		/// <since>2.14</since>
		bool SupportsResourceHandlers { get; }

		/// <inheritdoc cref="WebView.AddResourceHandler"/>
		/// <since>2.14</since>
		Uri AddResourceHandler(string host, Func<WebViewResourceRequest, WebViewResourceResponse> handler);

		/// <inheritdoc cref="WebView.RemoveResourceHandler"/>
		/// <since>2.14</since>
		void RemoveResourceHandler(string host);
#endif
	}
}