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
	public class ComboBoxHandler : AndroidControl<aw.Spinner, ComboBox, ComboBox.ICallback>, ComboBox.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public ComboBoxHandler()
		{
			Control = new aw.Spinner(aa.Application.Context);
		}

		public IEnumerable<object> DataStore
		{
			get;
			set;
		}

		public int SelectedIndex { get; set; }

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

		public Eto.Drawing.Color TextColor
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
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

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

		public bool AutoComplete
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
	}
}