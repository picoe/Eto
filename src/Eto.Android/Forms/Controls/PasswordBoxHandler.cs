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
using at = Android.Text;
using au = Android.Util;

namespace Eto.Android.Forms.Controls
{
	public class PasswordBoxHandler : AndroidTextControl<aw.EditText, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
	{
		int maxLength = int.MaxValue;

		public override av.View ContainerControl { get { return Control; } }

		public PasswordBoxHandler()
		{
			Control = new aw.EditText(Platform.AppContextThemed);
			Control.InputType = at.InputTypes.ClassText | at.InputTypes.TextVariationPassword;
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		public Char PasswordChar { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.TextChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public int MaxLength
		{
			get { return maxLength; }
			set
			{
				maxLength = value;
				Control.SetFilters(new[] { new at.InputFilterLengthFilter(maxLength) });
			}
		}
	}
}