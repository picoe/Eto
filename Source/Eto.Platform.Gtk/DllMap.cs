using System.Collections.Generic;

namespace Eto.Platform.GtkSharp
{
	public class DllMap : Dictionary<string, string>
	{
		public DllMap ()
		{
			if (EtoEnvironment.Platform.IsUnix) {
				this.Add ("libgio-2.0-0.dll", "libgio-2.0.so.0");
				this.Add ("libglib-2.0-0.dll", "libglib-2.0.so.0");
				this.Add ("libgobject-2.0-0.dll", "libgobject-2.0.so.0");
				this.Add ("libgdk-win32-3.0-0.dll", "libgdk-3.so.0");
				this.Add ("libgdk_pixbuf-2.0-0.dll", "libgdk_pixbuf-2.0.so.0");
				this.Add ("libatk-1.0-0.dll", "libatk-1.0.so.0");
				this.Add ("libgdk-win32-3.0-0.dll", "libgdk-3.so.0");
				this.Add ("libgtk-win32-3.0-0.dll", "libgtk-3.so.0");
				this.Add ("libpango-1.0-0.dll", "libpango-1.0.so.0");
				this.Add ("libpangocairo-1.0-0.dll", "libpangocairo-1.0.so.0");
			}
		}
	}
}

