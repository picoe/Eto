using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Interface for brushes with a transform
	/// </summary>
	/// <remarks>
	/// The transform is used to specify how the brush will be applied to the drawing.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface ITransformBrush
	{
		/// <summary>
		/// Gets or sets the transform for this brush
		/// </summary>
		/// <value>The transform for the brush</value>
		IMatrix Transform { get; set; }
	}

	/// <summary>
	/// Defines a brush with an image texture for use with <see cref="Graphics"/> fill operations
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class TextureBrush : Brush, ITransformBrush
	{
		readonly IHandler handler;
		float opacity;

		/// <summary>
		/// Gets the texture's image to paint with
		/// </summary>
		/// <value>The image used to paint</value>
		public Image Image { get; private set; }

		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		public override object Handler { get { return handler; } }

		/// <summary>
		/// Gets an instantiator for the texture brush to create instances
		/// </summary>
		/// <remarks>
		/// This can be used to instantiate texture brushes when creating many brushes to minimize overhead
		/// </remarks>
		[Obsolete("Since 2.4: Use new TextureBrush() instead")]
		public static Func<Image, float, TextureBrush> Instantiator
		{
			get
			{
				var sharedHandler = Platform.Instance.TextureBrushHandler;
				return (image, opacity) => new TextureBrush(sharedHandler, image, opacity);
			}
		}

		TextureBrush (IHandler handler, Image image, float opacity)
		{
			this.handler = handler;
			this.ControlObject = handler.Create (image, opacity);
			this.Image = image;
			this.opacity = opacity;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.TextureBrush"/> class.
		/// </summary>
		/// <param name="image">Image for the brush</param>
		/// <param name="opacity">Opacity of the texture to apply to the brush when painting</param>
		public TextureBrush (Image image, float opacity = 1f)
		{
			this.Image = image;
			handler = Platform.Instance.TextureBrushHandler;
			ControlObject = handler.Create (image, opacity);
		}

		/// <summary>
		/// Gets or sets the transform for this brush
		/// </summary>
		/// <value>The transform for the brush</value>
		public IMatrix Transform
		{
			get { return handler.GetTransform (this); }
			set { handler.SetTransform (this, value); }
		}

		/// <summary>
		/// Gets or sets the opacity of the brush texture
		/// </summary>
		/// <value>The opacity for the brush texture</value>
		public float Opacity
		{
			get { return opacity; }
			set
			{
				opacity = value;
				handler.SetOpacity (this, value);
			}
		}

		/// <summary>
		/// Platform handler interface for <see cref="TextureBrush"/>
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Brush.IHandler
		{
			/// <summary>
			/// Gets the transform for the specified brush
			/// </summary>
			/// <returns>The transform for the brush</returns>
			/// <param name="widget">Brush to get the transform</param>
			IMatrix GetTransform (TextureBrush widget);

			/// <summary>
			/// Sets the transform for the specified brush
			/// </summary>
			/// <param name="widget">Brush to set the transform</param>
			/// <param name="transform">Transform to set to the brush</param>
			void SetTransform (TextureBrush widget, IMatrix transform);

			/// <summary>
			/// Sets the opacity of the texture brush
			/// </summary>
			/// <param name="widget">Brush to set the opacity</param>
			/// <param name="opacity">Opacity to set to the brush</param>
			void SetOpacity (TextureBrush widget, float opacity);

			/// <summary>
			/// Creates a brush object with the specified image and opacity
			/// </summary>
			/// <param name="image">Image.</param>
			/// <param name="opacity">Opacity.</param>
			/// <returns>ControlObject for the brush</returns>
			object Create (Image image, float opacity);
		}

	}
}

