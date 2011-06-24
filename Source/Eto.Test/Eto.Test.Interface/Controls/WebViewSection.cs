using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class WebViewSection : Panel
	{
		public WebViewSection ()
		{
			var webview = new WebView();
			webview.Url = new Uri("http://www.google.com");
			this.AddDockedControl(webview);
		}
	}
}

