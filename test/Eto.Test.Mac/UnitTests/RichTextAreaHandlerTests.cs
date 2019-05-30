using System;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Forms;
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

				Assert.IsTrue(richTextArea.Enabled, "#1");
				Assert.IsFalse(richTextArea.ReadOnly, "#2");
				Assert.IsTrue(handler.Control.Selectable, "#3");
				Assert.IsTrue(handler.Control.Editable, "#4");
				richTextArea.Enabled = false;

				Assert.IsFalse(handler.Control.Selectable, "#5");
				Assert.IsFalse(handler.Control.Editable, "#6");
				richTextArea.Enabled = true;

				Assert.IsTrue(handler.Control.Selectable, "#7");
				Assert.IsTrue(handler.Control.Editable, "#8");

				richTextArea.ReadOnly = true;
				Assert.IsTrue(handler.Control.Selectable, "#9");
				Assert.IsFalse(handler.Control.Editable, "#10");

				richTextArea.Enabled = false;
				Assert.IsFalse(handler.Control.Selectable, "#11");
				Assert.IsFalse(handler.Control.Editable, "#12");

				richTextArea.Enabled = true;
				Assert.IsTrue(handler.Control.Selectable, "#13");
				Assert.IsFalse(handler.Control.Editable, "#14");

				richTextArea.ReadOnly = false;
				Assert.IsTrue(handler.Control.Selectable, "#15");
				Assert.IsTrue(handler.Control.Editable, "#16");

				richTextArea.Enabled = false;
				Assert.IsFalse(handler.Control.Selectable, "#17");
				Assert.IsFalse(handler.Control.Editable, "#18");

				richTextArea.ReadOnly = true;
				Assert.IsFalse(handler.Control.Selectable, "#19");
				Assert.IsFalse(handler.Control.Editable, "#20");

				richTextArea.Enabled = true;
				Assert.IsTrue(handler.Control.Selectable, "#21");
				Assert.IsFalse(handler.Control.Editable, "#22");

				richTextArea.ReadOnly = false;
				Assert.IsTrue(handler.Control.Selectable, "#23");
				Assert.IsTrue(handler.Control.Editable, "#24");
			});
		}
	}
}