using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Wrap mode for a gradient
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum GradientWrapMode
	{
		/// <summary>
		/// The start and end colors fill beyond the gradient
		/// </summary>
		Pad,
		/// <summary>
		/// The gradient repeats to fill the area
		/// </summary>
		Repeat,

		/// <summary>
		/// The gradient reflects (or reverses) each time it repeats to fill the area
		/// </summary>
		Reflect
	}

	/// <summary>
	/// Brush with a linear gradient at an angle
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrush : Brush, ITransformBrush
	{
		readonly IHandler handler;

		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		public override object Handler { get { return handler; } }

		/// <summary>
		/// Gets a delegate to instantiate <see cref="LinearGradientBrush"/> objects
		/// </summary>
		/// <remarks>
		/// Use this to instantiate many objects of this type
		/// </remarks>
		[Obsolete("Since 2.4: Use new LinearGradientBrush() instead")]
		public static Func<Color, Color, PointF, PointF, LinearGradientBrush> Instantiator
		{
			get
			{
				var handler = Platform.Instance.LinearGradientBrushHandler;
				return (startColor, endColor, startPoint, endPoint) =>
				{
					var control = handler.Create(startColor, endColor, startPoint, endPoint);
					return new LinearGradientBrush(handler, control);
				};
			}
		}

		LinearGradientBrush(IHandler handler, object control)
		{
			this.handler = handler;
			ControlObject = control;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.LinearGradientBrush"/> class between two points
		/// </summary>
		/// <param name="startColor">Start color for the gradient</param>
		/// <param name="endColor">End color for the gradient</param>
		/// <param name="startPoint">Start point for the gradient</param>
		/// <param name="endPoint">End point for the gradient</param>
		public LinearGradientBrush(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			handler = Platform.Instance.LinearGradientBrushHandler;
			ControlObject = handler.Create(startColor, endColor, startPoint, endPoint);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.LinearGradientBrush"/> class with a given <paramref name="rectangle"/> and <paramref name="angle"/>
		/// </summary>
		/// <param name="rectangle">Rectangle to define the area of the gradient</param>
		/// <param name="startColor">Start color for the gradient</param>
		/// <param name="endColor">End color for the gradient</param>
		/// <param name="angle">Angle of the gradient, in degrees</param>
		public LinearGradientBrush(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			handler = Platform.Instance.LinearGradientBrushHandler;
			ControlObject = handler.Create(rectangle, startColor, endColor, angle);
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
		/// Handler interface for the <see cref="LinearGradientBrush"/>
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Brush.IHandler
		{
			/// <summary>
			/// Creates a linear gradient brush
			/// </summary>
			/// <param name="startColor">Start color.</param>
			/// <param name="endColor">End color.</param>
			/// <param name="startPoint">Start point.</param>
			/// <param name="endPoint">End point.</param>
			/// <returns>ControlObject for the brush</returns>
			object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint);

			/// <summary>
			/// Create the specified rectangle, startColor, endColor and angle.
			/// </summary>
			/// <param name="rectangle">Rectangle.</param>
			/// <param name="startColor">Start color.</param>
			/// <param name="endColor">End color.</param>
			/// <param name="angle">Angle.</param>
			/// <returns>ControlObject for the brush</returns>
			object Create(RectangleF rectangle, Color startColor, Color endColor, float angle);

			/// <summary>
			/// Gets the transform for the specified brush
			/// </summary>
			/// <returns>The current transform for the specified brush</returns>
			/// <param name="widget">Brush to get the transform</param>
			IMatrix GetTransform(LinearGradientBrush widget);

			/// <summary>
			/// Sets the transform for the specified brush
			/// </summary>
			/// <param name="widget">Brush to set the transform</param>
			/// <param name="transform">Transform to set to the brush</param>
			void SetTransform(LinearGradientBrush widget, IMatrix transform);

			/// <summary>
			/// Gets the gradient wrap mode
			/// </summary>
			/// <returns>The gradient wrap mode for the brush</returns>
			/// <param name="widget">Brush to get the gradient wrap mode</param>
			GradientWrapMode GetGradientWrap(LinearGradientBrush widget);

			/// <summary>
			/// Sets the gradient wrap mode
			/// </summary>
			/// <param name="widget">Brush to set the wrap mode</param>
			/// <param name="gradientWrap">Gradient wrap mode to set</param>
			void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap);
		}

		#endregion
	}
}

