using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Designer
{

	public class PlatformColor
	{
		public string Name { get; set; }
		public Color Color { get; set; }
	}

	public interface IPlatformTheme
	{
		Color ProjectBackground { get; }
		Color ProjectForeground { get; }
		Color ProjectDialogBackground { get; }
		Color ErrorForeground { get; }
		Color SummaryBackground { get; }
		Color SummaryForeground { get; }
		Color DesignerBackground { get; }
		Color DesignerPanel { get; }

		IEnumerable<PlatformColor> AllColors { get; }
	}

	public static class Global
	{
		public static IPlatformTheme Theme => Platform.Instance.CreateShared<IPlatformTheme>();

	}
}
