using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(WebView))]
	public class WebViewSection : Panel
	{
		WebView webView;
		Button goBack;
		Button goForward;
		Button stopButton;
		Label titleLabel;
		CheckBox cancelLoad;

		public WebViewSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

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

				webView.Navigated += (sender, e) =>
				{
					Log.Write(webView, "Navigated, Uri: {0}", e.Uri);
					UpdateButtons();
				};
				webView.DocumentLoading += (sender, e) =>
				{
					Log.Write(webView, "DocumentLoading, Uri: {0}, IsMainFrame: {1}", e.Uri, e.IsMainFrame);
					e.Cancel = cancelLoad.Checked ?? false;
					if (!e.Cancel)
					{
						UpdateButtons();
						stopButton.Enabled = true;
					}
				};
				webView.DocumentLoaded += (sender, e) =>
				{
					Log.Write(webView, "DocumentLoaded, Uri: {0}", e.Uri);
					UpdateButtons();
					stopButton.Enabled = false;
				};
				webView.OpenNewWindow += (sender, e) =>
				{
					Log.Write(webView, "OpenNewWindow: {0}, Url: {1}", e.NewWindowName, e.Uri);
					e.Cancel = cancelLoad.Checked ?? false;
				};
				webView.DocumentTitleChanged += delegate (object sender, WebViewTitleEventArgs e)
				{
					titleLabel.Text = e.Title;
					Log.Write(webView, $"DocumentTitleChange: {e.Title}");
				};
				return webView;

			}
			catch (Exception ex)
			{
				var control = new Label
				{
					Text = $"WebView not supported on this platform with the {Platform.ID} generator\n{ex}",
					BackgroundColor = Colors.Red,

					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					TextColor = Colors.White
				};
				if (Platform.IsGtk)
					Log.Write(this, "You must install webkit-sharp for WebView to work under GTK. Note that GTK does not support webkit-sharp on any platform other than Linux.");
				return control;
			}


		}

		Control TitleLabel()
		{
			titleLabel = new Label();
			return titleLabel;
		}

		Control EnableContextMenu()
		{
			var control = new CheckBox { Text = "Enable Context Menu" };
			control.Bind(r => r.Checked, webView, w => w.BrowserContextMenuEnabled);
			return control;
		}

		void UpdateButtons()
		{
			goBack.Enabled = webView.CanGoBack;
			goForward.Enabled = webView.CanGoForward;
		}

		Control Buttons()
		{
			var layout = new DynamicLayout { };

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

		int executeScriptCount;

		Control ExecuteScriptButton()
		{
			var control = new Button
			{
				Text = "Execute Script"
			};
			control.Click += delegate
			{
				var script = $"alert('this is called from code {executeScriptCount}');\nreturn 'value from ExecuteScript: {executeScriptCount++}';";
				script = PromptString("Script", script, "Execute", true);
				if (!string.IsNullOrEmpty(script))
				{
					var ret = webView.ExecuteScript(script);
					Log.Write(this, "ExecuteScript, Return: {0}", ret);
				}
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
		var titleChange = 0;

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
		<p><h2>Test Custom Protocol</h2>
			<button onclick=""window.location='eto:dosomething'; return false;"">window.location='eto:dosomething'</button>
			<button onclick=""window.location.replace('eto://dosomething')"">window.location.replace('eto://dosomething')</button>
			<button onclick=""window.open('eto:dosomething'); return false;"">window.open('eto:dosomething')</button>
			<button onclick=""window.open('eto:dosomething', 'name_of_new_window'); return false;"">window.open('eto:dosomething', 'name_of_new_window')</button>
			<br>
			<a href='eto:dosomething'>href='eto:dosomething'</a>
			<a href='eto:dosomething' target='_blank'>href='eto:dosomething' target='_blank'</a>
			<a href='eto:dosomething' target='another_name_of_new_window'>href='eto:dosomething' target='another_name_of_new_window'</a>
		</p>
		<p><h2>Dynamic title changes</h2>
			<button onclick=""document.title = 'some title ' + titleChange++; return false;"">Change title</button>
		</p>
		<h2>Input Types</h2>
		<table>
			<tr>
				<td>Button</td>
				<td><input type='button'></td>
			</tr>
			<tr>
				<td>Checkbox</td>
				<td><input type='checkbox'></td>
			</tr>
			<tr>
				<td>Color</td>
				<td><input type='color'></td>
			</tr>
			<tr>
				<td>Date</td>
				<td><input type='date'></td>
			</tr>
			<tr>
				<td>DateTime</td>
				<td><input type='datetime'></td>
			</tr>
			<tr>
				<td>Email</td>
				<td><input type='email'></td>
			</tr>
			<tr>
				<td>File</td>
				<td><input type='file'></td>
			</tr>
			<tr>
				<td>Hidden</td>
				<td><input type='hidden'></td>
			</tr>
			<tr>
				<td>Image</td>
				<td><input type='image'></td>
			</tr>
			<tr>
				<td>Month</td>
				<td><input type='month'></td>
			</tr>
			<tr>
				<td>Number</td>
				<td><input type='number'></td>
			</tr>
			<tr>
				<td>Password</td>
				<td><input type='password'></td>
			</tr>
			<tr>
				<td>Radio</td>
				<td><input type='radio'></td>
			</tr>
			<tr>
				<td>Range</td>
				<td><input type='range'></td>
			</tr>
			<tr>
				<td>Reset</td>
				<td><input type='reset'></td>
			</tr>
			<tr>
				<td>Search</td>
				<td><input type='search'></td>
			</tr>
			<tr>
				<td>Submit</td>
				<td><input type='submit'></td>
			</tr>
			<tr>
				<td>Tel</td>
				<td><input type='tel'></td>
			</tr>
			<tr>
				<td>Text</td>
				<td><input type='text'></td>
			</tr>
			<tr>
				<td>Time</td>
				<td><input type='time'></td>
			</tr>
			<tr>
				<td>Url</td>
				<td><input type='url'></td>
			</tr>
			<tr>
				<td>Week</td>
				<td><input type='week'></td>
			</tr>
			<tr>
				<td>TextArea</td>
				<td><textarea></textarea></td>
			</tr>
		</table>
	</form>
</body>

</html>");
		}

		string PromptString(string title, string initialValue = null, string action = null, bool largeText = false)
		{
			if (!Platform.Supports<Dialog>())
				return initialValue;

			var dialog = new Dialog<bool>();
			if (Platform.IsDesktop)
				dialog.MinimumSize = new Size(300, 0);

			var layout = new DynamicLayout();
			layout.Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);
			layout.DefaultSpacing = new Size(5, 5);
			layout.Padding = 10;
			var textBox = largeText ? (TextControl)new TextArea { TextReplacements = TextReplacements.None } : new TextBox();
			textBox.Text = initialValue;

			var goButton = new Button { Text = action ?? "OK" };
			dialog.DefaultButton = goButton;
			goButton.Click += (sender, e) => dialog.Close(true);
			var cancelButton = new Button { Text = "Cancel" };
			dialog.AbortButton = cancelButton;
			cancelButton.Click += (sender, e) => dialog.Close(false);
			layout.BeginVertical();
			layout.AddRow(new Label { Text = title }, textBox);
			layout.EndBeginVertical();
			layout.AddRow(null, cancelButton, goButton);
			layout.EndVertical();

			dialog.Content = layout;

			if (dialog.ShowModal(this))
				return textBox.Text;
			return null;
		}

		Control LoadUrl()
		{
			var control = new Button
			{
				Text = "Load Url"
			};
			control.Click += delegate
			{
				var url = PromptString("Url", "https://html5test.com", "Go");
				if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
				{
					webView.Url = uri;
				}
			};
			return control;
		}
	}
}

