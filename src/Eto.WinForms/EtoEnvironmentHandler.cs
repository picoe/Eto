using System;
using Eto;
using System.Reflection;
using System.IO;

namespace Eto.WinForms
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
	{
		
		static System.Environment.SpecialFolder Convert (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationSettings:
				return System.Environment.SpecialFolder.ApplicationData;
			case EtoSpecialFolder.Documents:
				return System.Environment.SpecialFolder.MyDocuments;
			default:
				throw new NotSupportedException ();
			}

		}

		public string GetFolderPath (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
			default:
				return System.Environment.GetFolderPath(Convert(folder));
			}
		}
	}
}

