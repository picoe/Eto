using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Runtime.InteropServices;

namespace Eto.Platform.Windows
{
	public class SearchBoxHandler : WindowsControl<TextBoxHandler.WatermarkTextBox, SearchBox>, ISearchBox
	{
		public SearchBoxHandler()
		{
			Control = new TextBoxHandler.WatermarkTextBox();
		}
	}
}
