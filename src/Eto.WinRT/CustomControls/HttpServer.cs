#if TODO_XAML
using System;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Net.Mime;
using System.IO;

namespace Eto.Platform.CustomControls
{
	public class HttpServer : IDisposable
	{
		string html = String.Empty;
		string baseDirectory = String.Empty;

		public void SetHtml (string html, string baseDirectory)
		{
			this.html = html;
			this.baseDirectory = baseDirectory;
		}

		public Uri Url { get { return new Uri ("http://" + "localhost" + ":" + port + "/"); } }

		readonly HttpListener listener;
		int port = -1;

		public HttpServer ()
		{
			var rnd = new Random ();

			for (int i = 0; i < 100; i++) {
				int currentPort = rnd.Next (49152, 65536);

				try {
					listener = new HttpListener ();
					listener.Prefixes.Add ("http://localhost:" + currentPort + "/");
					listener.Start ();

					this.port = currentPort;
					listener.BeginGetContext (ListenerCallback, null);
					return;
				}
				catch (Exception x) {
					listener.Close ();
					Debug.WriteLine ("HttpListener.Start:\n" + x);
				}
			}

			throw new ApplicationException ("Failed to start HttpListener");
		}

		public void ListenerCallback (IAsyncResult ar)
		{
			listener.BeginGetContext (ListenerCallback, null);

			var context = listener.EndGetContext (ar);
			var request = context.Request;
			var response = context.Response;

			Debug.WriteLine ("SERVER: " + baseDirectory + " " + request.Url);

			response.AddHeader ("Cache-Control", "no-cache");

			try {
				if (request.Url.AbsolutePath == "/") {
					response.ContentType = MediaTypeNames.Text.Html;
					response.ContentEncoding = Encoding.UTF8;

					var buffer = Encoding.UTF8.GetBytes (html);
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write (buffer, 0, buffer.Length);

					return;
				}

				var filepath = Path.Combine (
					baseDirectory,
					request.Url.AbsolutePath.Substring (1)
				);

				Debug.WriteLine ("--FILE: " + filepath);

				if (!File.Exists (filepath)) {
					response.StatusCode = (int)HttpStatusCode.NotFound; // 404
					response.StatusDescription = response.StatusCode + " Not Found";

					response.ContentType = MediaTypeNames.Text.Html;
					response.ContentEncoding = Encoding.UTF8;

					var buffer = Encoding.UTF8.GetBytes ("<html><body>404 Not Found</body></html>");
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write (buffer, 0, buffer.Length);

					return;
				}

				byte[] entity = null;
				try {
					entity = File.ReadAllBytes (filepath);
				}
				catch (Exception x) {
					Debug.WriteLine ("Exception reading file: " + filepath + "\n" + x);

					response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
					response.StatusDescription = response.StatusCode + " Internal Server Error";

					response.ContentType = MediaTypeNames.Text.Html;
					response.ContentEncoding = Encoding.UTF8;

					var buffer = Encoding.UTF8.GetBytes ("<html><body>500 Internal Server Error</body></html>");
					response.ContentLength64 = buffer.Length;
					response.OutputStream.Write (buffer, 0, buffer.Length);

					return;
				}

				response.ContentLength64 = entity.Length;

				switch (Path.GetExtension (request.Url.AbsolutePath).ToLowerInvariant ()) {
				//images
				case ".gif":
					response.ContentType = MediaTypeNames.Image.Gif;
					break;
				case ".jpg":
				case ".jpeg":
					response.ContentType = MediaTypeNames.Image.Jpeg;
					break;
				case ".tiff":
					response.ContentType = MediaTypeNames.Image.Tiff;
					break;
				case ".png":
					response.ContentType = "image/png";
					break;

				// application
				case ".pdf":
					response.ContentType = MediaTypeNames.Application.Pdf;
					break;
				case ".zip":
					response.ContentType = MediaTypeNames.Application.Zip;
					break;

				// text
				case ".htm":
				case ".html":
					response.ContentType = MediaTypeNames.Text.Html;
					break;
				case ".txt":
					response.ContentType = MediaTypeNames.Text.Plain;
					break;
				case ".xml":
					response.ContentType = MediaTypeNames.Text.Xml;
					break;

				// let the user agent work it out
				default:
					response.ContentType = MediaTypeNames.Application.Octet;
					break;
				}

				response.OutputStream.Write (entity, 0, entity.Length);
			}
			catch (Exception x) {
				Debug.WriteLine ("Unexpected exception. Aborting...\n" + x);

				response.Abort ();
			}
		}

		public void Dispose ()
		{
			if (listener != null) {
				listener.Close ();
			}
		}
	}
}
#endif