using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swd = Windows.UI.Xaml.Data;
using swa = Windows.UI.Xaml.Automation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using System.Collections;
using Windows.UI.Xaml.Markup;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<EtoDropDown, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		public string Text { get; set; }

		public bool ReadOnly { get; set; }

		public bool AutoComplete { get; set; }
	}
}
