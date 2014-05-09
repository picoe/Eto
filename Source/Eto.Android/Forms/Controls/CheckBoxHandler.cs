using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Forms.Controls
{
	public class CheckBoxHandler : AndroidCommonControl<aw.CheckBox, CheckBox, CheckBox.ICallback>, ICheckBox
	{
		public CheckBoxHandler()
		{
			Control = new aw.CheckBox(aa.Application.Context);
		}

		public bool? Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value ?? false; }
		}

		public bool ThreeState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}