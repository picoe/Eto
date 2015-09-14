using System;

namespace Eto.Addin.XamarinStudio
{
	public static class EtoInitializer
	{
		static readonly string AddinPlatform = Eto.Platforms.Gtk2;

		public static void Initialize()
		{
			if (Platform.Instance == null)
			{
				new Eto.Forms.Application(AddinPlatform).Attach();
			}
		}
	}
}

