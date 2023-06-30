using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
namespace Eto.Android.Forms.Controls
{
	public class CheckBoxHandler : AndroidCommonControl<aw.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public CheckBoxHandler()
		{
			Control = new aw.CheckBox(Platform.AppContextThemed);
			Control.CheckedChange += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}

		public bool? Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value ?? false; }
		}

		// TODO:
		public bool ThreeState
		{
			get;
			set;
		}
	}
}