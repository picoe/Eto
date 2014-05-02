using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Eto.Test.UnitTests
{
	public static class FormTester
	{
		/// <summary>
		/// Default timeout for form operations
		/// </summary>
		public const int DefaultTimeout = 4000;

		/// <summary>
		/// Run a test on by invoking the test on the application
		/// </summary>
		/// <param name="test">Delegate to execute within the invoke</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Run(Action<Application, Action> test, int timeout = DefaultTimeout)
		{
			var ev = new ManualResetEvent(false);
			var app = Application.Instance;
			Exception exception = null;
			Action finished = () => ev.Set();
			app.AsyncInvoke(() =>
			{
				try
				{
					test(app, finished);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
			});
			if (!ev.WaitOne(timeout))
				Assert.Fail("Test did not complete in time");
			if (exception != null)
				throw new Exception("Invoke failed", exception);
		}

		/// <summary>
		/// Test operations on a form
		/// </summary>
		/// <param name="test">Delegate to execute on the form</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Form(Action<Form> test, int timeout = DefaultTimeout)
		{
			Form form;
			Run((app, finished) =>
			{
				form = new Form();

				test(form);

				form.Closed += (sender, e) => finished();
				form.Show();

			}, timeout);
		}

		/// <summary>
		/// Test paint operations on a drawable
		/// </summary>
		/// <param name="paint">Delegate to execute during the paint event</param>
		/// <param name="size">Size of the drawable, or null for 200x200</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Paint(Action<Drawable, PaintEventArgs> paint, Size? size = null, int timeout = DefaultTimeout)
		{
			Exception exception = null;
			Form(form =>
			{
				var drawable = new Drawable { Size = size ?? new Size(200, 200) };
				drawable.Paint += (sender, e) =>
				{
					try
					{
						paint(drawable, e);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						Application.Instance.AsyncInvoke(form.Close);
					}
				};
				form.Content = drawable;
			}, timeout);
			if (exception != null)
				throw new Exception("Paint event caused exception", exception);
		}
	}
}
