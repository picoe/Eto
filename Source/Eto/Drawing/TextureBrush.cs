using System;

namespace Eto.Drawing
{
	/// <summary>
	/// A brush with an image texture
	/// </summary>
	public interface ITextureBrush : IBrush
	{
		Image Image { get; }

		IMatrix Transform { get; set; }
	}

	/// <summary>
	/// Handler for the <see cref="ITextureBrush"/> interface
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ITextureBrushHandler : ITextureBrush
	{
		void Create (Image image);
	}

	/// <summary>
	/// Methods to create a brush with an image texture
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class TextureBrush
	{
		public static ITextureBrush Create (Image image, Generator generator = null)
		{
			var handler = generator.Create<ITextureBrushHandler> ();
			handler.Create (image);
			return handler;
		}
	}
}

