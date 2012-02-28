using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ProgressBarHandler : GtkControl<Gtk.ProgressBar, ProgressBar>, IProgressBar
	{
		public ProgressBarHandler()
		{
			this.Control = new Gtk.ProgressBar ();
			MinValue = 0;
			MaxValue = 100;
			this.Control.Fraction = 0;
		}

		
		public int MaxValue { get; set; }

		public int MinValue { get;set; }

		public int Value {
			get { 
				return (int)((Control.Fraction * MaxValue) + MinValue);
			}
			set { 
				Control.Fraction = ((double)value - MinValue) / (double)MaxValue;
			}
		}
	}
}

