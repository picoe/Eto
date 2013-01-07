using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	/// <summary>
	/// Manages transformations in a retained mode graphics system.
	/// </summary>
	public class TransformStack
	{
		Eto.Generator generator;
		Action<IMatrix> push;
		Action pop;
		Stack<StackEntry> stack;

		/// <summary>
		/// Initializes a new instance of the TransformStack class
		/// </summary>
		/// <param name="generator">Generator for the stack</param>
		/// <param name="push">A callback that should prepend the specified value to the current matrix </param>
		/// <param name="pop">A callback that should either pop the matrix stack or set the current matrix to the specified value</param>
		public TransformStack(Eto.Generator generator, Action<IMatrix> push, Action pop)
		{
			this.generator = generator;
			this.push = push;
			this.pop = pop;
		}

		public void TranslateTransform(float dx, float dy)
		{
			Push(Matrix.FromTranslation(dx, dy));
		}

		public void RotateTransform(float angle)
		{
			Push(Matrix.FromRotation(angle));
		}

		public void ScaleTransform(float sx, float sy)
		{
			Push(Matrix.FromScale(sx, sy));
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			Push(matrix);
		}

		class StackEntry
		{
			public int popCount;
		}

		StackEntry s = null;

		private void Push(IMatrix matrix)
		{
			// If we're in a SaveTransform block,
			// increment the pop count.
			if (s != null)
				s.popCount++;

			// push the transform
			push(matrix);
		}

		public void SaveTransform()
		{
			// Create a stack the first time.
			if (stack == null)
				stack = new Stack<StackEntry>();

			// If there is an existing
			// entry, push it
			if (s != null)
				stack.Push(s);

			// start a new entry
			s = new StackEntry { popCount = 0 };
		}

		public void RestoreTransform()
		{
			// If there is a current entry, use it.
			var t = s;

			if (t == null)
				throw new EtoException("RestoreTransform called without SaveTransform");

			// Pop the drawing context
			// popCount times
			while (t != null && t.popCount-- > 0)
				// return a cloned matrix
				// since the caller may dispose it.
				pop();

			// reset the current entry always
			s = null;

			// otherwise if the stack is nonempty
			// pop the value.
			if (stack != null && stack.Count > 0)
				s = stack.Pop();
		}
	}
}
