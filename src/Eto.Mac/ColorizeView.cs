namespace Eto.Mac
{
	class ColorizeView : IDisposable
	{
		CGBitmapContext _context;
		CGSize _size;
		bool _flipped;
		bool _active;

		// tables for the sRGB transfer function, as the colour is applied to linear values
		static readonly float[] s_toLinear = CreateToLinear();
		static readonly float[] s_fromLinear = CreateFromLinear();
		const int FromLinearMax = 4095;

		public Color Color { get; set; }

		public static void Create(ref ColorizeView colorize, Color? color)
		{
			if (color == null)
			{
				colorize?.Dispose();
				colorize = null;
				return;
			}

			colorize = colorize ?? new ColorizeView();

			colorize.Color = color.Value;
		}

		/// <summary>
		/// Redirects drawing into an offscreen bitmap until <see cref="End"/> draws it colourized.
		/// </summary>
		public void Begin(CGRect frame, NSView controlView)
		{
			Restore();

			var size = controlView.Frame.Size;
			if (size.Width <= 0 || size.Height <= 0)
				return;

			var scale = controlView.Window?.BackingScaleFactor ?? NSScreen.MainScreen?.BackingScaleFactor ?? 1;
			var width = (nint)Math.Ceiling(size.Width * scale);
			var height = (nint)Math.Ceiling(size.Height * scale);

			if (_context == null || _context.Width != width || _context.Height != height)
			{
				_context?.Dispose();
				using var colorSpace = CGColorSpace.CreateSrgb();
				_context = new CGBitmapContext(IntPtr.Zero, width, height, 8, 4 * width, colorSpace, CGBitmapFlags.PremultipliedLast);
			}
			else
				_context.ClearRect(new CGRect(0, 0, width, height));

			_size = size;
			_flipped = controlView.IsFlipped;

			_context.SaveState();
			_context.ScaleCTM(scale, scale);
			if (_flipped)
			{
				_context.TranslateCTM(0, size.Height);
				_context.ScaleCTM(1, -1);
			}

			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = NSGraphicsContext.FromCGContext(_context, _flipped);
			_active = true;
		}

		/// <summary>
		/// Draws what was drawn since <see cref="Begin"/> as a grayscale image tinted with <see cref="Color"/>.
		/// </summary>
		public void End()
		{
			if (!Restore())
				return;

			var cgColor = Color.ToCG();
			var components = cgColor.Components;
			if (components.Length == 0)
				return;
			var alpha = (float)cgColor.Alpha;
			if (alpha <= 0)
				return; // nothing would be visible

			// same as CIColorControls with no saturation, then CIColorMatrix moving the gray into the colour
			float r, g, b;
			if (components.Length >= 3)
			{
				r = (float)components[0];
				g = (float)components[1];
				b = (float)components[2];
			}
			else
				r = g = b = (float)components[0];

			Colorize(_context, r, g, b, alpha);

			using var image = _context.ToImage();
			if (image == null)
				return;

			var ctx = NSGraphicsContext.CurrentContext?.CGContext;
			if (ctx == null)
				return;

			ctx.SaveState();
			if (_flipped)
			{
				ctx.TranslateCTM(0, _size.Height);
				ctx.ScaleCTM(1, -1);
			}
			ctx.DrawImage(new CGRect(CGPoint.Empty, _size), image);
			ctx.RestoreState();
		}

		bool Restore()
		{
			if (!_active)
				return false;
			_active = false;

			NSGraphicsContext.GlobalRestoreGraphicsState();
			_context.RestoreState();
			return true;
		}

		static unsafe void Colorize(CGBitmapContext context, float r, float g, float b, float alpha)
		{
			var data = (byte*)context.Data;
			var width = (int)context.Width;
			var height = (int)context.Height;
			var bytesPerRow = (int)context.BytesPerRow;
			var toLinear = s_toLinear;
			var fromLinear = s_fromLinear;

			for (int y = 0; y < height; y++)
			{
				var p = data + y * bytesPerRow;
				for (int x = 0; x < width; x++, p += 4)
				{
					int a = p[3];
					if (a == 0)
						continue;

					// unpremultiply, then take the luminance (Rec. 709) in linear space
					var half = a / 2;
					var luma = 0.2126f * toLinear[Math.Min(255, (p[0] * 255 + half) / a)]
						+ 0.7152f * toLinear[Math.Min(255, (p[1] * 255 + half) / a)]
						+ 0.0722f * toLinear[Math.Min(255, (p[2] * 255 + half) / a)];

					var outAlpha = a * alpha;
					p[0] = (byte)(FromLinear(fromLinear, luma * r) * outAlpha + 0.5f);
					p[1] = (byte)(FromLinear(fromLinear, luma * g) * outAlpha + 0.5f);
					p[2] = (byte)(FromLinear(fromLinear, luma * b) * outAlpha + 0.5f);
					p[3] = (byte)(outAlpha + 0.5f);
				}
			}
		}

		static float FromLinear(float[] table, float value) => table[(int)(Math.Min(1f, Math.Max(0f, value)) * FromLinearMax + 0.5f)];

		static float[] CreateToLinear()
		{
			var table = new float[256];
			for (int i = 0; i < table.Length; i++)
			{
				var c = i / 255.0;
				table[i] = (float)(c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4));
			}
			return table;
		}

		static float[] CreateFromLinear()
		{
			var table = new float[FromLinearMax + 1];
			for (int i = 0; i < table.Length; i++)
			{
				var c = (double)i / FromLinearMax;
				table[i] = (float)(c <= 0.0031308 ? c * 12.92 : 1.055 * Math.Pow(c, 1 / 2.4) - 0.055);
			}
			return table;
		}

		public void Dispose()
		{
			Restore();
			_context?.Dispose();
			_context = null;
		}
	}
}
