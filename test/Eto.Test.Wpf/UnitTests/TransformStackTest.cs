using Eto.Wpf.Drawing;
using Eto.Test.UnitTests.Drawing;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
	/// <summary>
	/// Unit tests for TransformStack.
	/// </summary>	
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
    [TestFixture]
    public class TransformStackTest
    {       
        [Test]
        public void TransformStack_TranlateSaveRestore_Verify()
        {
			using (var context = new Eto.Wpf.Platform().Context)
			{
				var current = Matrix.Create();
				var stack = new Stack<IMatrix>();
				Action<IMatrix> push = m => {
					stack.Push(current);
					m.Append(current);
					current = m;
				};
				Action pop = () => current = stack.Pop();

				var target = new TransformStack(push, pop);

				Assert.That(MatrixTests.Equals(current, 1f, 0f, 0f, 1f, 0f, 0f), Is.True);

				target.SaveTransform(); // Save

				target.TranslateTransform(5f, 5f);
				Assert.That(MatrixTests.Equals(current, 1f, 0f, 0f, 1f, 5f, 5f), Is.True);

				target.SaveTransform();

				target.TranslateTransform(10f, 10f);
				Assert.That(MatrixTests.Equals(current, 1f, 0f, 0f, 1f, 15f, 15f), Is.True);

				target.RestoreTransform();
				Assert.That(MatrixTests.Equals(current, 1f, 0f, 0f, 1f, 5f, 5f), Is.True);

				target.RestoreTransform();
				Assert.That(MatrixTests.Equals(current, 1f, 0f, 0f, 1f, 0f, 0f), Is.True);
			}
        }
    }
}
