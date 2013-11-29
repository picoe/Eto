using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageView : IControl
	{
		Image Image { get; set; }
	}
	
	public class ImageView : Control
	{
		new IImageView Handler { get { return (IImageView)base.Handler; } }
		
		public ImageView()
			: this((Generator)null)
		{
		}

		public ImageView (Generator generator)
			: this (generator, typeof(IImageView))
		{
		}
		
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

