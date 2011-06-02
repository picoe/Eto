using System;
using System.Collections.Generic;

namespace Eto.Platform
{
	/// <summary>
	/// Summary description for PlatformHandlerCollection.
	/// </summary>
	public class PlatformHandlerCollection : List<IPlatformHandler>
	{
		public PlatformHandlerCollection()
		{
		}


		public IPlatformHandler Detect()
		{
			foreach (IPlatformHandler handler in this)
			{
				if (handler.Detect()) return handler;
			}
			return null;
		}
	}
}
