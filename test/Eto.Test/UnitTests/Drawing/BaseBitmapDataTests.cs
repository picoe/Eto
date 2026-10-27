using NUnit.Framework;
using Eto.Shared.Drawing;

namespace Eto.Test.UnitTests.Drawing
{
	/// <summary>
	/// Tests for <see cref="BaseBitmapData"/> using a data buffer we control, so we can verify exactly
	/// which bytes it reads/writes for each pixel.
	/// </summary>
	/// <remarks>
	/// The <see cref="BaseBitmapData"/> source is shared with each platform (see src/Shared) and compiled
	/// into this assembly as well so it can be tested directly without a platform-specific bitmap.
	/// </remarks>
	[TestFixture]
	public class BaseBitmapDataTests : TestBase
	{
		/// <summary>
		/// Records the raw (untranslated) value passed to <see cref="TranslateDataToArgb"/> so we can tell
		/// how many bytes GetPixel actually read for a pixel.
		/// </summary>
		class RecordingBitmapData : BaseBitmapData
		{
			public RecordingBitmapData(Image image, IntPtr data, int scanWidth, int bitsPerPixel)
				: base(image, data, scanWidth, bitsPerPixel, null, false)
			{
			}

			public int LastData { get; private set; }

			public override int TranslateArgbToData(int argb) => argb;

			public override int TranslateDataToArgb(int bitmapData)
			{
				LastData = bitmapData;
				return bitmapData;
			}

			protected override void Dispose(bool disposing)
			{
				// the data isn't from an actual locked image, so there's nothing to unlock
			}
		}

		// 4 pixels * 3 bytes == 12 bytes per row, so there is no padding at the end of each row and the
		// last pixel of the last row ends exactly at the end of the data.
		const int Width = 4;
		const int Height = 2;
		const int ScanWidth = Width * 3;
		const int DataLength = ScanWidth * Height;

		/// <summary>
		/// Byte written directly after the pixel data.  Reading it is out of bounds, but by owning it we get
		/// a deterministic failure instead of whatever the allocator happens to leave there.
		/// </summary>
		const byte GuardByte = 0xAB;

		static void TestData(Action<RecordingBitmapData> test) => Invoke(() =>
		{
			var image = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
			var buffer = Marshal.AllocHGlobal(DataLength + 1);
			try
			{
				unsafe
				{
					var ptr = (byte*)buffer;
					// fill with a recognizable sequence, 1..DataLength
					for (int i = 0; i < DataLength; i++)
						ptr[i] = (byte)(i + 1);
					ptr[DataLength] = GuardByte;
				}

				using (var data = new RecordingBitmapData(image, buffer, ScanWidth, 24))
					test(data);
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}
		});

		[Test]
		public void GetPixel24bppShouldNotReadPastEndOfData() => TestData(data =>
		{
			// the last pixel of the last row is the final three bytes of the data
			var color = data.GetPixel(Width - 1, Height - 1);

			Assert.That(data.LastData, Is.EqualTo(0x181716), $"Should only read the 3 bytes of the pixel, but read 0x{data.LastData:X8}");
			Assert.That(color, Is.EqualTo(Color.FromRgb(0x181716)));
		});

		[Test]
		public void GetPixel24bppShouldReadFirstPixel() => TestData(data =>
		{
			// the first pixel has no data before it, so it can't be read by looking backwards either
			var color = data.GetPixel(0, 0);

			Assert.That(data.LastData, Is.EqualTo(0x030201), $"Should only read the 3 bytes of the pixel, but read 0x{data.LastData:X8}");
			Assert.That(color, Is.EqualTo(Color.FromRgb(0x030201)));
		});

		[TestCase(0, 0, 0x030201)]
		[TestCase(1, 0, 0x060504)]
		[TestCase(3, 0, 0x0C0B0A)]
		[TestCase(0, 1, 0x0F0E0D)]
		[TestCase(2, 1, 0x151413)]
		[TestCase(3, 1, 0x181716)]
		public void GetPixel24bppShouldReadCorrectBytes(int x, int y, int expected) => TestData(data =>
		{
			var color = data.GetPixel(x, y);

			Assert.That(data.LastData, Is.EqualTo(expected), $"Read 0x{data.LastData:X8} for pixel {x},{y}");
			Assert.That(color, Is.EqualTo(Color.FromRgb(expected)));
		});
	}
}
