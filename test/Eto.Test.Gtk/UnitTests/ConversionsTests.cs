using NUnit.Framework;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class ConversionsTests
	{
		[TestCase(null, "", null)]
		[TestCase("", "", "")]
		[TestCase("&Open", "_Open", "&Open")]
		[TestCase("_Open", "__Open", "&Open")]
		[TestCase("A&&B", "A&B", "A&&B")]
		[TestCase("A__B", "A____B", "A_B")]
		[TestCase("A&&&B", "A&_B", "A&&&B")]
		[TestCase("A___B", "A______B", "A_&B")]
		[TestCase("&&&a&b&c", "&&a_b&c", "&&&a&b&c")]
		[TestCase("___a_b_c", "______a__b__c", "__a&b_c")]
		public void MnemonicConversionsShouldPreserveEscapedMarkers(string text, string platformText, string etoText)
		{
			Assert.That(Conversions.ToPlatformMnemonic(text), Is.EqualTo(platformText));
			Assert.That(Conversions.ToEtoMnemonic(text), Is.EqualTo(etoText));
		}
	}
}
