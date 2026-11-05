using System.Net.Mime;

namespace Eto
{
	/// <summary>
	/// Serves html given to WebView.LoadHtml, and the files beside it, through a resource handler.
	/// </summary>
	static class WebViewHtmlServer
	{
		// The .example domain is reserved and can never be a real site.
		public const string Host = "eto-webview.example";

		/// <summary>
		/// Answers a request for the page at / or for a file under the base directory.
		/// </summary>
		public static WebViewResourceResponse Serve(string html, string baseDirectory, WebViewResourceRequest request)
		{
			var path = Uri.UnescapeDataString(request.Uri.AbsolutePath);
			WebViewResourceResponse response;
			if (path == "/")
			{
				response = new WebViewResourceResponse(html, "text/html");
			}
			else
			{
				var file = GetBaseDirectoryFile(baseDirectory, path);
				if (file == null)
					return null;
				try
				{
					response = new WebViewResourceResponse(new MemoryStream(File.ReadAllBytes(file)), GetContentType(file));
				}
				catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
				{
					return null;
				}
			}
			response.Headers["Cache-Control"] = "no-cache";
			return response;
		}

		/// <summary>
		/// Gets the file under the base directory for a requested path, or null when there is none.
		/// </summary>
		static string GetBaseDirectoryFile(string baseDirectory, string path)
		{
			if (string.IsNullOrEmpty(baseDirectory))
				return null;
			try
			{
				var root = Path.GetFullPath(baseDirectory);
				if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()))
					root += Path.DirectorySeparatorChar;
				var file = Path.GetFullPath(Path.Combine(root, path.TrimStart('/')));
				// Never serve anything outside the base directory.
				if (!file.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(file))
					return null;
				return file;
			}
			catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is IOException)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the MIME type to serve a file with, based on its extension.
		/// </summary>
		public static string GetContentType(string path)
		{
			switch (Path.GetExtension(path).ToLowerInvariant())
			{
				//images
				case ".gif":
					return MediaTypeNames.Image.Gif;
				case ".jpg":
				case ".jpeg":
					return MediaTypeNames.Image.Jpeg;
				case ".tiff":
					return MediaTypeNames.Image.Tiff;
				case ".png":
					return "image/png";
				case ".svg":
					return "image/svg+xml";

				// application
				case ".pdf":
					return MediaTypeNames.Application.Pdf;
				case ".zip":
					return MediaTypeNames.Application.Zip;
				case ".json":
					return "application/json";

				// text
				case ".htm":
				case ".html":
					return MediaTypeNames.Text.Html;
				case ".css":
					return "text/css";
				case ".js":
				case ".mjs":
					return "text/javascript";
				case ".txt":
					return MediaTypeNames.Text.Plain;
				case ".xml":
					return MediaTypeNames.Text.Xml;

				// let the user agent work it out
				default:
					return MediaTypeNames.Application.Octet;
			}
		}
	}
}
