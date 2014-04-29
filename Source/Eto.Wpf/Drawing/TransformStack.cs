using Eto.Drawing;
using System;
using System.Collections.Generic;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Manages transformations in a retained mode graphics system.
	/// </summary>
	public class TransformStack
	{
		readonly Eto.Platform generator;
		readonly Action<IMatrix> push;
		readonly Action pop;
		Stack<StackEntry> stack;
		StackEntry current;

		/// <summary>
		/// Initializes a new instance of the TransformStack class
		/// </summary>
		/// <param name="generator">Generator for the stack</param>
		/// <param name="push">A callback that should prepend the specified value to the current matrix </param>
		/// <param name="pop">A callback that should either pop the matrix stack or set the current matrix to the specified value</param>
		public TransformStack(Eto.Platform generator, Action<IMatrix> push, Action pop)
		{
			this.generator = generator;
			this.push = push;
			this.pop = pop;
		}

		public void TranslateTransform (float dx, float dy)
		{
			Push (Matrix.FromTranslation (dx, dy, generator));
		}

		public void RotateTransform (float angle)
		{
			Push (Matrix.FromRotation (angle, generator));
		}

		public void ScaleTransform (float sx, float sy)
		{
			Push (Matrix.FromScale (sx, sy, generator));
		}

		public void MultiplyTransform (IMatrix matrix)
		{
			Push (matrix);
		}

		class StackEntry
		{
			readonly List<IMatrix> matrices = new List<IMatrix>();
			public int PopCount { get; set; }
			public List<IMatrix> Matrices { get { return matrices; } }
		}


		public void PushAll ()
		{
			if (stack != null) {
				foreach (var entry in stack) {
					foreach (var matrix in entry.Matrices) {
						push (matrix);
					}
				}
			}
		}

		StackEntry NewEntry ()
		{
			stack = stack ?? new Stack<StackEntry> ();
			var entry = new StackEntry ();
			stack.Push (entry);
			return entry;
		}

		void Push (IMatrix matrix)
		{
			// If we're in a SaveTransform block,
			// increment the pop count.
			current = current ?? NewEntry ();
			current.Matrices.Add (matrix);

			current.PopCount++;
			
			// push the transform
			push (matrix);
		}

		public void SaveTransform ()
		{
			// Create a stack the first time.
			current = NewEntry ();
		}

		public void RestoreTransform ()
		{
			// If there is a current entry, use it.
			if (current == null)
				throw new EtoException ("RestoreTransform called without SaveTransform");

			// Pop the drawing context
			// popCount times
			while (current.PopCount-- > 0) {
				// return a cloned matrix
				// since the caller may dispose it.
				pop ();
			}
			stack.Pop ();
			current = (stack.Count == 0) ? NewEntry () : stack.Peek ();
		}
	}
}
