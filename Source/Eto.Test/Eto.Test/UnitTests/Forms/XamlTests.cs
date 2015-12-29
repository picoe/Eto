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
			string xaml = string.Format("<DynamicTable Padding='10' Spacing='5, 5' xmlns='{0}' />", XamlReader.EtoFormsNamespace);

			var table = XamlReader.Load<DynamicTable>(new StringReader(xaml), null);

			Assert.IsNotNull(table, "#1");
			Assert.AreEqual(table.Padding, new Padding(10), "#2");
			Assert.AreEqual(table.Spacing, new Size(5, 5), "#3");
		}
	}
}

