using System;
using System.Globalization;
using System.Linq;

namespace Eto.Drawing
{
	/// <summary>
	/// Defines a matrix used for transforms in <see cref="Graphics"/> and <see cref="Brushes"/>
	/// </summary>
	/// <remarks>
	/// A matrix is defined by six elements that are used to transform a coordinate system. The elements
	/// of the matrix are defined as:
	/// <para>
	/// 	| xx yx 0 |
	/// 	| xy yy 0 |
	/// 	| x0 y0 1 |
	/// </para>
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IMatrix : IControlObjectSource, IDisposable
	{
		/// <summary>
		/// Gets the elements of this matrix
		/// </summary>
		/// <value>The elements of the matrix</value>
		float[] Elements { get; }

		/// <summary>
		/// Gets or sets the value at position [1,1] in the matrix
		/// </summary>
		float Xx { get; set; }

		/// <summary>
		/// Gets or sets the value at position [1,2] in the matrix
		/// </summary>
		float Yx { get; set; }

		/// <summary>
		/// Gets or sets the value at position [2,1] in the matrix
		/// </summary>
		float Xy { get; set; }

		/// <summary>
		/// Gets or sets the value at position [2,2] in the matrix
		/// </summary>
		float Yy { get; set; }

		/// <summary>
		/// Gets or sets the value at position [3,1] in the matrix
		/// </summary>
		float X0 { get; set; }

		/// <summary>
		/// Gets or sets the value at position [3,2] in the matrix
		/// </summary>
		float Y0 { get; set; }

		/// <summary>
		/// Prepend a rotation to the matrix around the origin (0,0)
		/// </summary>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		void Rotate(float angle);

		/// <summary>
		/// Prepend a rotation around the specified point to the matrix
		/// </summary>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		/// <param name="centerX">X co-ordinate of the point to rotate around</param>
		/// <param name="centerY">Y co-ordinate of the point to rotate around</param>
		void RotateAt(float angle, float centerX, float centerY);

		/// <summary>
		/// Prepend a translation to the matrix
		/// </summary>
		/// <param name="offsetX">The amount to offset along the x axis</param>
		/// <param name="offsetY">The amount to offset along the y axis</param>
		void Translate(float offsetX, float offsetY);

		/// <summary>
		/// Prepend a scale to the matrix from the origin (0, 0)
		/// </summary>
		/// <param name="scaleX">The amount to multiply coordinates along the x axis</param>
		/// <param name="scaleY">The amount to multiply coordinates along the y axis</param>
		void Scale(float scaleX, float scaleY);

		/// <summary>
		/// Prepend a scale to the matrix from the specified point
		/// </summary>
		/// <param name="scaleX">The amount to multiply coordinates along the x axis</param>
		/// <param name="scaleY">The amount to multiply coordinates along the y axis</param>
		/// <param name="centerX">X co-ordinate of the point to scale from</param>
		/// <param name="centerY">Y co-ordinate of the point to scale from</param>
		void ScaleAt(float scaleX, float scaleY, float centerX, float centerY);

		/// <summary>
		/// Prepend a skew to the matrix
		/// </summary>
		/// <param name="skewX">Amount to skew along the X axis, 1.0 does not skew</param>
		/// <param name="skewY">Amount to skew along the Y axis, 1.0 does not skew</param>
		void Skew(float skewX, float skewY);

		/// <summary>
		/// Append the specified <paramref name="matrix"/> to this matrix
		/// </summary>
		/// <param name="matrix">Matrix to append to this matrix</param>
		void Append(IMatrix matrix);

		/// <summary>
		/// Prepend the specified matrix to this matrix
		/// </summary>
		/// <param name="matrix">Matrix to prepend to this matrix</param>
		void Prepend(IMatrix matrix);

		/// <summary>
		/// Inverts this matrix
		/// </summary>
		void Invert();

		/// <summary>
		/// Transforms the specified point using this matrix transform
		/// </summary>
		/// <returns>The value of the point transformed by this matrix</returns>
		/// <param name="point">Point to transform</param>
		PointF TransformPoint(Point point);

		/// <summary>
		/// Transforms the specified point using this matrix transform
		/// </summary>
		/// <returns>The value of the point transformed by this matrix</returns>
		/// <param name="point">Point to transform</param>
		PointF TransformPoint(PointF point);

		/// <summary>
		/// Clone this instance
		/// </summary>
		IMatrix Clone();
	}

	/// <summary>
	/// Methods to create and manage an <see cref="IMatrix"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class Matrix
	{
		/// <summary>
		/// Multiply the specified <paramref name="matrix"/> and <paramref name="matrices"/>.
		/// </summary>
		/// <returns>A new matrix with the product of multiplying each of the specified matrix and matrices</returns>
		/// <param name="matrix">Matrix to multiply with</param>
		/// <param name="matrices">Matrices to append</param>
		public static IMatrix Multiply(IMatrix matrix, params IMatrix[] matrices)
		{
			matrix = matrix.Clone();
			for (int i = 0; i < matrices.Length; i++)
				matrix.Append(matrices[i]);
			return matrix;
		}

		/// <summary>
		/// Creates a new matrix with the specified <paramref name="scale"/>
		/// </summary>
		/// <returns>A new matrix with a scale transform</returns>
		/// <param name="scale">Scale size for the X and Y coordinates</param>
		public static IMatrix FromScale(SizeF scale)
		{
			return FromScale(scale.Width, scale.Height);
		}

		/// <summary>
		/// Creates a new matrix with the specified scale factor
		/// </summary>
		/// <returns>A new matrix with a scale transform</returns>
		/// <param name="scaleX">The amount to multiply coordinates along the x axis</param>
		/// <param name="scaleY">The amount to multiply coordinates along the y axis</param>
		public static IMatrix FromScale(float scaleX, float scaleY)
		{
			return Matrix.Create(scaleX, 0, 0, scaleY, 0, 0);
		}

		/// <summary>
		/// Creates a new matrix with a <paramref name="scale"/> at the specified <paramref name="center"/> point
		/// </summary>
		/// <returns>A new matrix with a scale transform</returns>
		/// <param name="scale">The amount to multiply coordinates by</param>
		/// <param name="center">Point to scale from</param>
		public static IMatrix FromScaleAt(SizeF scale, PointF center)
		{
			return FromScaleAt(scale.Width, scale.Height, center.X, center.Y);
		}

		/// <summary>
		/// Creates a new matrix with a scale at the specified point
		/// </summary>
		/// <returns>A new matrix with a scale transform</returns>
		/// <param name="scaleX">The amount to multiply coordinates along the x axis</param>
		/// <param name="scaleY">The amount to multiply coordinates along the y axis</param>
		/// <param name="centerX">X co-ordinate of the point to scale from</param>
		/// <param name="centerY">Y co-ordinate of the point to scale from</param>
		public static IMatrix FromScaleAt(float scaleX, float scaleY, float centerX, float centerY)
		{
			var matrix = Matrix.Create();
			matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
			return matrix;
		}

		/// <summary>
		/// Creates a new matrix with a translation
		/// </summary>
		/// <returns>A new translation matrix</returns>
		/// <param name="offset">Offset to translate by</param>
		public static IMatrix FromTranslation(SizeF offset)
		{
			return FromTranslation(offset.Width, offset.Height);
		}

		/// <summary>
		/// Creates a new matrix with a translation
		/// </summary>
		/// <returns>A new translation matrix</returns>
		/// <param name="offset">Offset to translate by</param>
		public static IMatrix FromTranslation(PointF offset)
		{
			return FromTranslation(offset.X, offset.Y);
		}

		/// <summary>
		/// Creates a new matrix with a translation
		/// </summary>
		/// <returns>A new translation matrix</returns>
		/// <param name="distanceX">Distance to translate along the x axis</param>
		/// <param name="distanceY">Distance to translate along the y axis</param>
		public static IMatrix FromTranslation(float distanceX, float distanceY)
		{
			var matrix = Matrix.Create();
			matrix.Translate(distanceX, distanceY);
			return matrix;
		}

		/// <summary>
		/// Creates a new rotation matrix
		/// </summary>
		/// <returns>A new rotation matrix</returns>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		public static IMatrix FromRotation(float angle)
		{
			var matrix = Matrix.Create();
			matrix.Rotate(angle);
			return matrix;
		}

		/// <summary>
		/// Creates a new rotation matrix around a center point with the specified <paramref name="angle"/>
		/// </summary>
		/// <returns>A new rotation matrix</returns>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		/// <param name="center">the point to rotate around</param>
		public static IMatrix FromRotationAt(float angle, PointF center)
		{
			return FromRotationAt(angle, center.X, center.Y);
		}

		/// <summary>
		/// Creates a new rotation matrix around a (<paramref name="centerX"/>, <paramref name="centerY"/>) point with the specified <paramref name="angle"/>
		/// </summary>
		/// <returns>A new rotation matrix</returns>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		/// <param name="centerX">X co-ordinate of the point to rotate around</param>
		/// <param name="centerY">Y co-ordinate of the point to rotate around</param>
		public static IMatrix FromRotationAt(float angle, float centerX, float centerY)
		{
			var matrix = Matrix.Create();
			matrix.RotateAt(angle, centerX, centerY);
			return matrix;
		}

		/// <summary>
		/// Creates a new matrix with a skew
		/// </summary>
		/// <returns>A new skew matrix</returns>
		/// <param name="skewX">Amount to skew along the X axis, 1.0 does not skew</param>
		/// <param name="skewY">Amount to skew along the Y axis, 1.0 does not skew</param>
		public static IMatrix FromSkew(float skewX, float skewY)
		{
			var matrix = Matrix.Create();
			matrix.Skew(skewX, skewY);
			return matrix;
		}

		/// <summary>
		/// Gets a delegate that can be used to create an identity matrix with little overhead
		/// </summary>
		[Obsolete("Since 2.4: Use Matrix.Create() instead")]
		public static Func<IMatrix> Instantiator
		{
			get
			{
				var activator = Platform.Instance.Find<IHandler>();
				return () =>
				{
					var matrix = activator();
					matrix.Create();
					return matrix;
				};
			}
		}

		/// <summary>
		/// Gets a delegate that can be used to create instances of a matrix with specified components with little overhead
		/// </summary>
		/// <returns>The with elements.</returns>
		[Obsolete("Since 2.4: Use Matrix.Create() instead")]
		public static Func<float, float, float, float, float, float, IMatrix> InstantiatorWithElements
		{
			get
			{
				var activator = Platform.Instance.Find<IHandler>();
				return (xx, yx, xy, yy, x0, y0) =>
				{
					var matrix = activator();
					matrix.Create(xx, yx, xy, yy, x0, y0);
					return matrix;
				};
			}
		}

		/// <summary>
		/// Creates a new identity matrix
		/// </summary>
		public static IMatrix Create()
		{
			var handler = Platform.Instance.CreateMatrix();
			handler.Create();
			return handler;
		}

		/// <summary>
		/// Creates a new matrix with the specified <paramref name="elements"/>
		/// </summary>
		/// <param name="elements">Elements of the matrix (six components)</param>
		public static IMatrix Create(float[] elements)
		{
			if (elements == null)
				throw new ArgumentNullException("elements");
			if (elements.Length != 6)
				throw new ArgumentOutOfRangeException("elements", elements, string.Format(CultureInfo.CurrentCulture, "Elements must be an array with a length of 6"));
			var handler = Platform.Instance.CreateMatrix();
			handler.Create(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
			return handler;
		}

		/// <summary>
		/// Creates a new matrix with the specified components
		/// </summary>
		/// <param name="xx">Xx component of the matrix</param>
		/// <param name="yx">Yx component of the matrix</param>
		/// <param name="xy">Xy component of the matrix</param>
		/// <param name="yy">Yy component of the matrix</param>
		/// <param name="x0">X0 component of the matrix</param>
		/// <param name="y0">Y0 component of the matrix</param>
		public static IMatrix Create(float xx, float yx, float xy, float yy, float x0, float y0)
		{
			var handler = Platform.Instance.CreateMatrix();
			handler.Create(xx, yx, xy, yy, x0, y0);
			return handler;
		}

		/// <summary>
		/// Prepend a rotation around the specified point to the matrix
		/// </summary>
		/// <param name="matrix">Matrix to rotate</param>
		/// <param name="angle">Angle in degrees to rotate. A positive value indicates a clockwise rotation, whereas a negative value will rotate counter clockwise</param>
		/// <param name="center">Point to rotate around</param>
		public static void RotateAt(this IMatrix matrix, float angle, PointF center)
		{
			matrix.RotateAt(angle, center.X, center.Y);
		}

		/// <summary>
		/// Prepend a translation to the matrix
		/// </summary>
		/// <param name="matrix">Matrix to translate</param>
		/// <param name="offset">The amount to offset</param>
		public static void Translate(this IMatrix matrix, SizeF offset)
		{
			matrix.Translate(offset.Width, offset.Height);
		}

		/// <summary>
		/// Prepend a translation to the matrix
		/// </summary>
		/// <param name="matrix">Matrix to translate</param>
		/// <param name="offset">The amount to offset</param>
		public static void Translate(this IMatrix matrix, PointF offset)
		{
			matrix.Translate(offset.X, offset.Y);
		}

		/// <summary>
		/// Prepend a scale to the matrix from the origin (0, 0)
		/// </summary>
		/// <param name="matrix">Matrix to scale</param>
		/// <param name="scale">The amount to multiply coordinates</param>
		public static void Scale(this IMatrix matrix, SizeF scale)
		{
			matrix.Scale(scale.Width, scale.Height);
		}

		/// <summary>
		/// Prepend a scale to the matrix from the origin (0, 0)
		/// </summary>
		/// <param name="matrix">Matrix to scale</param>
		/// <param name="scale">The amount to multiply coordinates along both the x and y axis</param>
		public static void Scale(this IMatrix matrix, float scale)
		{
			matrix.Scale(scale, scale);
		}

		/// <summary>
		/// Prepend a scale to the matrix from the specified point
		/// </summary>
		/// <param name="matrix">Matrix to scale</param>
		/// <param name="scale">The amount to multiply coordinates</param>
		/// <param name="center">Point to scale from</param>
		public static void ScaleAt(this IMatrix matrix, SizeF scale, PointF center)
		{
			matrix.ScaleAt(scale.Width, scale.Height, center.X, center.Y);
		}

		/// <summary>
		/// Prepend a scale to the matrix from the specified point
		/// </summary>
		/// <param name="matrix">Matrix to scale</param>
		/// <param name="scale">The amount to multiply coordinates along both the x and y axis</param>
		/// <param name="center">Point to scale from</param>
		public static void ScaleAt(this IMatrix matrix, float scale, PointF center)
		{
			matrix.ScaleAt(scale, scale, center.X, center.Y);
		}

		/// <summary>
		/// Prepend a scale to the matrix from the specified point
		/// </summary>
		/// <param name="matrix">Matrix to scale</param>
		/// <param name="scale">The amount to multiply coordinates along both the x and y axis</param>
		/// <param name="centerX">X co-ordinate of the point to scale from</param>
		/// <param name="centerY">Y co-ordinate of the point to scale from</param>
		public static void ScaleAt(this IMatrix matrix, float scale, float centerX, float centerY)
		{
			matrix.ScaleAt(scale, scale, centerX, centerY);
		}

		/// <summary>
		/// Append the specified <paramref name="matrices"/> to the <paramref name="matrix"/>
		/// </summary>
		/// <param name="matrix">Matrix to append to</param>
		/// <param name="matrices">Matrices to append to the matrix</param>
		public static void Append(this IMatrix matrix, params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				matrix.Append(matrices[i]);
		}

		/// <summary>
		/// Prepends the specified <paramref name="matrices"/> to the <paramref name="matrix"/>
		/// </summary>
		/// <param name="matrix">Matrix to prepend to</param>
		/// <param name="matrices">Matrices to prepend to the matrix</param>
		public static void Prepend(this IMatrix matrix, params IMatrix[] matrices)
		{
			for (int i = 0; i < matrices.Length; i++)
				matrix.Prepend(matrices[i]);
		}

		/// <summary>
		/// Returns the scale coordinates Xx and Yy of the <paramref name="matrix"/> as a PointF.
		/// </summary>
		public static PointF GetScale(this IMatrix matrix)
		{
			return new PointF(matrix.Xx, matrix.Yy);
		}

		/// <summary>
		/// Transforms the rectangle with the current matrix.
		/// </summary>
		/// <remarks>
		/// This returns a rectangle that encompasses the specified <paramref name="rect"/> after it is translated.
		/// When rotating, this means that the new rectangle may be larger in size to encompass the translated rectangle.
		/// </remarks>
		/// <returns>A new rectangle that encompasses the translated <paramref name="rect"/>.</returns>
		/// <param name="matrix">Matrix to transform each point of the rectangle.</param>
		/// <param name="rect">Rectangle to transform.</param>
		public static RectangleF TransformRectangle(this IMatrix matrix, RectangleF rect)
		{
			var points = new[]
			{
				matrix.TransformPoint(rect.TopLeft),
				matrix.TransformPoint(rect.TopRight),
				matrix.TransformPoint(rect.BottomLeft),
				matrix.TransformPoint(rect.BottomRight)
			};
			return RectangleF.FromSides(points.Min(r => r.X), points.Min(r => r.Y), points.Max(r => r.X), points.Max(r => r.Y));
		}

		/// <summary>
		/// Transforms the size with the current matrix.
		/// </summary>
		/// <remarks>
		/// This will get the transformed size
		/// </remarks>
		/// <param name="matrix"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static SizeF TransformSize(this IMatrix matrix, SizeF size)
		{
			return TransformRectangle(matrix, new RectangleF(size)).Size;
		}

		/// <summary>
		/// Creates an inverted copy of the specified matrix.
		/// </summary>
		/// <param name="matrix">Matrix to invert.</param>
		public static IMatrix Inverse(this IMatrix matrix)
		{
			matrix = matrix.Clone();
			matrix.Invert();
			return matrix;
		}

		/// <summary>
		/// Handler interface for the <see cref="IMatrix"/>
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public interface IHandler : IMatrix
		{
			/// <summary>
			/// Creates a new identiy matrix
			/// </summary>
			/// <remarks>
			/// An identity matrix is defined as:
			/// <para>
			/// 	| 1  0  0 |
			/// 	| 0  1  0 |
			/// 	| 0  0  1 |
			/// </para>
			/// </remarks>
			void Create();

			/// <summary>
			/// Creates a new matrix with the specified components
			/// </summary>
			/// <remarks>
			/// The components of the matrix are defined as:
			/// <para>
			/// 	| xx xy 0 |
			/// 	| yx yy 0 |
			/// 	| x0 y0 1 |
			/// </para>
			/// </remarks>
			/// <param name="xx">Xx component of the matrix (scaleX)</param>
			/// <param name="yx">Yx component of the matrix</param>
			/// <param name="xy">Xy component of the matrix</param>
			/// <param name="yy">Yy component of the matrix (scaleY)</param>
			/// <param name="x0">X0 component of the matrix (translateX)</param>
			/// <param name="y0">Y0 component of the matrix (translateY)</param>
			void Create(float xx, float yx, float xy, float yy, float x0, float y0);
		}

	}
}
