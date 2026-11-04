using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using g = Gtk;
using Range = Eto.Forms.Range;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class RichTextAreaTests : TestBase
	{
		[Test]
		public void TagTableShouldBeCollectibleWithoutHandlerDisposal() => Async(async () =>
		{
			var table = CreateAbandonedTagTable();
			for (var i = 0; i < 80 && table.IsAlive; i++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				await Task.Delay(25);
			}
			Assert.That(table.IsAlive, Is.False);
		});

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference CreateAbandonedTagTable()
		{
			var richText = new RichTextArea();
			var view = (g.TextView)richText.ControlObject;
			var buffer = view.Buffer;
			var table = new WeakReference(buffer.TagTable);
			using (var replacement = new g.TextBuffer(null))
				view.Buffer = replacement;
			// Release the existing buffer subscription without disposing the Eto handler.
			buffer.Dispose();
			view.Destroy();
			view.Dispose();
			return table;
		}

		[Test]
		public void FormattingShouldRemoveNativeTagsAfterWrapperDisposal() => Invoke(() =>
		{
			using var richText = new RichTextArea { Text = "text" };
			var range = Range.FromLength(0, richText.TextLength);
			var buffer = ((g.TextView)richText.ControlObject).Buffer;
			richText.Buffer.SetBold(range, true);
			using (var nativeTag = new g.TextTag("w-native") { Weight = Pango.Weight.Normal })
			{
				buffer.TagTable.Add(nativeTag);
				richText.Buffer.SetBold(range, true);
			}
			using var native = buffer.TagTable.Lookup("w-native");
			buffer.ApplyTag(native, buffer.StartIter, buffer.EndIter);
			richText.Buffer.SetBold(range, true);
			Assert.That(buffer.StartIter.HasTag(native), Is.False);
			Assert.That(buffer.StartIter.HasTag(buffer.TagTable.Lookup("w-Bold")), Is.True);
		});
	}
}
