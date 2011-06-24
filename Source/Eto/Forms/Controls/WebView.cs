using System;

namespace Eto.Forms
{
	public interface IWebView : IControl
	{
		Uri Url { get; set; }
	}
	
	public class WebView : Control
	{
		IWebView inner;
		
		public WebView ()
			: this (Generator.Current)
		{
		}
		
		public WebView (Generator generator)
			: base(generator, typeof(IWebView))
		{
			inner = (IWebView)Handler;
		}

		public Uri Url
		{
			get { return inner.Url; }
			set { inner.Url = value; }
		}
	}
}

