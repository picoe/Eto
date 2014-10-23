using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Wpf.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler, ComboBox.IHandler
	{
		bool editable;

		public void Create(bool isEditable)
		{
			Create();
			Control.IsEditable = editable = isEditable;
		}

		public string Text
		{
			get
			{
				return editable ? Control.Text : "";
			}
			set
			{
				if (editable && value != null)
				{
					Control.Text = value;
				}
			}
		}

		public bool IsEditable
		{
			get { return editable; }
			set
			{
				editable = value;
				Control.IsEditable = editable;
			}
		}
	}
}
