using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using UIKit;
using Eto.Drawing;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<ComboBox, ComboBox.ICallback, UIPickerView>, ComboBox.IHandler
	{
		public string Text
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public bool AutoComplete
		{
			get;
			set;
		}
	}
}
