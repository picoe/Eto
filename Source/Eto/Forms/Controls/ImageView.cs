using System;
using Eto.Drawing;

namespace Eto.Forms
{
	[Handler(typeof(ImageView.IHandler))]
	public class ImageView : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public ImageView()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ImageView (Generator generator)
			: this (generator, typeof(IHandler))
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

		public new interface IHandler : Control.IHandler
		{
			Image Image { get; set; }
		}
	}
}

