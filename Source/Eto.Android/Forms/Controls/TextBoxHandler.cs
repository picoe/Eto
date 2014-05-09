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
	/// <summary>
	/// Handler for <see cref="TextBox"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class TextBoxHandler : AndroidControl<aw.TextView, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		public bool ReadOnly
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

		public int MaxLength
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

		public void SelectAll()
		{
			throw new NotImplementedException();
		}

		public string PlaceholderText { get; set; }

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public Eto.Drawing.Font Font
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

		public override av.View ContainerControl
		{
			get { throw new NotImplementedException(); }
		}
	}
}