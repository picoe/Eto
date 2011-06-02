using System;
using Eto.Platform;

namespace Eto.Platform.GtkSharp.GdkHandler
{
	/// <summary>
	/// Summary description for GdkHandler.
	/// </summary>
	public interface IGlobal : IPlatformHandler
	{
		void Flush();
	}


	public class Global
	{
		//static IGlobal global;

		public static void Flush()
		{
			/*if (global == null)
			{
				PlatformHandlerCollection coll = new PlatformHandlerCollection();
				coll.Add(new Win32.Global());
				coll.Add(new x11.Global());
				global = (IGlobal)coll.Detect();
			}
			
			if (global != null) global.Flush();*/
		}



	}
}
