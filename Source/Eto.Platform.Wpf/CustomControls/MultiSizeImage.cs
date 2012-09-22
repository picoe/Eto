using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Eto.Platform.Wpf.CustomControls
{
	/// <summary>
	/// An image that supports multiple sizes depending on its own rendering size.
	/// </summary>
	/// <remarks>
	/// Extends the standard <see cref="Image"/> control with the functionality to 
	/// load all the available frames in its <see cref="Image.Source"/> and render the one
	/// that best matches the current rendering size of the control.
	/// 
	/// For example, if the Source is a TIFF file containing frames of sizes 24x24, 48x48 and 128x128
	/// and the image is rendered at size 40x40, the frame with resolution 48x48 is used.
	/// The same control with the same source rendered at 24x24 would use the 24x24 frame.
	/// 
	/// <para>Written by Isak Savo - isak.savo@gmail.com, (c) 2011-2012. Licensed under the Code Project  </para>
	/// </remarks>
	public class MultiSizeImage : Image
	{
		static MultiSizeImage ()
		{
			// Tell WPF to inform us whenever the Source dependency property is changed
			SourceProperty.OverrideMetadata (typeof (MultiSizeImage), new FrameworkPropertyMetadata (HandleSourceChanged));
		}

		private static void HandleSourceChanged (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MultiSizeImage img = (MultiSizeImage)sender;
			img.UpdateAvailableFrames ();
		}
		/// <summary>
		/// List containing one frame of every size available in the original file. The frame 
		/// stored in this list is the one with the highest pixel depth for that size.
		/// </summary>
		private List<BitmapSource> _availableFrames = new List<BitmapSource> ();

		/// <summary>
		/// Gets the pixel depth (in bits per pixel, bpp) of the specified frame
		/// </summary>
		/// <param name="frame">The frame to get BPP for</param>
		/// <returns>The number of bits per pixel in the frame</returns>
		private int GetFramePixelDepth (BitmapFrame frame)
		{
			if (frame.Decoder.CodecInfo.ContainerFormat == new Guid ("{a3a860c4-338f-4c17-919a-fba4b5628f21}")
				&& frame.Thumbnail != null)
			{
				// Windows Icon format, original pixel depth is in the thumbnail
				return frame.Thumbnail.Format.BitsPerPixel;
			}
			else
			{
				// Other formats, just assume the frame has the correct BPP info
				return frame.Format.BitsPerPixel;
			}
		}

		protected override Size ArrangeOverride (Size arrangeSize)
		{
			return MeasureArrangeHelper (arrangeSize);
		}

		protected override Size MeasureOverride (Size constraint)
		{
			return MeasureArrangeHelper (constraint);
		}

		static bool IsZero (double value)
		{
			return Math.Abs (value) < 2.2204460492503131E-15;
		}

		static Size ComputeScaleFactor (Size availableSize, Size contentSize, Stretch stretch, StretchDirection stretchDirection)
		{
			double widthFactor = 1.0;
			double heightFactor = 1.0;
			bool widthSet = !double.IsPositiveInfinity (availableSize.Width);
			bool heightSet = !double.IsPositiveInfinity (availableSize.Height);
			if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill) && (widthSet || heightSet))
			{
				widthFactor = (IsZero (contentSize.Width) ? 0.0 : (availableSize.Width / contentSize.Width));
				heightFactor = (IsZero (contentSize.Height) ? 0.0 : (availableSize.Height / contentSize.Height));
				if (!widthSet)
					widthFactor = heightFactor;
				else
				{
					if (!heightSet)
						heightFactor = widthFactor;
					else
					{
						switch (stretch)
						{
						case Stretch.Uniform:
							{
								double num3 = (widthFactor < heightFactor) ? widthFactor : heightFactor;
								heightFactor = (widthFactor = num3);
								break;
							}
						case Stretch.UniformToFill:
							{
								double num4 = (widthFactor > heightFactor) ? widthFactor : heightFactor;
								heightFactor = (widthFactor = num4);
								break;
							}
						}
					}
				}
				switch (stretchDirection)
				{
				case StretchDirection.UpOnly:
					if (widthFactor < 1.0)
						widthFactor = 1.0;
					if (heightFactor < 1.0)
						heightFactor = 1.0;
					break;
				case StretchDirection.DownOnly:
					if (widthFactor > 1.0)
						widthFactor = 1.0;
					if (heightFactor > 1.0)
						heightFactor = 1.0;
					break;
				}
			}
			return new Size (widthFactor, heightFactor);
		}

		Size MeasureArrangeHelper (Size inputSize)
		{
			var first = _availableFrames.LastOrDefault ();
			Size size = new Size (this.Width, this.Height);
			if (first == null)
				return size;

			size = new Size (first.Width, first.Height);

			Size size2 = ComputeScaleFactor (inputSize, size, this.Stretch, this.StretchDirection);
			return new Size (size.Width * size2.Width, size.Height * size2.Height);
		}
		/// <summary>
		/// Scans the ImageSource for available frames and stores 
		/// them as individual bitmap sources. This is done once, 
		/// when the <see cref="Image.Source"/> property is set (or changed)
		/// </summary>
		private void UpdateAvailableFrames ()
		{
			_availableFrames.Clear ();
			BitmapFrame bmFrame = Source as BitmapFrame;
			if (bmFrame == null)
				return;

			var decoder = bmFrame.Decoder;
			if (decoder != null && decoder.Frames != null)
			{
				var framesInSizeOrder = from frame in decoder.Frames
										group frame by frame.PixelHeight * frame.PixelWidth into g
										orderby g.Key
										select new
											{
												Size = g.Key,
												Frames = g.OrderByDescending (GetFramePixelDepth)
											};
				_availableFrames.AddRange (framesInSizeOrder.Select (group => group.Frames.First ()));
			}
		}

		/// <summary>
		/// Renders the contents of the image
		/// </summary>
		/// <param name="dc">An instance of <see cref="T:System.Windows.Media.DrawingContext"/> used to render the control.</param>
		protected override void OnRender (DrawingContext dc)
		{
			if (Source == null)
			{
				base.OnRender (dc);
				return;
			}
			ImageSource src = Source;
			var ourSize = RenderSize.Width * RenderSize.Height;
			foreach (var frame in _availableFrames)
			{
				src = frame;
				if (frame.PixelWidth * frame.PixelHeight >= ourSize)
					break;
			}
			if (src is BitmapSource)
			{
				var bs = (BitmapSource)src;
				//Console.WriteLine("Rendering frame of size {0}x{1}", bs.PixelWidth, bs.PixelHeight);
			}
			dc.DrawImage (src, new Rect (new Point (0, 0), RenderSize));
		}
	}
}
