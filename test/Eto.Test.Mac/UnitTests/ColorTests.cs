using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Runtime.ExceptionServices;
using Eto.Mac;

namespace Eto.Test.Mac.UnitTests;


[TestFixture]
public class ColorTests : TestBase
{
	[Test]
	public void ColorShouldRoundTrip()
	{
		var color = new Color(0.1f, 0.2f, 0.3f, 0.4f);
		var nscolor = color.ToNSUI();
		var etoColor = nscolor.ToEto();
		Assert.That(etoColor, Is.EqualTo(color), "#1");
	}
}