using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Brush with an elliptical radial gradient from a specified origin.
	/// </summary>
	/// <remarks>
	/// The gradient origin must fall within the ellipse, otherwise the behavior is undefined.
	/// </remarks>
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class RadialGradientBrush : Brush, ITransformBrush
	{
		readonly IHandler handler;

		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		public override object Handler { get { return handler; } }

		/// <summary>
		/// Gets a delegate to instantiate <see cref="RadialGradientBrush"/> objects
		/// </summary>
		/// <remarks>
		/// Use this to instantiate many objects of this type
		/// </remarks>
		[Obsolete("Since 2.4: Use new RadialGradientBrush() instead")]
		public static Func<Color, Color, PointF, PointF, SizeF, RadialGradientBrush> Instantiator
		{
			get
			{
				var handler = Platform.Instance.RadialGradientBrushHandler;
				return (startColor, endColor, center, gradientOrigin, radius) =>
				{
					var control = handler.Create(startColor, endColor, center, gradientOrigin, radius);
					return new RadialGradientBrush(handler, control);
				};
			}
		}

		RadialGradientBrush(IHandler handler, object control)
		{
			this.handler = handler;
			ControlObject = control;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.RadialGradientBrush"/>.
		/// </summary>
		/// <param name="startColor">Start color from the <paramref name="gradientOrigin"/></param>
		/// <param name="endColor">End color at the outer edge of the ellipse</param>
		/// <param name="center">Center of the ellipse</param>
		/// <param name="gradientOrigin">Origin of the gradient.</param>
		/// <param name="radius">Radius of the ellipse.</param>
		public RadialGradientBrush(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			handler = Platform.Instance.RadialGradientBrushHandler;
			ControlObject = handler.Create(startColor, endColor, center, gradientOrigin, radius);
		}

		/// <summary>
		/// Gets or sets the transform to apply to the gradient
		/// </summary>
		/// <value>The transform to apply to the gradient</value>
		public IMatrix Transform
		{
			get { return handler.GetTransform(this); }
			set { handler.SetTransform(this, value); }
		}

		/// <summary>
		/// Gets or sets the wrap mode for the gradient
		/// </summary>
		/// <value>The wrap mode for the gradient</value>
		public GradientWrapMode Wrap
		{
			get { return handler.GetGradientWrap(this); }
			set { handler.SetGradientWrap(this, value); }
		}

#region Handler

		/// <summary>
		/// Handler interface for the <see cref="RadialGradientBrush"/>
		/// </summary>
		public new interface IHandler : Brush.IHandler
		{
			/// <summary>
			/// Creates a new radial gradient brush object.
			/// </summary>
			/// <param name="startColor">Start color from the <paramref name="gradientOrigin"/></param>
			/// <param name="endColor">End color at the outer edge of the ellipse</param>
			/// <param name="center">Center of the ellipse</param>
			/// <param name="gradientOrigin">Origin of the gradient.</param>
			/// <param name="radius">Radius of the ellipse.</param>
			object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius);

			/// <summary>
			/// Gets the transform for the specified brush
			/// </summary>
			/// <returns>The current transform for the specified brush</returns>
			/// <param name="widget">Brush to get the transform</param>
			IMatrix GetTransform(RadialGradientBrush widget);

			/// <summary>
			/// Sets the transform for the specified brush
			/// </summary>
			/// <param name="widget">Brush to set the transform</param>
			/// <param name="transform">Transform to set to the brush</param>
			void SetTransform(RadialGradientBrush widget, IMatrix transform);

			/// <summary>
			/// Gets the gradient wrap mode
			/// </summary>
			/// <returns>The gradient wrap mode for the brush</returns>
			/// <param name="widget">Brush to get the gradient wrap mode</param>
			GradientWrapMode GetGradientWrap(RadialGradientBrush widget);

			/// <summary>
			/// Sets the gradient wrap mode
			/// </summary>
			/// <param name="widget">Brush to set the wrap mode</param>
			/// <param name="gradientWrap">Gradient wrap mode to set</param>
			void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap);
		}

#endregion
	}
}

