using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class NumericUpDownHandler : MacView<NSTextField, NumericUpDown>, INumericUpDown
	{
		
		public NumericUpDownHandler()
		{
			Control = new NSTextField();
		}
		
		#region INumericUpDown Members
		
		public bool ReadOnly
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		
		public decimal Value
		{
			get { return Convert.ToDecimal(Control.StringValue); }
			set { Control.StringValue = value.ToString(); }
		}
		
		#endregion
	}
}
