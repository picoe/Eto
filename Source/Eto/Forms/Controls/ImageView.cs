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
		IImageView handler;
		
		public ImageView ()
			: this(Generator.Current)
		{
		}
		
		public ImageView (Generator g)
			: this (g, typeof(IImageView))
		{
		}
		
		protected ImageView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IImageView)Handler;
		}

		
		public Image Image
		{
			get { return handler.Image; }
			set { handler.Image = value; }
		}
	}
}

