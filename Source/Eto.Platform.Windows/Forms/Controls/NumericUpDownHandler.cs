using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class NumericUpDownHandler : WindowsControl<SWF.NumericUpDown, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler()
		{
			Control = new SWF.NumericUpDown();
			Control.Width = 40;
		}
		
		#region INumericUpDown Members
		
		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public decimal Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}
		
		#endregion
	}
}
