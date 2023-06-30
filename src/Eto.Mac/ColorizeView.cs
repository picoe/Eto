namespace Eto.Mac
{
	class ColorizeView : IDisposable
	{
		NSImage _image;

		public Color Color { get; set; }

		static readonly NSString CIOutputImage = new NSString("outputImage");
		static readonly bool supportsConvertSizeToBacking = ObjCExtensions.InstancesRespondToSelector<NSView>("convertSizeToBacking:");

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

		public void Begin(CGRect frame, NSView controlView)
		{
			var size = controlView.Frame.Size;
			if (size.Width <= 0 || size.Height <= 0)
			{
				return;
			}
			
			if (_image == null || size != _image.Size)
			{
				if (_image != null)
					_image.Dispose();

				_image = new NSImage(size);
			}

			_image.LockFocusFlipped(!controlView.IsFlipped);
			NSGraphicsContext.CurrentContext.GraphicsPort.ClearRect(new CGRect(CGPoint.Empty, size));
		}

		public void End()
		{
			if (_image == null)
				return;

			_image.UnlockFocus();

			var cgImage = _image.CGImage;
			var ciImage = CIImage.FromCGImage(cgImage);

			var filter2 = new CIColorControls();
			filter2.SetDefaults();
			filter2.InputImage = ciImage;
			filter2.Saturation = 0.0f;
			ciImage = (CIImage)filter2.ValueForKey(CIOutputImage);
			
			var filter3 = new CIColorMatrix();
			filter3.SetDefaults();
			filter3.InputImage = ciImage;

			var cgColor = Color.ToCG();
			var components = cgColor.Components;
			if (components.Length >= 3)
			{
				filter3.RVector = new CIVector(0, components[0], 0);
				filter3.GVector = new CIVector(components[1], 0, 0);
				filter3.BVector = new CIVector(0, 0, components[2]);
			}
			else if (components.Length >= 1)
			{
				// grayscale
				filter3.RVector = new CIVector(0, components[0], 0);
				filter3.GVector = new CIVector(components[0], 0, 0);
				filter3.BVector = new CIVector(0, 0, components[0]);
			}
			
			filter3.AVector = new CIVector(0, 0, 0, cgColor.Alpha);
			ciImage = (CIImage)filter3.ValueForKey(CIOutputImage);

			// create separate context so we can force using the software renderer, which is more than fast enough for this
			var ciContext = CIContext.FromContext(NSGraphicsContext.CurrentContext.GraphicsPort, new CIContextOptions { UseSoftwareRenderer = true });
			ciContext.DrawImage(ciImage, new CGRect(CGPoint.Empty, _image.Size), new CGRect(0, 0, cgImage.Width, cgImage.Height));

			ciImage.Dispose();
			ciContext.Dispose();
			filter2.Dispose();
			filter3.Dispose();
		}

		public void Dispose()
		{
			_image?.Dispose();
			_image = null;
		}
	}
}

