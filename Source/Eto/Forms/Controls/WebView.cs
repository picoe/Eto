using System;

namespace Eto.Forms
{
	public interface IWebView : IControl
	{
		Uri Url { get; set; }

		void SetHtml (string html, string baseUrl);
		
		void GoBack ();
		
		bool CanGoBack { get; }

		void GoForward ();
		
		bool CanGoForward { get; }
		
		void Stop ();
		
		void Reload ();
		
		string DocumentTitle { get; }
		
		void ExecuteScript (string script);
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
		
		public WebViewLoadingEventArgs (Uri uri)
			: base(uri)
		{
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
	
	public class WebView : Control
	{
		IWebView inner;
		#region Events
		
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
			: base(generator, typeof(IWebView))
		{
			inner = (IWebView)Handler;
		}
		
		public void GoBack ()
		{
			inner.GoBack ();
		}
		
		public bool CanGoBack {
			get{ return inner.CanGoBack; }
		}
		
		public void GoForward ()
		{
			inner.GoForward ();
		}
		
		public bool CanGoForward {
			get { return inner.CanGoForward; }
		}

		public Uri Url {
			get { return inner.Url; }
			set { inner.Url = value; }
		}
		
		public void Stop ()
		{
			inner.Stop ();
		}
		
		public void Reload ()
		{
			inner.Reload ();
		}
		
		public void ExecuteScript (string script)
		{
			inner.ExecuteScript (script);
		}

		public string DocumentTitle {
			get { return inner.DocumentTitle; }
		}
		
		public void SetHtml (string html, string baseUrl)
		{
			inner.SetHtml (html, baseUrl);
		}
	}
}

