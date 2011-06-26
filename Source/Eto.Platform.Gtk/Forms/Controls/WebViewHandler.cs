using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class WebViewHandler : GtkControl<Gtk.ScrolledWindow, WebView>, IWebView
	{
		WebKit.WebView webView;
		
		public WebViewHandler ()
		{
			Control = new Gtk.ScrolledWindow ();

			webView = new WebKit.WebView(); 
			
			Control.Add (webView);
		}

		#region IWebView implementation
		
		public Uri Url {
			get { return new Uri(webView.Uri); }
			set {
				if (value != null) 
				{
					webView.LoadUri (value.AbsoluteUri);
				}
				else 
					webView.LoadHtmlString (string.Empty, string.Empty);
			}
		}
		#endregion

	}
}

