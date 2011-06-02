using System.Web;


namespace Eto.Web
{
	public class Configuration
	{
		VirtualPageCollection virtualPages;
		
		public Configuration()
		{
			virtualPages = new VirtualPageCollection();
		}
		
		public VirtualPageCollection VirtualPages
		{
			get { return virtualPages; }
		}
		
		public static Configuration Get(HttpContext context)
		{
			Configuration config = context.Application["configuration"] as Configuration;
			if (config == null)
			{
				config = new Configuration();
				context.Application["configuration"] = config;
			}
			return config;
		}
	}
}

