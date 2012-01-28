using System;
using Eto;

namespace Eto.Platform.Wpf
{
	public class EtoEnvironmentHandler : IEtoEnvironment
	{
		
		Environment.SpecialFolder Convert(EtoSpecialFolder folder)
		{
			switch (folder)
			{
			case EtoSpecialFolder.ApplicationSettings: return Environment.SpecialFolder.ApplicationData;
			case EtoSpecialFolder.Documents: return Environment.SpecialFolder.MyDocuments;
			default: throw new NotSupportedException();
			}

		}

		public string GetFolderPath (EtoSpecialFolder folder)
		{
			return Environment.GetFolderPath(Convert(folder));
		}
		
		public void Initialize ()
		{
		}

		public IWidget Handler { get; set; }
	}
}

