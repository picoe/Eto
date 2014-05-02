using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ComboBoxHandler : IosControl<UILabel, ComboBox>, IComboBox
	{
		
		public ComboBoxHandler()
		{
			Control = new UILabel();
		}

		public int SelectedIndex
		{
			get	{ return 0; }
			set {  }
		}

		public IListStore DataStore {
			get; set;
		}
	}
}
