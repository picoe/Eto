using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines a brush with a solid color for use with <see cref="Graphics"/> fill operations
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class SolidBrush : Brush
	{
		readonly IHandler handler;

		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		public override object Handler { get { return handler; } }

		/// <summary>
		/// Gets a delegate to instantiate objects of this type with minimal overhead
		/// </summary>
		public static Func<Color, SolidBrush> Instantiator
		{
			get
			{
				var sharedHandler = Platform.Instance.CreateShared<IHandler>();
				return color =>
				{
					var control = sharedHandler.Create(color);
					return new SolidBrush(sharedHandler, control);
				};
			}
		}

		SolidBrush (IHandler handler, object control)
		{
			this.handler = handler;
			this.ControlObject = control;
		}

		/// <summary>
		/// Initializes a new instance of a SolidBrush with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the brush</param>
		public SolidBrush(Color color)
		{
			handler = Platform.Instance.CreateShared <IHandler> ();
			ControlObject = handler.Create (color);
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Initializes a new instance of a SolidBrush with the specified <paramref name="color"/>
		/// </summary>
		/// <param name="color">Color for the brush</param>
		/// <param name="generator">Generator to create the brush for</param>
		[Obsolete("Use variation without generator instead")]
		public SolidBrush(Color color, Generator generator = null)
		{
			handler = generator.CreateShared <IHandler>();
			ControlObject = handler.Create(color);
		}

		#pragma warning restore 612,618

		/// <summary>
		/// Gets or sets the fill color of this brush
		/// </summary>
		public Color Color
		{
			get { return handler.GetColor (this); }
			set { handler.SetColor (this, value); }
		}

		/// <summary>
		/// Platform handler interface for <see cref="SolidBrush"/>
		/// </summary>
		/// <copyright>(c) 2012 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public interface IHandler : Brush.IHandler
		{
			/// <summary>
			/// Gets the current fill color of the specified brush
			/// </summary>
			/// <param name="widget">Widget to get the color for</param>
			/// <returns>Color of the specified brush</returns>
			Color GetColor (SolidBrush widget);

			/// <summary>
			/// Sets the fill color of the specified brush
			/// </summary>
			/// <param name="widget">Widget to set the color for</param>
			/// <param name="color">Color to fill</param>
			void SetColor(SolidBrush widget, Color color);

			/// <summary>
			/// Creates a new solid brush with the specified color
			/// </summary>
			/// <param name="color">Color of the brush</param>
			/// <returns>ControlObject of the brush to store</returns>
			object Create (Color color);
		}
	}
}

