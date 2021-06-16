using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
    public class DropDownTests : TestBase
    {
		[Test, InvokeOnUI]
		public void DropDownInElementHostShouldHaveCorrectInitialValue()
		{
			var dropDown = new DropDown();
			dropDown.DataStore = new [] { "Item 1", "Item 2", "Item 3" };
			
			var content = TableLayout.AutoSized(dropDown);
			
			// forces template to be applied to WPF controls
			var native = content.ToNative(false);
			native.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

			// now set selected index now that template has applied
			dropDown.SelectedIndex = 1;
			
			// check that the index is correct when it is shown
			int? dropDownIndex = null;
			
			var dlg = new Dialog();
			dlg.ClientSize = new Drawing.Size(200, 200);
			dlg.Content = content;
			dlg.Shown += (sender, e) => {
				dropDownIndex = dropDown.SelectedIndex;	
				dlg.Close();
			};
			dlg.ShowModal(Application.Instance.MainForm);
			
			Assert.AreEqual(1, dropDownIndex);
		}
        
    }
}