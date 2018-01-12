using NUnit.Framework;

namespace Eto.Test.WinForms.UnitTests
{
    [TestFixture]
    public class NativeTests
    {
        [TestCase(0, 0)]
        [TestCase(-0x00780000, -0x0078)]
        [TestCase(0x00780000, 0x0078)]
        [TestCase(-0x01100000, -0x0110)]
        [TestCase(0x01100000, 0x0110)]
        public void SignedHiWord_TestCases_WorkCorrectly(int value, int expected)
        {
			var actual = Win32.SignedHIWORD(value);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0, 0)]
        [TestCase(-0x00000078, -0x0078)]
        [TestCase(0x00000078, 0x0078)]
        [TestCase(-0x00000110, -0x0110)]
        [TestCase(0x00000110, 0x0110)]
        public void SignedLoWord_TestCases_WorkCorrectly(int value, int expected)
        {
			var actual = Win32.SignedLOWORD(value);
            Assert.AreEqual(expected, actual);
        }
    }

}