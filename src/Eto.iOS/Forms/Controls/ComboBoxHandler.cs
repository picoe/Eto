using SD = System.Drawing;
using UIKit;
using Foundation;
namespace Eto.iOS.Forms.Controls
{
	public class ComboBoxHandler : DropDownHandler<ComboBox, ComboBox.ICallback, UIPickerView>, ComboBox.IHandler
	{
		public string Text
		{
			get;
			set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public bool AutoComplete
		{
			get;
			set;
		}
	}
}
