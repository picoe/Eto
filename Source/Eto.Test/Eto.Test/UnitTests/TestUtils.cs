using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Eto.Threading;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.IO;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Unit test utilities
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class TestUtils
	{
		/// <summary>
		/// Category to exclude when using the Test platform, and only run when on a "real" platform.
		/// </summary>
		public const string NoTestPlatformCategory = "NoTestPlatform";

		/// <summary>
		/// Default timeout for form operations
		/// </summary>
		const int DefaultTimeout = 4000;

		/// <summary>
		/// Timeout for application initialization
		/// </summary>
		const int ApplicationTimeout = 10000;

		/// <summary>
		/// initializes the application when running unit tests directly through the IDE or NUnit gui.
		/// To run on specific platforms, run it through the test runner in the Eto.Test app
		/// </summary>
		public static void Initialize()
		{
			var platform = Platform.Instance;
			if (platform == null)
			{
				try
				{
					// use config file to specify which generator to use for testing
					#if PCL
					var doc = System.Xml.Linq.XDocument.Load("Eto.Test.dll.config");
					var setting = doc != null ? doc.Root.Element("appSettings").Elements("add").FirstOrDefault(r => r.Attribute("key").Value == "generator") : null;
					var generatorTypeName = setting != null ? setting.Attribute("value").Value : null;
					#else
					var generatorTypeName = System.Configuration.ConfigurationManager.AppSettings["generator"];
					#endif
					if (!string.IsNullOrEmpty(generatorTypeName))
						platform = Platform.Get(generatorTypeName);
				}
				catch (FileNotFoundException)
				{
				}
				if (platform == null)
					platform = new Handlers.TestPlatform();
				Platform.Initialize(platform);
			}

			if (Application.Instance == null)
			{
				if (platform.Supports<Application>())
				{
					var ev = new ManualResetEvent(false);
					Exception exception = null;
					Task.Factory.StartNew(() =>
					{
						try
						{
							var app = new Application(platform);
							app.Initialized += (sender, e) => ev.Set();
							app.Run();
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Error running test application: {0}", ex);
							exception = ex;
							ev.Set();
						}
					});
					if (!ev.WaitOne(ApplicationTimeout))
						Assert.Fail("Could not initialize application");
					if (exception != null)
						ExceptionDispatchInfo.Capture(exception).Throw();
				}
			}
		}

		static Application Application
		{
			get
			{
				var application = Application.Instance;
				if (application != null && application.Platform != Platform.Instance)
					application = null;
				return application;
			}

		}

		/// <summary>
		/// Run a test on by invoking the test on the application
		/// </summary>
		/// <param name="test">Delegate to execute within the invoke</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Run(Action<Application, Action> test, int timeout = DefaultTimeout)
		{
			Initialize();
			var ev = new ManualResetEvent(false);
			var application = Application;
			Exception exception = null;
			Action finished = () => ev.Set();
			Action run = () =>
			{
				try
				{
					test(application, finished);
				}
				catch (Exception ex)
				{
					exception = ex;
					ev.Set();
				}
			};
			if (application != null)
				application.AsyncInvoke(run);
			else
				run();
			if (!ev.WaitOne(timeout))
			{
				Assert.Fail("Test did not complete in time");
			}
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}

		public static void Invoke(Action test, int timeout = DefaultTimeout)
		{
			Run((app, finished) =>
			{
				test();
				finished();
			}, timeout);
		}

		/// <summary>
		/// Test operations on a form
		/// </summary>
		/// <param name="test">Delegate to execute on the form</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Form(Action<Form> test, int timeout = DefaultTimeout)
		{
			Form form = null;
			bool shown = false;
			try
			{
				Run((app, finished) =>
				{
					if (!Platform.Instance.Supports<Form>())
						Assert.Inconclusive("This platform does not support IForm");

					form = new Form();

					test(form);

					form.Closed += (sender, e) => 
					{
						form = null;
						finished();
					};
					shown = true;
					form.Show();

				}, timeout);
			}
			catch
			{
				if (form != null && shown)
				{
					var application = Application;
					if (application != null)
						application.Invoke(form.Close);
					else
						form.Close();
				}
				throw;
			}
		}

		/// <summary>
		/// Test paint operations on a drawable
		/// </summary>
		/// <param name="paint">Delegate to execute during the paint event</param>
		/// <param name="size">Size of the drawable, or null for 200x200</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Paint(Action<Drawable, PaintEventArgs> paint, Size? size = null, int timeout = DefaultTimeout)
		{
			var application = Application;
			bool finished = false;
			Exception exception = null;
			Form(form =>
			{
				var drawable = new Drawable { Size = size ?? new Size(200, 200) };
				drawable.Paint += (sender, e) =>
				{
					try
					{
						paint(drawable, e);
						finished = true;
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						if (application != null)
							application.AsyncInvoke(form.Close);
						else
							form.Close();
					}
				};
				form.Content = drawable;
			}, timeout);
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
			if (!finished)
				Assert.Fail("Paint event did not finish");
		}

		public static Task<TEventArgs> WaitEventAsync<TEventArgs>(Action<EventHandler<TEventArgs>> hookEvent)
			where TEventArgs : EventArgs
		{
			var tcs = new TaskCompletionSource<TEventArgs>();
			hookEvent((sender, e) => tcs.SetResult(e));
			return tcs.Task;
		}
	}
}
