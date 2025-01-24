using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Mac.Forms.Controls;
namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class RichTextAreaHandlerTests : TestBase
	{
		/// <summary>
		/// Tests the interaction of the Enabled and ReadOnly properties with NSTextArea's Selectable and Editable 
		/// properties since they are sometimes changed automatically by AppKit when Selectable is set.
		/// </summary>
		[Test]
		public void EnabledShouldChangeEditable()
		{
			Invoke(() =>
			{
				var richTextArea = new RichTextArea();
				var handler = richTextArea.Handler as RichTextAreaHandler;

				Assert.That(richTextArea.Enabled, Is.True, "#1");
				Assert.That(richTextArea.ReadOnly, Is.False, "#2");
				Assert.That(handler.Control.Selectable, Is.True, "#3");
				Assert.That(handler.Control.Editable, Is.True, "#4");
				richTextArea.Enabled = false;

				Assert.That(handler.Control.Selectable, Is.False, "#5");
				Assert.That(handler.Control.Editable, Is.False, "#6");
				richTextArea.Enabled = true;

				Assert.That(handler.Control.Selectable, Is.True, "#7");
				Assert.That(handler.Control.Editable, Is.True, "#8");

				richTextArea.ReadOnly = true;
				Assert.That(handler.Control.Selectable, Is.True, "#9");
				Assert.That(handler.Control.Editable, Is.False, "#10");

				richTextArea.Enabled = false;
				Assert.That(handler.Control.Selectable, Is.False, "#11");
				Assert.That(handler.Control.Editable, Is.False, "#12");

				richTextArea.Enabled = true;
				Assert.That(handler.Control.Selectable, Is.True, "#13");
				Assert.That(handler.Control.Editable, Is.False, "#14");

				richTextArea.ReadOnly = false;
				Assert.That(handler.Control.Selectable, Is.True, "#15");
				Assert.That(handler.Control.Editable, Is.True, "#16");

				richTextArea.Enabled = false;
				Assert.That(handler.Control.Selectable, Is.False, "#17");
				Assert.That(handler.Control.Editable, Is.False, "#18");

				richTextArea.ReadOnly = true;
				Assert.That(handler.Control.Selectable, Is.False, "#19");
				Assert.That(handler.Control.Editable, Is.False, "#20");

				richTextArea.Enabled = true;
				Assert.That(handler.Control.Selectable, Is.True, "#21");
				Assert.That(handler.Control.Editable, Is.False, "#22");

				richTextArea.ReadOnly = false;
				Assert.That(handler.Control.Selectable, Is.True, "#23");
				Assert.That(handler.Control.Editable, Is.True, "#24");
			});
		}
	}
}