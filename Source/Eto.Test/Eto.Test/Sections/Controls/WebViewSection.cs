using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class WebViewSection : Panel
	{
		WebView webView;
		Button goBack;
		Button goForward;
		Button stopButton;
        Button printButton;
		Label titleLabel;
		
		public WebViewSection ()
		{
			var layout = new TableLayout (this, 1, 3);
			
			int row = 0;
			layout.Add (Buttons (), 0, row++);
			layout.Add (TitleLabel (), 0, row++);
			layout.Add (WebView (), 0, row++, true, true);
		}
		
		Control WebView ()
		{
			try {
				var control = webView = new WebView ();
			
				control.DocumentLoading += delegate(object sender, WebViewLoadingEventArgs e) {
					UpdateButtons ();
					stopButton.Enabled = true;
				};
				control.DocumentLoaded += delegate(object sender, WebViewLoadedEventArgs e) {
					UpdateButtons ();
					stopButton.Enabled = false;
				};
				control.DocumentTitleChanged += delegate(object sender, WebViewTitleEventArgs e) {
					titleLabel.Text = e.Title;
				};
				control.Url = new Uri ("http://www.google.com");
				return control;

			} catch (HandlerInvalidException ex) {
				var control = new Label { 
					Text = string.Format ("WebView not supported on this platform with the {0} generator", Generator.ID),
					BackgroundColor = Colors.Red,
					HorizontalAlign = HorizontalAlign.Center,
					VerticalAlign = VerticalAlign.Middle,
					TextColor = Colors.White
				};
				if (Generator.ID == Generators.Gtk)
					Log.Write (this, "You must install webkit-sharp for WebView to work under GTK. Note that GTK does not support webkit-sharp on any platform other than Linux.");
				return control;
			}

			
		}

		Control TitleLabel ()
		{
			titleLabel = new Label{};
			return titleLabel;
		}
		
		void UpdateButtons ()
		{
			goBack.Enabled = webView.CanGoBack;
			goForward.Enabled = webView.CanGoForward;
		}
		
		Control Buttons ()
		{
			var layout = new TableLayout (new Panel (), 8, 1);
			
			int col = 0;
			layout.Add (BackButton (), col++, 0);
			layout.Add (ForwardButton (), col++, 0);
			layout.Add (LoadHtmlButton (), col++, 0);
			layout.Add (ReloadButton (), col++, 0);
			layout.Add (StopButton (), col++, 0);
			layout.Add (ExecuteScriptButton (), col++, 0);
            layout.Add (PrintButton(), col++, 0);
			
			layout.SetColumnScale (col++);
			
			
			return layout.Container;
		}
		
		Control BackButton ()
		{
			var control = goBack = new Button{
				Text = "Back"
			};
			control.Click += delegate {
				webView.GoBack ();
			};
			return control;
		}

		Control ForwardButton ()
		{
			var control = goForward = new Button{
				Text = "Forward"
			};
			control.Click += delegate {
				webView.GoForward ();
			};
			return control;
		}
		Control ReloadButton ()
		{
			var control = new Button{
				Text = "Reload"
			};
			control.Click += delegate {
				webView.Reload ();
			};
			return control;
		}

		Control StopButton ()
		{
			var control = stopButton = new Button{
				Text = "Stop",
				Enabled = false
			};
			control.Click += delegate {
				webView.Stop ();
				stopButton.Enabled = false;
			};
			return control;
		}

        Control PrintButton()
        {
            var control = printButton = new Button
            {
                Text = "Print",
            };
            control.Click += delegate
            {
                webView.ShowPrintDialog();
            };
            return control;
        }
		
		Control ExecuteScriptButton ()
		{
			var control = new Button{
				Text = "Execute Script"
			};
			control.Click += delegate {
				var ret = webView.ExecuteScript("alert('this is called from code'); return 'return value from ExecuteScript';");
				Log.Write (this, "ExecuteScript, Return: {0}", ret);
			};
			return control;
		}

		Control LoadHtmlButton ()
		{
			var control = new Button{
				Text = "Load HTML"
			};
			control.Click += delegate {
				webView.LoadHtml ("<html><head><title>Hello!</title></head><body><h1>Some custom html</h1></body></html>");
			};
			return control;
		}
	}
}

