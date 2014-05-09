using System;
using System.IO;

namespace Eto.Forms
{
	public interface IWebView : IControl
	{
		Uri Url { get; set; }

		void LoadHtml(string html, Uri baseUri);

		void GoBack();

		bool CanGoBack { get; }

		void GoForward();

		bool CanGoForward { get; }

		void Stop();

		void Reload();

		string DocumentTitle { get; }

		string ExecuteScript(string script);

		void ShowPrintDialog();

		bool BrowserContextMenuEnabled { get; set; }
	}

	public class WebViewLoadedEventArgs : EventArgs
	{
		public Uri Uri { get; private set; }

		public WebViewLoadedEventArgs(Uri uri)
		{
			this.Uri = uri;
		}
	}

	public class WebViewLoadingEventArgs : WebViewLoadedEventArgs
	{
		public bool Cancel { get; set; }

		public bool IsMainFrame { get; set; }

		public WebViewLoadingEventArgs(Uri uri, bool isMainFrame)
			: base(uri)
		{
			this.IsMainFrame = isMainFrame;
		}
	}

	public class WebViewTitleEventArgs : EventArgs
	{
		public string Title { get; private set; }

		public WebViewTitleEventArgs(string title)
		{
			this.Title = title;
		}
	}

	public class WebViewNewWindowEventArgs : WebViewLoadingEventArgs
	{
		public string NewWindowName { get; private set; }

		public WebViewNewWindowEventArgs(Uri uri, string newWindowName)
			: base(uri, false)
		{
			this.NewWindowName = newWindowName;
		}
	}

	[Handler(typeof(IWebView))]
	public class WebView : Control
	{
		new IWebView Handler { get { return (IWebView)base.Handler; } }

		#region Events

		public const string NavigatedEvent = "WebView.Navigated";

		public event EventHandler<WebViewLoadedEventArgs> Navigated
		{
			add { Properties.AddHandlerEvent(NavigatedEvent, value); }
			remove { Properties.RemoveEvent(NavigatedEvent, value); }
		}

		protected virtual void OnNavigated(WebViewLoadedEventArgs e)
		{
			Properties.TriggerEvent(NavigatedEvent, this, e);
		}

		public const string DocumentLoadedEvent = "WebView.DocumentLoaded";

		public event EventHandler<WebViewLoadedEventArgs> DocumentLoaded
		{
			add { Properties.AddHandlerEvent(DocumentLoadedEvent, value); }
			remove { Properties.RemoveEvent(DocumentLoadedEvent, value); }
		}

		protected virtual void OnDocumentLoaded(WebViewLoadedEventArgs e)
		{
			Properties.TriggerEvent(DocumentLoadedEvent, this, e);
		}

		public const string DocumentLoadingEvent = "WebView.DocumentLoading";

		public event EventHandler<WebViewLoadingEventArgs> DocumentLoading
		{
			add { Properties.AddHandlerEvent(DocumentLoadingEvent, value); }
			remove { Properties.RemoveEvent(DocumentLoadingEvent, value); }
		}

		protected virtual void OnDocumentLoading(WebViewLoadingEventArgs e)
		{
			Properties.TriggerEvent(DocumentLoadingEvent, this, e);
		}

		public const string OpenNewWindowEvent = "WebView.OpenNewWindow";

		public event EventHandler<WebViewNewWindowEventArgs> OpenNewWindow
		{
			add { Properties.AddHandlerEvent(OpenNewWindowEvent, value); }
			remove { Properties.RemoveEvent(OpenNewWindowEvent, value); }
		}

		protected virtual void OnOpenNewWindow(WebViewNewWindowEventArgs e)
		{
			Properties.TriggerEvent(OpenNewWindowEvent, this, e);
		}

		public const string DocumentTitleChangedEvent = "WebView.DocumentTitleChanged";

		public event EventHandler<WebViewTitleEventArgs> DocumentTitleChanged
		{
			add { Properties.AddHandlerEvent(DocumentTitleChangedEvent, value); }
			remove { Properties.RemoveEvent(DocumentTitleChangedEvent, value); }
		}

		protected virtual void OnDocumentTitleChanged(WebViewTitleEventArgs e)
		{
			Properties.TriggerEvent(DocumentTitleChangedEvent, this, e);
		}

		#endregion

		static WebView()
		{
			EventLookup.Register<WebView>(c => c.OnNavigated(null), WebView.NavigatedEvent);
			EventLookup.Register<WebView>(c => c.OnDocumentLoaded(null), WebView.DocumentLoadedEvent);
			EventLookup.Register<WebView>(c => c.OnDocumentLoading(null), WebView.DocumentLoadingEvent);
			EventLookup.Register<WebView>(c => c.OnDocumentTitleChanged(null), WebView.DocumentTitleChangedEvent);
			EventLookup.Register<WebView>(c => c.OnOpenNewWindow(null), WebView.OpenNewWindowEvent);
		}

		public WebView()
		{
		}

		[Obsolete("Use default constructor instead")]
		public WebView(Generator generator)
			: this(generator, typeof(IWebView))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected WebView(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public void GoBack()
		{
			Handler.GoBack();
		}

		public bool CanGoBack
		{
			get{ return Handler.CanGoBack; }
		}

		public void GoForward()
		{
			Handler.GoForward();
		}

		public bool CanGoForward
		{
			get { return Handler.CanGoForward; }
		}

		public Uri Url
		{
			get { return Handler.Url; }
			set { Handler.Url = value; }
		}

		public void Stop()
		{
			Handler.Stop();
		}

		public void Reload()
		{
			Handler.Reload();
		}

		public string ExecuteScript(string script)
		{
			return Handler.ExecuteScript(script);
		}

		public string DocumentTitle
		{
			get { return Handler.DocumentTitle; }
		}

		public void LoadHtml(Stream stream, Uri baseUri = null)
		{
			using (var reader = new StreamReader(stream))
			{
				Handler.LoadHtml(reader.ReadToEnd(), baseUri);
			}
		}

		public void LoadHtml(string html, Uri baseUri = null)
		{
			Handler.LoadHtml(html, baseUri);
		}

		public void ShowPrintDialog()
		{
			Handler.ShowPrintDialog();
		}

		public bool BrowserContextMenuEnabled
		{
			get { return Handler.BrowserContextMenuEnabled; }
			set { Handler.BrowserContextMenuEnabled = value; }
		}

		public interface ICallback : Control.ICallback
		{
			void OnNavigated(WebView widget, WebViewLoadedEventArgs e);
			void OnDocumentLoaded(WebView widget, WebViewLoadedEventArgs e);
			void OnDocumentLoading(WebView widget, WebViewLoadingEventArgs e);
			void OnOpenNewWindow(WebView widget, WebViewNewWindowEventArgs e);
			void OnDocumentTitleChanged(WebView widget, WebViewTitleEventArgs e);
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		protected class Callback : Control.Callback, ICallback
		{
			public void OnNavigated(WebView widget, WebViewLoadedEventArgs e)
			{
				widget.OnNavigated(e);
			}
			public void OnDocumentLoaded(WebView widget, WebViewLoadedEventArgs e)
			{
				widget.OnDocumentLoaded(e);
			}
			public void OnDocumentLoading(WebView widget, WebViewLoadingEventArgs e)
			{
				widget.OnDocumentLoading(e);
			}
			public void OnOpenNewWindow(WebView widget, WebViewNewWindowEventArgs e)
			{
				widget.OnOpenNewWindow(e);
			}
			public void OnDocumentTitleChanged(WebView widget, WebViewTitleEventArgs e)
			{
				widget.OnDocumentTitleChanged(e);
			}
		}
	}
}

