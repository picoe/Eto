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
	public class NumericStepperHandler : AndroidControl<aw.EditText, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		public NumericStepperHandler()
		{
			Control = new EditText(Platform.AppContextThemed);
		}

		public override View ContainerControl => Control;

		public double Value
		{
			get => double.TryParse(Control.Text, out var v) ? v : -1;
			set => UpdateText(value);
		}

		private void UpdateText(double value)
		{
			value = Math.Max(Math.Min(value, MaxValue), MinValue);
			Control.Text = value.ToString("#0." + new string('0', DecimalPlaces));
		}

		public double MinValue
		{
			get;
			set;
		}

		public double MaxValue
		{
			get;
			set;
		}

		public int DecimalPlaces
		{
			get;
			set;
		}

		public double Increment
		{
			get;
			set;
		}

		public int MaximumDecimalPlaces
		{
			get;
			set;
		}

		public string FormatString { get; set; }

		public CultureInfo CultureInfo { get; set; }

		public bool Wrap { get; set; }
		
		public bool ReadOnly { get; set; }

		public Color TextColor { get; set; }
		
		public Font Font { get; set; }
	}
}