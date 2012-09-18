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
				LoadHtml();
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
			var layout = new DynamicLayout (new Panel (), spacing: Size.Empty);

			layout.BeginHorizontal ();
			layout.Add (null);
			layout.Add (BackButton ());
			layout.Add (ForwardButton ());
			layout.Add (LoadHtmlButton ());
			layout.Add (LoadUrl ());
			layout.Add (ReloadButton ());
			layout.Add (StopButton ());
			layout.Add (ExecuteScriptButton ());
			layout.Add (PrintButton ());
			layout.Add (null);
			layout.EndHorizontal ();

			
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

		Control PrintButton ()
		{
			var control = printButton = new Button {
				Text = "Print",
			};
			control.Click += delegate {
				webView.ShowPrintDialog ();
			};
			return control;
		}
		
		Control ExecuteScriptButton ()
		{
			var control = new Button{
				Text = "Execute Script"
			};
			control.Click += delegate {
				var ret = webView.ExecuteScript ("alert('this is called from code'); return 'return value from ExecuteScript';");
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
				LoadHtml();
			};
			return control;
		}

		void LoadHtml()
		{
			webView.LoadHtml (@"<html>
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
			<button onclick='print()'>Print</button>
		</p>
		<p><h2>Test Selecting a File</h2>
			<input type='file'>
		</p>
		<p><h2>Test Alert</h2>
			<button onclick='alert(""This is an alert"")'>Show Alert</button>
		</p>
		<p><h2>Test Confirm</h2>
			<button onclick=""appendResult('confirmResult', confirm('Confirm yes or no'));"">Show Confirm</button>
			<ul id='confirmResult'></ul>
		</p>
		<p><h2>Test Prompt</h2>
			<button onclick=""appendResult('inputResult', prompt('Enter some text', 'some default text'));"">Show Prompt</button>
			<ul id='inputResult'></ul>
		</p>
	</form>
</body>

</html>");
		}

		Control LoadUrl ()
		{
			var control = new Button{
				Text = "Load Url"
			};
			control.Click += delegate {
				var dialog = new Dialog();
				dialog.MinimumSize = new Size(300, 0);
				var layout = new DynamicLayout(dialog);
				var textBox = new TextBox { Text = "http://google.com" };
				var goButton = new Button { Text = "Go" };
				dialog.DefaultButton = goButton;
				goButton.Click += (sender, e) => {
					dialog.DialogResult = DialogResult.Ok;
					dialog.Close ();
				};
				var cancelButton = new Button { Text = "Cancel" };
				dialog.AbortButton = cancelButton;
				cancelButton.Click += (sender, e) => {
					dialog.Close ();
				};
				layout.BeginVertical ();
				layout.AddRow (new Label { Text = "Url" }, textBox);
				layout.EndBeginVertical ();
				layout.AddRow (null, cancelButton, goButton);
				layout.EndVertical ();

				if (dialog.ShowDialog (this) == DialogResult.Ok) {
					Uri uri;
					if (Uri.TryCreate(textBox.Text, UriKind.Absolute, out uri))
						webView.Url = uri;
				}
			};
			return control;
		}
	}
}

