using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : Matrix.IHandler
	{
		swm.Matrix control;
		
		public swm.Matrix Control { get { return control; } }
		
		public object ControlObject { get { return control; } }
		
		public MatrixHandler ()
		{
		}
		
		public MatrixHandler (swm.Matrix matrix)
		{
			control = matrix;
		}
		
		public float[] Elements
		{
			get
			{
				return new float[] {
					(float)control.M11,
					(float)control.M12,
					(float)control.M21,
					(float)control.M22,
					(float)control.OffsetX,
					(float)control.OffsetY
				};
			}
		}
		
		public float Xx { get { return (float)control.M11; } set { control.M11 = value; } }
		
		public float Xy { get { return (float)control.M12; } set { control.M12 = value; } }
		
		public float Yx { get { return (float)control.M21; } set { control.M21 = value; } }
		
		public float Yy { get { return (float)control.M22; } set { control.M22 = value; } }
		
		public float X0 { get { return (float)control.OffsetX; } set { control.OffsetX = value; } }
		
		public float Y0 { get { return (float)control.OffsetY; } set { control.OffsetY = value; } }
		
		public void Rotate (float angle)
		{
			control.RotatePrepend (angle);
		}
		
		public void RotateAt (float angle, float centerX, float centerY)
		{
			control.RotateAtPrepend (angle, centerX, centerY);
		}
		
		public void Translate (float x, float y)
		{
			control.TranslatePrepend (x, y);
		}
		
		public void Scale (float scaleX, float scaleY)
		{
			control.ScalePrepend (scaleX, scaleY);
		}
		
		public void ScaleAt (float scaleX, float scaleY, float centerX, float centerY)
		{
			control.ScaleAtPrepend (scaleX, scaleY, centerX, centerY);
		}
		
		public void Skew (float skewX, float skewY)
		{
			control.SkewPrepend (skewX, skewY);
		}
		
		public void Append (IMatrix matrix)
		{
			var m2 = (swm.Matrix)matrix.ControlObject;
			control.Append (m2);
		}
		
		public void Prepend (IMatrix matrix)
		{
			var m2 = (swm.Matrix)matrix.ControlObject;
			control.Prepend (m2);
		}
		
		public void Create ()
		{
			control = swm.Matrix.Identity;
		}
		
		public void Create (float xx, float yx, float xy, float yy, float dx, float dy)
		{
			control = new swm.Matrix (xx, yx, xy, yy, dx, dy);
		}
		
		public void Invert ()
		{
			control.Invert ();
		}
		
		public PointF TransformPoint (Point point)
		{
			return control.Transform (point.ToWpf ()).ToEto ();
		}
		
		public PointF TransformPoint (PointF point)
		{
			return control.Transform (point.ToWpf ()).ToEto ();
		}
		
		public IMatrix Clone ()
		{
			return new MatrixHandler (control);
		}
		
		public void Dispose()
		{
			// nothing to do
		}
	}
}
