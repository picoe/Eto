using System;
using Eto.Forms;
using Eto.Drawing;
using VerticalAlign = Eto.Forms.VerticalAlign;

namespace Eto.Test.Sections.Controls
{
	public class WebViewSection : Scrollable
	{
		WebView webView;
		Button goBack;
		Button goForward;
		Button stopButton;
		Label titleLabel;
		CheckBox cancelLoad;

		public WebViewSection()
		{
			var layout = new DynamicLayout();

			var webContainer = WebView();
			layout.Add(Buttons());
			layout.AddSeparateRow(TitleLabel(), null, CancelLoad(), EnableContextMenu());
			layout.Add(webContainer, yscale: true);

			if (webView != null)
				LoadHtml();

			Content = layout;
		}

		Control CancelLoad()
		{
			return cancelLoad = new CheckBox { Text = "Cancel Load" };
		}

		Control WebView()
		{
			try
			{
				webView = new WebView();

				webView.Navigated += (sender, e) => {
					Log.Write(webView, "Navigated, Uri: {0}", e.Uri);
					UpdateButtons();
				};
				webView.DocumentLoading += (sender, e) => {
					Log.Write(webView, "DocumentLoading, Uri: {0}, IsMainFrame: {1}", e.Uri, e.IsMainFrame);
					e.Cancel = cancelLoad.Checked ?? false;
					if (!e.Cancel)
					{
						UpdateButtons();
						stopButton.Enabled = true;
					}
				};
				webView.DocumentLoaded += (sender, e) => {
					Log.Write(webView, "DocumentLoaded, Uri: {0}", e.Uri);
					UpdateButtons();
					stopButton.Enabled = false;
				};
				webView.OpenNewWindow += (sender, e) => {
					Log.Write(webView, "OpenNewWindow: {0}, Url: {1}", e.NewWindowName, e.Uri);
				};
				webView.DocumentTitleChanged += delegate (object sender, WebViewTitleEventArgs e)
				{
					titleLabel.Text = e.Title;
				};
				return webView;

			}
			catch (HandlerInvalidException)
			{
				var control = new Label
				{ 
					Text = string.Format ("WebView not supported on this platform with the {0} generator", Generator.ID),
					BackgroundColor = Colors.Red,
					HorizontalAlign = HorizontalAlign.Center,
					VerticalAlign = VerticalAlign.Middle,
					TextColor = Colors.White
				};
				if (Generator.ID == Generators.Gtk)
					Log.Write(this, "You must install webkit-sharp for WebView to work under GTK. Note that GTK does not support webkit-sharp on any platform other than Linux.");
				return control;
			}

			
		}

		Control TitleLabel()
		{
			titleLabel = new Label { };
			return titleLabel;
		}

		Control EnableContextMenu()
		{
			var control = new CheckBox { Text = "Enable Context Menu" };
			control.Bind(r => r.Checked, this.webView, w => w.BrowserContextMenuEnabled);
			return control;
		}

		void UpdateButtons()
		{
			goBack.Enabled = webView.CanGoBack;
			goForward.Enabled = webView.CanGoForward;
		}

		Control Buttons()
		{
			var layout = new DynamicLayout(spacing: Size.Empty);

			layout.BeginHorizontal();
			layout.Add(null);
			layout.Add(BackButton());
			layout.Add(ForwardButton());
			layout.Add(LoadHtmlButton());
			layout.Add(LoadUrl());
			layout.Add(ReloadButton());
			layout.Add(StopButton());
			layout.Add(ExecuteScriptButton());
			layout.Add(PrintButton());
			layout.Add(null);
			layout.EndHorizontal();

			return layout;
		}

		Control BackButton()
		{
			var control = goBack = new Button
			{
				Text = "Back"
			};
			control.Click += delegate
			{
				webView.GoBack();
			};
			return control;
		}

		Control ForwardButton()
		{
			var control = goForward = new Button
			{
				Text = "Forward"
			};
			control.Click += delegate
			{
				webView.GoForward();
			};
			return control;
		}

		Control ReloadButton()
		{
			var control = new Button
			{
				Text = "Reload"
			};
			control.Click += delegate
			{
				webView.Reload();
			};
			return control;
		}

		Control StopButton()
		{
			var control = stopButton = new Button
			{
				Text = "Stop",
				Enabled = false
			};
			control.Click += delegate
			{
				webView.Stop();
				stopButton.Enabled = false;
			};
			return control;
		}

		Control PrintButton()
		{
			var control = new Button
			{
				Text = "Print",
			};
			control.Click += delegate
			{
				webView.ShowPrintDialog();
			};
			return control;
		}

		Control ExecuteScriptButton()
		{
			var control = new Button
			{
				Text = "Execute Script"
			};
			control.Click += delegate
			{
				var ret = webView.ExecuteScript("alert('this is called from code'); return 'return value from ExecuteScript';");
				Log.Write(this, "ExecuteScript, Return: {0}", ret);
			};
			return control;
		}

		Control LoadHtmlButton()
		{
			var control = new Button
			{
				Text = "Load HTML"
			};
			control.Click += delegate
			{
				LoadHtml();
			};
			return control;
		}

		void LoadHtml()
		{
			webView.LoadHtml(@"<html>
<head><title>Hello!</title></head>
<body>
	<h1>Some custom html</h1>
	<script type='text/javascript'>
		function appendResult(id, value) {
			var element = document.getElementById(id);

			var result = document.createElement('li');
			result.appendChild(document.createTextNode('result: ' + value));
			element.appendChild(result);
		}
	</script>
	<form method='post' enctype='multipart/form-data'>
		<p><h2>Test Printing</h2>
			<button onclick='print(); return false;'>Print</button>
		</p>
		<p><h2>Test Selecting a File</h2>
			<input type='file'>
		</p>
		<p><h2>Test Alert</h2>
			<button onclick='alert(""This is an alert""); return false;'>Show Alert</button>
		</p>
		<p><h2>Test Confirm</h2>
			<button onclick=""appendResult('confirmResult', confirm('Confirm yes or no')); return false;"">Show Confirm</button>
			<ul id='confirmResult'></ul>
		</p>
		<p><h2>Test Prompt</h2>
			<button onclick=""appendResult('inputResult', prompt('Enter some text', 'some default text')); return false;"">Show Prompt</button>
			<ul id='inputResult'></ul>
		</p>
		<p><h2>Test Navigation</h2>
			<button onclick=""window.location = 'http://www.example.com'; return false;"">Set location</button>
			<button onclick=""window.open('http://www.example.com'); return false;"">Open new window</button>
			<button onclick=""window.open('http://www.example.com', 'name_of_new_window'); return false;"">Open named window</button>
			<br>
			<a href='http://www.example.com'>Open link in this window</a>
			<a href='http://www.example.com' target='_blank'>Open in new window</a>
			<a href='http://www.example.com' target='another_name_of_new_window'>Open in named window</a>
		</p>
	</form>
</body>

</html>");
		}

		Control LoadUrl()
		{
			var control = new Button
			{
				Text = "Load Url"
			};
			control.Click += delegate
			{
				var dialog = new Dialog();
#if DESKTOP
				dialog.MinimumSize = new Size(300, 0);
#endif
				var layout = new DynamicLayout();
				var textBox = new TextBox { Text = "http://google.com" };
				var goButton = new Button { Text = "Go" };
				dialog.DefaultButton = goButton;
				goButton.Click += (sender, e) => {
					dialog.DialogResult = DialogResult.Ok;
					dialog.Close();
				};
				var cancelButton = new Button { Text = "Cancel" };
				dialog.AbortButton = cancelButton;
				cancelButton.Click += (sender, e) => {
					dialog.Close();
				};
				layout.BeginVertical();
				layout.AddRow(new Label { Text = "Url" }, textBox);
				layout.EndBeginVertical();
				layout.AddRow(null, cancelButton, goButton);
				layout.EndVertical();

				dialog.Content = layout;

				if (dialog.ShowDialog(this) == DialogResult.Ok)
				{
					Uri uri;
					if (Uri.TryCreate(textBox.Text, UriKind.Absolute, out uri))
						webView.Url = uri;
				}
			};
			return control;
		}
	}
}

