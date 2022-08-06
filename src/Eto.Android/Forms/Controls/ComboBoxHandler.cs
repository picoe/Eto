using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using a = Android;

namespace Eto.Android.Forms.Controls
{
	// TODO: https://stackoverflow.com/a/28441563/55559
	public class ComboBoxHandler : DropDownHandler<aw.Spinner, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public ComboBoxHandler()
		{
		}

		public string Text
		{
			get
			{
				return adapterSource[Control.SelectedItemPosition].ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public bool ReadOnly
		{
			get
			{
				return false;
			}
			set { }
		}

		public bool AutoComplete
		{
			get
			{
				return false;
			}
			set { }
		}
	}
}
