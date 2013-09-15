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

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public int MaxLength
		{
			get { return this.Control.MaxLength; }
			set { this.Control.MaxLength = value; }
		}

		public string PlaceholderText
		{
			get { return Control.WatermarkText; }
			set { Control.WatermarkText = value; }
		}

		public void SelectAll()
		{
			this.Control.Focus();
			this.Control.SelectAll();
		}
	}
}
