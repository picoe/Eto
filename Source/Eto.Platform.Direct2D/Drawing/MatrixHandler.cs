using Eto.Drawing;
using s = SharpDX;

namespace Eto.Platform.Direct2D.Drawing
{
    public class MatrixHandler : IMatrixHandler
    {
		s.Matrix3x2 Control;

        public MatrixHandler()
        {
            this.Control = s.Matrix3x2.Identity;
        }

        public MatrixHandler(s.Matrix3x2 m)
        {
            this.Control = m;
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
            Control = 
                s.Matrix3x2.Multiply(
                    s.Matrix3x2.Rotation(angle), // premultiply
                    Control);
        }

        public void Translate(float x, float y)
        {
            var c = Control;
            c.TranslationVector = new s.Vector2(x, y);
            Control = c;
        }

        public void Scale(float sx, float sy)
        {            
            var c = Control;
            c.ScaleVector = new s.Vector2(sx, sy);
            Control = c;
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
            this.Control =
                new s.Matrix3x2(m11, m12, m21, m22, dx, dy);
        }

        public void Invert()
        {
            var m1 = s.Matrix3x2.Identity;
            var m2 = this.Control;
            s.Matrix3x2 result;
            s.Matrix3x2.Divide(
                ref m1, 
                ref m2,
                out result);
            this.Control = result;
        }

        public PointF TransformPoint(Point p)
        {
            s.DrawingPointF v =
                s.Matrix3x2.TransformPoint(
                    this.Control,
                    p.ToWpf()); // implicit conversion from Vector2 to DrawingPointF

            return v.ToEto();
        }

        public PointF TransformPoint(PointF p)
        {
            s.DrawingPointF v =
                s.Matrix3x2.TransformPoint(
                    this.Control,
                    p.ToWpf()); // implicit conversion from Vector2 to DrawingPointF

            return v.ToEto();
        }

		public void Create()
		{
			throw new System.NotImplementedException();
		}


		public float Xx
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public float Xy
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public float Yx
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public float Yy
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public float X0
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public float Y0
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public void RotateAt(float angle, float centerX, float centerY)
		{
			throw new System.NotImplementedException();
		}

		public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
		{
			throw new System.NotImplementedException();
		}

		public void Skew(float skewX, float skewY)
		{
			throw new System.NotImplementedException();
		}

		public object ControlObject
		{
			get { throw new System.NotImplementedException(); }
		}

		public IMatrix Clone()
		{
			throw new System.NotImplementedException();
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}
