using System;
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
	class TextAreaHandler : AndroidTextControl<aw.EditText, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public TextAreaHandler()
		{
			Control = new aw.EditText(Platform.AppContextThemed);
			Control.SetSingleLine(false);
			Control.InputType = at.InputTypes.ClassText | at.InputTypes.TextFlagMultiLine;
			Control.Gravity = av.GravityFlags.Top | av.GravityFlags.Start;
		}

		public Boolean Wrap
		{
			get;
			set;
		}

		public void Append(String text, Boolean scrollToCursor)
		{
			Control.Append(text);
			if (scrollToCursor)
				Control.ScrollTo(0, Control.Bottom);
		}

		public String SelectedText
		{
			get
			{
				if (!Control.HasSelection)
					return String.Empty;
				return Control.Text.Substring(Control.SelectionStart, Control.SelectionEnd - Control.SelectionStart);
			}

			set
			{
				if (!Control.HasSelection)
					return;
				Control.EditableText.Replace(Control.SelectionStart, Control.SelectionEnd, new Java.Lang.String(value));
			}
		}

		public Range<Int32> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionEnd - 1); }
			set { Control.SetSelection(value.Start, value.End); }
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		public Int32 CaretIndex
		{
			get
			{
				return Control.SelectionStart;
			}

			set
			{
				Control.SetSelection(value);
			}
		}

		public Boolean AcceptsTab
		{
			get;
			set;
		}

		public Boolean AcceptsReturn
		{
			get;
			set;
		}

		public TextReplacements TextReplacements
		{
			get;
			set;
		}

		public TextReplacements SupportedTextReplacements
		{
			get
			{
				return TextReplacements.None;
			}
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
		
		public override Size Size
			{
			get => base.Size;
			set
			{
				base.Size = value;

				var pxw = value.Width >= 0 ? Platform.DpToPx(value.Width) : av.ViewGroup.LayoutParams.WrapContent;
				var pxh = value.Height >= 0 ? Platform.DpToPx(value.Height) : av.ViewGroup.LayoutParams.WrapContent;

				if (pxw >= 0)
					Control.SetMaxWidth(pxw);

				if (pxh >= 0)
					Control.SetMaxHeight(pxh);

				Control.LayoutParameters = AndroidHelpers.CreateOrAdjustLayoutParameters(Control.LayoutParameters, pxw, pxh);
				Control.SetMaxLines(9999);
			}
		}

		public Boolean SpellCheck
		{
			get;
			set;
		}

		public Boolean SpellCheckIsSupported
		{
			get
			{
				return false;
			}
		}
	}
}
