using System;

namespace Eto
{
	public enum EtoSpecialFolder
	{
		ApplicationSettings,
		ApplicationResources,
		Documents
	}
	
	public interface IEtoEnvironment : IWidget
	{
		string GetFolderPath(EtoSpecialFolder folder);
	}
	
	public static class EtoEnvironment
	{
		public static string GetFolderPath(EtoSpecialFolder folder)
		{
			return GetFolderPath(Generator.Current, folder);
		}
		
		public static string GetFolderPath(Generator g, EtoSpecialFolder folder)
		{
			var handler = g.CreateControl<IEtoEnvironment>();
			return handler.GetFolderPath(folder);
		}
	}
}

