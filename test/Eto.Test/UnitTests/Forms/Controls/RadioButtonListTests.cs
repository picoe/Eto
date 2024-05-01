using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
    public class RadioButtonListTests : TestBase
    {
		[Test]
		public void ShouldBeAbleToUnselectAll()
		{
			Shown(form => {
				var rbl = new RadioButtonList 
				{
					Items = { "Item 1", "Item 2", "Item 3" },
					SelectedIndex = 1
				};
				Assert.AreEqual(1, rbl.SelectedIndex, "#1.1");
				
				return rbl;
			}, rbl => {
				Assert.AreEqual(1, rbl.SelectedIndex, "#2.1");
				
				
				rbl.SelectedIndex = -1;
				Assert.AreEqual(-1, rbl.SelectedIndex, "#3.1");
				
				rbl.SelectedKey = "Item 3";
				Assert.AreEqual(2, rbl.SelectedIndex, "#3.1");

				rbl.SelectedKey = null;
				Assert.AreEqual(-1, rbl.SelectedIndex, "#3.1");
			});
		}
        
    }
}