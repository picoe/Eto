using System;
using Eto;
using System.Reflection;
using System.IO;

namespace Eto.WinRT
{
	/// <summary>
	/// Environment handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
	{
#if TODO_XAML		
		static Environment.SpecialFolder Convert (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationSettings:
				return Environment.SpecialFolder.ApplicationData;
			case EtoSpecialFolder.Documents:
				return Environment.SpecialFolder.MyDocuments;
			default:
				throw new NotSupportedException ();
			}
		}
#endif

		public string GetFolderPath (EtoSpecialFolder folder)
		{
#if TODO_XAML
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return Path.GetDirectoryName(Assembly.GetEntryAssembly ().Location);
			default:
				return Environment.GetFolderPath (Convert (folder));
			}
#else
					throw new NotImplementedException();
#endif
		}
	}
}

