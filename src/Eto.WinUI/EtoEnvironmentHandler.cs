using Eto;
using Windows.Storage;
namespace Eto.WinUI;

public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
{

	public string GetFolderPath(EtoSpecialFolder folder)
	{
		switch (folder)
		{
			case EtoSpecialFolder.ApplicationResources:
			case EtoSpecialFolder.EntryExecutable:
				return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			case EtoSpecialFolder.Downloads:
				return UserDataPaths.GetDefault().Downloads;
			case EtoSpecialFolder.ApplicationSettings:
				return UserDataPaths.GetDefault().RoamingAppData;
			case EtoSpecialFolder.Documents:
				return UserDataPaths.GetDefault().Documents;
			default:
				throw new NotSupportedException();
		}
	}
}

