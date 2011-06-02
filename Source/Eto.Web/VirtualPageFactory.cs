using System;
using System.Web;
using System.Web.UI;

namespace Eto.Web
{
	public class VirtualPageFactory : IHttpHandlerFactory
	{
		
		public IHttpHandler GetHandler(HttpContext context, String requestType, String url, String pathTranslated)
		{
			Configuration config = Configuration.Get(context);

			string pageUrl = url;
			if (pageUrl.StartsWith(context.Request.ApplicationPath)) pageUrl = url.Substring(context.Request.ApplicationPath.Length);
			if (pageUrl.StartsWith("/")) pageUrl = pageUrl.TrimStart('/');
			VirtualPage virtualPage = config.VirtualPages.FindMap(pageUrl);
			
			IHttpHandler handler = null;
			if (virtualPage != null) handler = virtualPage.CreateInstance(this, context, pageUrl);
			
			if (handler == null)
			{
				int extPos = url.LastIndexOf('.');
				string extension = (extPos > 0) ? url.Substring(extPos).ToLower() : string.Empty;
				if (IsForbidden(extension)) throw new HttpException(404, "File not found");

				if (Eto.Misc.Platform.IsMono)
				{
					handler = PageParser.GetCompiledPageInstance(url, pathTranslated, context);
					context.Response.ContentType = GetMimeType(url.Substring(extPos));
				}
				else
				{
					if (extension == ".aspx")
						handler = PageParser.GetCompiledPageInstance(url, pathTranslated, context);
					else
					{
						handler = new DefaultHttpHandler();
						context.Response.ContentType = GetMimeType(url.Substring(extPos));
					}
				}
			}
			
			
			return handler;
		}

		public bool IsForbidden(string type)
		{
			switch (type)
			{
				case ".asax":
				case ".ascx":
				case ".master":
				case ".skin":
				case ".browser":
				case ".sitemap":
				case ".config":
				case ".cs":
				case ".csproj":
				case ".vb":
				case ".vbproj":
				case ".webinfo":
				case ".licx":
				case ".resx":
				case ".resources":
				case ".mdb":
				case ".vjproj":
				case ".java":
				case ".jsl":
					return true;
			}
			return false;
		}
		
		public string GetMimeType(string type)
		{
			switch (type)
			{
				case ".css": return "text/css";
				case ".jpeg":
				case ".jpg": return "image/jpeg";
				case ".png": return "image/png";
				case ".gif": return "image/gif";
				case ".html":
				default: return "text/html";
			}
		}
		
		public void ReleaseHandler(IHttpHandler handler)
		{
		}
	}
}

