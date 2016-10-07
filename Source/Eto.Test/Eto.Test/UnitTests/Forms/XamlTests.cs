using System;
using NUnit.Framework.Internal;
using Eto.Serialization.Xaml;
using System.IO;
using Eto.Forms;
using NUnit.Framework;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class XamlTests
	{
		/// <summary>
		/// Issue #403
		/// </summary>
		[Test]
		public void NullableSizeAndPaddingShouldDeserialize()
		{
			TestBase.Invoke(() =>
			{
				string xaml = string.Format("<DynamicTable Padding='10' Spacing='5, 5' xmlns='{0}' />", XamlReader.EtoFormsNamespace);

				var table = XamlReader.Load<DynamicTable>(new StringReader(xaml), null);

				Assert.IsNotNull(table, "#1");
				Assert.AreEqual(table.Padding, new Padding(10), "#2");
				Assert.AreEqual(table.Spacing, new Size(5, 5), "#3");
			});
		}

		/// <summary>
		/// Tests that you can use ID or x:Name for your custom subclasses, and have them automatically register in a 
		/// field or property.
		/// </summary>
		[Test]
		public void IdOrNameShouldWorkWithSubclass()
		{
			TestBase.Invoke(() =>
			{
				string xaml = string.Format(
					@"<StackLayout xmlns='{0}' xmlns:x='{1}' xmlns:c='clr-namespace:Eto.Test.UnitTests.Forms;assembly=Eto.Test'>
					<c:TestXamlChild ID='myControl1'/>
					<c:TestXamlChild x:Name='myControl2'/>
					<c:TestXamlChild ID='MyControlProperty1'/>
					<c:TestXamlChild x:Name='MyControlProperty2'/>
					<Panel ID='panel1'/>
					<Panel x:Name='panel2'/>
				</StackLayout>", XamlReader.EtoFormsNamespace, Portable.Xaml.XamlLanguage.Xaml2006Namespace);


				var parent = XamlReader.Load(new StringReader(xaml), new TestXamlParent());

				// sanity check with parent and eto controls
				Assert.IsNotNull(parent, "#1");
				Assert.IsNotNull(parent.panel1, "#6 - field should have been set from ID");
				Assert.IsNotNull(parent.panel2, "#7 - field should have been set from x:Name");

				// test subclass is also set
				Assert.IsNotNull(parent.myControl1, "#2 - field should have been set from ID");
				Assert.IsNotNull(parent.myControl2, "#3 - field should have been set from x:Name");
				Assert.IsNotNull(parent.MyControlProperty1, "#4 - property should have been set from ID");
				Assert.IsNotNull(parent.MyControlProperty2, "#5 - property should have been set from x:Name");
			});
		}

		[Test]
		public void LocalAssemblyShouldBeDefault()
		{
			TestBase.Invoke(() =>
			{
				string xaml = string.Format(
				@"<StackLayout xmlns='{0}' xmlns:x='{1}' xmlns:c='clr-namespace:Eto.Test.UnitTests.Forms'>
					<c:TestXamlChild ID='myControl1'/>
					<c:TestXamlChild x:Name='myControl2'/>
					<c:TestXamlChild ID='MyControlProperty1'/>
					<c:TestXamlChild x:Name='MyControlProperty2'/>
					<Panel ID='panel1'/>
					<Panel x:Name='panel2'/>
				</StackLayout>", XamlReader.EtoFormsNamespace, Portable.Xaml.XamlLanguage.Xaml2006Namespace);


				var parent = XamlReader.Load(new StringReader(xaml), new TestXamlParent());
				// sanity check with parent and eto controls
				Assert.IsNotNull(parent, "#1");
				Assert.IsNotNull(parent.panel1, "#6 - field should have been set from ID");
				Assert.IsNotNull(parent.panel2, "#7 - field should have been set from x:Name");

				// test subclass is also set
				Assert.IsNotNull(parent.myControl1, "#2 - field should have been set from ID");
				Assert.IsNotNull(parent.myControl2, "#3 - field should have been set from x:Name");
				Assert.IsNotNull(parent.MyControlProperty1, "#4 - property should have been set from ID");
				Assert.IsNotNull(parent.MyControlProperty2, "#5 - property should have been set from x:Name");
			});
		}
	}
}

