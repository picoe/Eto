using System.Web;

namespace Eto.Web
{
	public class Application : HttpApplication
	{
		public static string Path
		{
			get
			{
				string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
				if (!appPath.EndsWith("/")) appPath += "/";
				return appPath;
			}
		}
	}
}

