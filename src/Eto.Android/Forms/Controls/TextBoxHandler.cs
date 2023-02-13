using System;
using Eto.Forms;

using av = Android.Views;
using aw = Android.Widget;
using at = Android.Text;

namespace Eto.Android.Forms.Controls
{
	public class TextBoxHandler : AndroidTextControl<aw.EditText, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public TextBoxHandler()
		{
		}

		protected override aw.EditText CreateControl()
		{
			var C = new aw.EditText(Platform.AppContextThemed);
			SetInputType(C);
			return C;
		}

		public void SelectAll()
		{
			Control.SelectAll();
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
				return Control.TextAlignment.ToEto();
			}

			set
			{
				Control.TextAlignment = value.ToAndroid();
			}
		}

		public AutoSelectMode AutoSelectMode
		{
			get
			{
				return AutoSelectMode.Never;
			}

			set { }
		}
	}
}
