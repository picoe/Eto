using System;
using System.Collections.Generic;
using System.Linq;
using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
	public class ComboBoxHandler : DropDownHandler, ComboBox.IHandler
	{
		bool editable;

		public void Create(bool isEditable)
		{
			Create();
			IsEditable = editable = isEditable;
		}

		public override string Text
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
				Control.DropDownStyle = editable ? swf.ComboBoxStyle.DropDown : swf.ComboBoxStyle.DropDownList;
			}
		}
	}
}
