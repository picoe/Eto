using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="Form"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FormHandler : AndroidWindow<Form, Form.ICallback>, Form.IHandler
	{
		public void Show()
		{
			// TODO: create activity if it doesn't exist
			Activity.SetContentView(ContainerControl);
		}

		public bool ShowActivated { get; set; }

		public bool CanFocus { get; set; }
	}
}

