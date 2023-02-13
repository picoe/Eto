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
using System.Globalization;
using Android.Widget;
using Eto.Drawing;
using Android.Views;

namespace Eto.Android.Forms.Controls
{
	public class NumericUpDownHandler : AndroidControl<aw.NumberPicker, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new NumberPicker(Platform.AppContextThemed);
		}

		public override View ContainerControl => Control;

		public double Value
		{
			get => Control.Value;
			set => Control.Value = (int)value;
		}

		public double MinValue
		{
			get => Control.MinValue;
			set => Control.MinValue = (int)value;
		}

		public double MaxValue
		{
			get => Control.MaxValue;
			set => Control.MaxValue = (int)value;
		}

		public int DecimalPlaces
		{
			get => 0;
			set { }
		}

		public double Increment
		{
			get => 1;
			set { }
		}

		public int MaximumDecimalPlaces
		{
			get => 0;
			set { }
		}

		public string FormatString { get; set; }

		public CultureInfo CultureInfo { get; set; }

		public bool Wrap { get; set; }
		
		public bool ReadOnly { get; set; }

		public Color TextColor { get; set; }
		
		public Font Font { get; set; }
	}
}