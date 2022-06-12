using System;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class GraphicsOffsetModeTests : TestBase
	{
		string folder;

		static PixelOffsetMode defaultMode = PixelOffsetMode.Aligned;

		public GraphicsOffsetModeTests()
		{
			folder = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.Documents), "TestShapeDrawing", Platform.Instance.ID);
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
		}

		[ManualTest, Test]
		public void TestEdgeInconsistency()
		{
			int Distance(Color a, Color b)
			{
				var d = 0f;
				d += Math.Abs(a.R - b.R);
				d += Math.Abs(a.G - b.G);
				d += Math.Abs(a.B - b.B);
				return (int)(255 * d / 3);
			}

			using (var small = Chequer(20, Colors.White, Colors.White))
			{
				using (var sg = new Graphics(small))
				{
					sg.PixelOffsetMode = defaultMode;
					sg.DrawEllipse(Colors.Black, new RectangleF(1, 1, 17, 17));
				}

				var map = small.Clone();
				for (int x = 0; x < map.Width; x++)
					for (int y = 0; y < map.Height; y++)
					{
						// We're only mirroring along the vertical axis.
						var a = small.GetPixel(x, y);
						var b = small.GetPixel(small.Width - x - 1, y);
						var d = Distance(a, b);

						if (d > 24)
							map.SetPixel(x, y, Colors.Purple);
						else if (d > 12)
							map.SetPixel(x, y, Colors.Red);
						else if (d > 6)
							map.SetPixel(x, y, Colors.Orange);
						else if (d > 1)
							map.SetPixel(x, y, Colors.Gold);
						else
							map.SetPixel(x, y, Colors.White);
					}

				using (var large = Enlarge(small, 400))
				using (var largeMap = Enlarge(map, 400))
				{
					var stacked = Stack(large, largeMap);
					DisplayImage(stacked);
				}
			}
		}

		[ManualTest, Test]
		public void TestEdgeFillMisalignment()
		{
			using (var small = Chequer(20))
			{
				using (var graphics = new Graphics(small))
				{
					graphics.PixelOffsetMode = defaultMode;
					graphics.FillEllipse(Colors.Crimson, new RectangleF(1, 1, 17, 17));
					graphics.DrawEllipse(Colors.Black, new RectangleF(1, 1, 17, 17));
				}

				var pure = small.Clone();
				for (int x = 0; x < pure.Width; x++)
					for (int y = 0; y < pure.Height; y++)
					{
						var c = pure.GetPixel(x, y);
						var hsl = new ColorHSL(c);
						if (hsl.S > 0f)
						{
							hsl.S = 1;
							pure.SetPixel(x, y, hsl);
						}
					}

				using (var large = Enlarge(small, 400))
				using (var largePure = Enlarge(pure, 400))
				{
					var stacked = Stack(large, largePure);
					DisplayImage(stacked);
				}

				pure.Dispose();
			}
		}

		[ManualTest, Test]
		public void TestPlugIcon()
		{
			var polygon = new PointF[]
			{
		new PointF(7.5f, 4.5f),
		new PointF(14.5f, 11.5f),
		new PointF(10.5f, 15.5f),
		new PointF(6.5f, 15.5f),
		new PointF(3.5f, 12.5f),
		new PointF(3.5f, 8.5f),
			};

			var linesA = new float[4][];
			linesA[0] = new float[] { 5.5f, 13.5f, 2.5f, 16.5f };
			linesA[1] = new float[] { 9.5f, 5.5f, 13.5f, 1.5f };
			linesA[2] = new float[] { 15.5f, 11.5f, 7.5f, 3.5f };
			linesA[3] = new float[] { 13.5f, 9.5f, 17.5f, 5.5f };

			var linesB = new float[2][];
			linesB[0] = new float[] { 8, 8, 11, 11 };
			linesB[1] = new float[] { 6, 10, 9, 13 };

			using (var small = Chequer(20, Colors.White, Colors.White))
			{
				using (var graphics = new Graphics(small))
				{
					graphics.PixelOffsetMode = defaultMode;
					graphics.FillPolygon(Colors.DimGray, polygon);

					using (var edge = new Pen(Colors.DimGray, 2))
						foreach (var line in linesA)
							graphics.DrawLine(edge, line[0], line[1], line[2], line[3]);

					using (var edge = new Pen(Colors.LightGrey, 1))
						foreach (var line in linesB)
							graphics.DrawLine(edge, line[0], line[1], line[2], line[3]);
				}

				using (var large = Enlarge(small, 400))
				{
					// Multiply all coordinates by 800/20.
					var factor = large.Width / small.Width;
					for (int i = 0; i < polygon.Length; i++)
						polygon[i] *= factor;

					for (int i = 0; i < linesA.Length; i++)
						for (int j = 0; j < linesA[i].Length; j++)
							linesA[i][j] *= factor;

					for (int i = 0; i < linesB.Length; i++)
						for (int j = 0; j < linesB[i].Length; j++)
							linesB[i][j] *= factor;

					DrawPixelGrid(large, factor);
					using (var graphics = new Graphics(large))
					{
						//graphics.PixelOffsetMode = PixelOffsetMode.Half;
						graphics.DrawPolygon(Colors.Red, polygon);
						foreach (var line in linesA)
							graphics.DrawLine(Colors.Red, line[0], line[1], line[2], line[3]);
						foreach (var line in linesB)
							graphics.DrawLine(Colors.Red, line[0], line[1], line[2], line[3]);
					}

					DisplayImage(large);
				}
			}
		}

		void DisplayImage(Bitmap image)
		{
			ManualForm("Inspect image", form => 
			{
				form.WindowState = WindowState.Maximized;
				return TableLayout.AutoSized(new ImageView { Image = image }, centered: true);
			});

		}

		private static Bitmap Chequer(int size, Color? a = null, Color? b = null)
		{
			var ca = a ?? Colors.White;
			var cb = b ?? Colors.LightGrey;

			var image = new Bitmap(size, size, PixelFormat.Format32bppRgba);
			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
					if ((x + y) % 2 == 0)
						image.SetPixel(x, y, ca);
					else
						image.SetPixel(x, y, cb);

			return image;
		}
		private static Bitmap Enlarge(Bitmap image, int newSize)
		{
			var bitmap = new Bitmap(newSize, newSize, PixelFormat.Format32bppRgba);
			using (var graphics = new Graphics(bitmap))
			{
				//graphics.PixelOffsetMode = PixelOffsetMode.Half;
				graphics.Clear(Colors.DeepPink);
				graphics.AntiAlias = false;
				graphics.ImageInterpolation = ImageInterpolation.None;
				graphics.DrawImage(image, 0, 0, newSize, newSize);
			}

			return bitmap;
		}
		private static Bitmap Stack(Bitmap a, Bitmap b)
		{
			var bitmap = new Bitmap(a.Width + b.Width, Math.Max(a.Height, b.Height), PixelFormat.Format32bppRgba);
			using (var graphics = new Graphics(bitmap))
			{
				//graphics.PixelOffsetMode = PixelOffsetMode.Half;
				graphics.AntiAlias = false;
				graphics.ImageInterpolation = ImageInterpolation.None;
				graphics.DrawImage(a, 0, 0, a.Width, a.Height);
				graphics.DrawImage(b, a.Width, 0, b.Width, b.Height);
			}

			return bitmap;
		}
		private static void DrawPixelGrid(Bitmap image, int pixelSize, Color? colour = null)
		{
			var c = colour ?? new Color(0, 0, 0, 0.3f);

			using (var graphics = new Graphics(image))
			{
				//graphics.PixelOffsetMode = PixelOffsetMode.Half;
				for (int x = 0; x < image.Width; x += pixelSize)
					graphics.DrawLine(c, x, 0, x, image.Height);
				for (int y = 0; y < image.Height; y += pixelSize)
					graphics.DrawLine(c, 0, y, image.Width, y);
			}
		}
	}
}