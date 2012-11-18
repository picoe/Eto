using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Handler interface for the <see cref="Image"/> class
	/// </summary>
	public interface IImage : IInstanceWidget
	{
		/// <summary>
		/// Gets the size of the image, in pixels
		/// </summary>
		Size Size { get; }

        int Width { get; }

        int Height { get; }
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
	[TypeConverter(typeof(ImageConverter))]
	public abstract class Image : InstanceWidget, IImage
	{
		IImage handler;

		/// <summary>
		/// Initializes a new instance of an image with the specified type
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler to create (must be derived from <see cref="IImage"/>)</param>
		protected Image(Generator generator, Type type) : base(generator, type)
		{
			handler = (IImage)Handler;
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
		protected Image(Generator generator, IImage handler) : base(generator, handler)
		{
			handler = (IImage)Handler;
		}
				
		/// <summary>
		/// Gets the size of the image, in pixels
		/// </summary>
		public Size Size
		{
            get { return handler != null ? handler.Size : Size.Empty; }
		}

        public int Width
        {
            get { return handler != null ? handler.Width : 0; }
        }

        public int Height
        {
            get { return handler != null ? handler.Height : 0; }
        }

        public new Widget Widget
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

		void IWidget.Initialize()
		{
			base.Initialize();
		}
    }
}
