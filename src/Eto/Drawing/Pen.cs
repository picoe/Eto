using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines attributes for line drawing methods in <see cref="Graphics"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class Pen : IHandlerSource, IDisposable, IControlObjectSource
	{
		readonly IHandler handler;
		DashStyle dashStyle;

		/// <summary>
		/// Gets the control object for this widget
		/// </summary>
		/// <value>The control object for the widget</value>
		public object ControlObject { get; set; }

		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		public object Handler { get { return handler; } }

		/// <summary>
		/// Gets a delegate that can be used to create instances of the Pen with low overhead
		/// </summary>
		[Obsolete("Since 2.4, use new Pen() instead.")]
		public static Func<Color, float, Pen> Instantiator
		{
			get
			{
				var sharedHandler = Platform.Instance.CreateShared<IHandler>();
				return (color, thickness) =>
				{
					var control = sharedHandler.Create(new SolidBrush(color), thickness);
					return new Pen(sharedHandler, control);
				};
			}
		}

		Pen (IHandler handler, object control)
		{
			this.handler = handler;
			this.ControlObject = control;
		}

		/// <summary>
		/// Creates a new pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color for the new pen</param>
		/// <param name="thickness">Thickness of the new pen</param>
		public Pen(Color color, float thickness = 1f)
			: this(new SolidBrush(color), thickness)
		{
		}

		/// <summary>
		/// Creates a new pen with the specified <paramref name="brush"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="brush">Brush to stroke the pen with.</param>
		/// <param name="thickness">Thickness of the new pen</param>
		public Pen(Brush brush, float thickness = 1f)
		{
			handler = Platform.Instance.PenHandler;
			ControlObject = handler.Create(brush, thickness);
		}

		/// <summary>
		/// Gets or sets the color of the pen
		/// </summary>
		/// <value>The color of the pen</value>
		[Obsolete("Since 2.3, Use Brush property instead")]
		public Color Color
		{
			get { return (Brush as SolidBrush)?.Color ?? Colors.Transparent; }
			set
			{
				var brush = Brush as SolidBrush;
				if (brush != null)
					brush.Color = value;
			}
		}

		/// <summary>
		/// Gets the brush associated with the pen.
		/// </summary>
		/// <value>The brush the pen will use to stroke the path.</value>
		public Brush Brush
		{
			get { return handler.GetBrush(this); }
		}

		/// <summary>
		/// Gets or sets the thickness (width) of the pen
		/// </summary>
		public float Thickness
		{
			get { return handler.GetThickness (this); }
			set { handler.SetThickness (this, value); }
		}

		/// <summary>
		/// Gets or sets the line join style of the pen
		/// </summary>
		/// <value>The line join style</value>
		public PenLineJoin LineJoin
		{
			get { return handler.GetLineJoin (this); }
			set { handler.SetLineJoin (this, value); }
		}

		/// <summary>
		/// Gets or sets the line cap style of the pen
		/// </summary>
		/// <value>The line cap style</value>
		public PenLineCap LineCap
		{
			get { return handler.GetLineCap (this); }
			set { handler.SetLineCap (this, value); }
		}

		/// <summary>
		/// Gets or sets the miter limit on the ratio of the length vs. the <see cref="Thickness"/> of this pen
		/// </summary>
		/// <remarks>
		/// This is only used if the <see cref="LineJoin"/> style is <see cref="PenLineJoin.Miter"/>
		/// </remarks>
		/// <value>The miter limit of this pen</value>
		public float MiterLimit
		{
			get { return handler.GetMiterLimit (this); }
			set { handler.SetMiterLimit (this, value); }
		}

		/// <summary>
		/// Gets or sets the dash style of the pen
		/// </summary>
		/// <remarks>
		/// The <see cref="LineCap"/> specifies the dash cap used
		/// </remarks>
		/// <value>The dash style.</value>
		public DashStyle DashStyle
		{
			get { return dashStyle; }
			set
			{
				dashStyle = value;
				handler.SetDashStyle (this, value);
			}
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Eto.Drawing.Pen"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Eto.Drawing.Pen"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Eto.Drawing.Pen"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Eto.Drawing.Pen"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Eto.Drawing.Pen"/> was occupying.</remarks>
		public void Dispose ()
		{
			var controlDispose = ControlObject as IDisposable;
			if (controlDispose != null) {
				controlDispose.Dispose ();
			}
		}

		#region Handler

		/// <summary>
		/// Defines a pen to be used with the <see cref="Graphics"/> drawing methods
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public interface IHandler
		{
			/// <summary>
			/// Creates a new pen with the specified <paramref name="brush"/> and <paramref name="thickness"/>
			/// </summary>
			/// <param name="brush">Brush for the new pen</param>
			/// <param name="thickness">Thickness of the new pen</param>
			/// <returns>ControlObject for the pen</returns>
			object Create(Brush brush, float thickness);

			/// <summary>
			/// Gets the brush of the pen
			/// </summary>
			/// <param name="widget">Pen to get the brush</param>
			Brush GetBrush (Pen widget);

			/// <summary>
			/// Gets or sets the thickness (width) of the pen
			/// </summary>
			/// <param name="widget">Pen to get the thickness</param>
			float GetThickness (Pen widget);

			/// <summary>
			/// Sets the thickness (width) of the pen
			/// </summary>
			/// <param name="widget">Pen to set the thickness</param>
			/// <param name="thickness">Thickness for the pen</param>
			void SetThickness (Pen widget, float thickness);

			/// <summary>
			/// Gets the style of line join for the pen
			/// </summary>
			/// <param name="widget">Pen to get the line join style</param>
			PenLineJoin GetLineJoin (Pen widget);

			/// <summary>
			/// Sets the style of line join for the pen
			/// </summary>
			/// <param name="widget">Pen to set the line join style</param>
			/// <param name="lineJoin">Line join to set</param>
			void SetLineJoin (Pen widget, PenLineJoin lineJoin);

			/// <summary>
			/// Gets the line cap style
			/// </summary>
			/// <param name="widget">Pen to get the line cap style</param>
			PenLineCap GetLineCap (Pen widget);

			/// <summary>
			/// Sets the line cap style
			/// </summary>
			/// <param name="widget">Pen to set the line cap</param>
			/// <param name="lineCap">Line cap to set</param>
			void SetLineCap (Pen widget, PenLineCap lineCap);

			/// <summary>
			/// Gets the miter limit for the pen
			/// </summary>
			/// <remarks>
			/// The miter limit specifies the maximum allowed ratio of miter lenth to stroke length in which a 
			/// miter will be converted to a bevel.  The default miter limit is 10.
			/// </remarks>
			/// <param name="widget">Pen to get the miter limit</param>
			float GetMiterLimit (Pen widget);

			/// <summary>
			/// Sets the miter limit of the pen
			/// </summary>
			/// <remarks>
			/// The miter limit specifies the maximum allowed ratio of miter lenth to stroke length in which a 
			/// miter will be converted to a bevel.  The default miter limit is 10.
			/// </remarks>
			/// <param name="widget">Pen to set the miter limit</param>
			/// <param name="miterLimit">Miter limit to set to the pen</param>
			void SetMiterLimit (Pen widget, float miterLimit);

			/// <summary>
			/// Sets the dash style of the pen
			/// </summary>
			/// <param name="widget">Pen to set the dash style</param>
			/// <param name="dashStyle">Dash style to set to the pen</param>
			void SetDashStyle (Pen widget, DashStyle dashStyle);
		}

		#endregion
	}
}

