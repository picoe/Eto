using System;
using sc = System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Interface for an image that can have its data locked for direct access
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ILockableImage
	{
		/// <summary>
		/// Locks the data of the image to directly access the bytes of the image
		/// </summary>
		/// <remarks>
		/// This locks the data to read and write to directly using unsafe pointers. After reading or updating
		/// the data, you must call <see cref="Unlock"/> to unlock the data.
		/// </remarks>
		/// <returns>A <see cref="BitmapData"/> object with information about the locked data</returns>
		BitmapData Lock ();
		
		/// <summary>
		/// Unlocks the previously locked data
		/// </summary>
		/// <remarks>
		/// This will unlock the data, and in some platforms write the data back to the image.  You must
		/// call this method before using the bitmap again.
		/// </remarks>
		/// <param name="bitmapData">The data previously locked via the <see cref="Lock"/> method</param>
		void Unlock (BitmapData bitmapData);
	}
	
	/// <summary>
	/// Base class for images
	/// </summary>
	/// <remarks>
	/// This provides a base for image functionality so that drawing and widgets can 
	/// reference any type of image, if supported.
	/// For instance, <see cref="Graphics"/> and <see cref="Forms.ImageView"/> can reference
	/// any Image-derived object.
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[sc.TypeConverter(typeof(ImageConverterInternal))]
	public abstract class Image : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.Image"/> class.
		/// </summary>
		protected Image()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.Image"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler implementation to use for the image.</param>
		protected Image(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Gets the size of the image, in pixels
		/// </summary>
		public Size Size
		{
			get { return Handler.Size; }
		}

		/// <summary>
		/// Gets the width of the image, in pixels.
		/// </summary>
		/// <remarks>
		/// Use <see cref="Size"/> if you wish to get the width and height at the same time.
		/// </remarks>
		/// <value>The width of the image, in pixels</value>
		/// <seealso cref="Size"/>
		public int Width { get { return Size.Width; } }

		/// <summary>
		/// Gets the height of the image, in pixels.
		/// </summary>
		/// <remarks>
		/// Use <see cref="Size"/> if you wish to get the width and height at the same time.
		/// </remarks>
		/// <value>The height of the image, in pixels</value>
		/// <seealso cref="Size"/>
		public int Height { get { return Size.Height; } }

		/// <summary>
		/// Handler interface for the <see cref="Image"/> class
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets the size of the image, in pixels
			/// </summary>
			Size Size { get; }
		}
	}
}
