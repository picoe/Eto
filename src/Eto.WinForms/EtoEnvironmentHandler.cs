using System;
using Eto;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Eto.WinForms
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
	{

		static Environment.SpecialFolder Convert(EtoSpecialFolder folder)
		{
			switch (folder)
			{
				case EtoSpecialFolder.ApplicationSettings:
					return Environment.SpecialFolder.ApplicationData;
				case EtoSpecialFolder.Documents:
					return Environment.SpecialFolder.MyDocuments;
				default:
					throw new NotSupportedException();
			}

		}

		public string GetFolderPath(EtoSpecialFolder folder)
		{
			switch (folder)
			{
				case EtoSpecialFolder.ApplicationResources:
				case EtoSpecialFolder.EntryExecutable:
					return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				case EtoSpecialFolder.Downloads:
					return Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads.Path;
				default:
					return Environment.GetFolderPath(Convert(folder));
			}
		}
	}
}

