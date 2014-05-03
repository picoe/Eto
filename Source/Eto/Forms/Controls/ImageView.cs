using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageView : IControl
	{
		Image Image { get; set; }
	}
	
	[Handler(typeof(IImageView))]
	public class ImageView : Control
	{
		new IImageView Handler { get { return (IImageView)base.Handler; } }
		
		public ImageView()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ImageView (Generator generator)
			: this (generator, typeof(IImageView))
		{
		}
		
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ImageView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}
	}
}

