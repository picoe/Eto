using aw = Android.Widget;
using at = Android.Text;
using av = Android.Views;
using au = Android.Util;
using Eto.Android.Drawing;

namespace Eto.Android.Forms
{
	public abstract class AndroidTextControl<TControl, TWidget, TCallback> : AndroidControl<TControl, TWidget, TCallback>, TextControl.IHandler, IAndroidControl
		where TControl : aw.TextView
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		private bool readOnly;
		private Font font;

		public override av.View ContainerControl => Control;

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public Color TextColor
		{
			//return Color.FromArgb(Control.CurrentTextColor); // This is only the active colour depending on state, not necessarily the desired colour?
			get { return Control.TextColors.ToEto(); }
			set { Control.SetTextColor(value.ToAndroid()); }
		}

		public bool ReadOnly
		{
			get { return readOnly; }
			set
			{
				readOnly = value;

				if (HasControl)
					SetInputType();
			}
		}

		protected void SetInputType(aw.TextView control = null)
		{
			at.InputTypes Types;

			if (readOnly)
				Types = at.InputTypes.Null;

			else
				Types = at.InputTypes.ClassText | at.InputTypes.TextVariationNormal;

			(control ?? Control).InputType = Types;
		}

		public Font Font
		{
			get
			{
				if (font == null)
				{
					if (Control?.Paint != null)
						font = new Font(new FontHandler(Control.Paint));

					else if(Control?.Typeface != null)
						font = Control.Typeface.ToEto(Control.TextSize);

					else
						return SystemFonts.Default();
				}
				return font;
			}
			set
			{
				if (Control is aw.TextView textControl && font != value)
				{
					font = value;
					textControl.Typeface = font.ToAndroid();
					textControl.SetTextSize(au.ComplexUnitType.Dip, Platform.PtToDp(font.Size));
				}
			}
		}
	}
}
