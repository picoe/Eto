using System;
using Eto.Forms;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class WebViewHandler : MacView<MonoMac.WebKit.WebView, WebView>, IWebView
	{
		public WebViewHandler ()
		{
			Control = new MonoMac.WebKit.WebView ();
		}

		#region IWebView implementation
		
		public Uri Url {
			get { 
				return new Uri(Control.MainFrameUrl);
			}
			set { 
				if (value != null) Control.MainFrameUrl = value.AbsoluteUri;
				else Control.MainFrameUrl = null;
			}
		}
		
		#endregion

	}
}

