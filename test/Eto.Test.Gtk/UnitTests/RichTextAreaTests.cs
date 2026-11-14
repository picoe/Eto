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
			await TextAreaTests.WaitForCollection(table);
			Assert.That(table.IsAlive, Is.False);
		});

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference CreateAbandonedTagTable()
		{
			var richText = new RichTextArea();
			var view = (g.TextView)richText.ControlObject;
			var table = new WeakReference(view.Buffer.TagTable);
			view.Destroy();
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
