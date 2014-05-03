using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines a pen to be used with the <see cref="Graphics"/> drawing methods
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IPen
	{
		/// <summary>
		/// Creates a new pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color for the new pen</param>
		/// <param name="thickness">Thickness of the new pen</param>
		/// <returns>ControlObject for the pen</returns>
		object Create (Color color, float thickness);

		/// <summary>
		/// Gets the color of the pen
		/// </summary>
		/// <param name="widget">Pen to get the color</param>
		Color GetColor (Pen widget);

		/// <summary>
		/// Sets the color of the pen
		/// </summary>
		/// <param name="widget">Pen to set the color</param>
		/// <param name="color">Color of the pen</param>
		void SetColor (Pen widget, Color color);

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

	/// <summary>
	/// Methods to create <see cref="IPen"/> objects for use with drawing methods in <see cref="Graphics"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class Pen : IHandlerSource, IDisposable, IControlObjectSource
	{
		readonly IPen handler;
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
		public static Func<Color, float, Pen> Instantiator
		{
			get
			{
				var sharedHandler = Platform.Instance.CreateShared<IPen>();
				return (color, thickness) =>
				{
					var control = sharedHandler.Create(color, thickness);
					return new Pen(sharedHandler, control);
				};
			}
		}

		Pen (IPen handler, object control)
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
		{
			handler = Platform.Instance.CreateShared<IPen>();
			ControlObject = handler.Create(color, thickness);
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Creates a new pen with the specified <paramref name="color"/> and <paramref name="thickness"/>
		/// </summary>
		/// <param name="color">Color for the new pen</param>
		/// <param name="thickness">Thickness of the new pen</param>
		/// <param name="generator">Generator to create the pen for</param>
		[Obsolete("Use variation without generator instead")]
		public Pen(Color color, float thickness, Generator generator)
		{
			handler = generator.CreateShared<IPen>();
			ControlObject = handler.Create(color, thickness);
		}

		#pragma warning restore 612,618

		/// <summary>
		/// Gets or sets the color of the pen
		/// </summary>
		/// <value>The color of the pen</value>
		public Color Color
		{
			get { return handler.GetColor (this); }
			set { handler.SetColor (this, value); }
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
	}
}

