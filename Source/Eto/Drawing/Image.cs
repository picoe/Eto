using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Handler interface for the <see cref="Image"/> class
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IImage : IInstanceWidget
	{
		/// <summary>
		/// Gets the size of the image, in pixels
		/// </summary>
		Size Size { get; }
	}

	/// <summary>
	/// Interface for an image that can have its data locked for direct access
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
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
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[TypeConverter(typeof(ImageConverter))]
	public abstract class Image : InstanceWidget
	{
		new IImage Handler { get { return (IImage)base.Handler; } }

		protected Image()
		{
		}

		protected Image(IImage handler)
			: base(handler)
		{
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Initializes a new instance of an image with the specified type
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler to create (must be derived from <see cref="IImage"/>)</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Image(Generator generator, Type type) : base(generator, type)
		{
		}

		/// <summary>
		/// Initializes a new instance of an image with the specified handler instance
		/// </summary>
		/// <remarks>
		/// This is useful when you want to create an image that wraps around an existing instance of the 
		/// handler. This is typically only done from a platform implementation that returns an image instance.
		/// </remarks>
		/// <param name="generator">Generator for the handler</param>
		/// <param name="handler">Instance of the handler to attach to this instance</param>
		[Obsolete("Use variation without generator instead")]
		protected Image(Generator generator, IImage handler) : base(generator, handler)
		{
		}

		#pragma warning restore 612,618

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
	}
}
