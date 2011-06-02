using System;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp.GdkHandler.x11
{
	/// <summary>
	/// Summary description for Platform.
	/// </summary>
	public class Global : IGlobal
	{
		public Global()
		{
		}

		[DllImport("libgdk-x11-2.0.so.0")]
		static extern void gdk_flush();


		#region IGdk Members

		public void Flush()
		{
			gdk_flush();
		}

		#endregion

		#region IPlatformHandler Members

		public bool Detect()
		{
			try
			{
				gdk_flush();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}
