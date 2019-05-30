using System;
using Eto.Drawing;
using s = SharpDX;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class MatrixHandler : Matrix.IHandler
    {
		s.Matrix3x2 Control;

        public MatrixHandler()
        {
            this.Control = s.Matrix3x2.Identity;
        }

        public MatrixHandler(ref s.Matrix3x2 m)
        {
            this.Control = m; // copied the value as Control is a struct
        }

        public float[] Elements
        {
            get
            {
                return new float[] {
                (float)Control.M11, 
                (float)Control.M12, 
                (float)Control.M21, 
                (float)Control.M22, 
                (float)Control.M31, 
                (float)Control.M32};
            }
        }

        public float OffsetX
        {
            get { return (float)Control.M31; }
        }

        public float OffsetY
        {
            get { return (float)Control.M32; }
        }

        public void Rotate(float angle)
        {
			Control = s.Matrix3x2.Multiply(s.Matrix3x2.Rotation(Conversions.DegreesToRadians(angle)), Control); // premultiply
        }

		public void RotateAt(float angle, float centerX, float centerY)
		{
			angle = Conversions.DegreesToRadians(angle);
			var sina = (float)Math.Sin(angle);
			var cosa = (float)Math.Cos(angle);
			var matrix = new s.Matrix3x2(cosa, sina, -sina, cosa, centerX - centerX * cosa + centerY * sina, centerY - centerX * sina - centerY * cosa);
			Control = s.Matrix3x2.Multiply(matrix, Control);
		}

        public void Translate(float x, float y)
        {
			Control = s.Matrix3x2.Multiply(s.Matrix3x2.Translation(x, y), Control); // premultiply
        }

        public void Scale(float sx, float sy)
        {
			Control = s.Matrix3x2.Multiply(s.Matrix3x2.Scaling(sx, sy), Control); // premultiply
        }

		public void Skew(float skewX, float skewY)
		{
			var matrix = new s.Matrix3x2(1, (float)Math.Tan(Conversions.DegreesToRadians(skewX)), (float)Math.Tan(Conversions.DegreesToRadians(skewY)), 1, 0, 0);
			Control = s.Matrix3x2.Multiply(matrix, Control);
		}

		public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
		{
			var matrix = new s.Matrix3x2(scaleX, 0f, 0f, scaleY, centerX - centerX * scaleX, centerY - centerY * scaleY);
			Control = s.Matrix3x2.Multiply(matrix, Control);
		}

		public void Prepend(IMatrix matrix)
		{
			Control = s.Matrix3x2.Multiply(matrix.ToDx(), this.Control);
		}

		public void Append(IMatrix matrix)
		{
			Control = s.Matrix3x2.Multiply(this.Control, matrix.ToDx());
		}

        public void Create(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.Control = new s.Matrix3x2(m11, m12, m21, m22, dx, dy);
        }

		public void Create()
		{
			this.Control = s.Matrix3x2.Identity;
		}

        public void Invert()
        {
			this.Control = s.Matrix3x2.Invert(this.Control);
        }

		public PointF TransformPoint(Point p)
		{
			s.Vector2 v = s.Matrix3x2.TransformPoint(this.Control, new s.Vector2(p.X, p.Y)); // implicit conversion from Vector2 to Vector2
			return v.ToEto();
		}

		public PointF TransformPoint(PointF p)
		{
			s.Vector2 v = s.Matrix3x2.TransformPoint(this.Control, p.ToDx()); // implicit conversion from Vector2 to Vector2
			return v.ToEto();
		}

		public float Xx
		{
			get { return Control.M11; }
			set { Control.M11 = value; }
		}

		public float Xy
		{
			get { return Control.M21; }
			set { Control.M21 = value; }
		}

		public float Yx
		{
			get { return Control.M12; }
			set { Control.M12 = value; }
		}

		public float Yy
		{
			get { return Control.M22; }
			set { Control.M22 = value; }
		}

		public float X0
		{
			get { return Control.M31; }
			set { Control.M31 = value; }
		}

		public float Y0
		{
			get { return Control.M32; }
			set { Control.M32 = value; }
		}

		public object ControlObject
		{
			get { return Control; }
		}

		public IMatrix Clone()
		{
			return new MatrixHandler(ref Control);
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}
