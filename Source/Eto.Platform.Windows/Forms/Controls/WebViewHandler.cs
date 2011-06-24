using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class WebViewHandler : WindowsControl<SWF.WebBrowser, WebView>, IWebView
	{
		public WebViewHandler ()
		{
			this.Control = new SWF.WebBrowser();
		}

		#region IWebView implementation
		
		public Uri Url {
			get { return this.Control.Url; }
			set { this.Control.Url = value; }
		}
		#endregion

	}
}

