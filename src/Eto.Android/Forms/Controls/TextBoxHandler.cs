using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using at = Android.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Android.Forms.Controls
{
	public class TextBoxHandler : AndroidControl<aw.EditText, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public TextBoxHandler()
		{
			Control = new aw.EditText(aa.Application.Context);
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		// TODO
		public bool ReadOnly
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

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

		int maxLength = int.MaxValue;
		public int MaxLength
		{
			get { return maxLength; }
			set
			{
				maxLength = value;
				Control.SetFilters(new [] { new at.InputFilterLengthFilter(maxLength) });
			}
		}
		public string PlaceholderText
		{
			get { return Control.Hint; }
			set { Control.Hint = value; }
		}
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

		public Color TextColor
		{
			get { return Control.TextColors.ToEto(); }
			set { Control.SetTextColor(value.ToAndroid()); }
		}

		public int CaretIndex
		{
			get { return Control.SelectionStart; }
			set
			{
				Control.SetSelection(value);
			}
		}

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionEnd - 1); }
			set { Control.SetSelection(value.Start, value.End); }
		}

		public TextAlignment TextAlignment
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