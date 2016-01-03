using System;
using Eto.Forms;

namespace Eto.Test.UnitTests.Forms
{
	public class TestXamlChild : Panel
	{
	}

	public class TestXamlParent : StackLayout
	{
		public TestXamlChild myControl1;
		public TestXamlChild myControl2;
		public Panel panel1;
		public Panel panel2;

		public TestXamlChild MyControlProperty1 { get; set; }
		public TestXamlChild MyControlProperty2 { get; set; }
	}
}

