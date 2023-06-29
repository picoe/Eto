namespace Eto.Android.Forms
	{
	internal class EtoEnvironmentHandler : EtoEnvironment.IHandler
		{
		public EtoEnvironmentHandler()
			{
			}

		public string GetFolderPath(EtoSpecialFolder folder)
			{
			return String.Empty;
			}
		}
	}