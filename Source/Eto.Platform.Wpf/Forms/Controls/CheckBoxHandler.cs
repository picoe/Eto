using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class CheckBoxHandler : WpfControl<swc.CheckBox, CheckBox>, ICheckBox
	{
		public CheckBoxHandler ()
		{
			Control = new swc.CheckBox {
				IsThreeState = false
			};

			Control.Checked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			Control.Unchecked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
		}

		public override bool UseMousePreview { get { return true; } }

		public bool? Checked
		{
			get { return Control.IsChecked; }
			set { Control.IsChecked = value; }
		}

		public string Text
		{
			get { return (Control.Content as string).ToEtoMneumonic(); }
			set { Control.Content = value.ToWpfMneumonic(); }
		}

		public bool ThreeState
		{
			get { return Control.IsThreeState; }
			set { Control.IsThreeState = value; }
		}
	}
}
