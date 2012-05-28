using System;
using Eto;
using System.Reflection;
using System.IO;

namespace Eto.Platform.GtkSharp
{
	public class EtoEnvironmentHandler : IEtoEnvironment, IWidget
	{
		
		Environment.SpecialFolder Convert (EtoSpecialFolder folder)
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

		public string GetFolderPath (EtoSpecialFolder folder)
		{
			switch (folder) {
			case EtoSpecialFolder.ApplicationResources:
				return Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
			default:
				return Environment.GetFolderPath (Convert (folder));
			}
		}

		#region IWidget implementation
		
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }
		
		#endregion
	}
}

