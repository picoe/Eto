using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of possible gesture types to capture
	/// </summary>
	[Flags]
	public enum GesturesOfInterest
	{
		/// <summary>
		/// No gesture 
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Swipe gesture 
		/// </summary>
		Swipe = 0x01,
		/// <summary>
		/// long press gesture
		/// </summary>
		LongPress = 0x02,
		/// <summary>
		/// horizontal pan
		/// </summary>
		PanH = 0x04,
		/// <summary>
		/// vertical pan
		/// </summary>
		PanV = 0x08,
		/// <summary>
		/// rotate
		/// </summary>
		Rotate = 0x10,
		/// <summary>
		/// zoom/expand
		/// </summary>
		Zoom = 0x20,
		/// <summary>
		/// drag is an XY pan
		/// </summary>
		Drag = 0x40
	}


	/// <summary>
	/// touch swipe gesture event arguments.
	/// </summary>
	public class SwipeGestureEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.SwipeGestureEventArgs"/> class.
		/// </summary>
		/// <param name="VelocityX">Velocity x.</param>
		/// <param name="VelocityY">Velocity y.</param>
		public SwipeGestureEventArgs(double VelocityX, double VelocityY)
		{
			this.VelocityX = VelocityX;
			this.VelocityY = VelocityY;
		}

		/// <summary>
		/// Gets the location of the mouse relative to the control that raised the event.
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double VelocityX { get; private set; }
		/// <summary>
		/// Gets the location of the mouse relative to the control that raised the event.
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double VelocityY { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}


	/// <summary>
	/// Enumeration of pan direction buttons
	/// </summary>
	public enum PanDirection
	{		
		/// <summary>
		/// pan gesture was in the up direction
		/// </summary>
		Up,
		/// <summary>
		/// pan gesture was in the down direction
		/// </summary>
		Down,
		/// <summary>
		/// pan gesture was in the left direction
		/// </summary>
		Left,
		/// <summary>
		/// The right.
		/// </summary>
		Right
	}
	
	/// <summary>
	/// touch pan gesture event arguments.
	/// </summary>
	public class PanGestureEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PanGestureEventArgs"/> class.
		/// </summary>
		/// <param name="Pressed">indicates the active phase of the gesture.</param>
		/// <param name="NPress">number of fingers used.</param>
		/// <param name="Direction">Direction.</param>
		/// <param name="Offset">Offset.</param>
		public PanGestureEventArgs(bool Pressed, int NPress, PanDirection Direction, double Offset)
		{
			this.Pressed = Pressed;
			this.NPress = NPress;
			this.Direction = Direction;
			this.Offset = Offset;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public bool Pressed { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public int NPress { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <value>The direction of the pan gesture.</value>
		public PanDirection Direction { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The offset of the pan.</value>
		public double Offset { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}

	/// <summary>
	/// touch long press gesture event arguments.
	/// </summary>
	public class LongPressGestureEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.LongPressGestureEventArgs"/> class.
		/// </summary>
		/// <param name="Pressed">If set to <c>true</c> pressed.</param>
		/// <param name="NPress">N press.</param>
		/// <param name="X">X.</param>
		/// <param name="Y">Y.</param>
		public LongPressGestureEventArgs(bool Pressed, int NPress, double X, double Y)
		{
			this.Pressed = Pressed;
			this.NPress = NPress;
			this.X = X;
			this.Y = Y;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public bool Pressed { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public int NPress { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double X { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double Y { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}


	/// <summary>
	/// touch rotate gesture event arguments.
	/// </summary>
	public class RotateGestureEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RotateGestureEventArgs"/> class.
		/// </summary>
		/// <param name="AngleDelta">Angle delta.</param>
		public RotateGestureEventArgs(double AngleDelta)
		{
			this.AngleDelta = AngleDelta;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double AngleDelta { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}

	/// <summary>
	/// touch rotate gesture event arguments.
	/// </summary>
	public class ZoomGestureEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ZoomGestureEventArgs"/> class.
		/// </summary>
		/// <param name="ScaleDelta">Scale delta.</param>
		public ZoomGestureEventArgs(double ScaleDelta)
		{
			this.ScaleDelta = ScaleDelta;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public double ScaleDelta { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}
	
	/// <summary>
	/// touch pan gesture event arguments.
	/// </summary>
	public class DragGestureEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DragGestureEventArgs"/> class.
		/// </summary>
		/// <param name="Pressed">Are we touching...</param>
		/// <param name="NPress">how many fingers do we have.</param>
		/// <param name="startX">Starting X position.</param>
		/// <param name="startY">Starting Y position.</param>
		/// <param name="deltaX">Offset in the X direction.</param>
		/// <param name="deltaY">Offset int the Y direction.</param>
		public DragGestureEventArgs(bool Pressed, int NPress, double startX, double startY, double deltaX, double deltaY)
		{
			this.Pressed = Pressed;
			this.NPress = NPress;
			
			this.startX = startX;
			this.startY = startY;
			this.deltaX = deltaX;
			this.deltaY = deltaY;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public bool Pressed { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public int NPress { get; private set; }
		
		/// <summary>
		/// starting X position of the gesture 
		/// </summary>
		public double startX { get; private set; }
		/// <summary>
		///  starting Y position of the gesture 
		/// </summary>
		public double startY { get; private set; }
		
		/// <summary>
		/// change in X position during the gesture
		/// </summary>
		public double deltaX { get; private set; }
		/// <summary>
		/// change in Y position during the gesture
		/// </summary>
		public double deltaY { get; private set; }


		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }
	}


}
