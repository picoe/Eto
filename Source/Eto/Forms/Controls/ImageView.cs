using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Control to display an image
	/// </summary>
	/// <remarks>
	/// By default, the ImageView will automatically size to the size of the specified <see cref="ImageView.Image"/>,
	/// otherwise the image will be scaled to fit inside the available area for the control.
	/// </remarks>
	[Handler(typeof(ImageView.IHandler))]
	public class ImageView : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageView"/> class.
		/// </summary>
		public ImageView()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ImageView (Generator generator)
			: this (generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ImageView"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ImageView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the image to display.
		/// </summary>
		/// <value>The image.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ImageView"/>
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the image to display.
			/// </summary>
			/// <value>The image.</value>
			Image Image { get; set; }
		}
	}
}

