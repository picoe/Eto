using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android.Forms.Controls
{
	public class LabelHandler : AndroidCommonControl<aw.TextView, Label>, ILabel
	{
		public LabelHandler()
		{
			Control = new aw.TextView(a.App.Application.Context);
		}

		public HorizontalAlign HorizontalAlign { get; set; }

		public VerticalAlign VerticalAlign { get; set; }

		public WrapMode Wrap
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
			get { return Color.FromArgb((uint)Control.CurrentTextColor); }
			set { Control.SetTextColor(value.ToAndroid()); }
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}

