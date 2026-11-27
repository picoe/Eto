using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using g = Gtk;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class TextAreaTests : TestBase
	{
		[TestCase(false)]
		[TestCase(true)]
		public void BufferShouldBeCollectibleAfterWindowIsDisposed(bool disposeControl) => Async(async () =>
		{
			var buffer = CreateBufferInDisposedWindow(disposeControl);
			await WaitForCollection(buffer);
			Assert.That(buffer.IsAlive, Is.False);
		});

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference CreateBufferInDisposedWindow(bool disposeControl)
		{
			var textArea = new TextArea { Font = SystemFonts.Default() };
			textArea.TextChanged += delegate { };
			textArea.SelectionChanged += delegate { };
			textArea.CaretIndexChanged += delegate { };
			var buffer = new WeakReference(((g.TextView)textArea.ControlObject).Buffer);
			var form = new Form { Content = textArea };
			form.Show();
			if (disposeControl)
				textArea.Dispose();
			form.Dispose();
			return buffer;
		}

		internal static async Task WaitForCollection(WeakReference target)
		{
			for (var i = 0; i < 80 && target.IsAlive; i++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				await Task.Delay(25);
			}
		}
	}
}
