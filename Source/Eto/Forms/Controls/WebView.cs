using System;
using System.IO;

namespace Eto.Forms
{
	public interface IWebView : IControl
	{
		Uri Url { get; set; }

		void LoadHtml (string html, Uri baseUri);
		
		void GoBack ();
		
		bool CanGoBack { get; }

		void GoForward ();
		
		bool CanGoForward { get; }
		
		void Stop ();
		
		void Reload ();
		
		string DocumentTitle { get; }
		
		string ExecuteScript (string script);

		void ShowPrintDialog();

		bool BrowserContextMenuEnabled { get; set; }
	}
	
	public class WebViewLoadedEventArgs : EventArgs
	{
		public Uri Uri { get; private set; }
		
		public WebViewLoadedEventArgs (Uri uri)
		{
			this.Uri = uri;
		}
	}
	
	public class WebViewLoadingEventArgs : WebViewLoadedEventArgs
	{
		public bool Cancel { get; set; }
		
		public bool IsMainFrame { get; set; }
		
		public WebViewLoadingEventArgs (Uri uri, bool isMainFrame)
			: base(uri)
		{
			this.IsMainFrame = isMainFrame;
		}
	}
	
	public class WebViewTitleEventArgs : EventArgs
	{
		public string Title { get; private set; }
		
		public WebViewTitleEventArgs (string title)
		{
			this.Title = title;
		}
	}
	
	public class WebViewNewWindowEventArgs : WebViewLoadingEventArgs
	{
		public string NewWindowName { get; private set; }
		
		public WebViewNewWindowEventArgs (Uri uri, string newWindowName)
			: base (uri, false)
		{
			this.NewWindowName = newWindowName;
		}
	}
	
	public class WebView : Control
	{
		new IWebView Handler { get { return (IWebView)base.Handler; } }
		
		#region Events

        #region Navigated
        public const string NavigatedEvent = "WebView.Navigated";
        EventHandler<WebViewLoadedEventArgs> navigated;

        public event EventHandler<WebViewLoadedEventArgs> Navigated
        {
            add
            {
                HandleEvent(NavigatedEvent);
                navigated += value;
            }
            remove { navigated -= value; }
        }

        public virtual void OnNavigated(WebViewLoadedEventArgs e)
        {
            if (navigated != null)
                navigated(this, e);
        }

        #endregion

        public const string DocumentLoadedEvent = "WebView.DocumentLoaded";
		EventHandler<WebViewLoadedEventArgs> documentLoaded;

		public event EventHandler<WebViewLoadedEventArgs> DocumentLoaded {
			add { 
				HandleEvent (DocumentLoadedEvent);
				documentLoaded += value;
			}
			remove { documentLoaded -= value; }
		}
		
		public virtual void OnDocumentLoaded (WebViewLoadedEventArgs e)
		{
			if (documentLoaded != null)
				documentLoaded (this, e);
		}
		
		public const string DocumentLoadingEvent = "WebView.DocumentLoading";
		EventHandler<WebViewLoadingEventArgs> documentLoading;

		public event EventHandler<WebViewLoadingEventArgs> DocumentLoading {
			add { 
				HandleEvent (DocumentLoadingEvent);
				documentLoading += value;
			}
			remove { documentLoading -= value; }
		}
		
		public virtual void OnDocumentLoading (WebViewLoadingEventArgs e)
		{
			if (documentLoading != null)
				documentLoading (this, e);
		}

		public const string DocumentTitleChangedEvent = "WebView.DocumentTitleChanged";
		EventHandler<WebViewTitleEventArgs> documentTitleChanged;

		public event EventHandler<WebViewTitleEventArgs> DocumentTitleChanged {
			add { 
				HandleEvent (DocumentTitleChangedEvent);
				documentTitleChanged += value;
			}
			remove { documentTitleChanged -= value; }
		}
		
		public const string OpenNewWindowEvent = "WebView.OpenNewWindow";

		EventHandler<WebViewNewWindowEventArgs> _OpenNewWindow;

		public event EventHandler<WebViewNewWindowEventArgs> OpenNewWindow {
			add {
				HandleEvent (OpenNewWindowEvent);
				_OpenNewWindow += value;
			}
			remove { _OpenNewWindow -= value; }
		}

		public virtual void OnOpenNewWindow (WebViewNewWindowEventArgs e)
		{
			if (_OpenNewWindow != null)
				_OpenNewWindow (this, e);
		}
		
		
		public virtual void OnDocumentTitleChanged (WebViewTitleEventArgs e)
		{
			if (documentTitleChanged != null)
				documentTitleChanged (this, e);
		}
		
		#endregion
		
		public WebView ()
			: this (Generator.Current)
		{
		}
		
		public WebView (Generator generator)
			: this (generator, typeof(IWebView))
		{
		}
		
		protected WebView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public void GoBack ()
		{
			Handler.GoBack ();
		}
		
		public bool CanGoBack {
			get{ return Handler.CanGoBack; }
		}
		
		public void GoForward ()
		{
			Handler.GoForward ();
		}
		
		public bool CanGoForward {
			get { return Handler.CanGoForward; }
		}

		public Uri Url {
			get { return Handler.Url; }
			set { Handler.Url = value; }
		}
		
		public void Stop ()
		{
			Handler.Stop ();
		}
		
		public void Reload ()
		{
			Handler.Reload ();
		}
		
		public string ExecuteScript (string script)
		{
			return Handler.ExecuteScript (script);
		}

		public string DocumentTitle {
			get { return Handler.DocumentTitle; }
		}

		public void LoadHtml (Stream stream, Uri baseUri = null)
		{
			using (var reader = new StreamReader(stream)) {
				Handler.LoadHtml (reader.ReadToEnd (), baseUri);
			}
		}
		
		public void LoadHtml (string html, Uri baseUri = null)
		{
			Handler.LoadHtml (html, baseUri);
		}

        public void ShowPrintDialog ()
        {
			Handler.ShowPrintDialog ();
        }

		public bool BrowserContextMenuEnabled {
			get { return Handler.BrowserContextMenuEnabled; }
			set { Handler.BrowserContextMenuEnabled = value; }
		}
	}
}

