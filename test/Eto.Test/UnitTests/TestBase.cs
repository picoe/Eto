using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal;
using Container = Eto.Forms.Container;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Manual test category
	/// </summary>
	public class ManualTestAttribute : NUnit.Framework.CategoryAttribute
	{
		public ManualTestAttribute()
			: base(TestBase.ManualTestCategory)
		{
		}
	}

	[System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class InvokeOnUIAttribute : Attribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command) => new RunOnUICommand(command);

		class RunOnUICommand : DelegatingTestCommand
		{
			public RunOnUICommand(TestCommand innerCommand)
				: base(innerCommand)
			{
			}

			public override TestResult Execute(TestExecutionContext context)
			{
				Exception exception = null;

				var result = Application.Instance.Invoke(() =>
				{
					try
					{
						context.EstablishExecutionEnvironment();
						return innerCommand.Execute(context);
					}
					catch (Exception ex)
					{
						exception = ex;
						return null;
					}
				});

				if (exception != null)
				{
					ExceptionDispatchInfo.Capture(exception).Throw();
				}

				return result;
			}
		}
	}


	/// <summary>
	/// Unit test utilities
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TestBase
	{
		/// <summary>
		/// Category for tests that require user input to perform the test
		/// </summary>
		/// <remarks>
		/// This is useful to test behaviour of controls when actually in use, not just programmatically.
		/// </remarks>
		public const string ManualTestCategory = "ManualTest";

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
			var context = TestExecutionContext.CurrentContext;
			Action run = () =>
			{
				try
				{
					context.EstablishExecutionEnvironment();
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

		public static void Form(Action<Form> test, int timeout = DefaultTimeout)
		{
			Form<Form>(test, timeout);
		}

		/// <summary>
		/// Test operations on a form
		/// </summary>
		/// <param name="test">Delegate to execute on the form</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Form<T>(Action<T> test, int timeout = DefaultTimeout)
			where T : Form, new()
		{
			T form = null;
			bool shown = false;
			try
			{
				Run((app, finished) =>
				{
					if (!Platform.Instance.Supports<Form>())
						Assert.Inconclusive("This platform does not support IForm");

					form = new T();

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
						application.Invoke(() =>
						{
							if (form != null && form.Loaded)
								form.Close();
						});
					else
						form.Close();
				}
				throw;
			}
		}

		public static void Async(Func<Task> test)
		{
			Exception exception = null;
			var mre = new ManualResetEvent(false);
			Application.Instance.Invoke(async () =>
			{
				try
				{
					await test();
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				finally
				{
					mre.Set();
				}
			});
			mre.WaitOne();
			if (exception != null)
			{
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
		}

		public static void Shown(Action<Form> init, Action test, bool replay = false, int timeout = DefaultTimeout)
		{
			Shown(form =>
			{
				init(form);
				return null;
			},
				(Control c) =>
				{
					test();
				},
				replay,
				timeout
			);
		}

		public static void ManualForm(string description, Func<Form, Control> init, bool allowPass = true, bool allowFail = true)
		{
			ManualForm(description, (form, Label) => init(form), allowPass, allowFail);
		}

		public static void ManualForm(string description, Func<Form, Label, Control> init, bool allowPass = true, bool allowFail = true)
		{
			Exception exception = null;
			Form(form =>
			{
				var label = new Label { Text = description };
				var c = init(form, label);

				var layout = new StackLayout
				{
					Spacing = 10,
					Items =
					{
						new StackLayoutItem(c, HorizontalAlignment.Stretch, true),
						label
					}
				};

				if (allowFail || allowPass)
				{
					var table = new TableLayout { Spacing = new Size(2, 2) };
					var row = new TableRow();
					table.Rows.Add(row);

					if (allowFail)
					{
						var failButton = new Button { Text = "Fail" };
						failButton.Click += (sender, e) =>
						{
							try
							{
								Assert.Fail(description);
							}
							catch (Exception ex)
							{
								exception = ex;
							}
							finally
							{
								form.Close();
							}
						};
						row.Cells.Add(failButton);
					}

					if (allowPass)
					{
						var passButton = new Button { Text = "Pass" };
						passButton.Click += (sender, e) => form.Close();
						row.Cells.Add(passButton);
					}
					layout.Items.Add(new StackLayoutItem(table, HorizontalAlignment.Center));
				}

				form.Content = layout;
			}, timeout: -1);

			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}

		public static void Dialog(Action<Dialog> test, int timeout = DefaultTimeout)
		{
			Dialog<Dialog>(test, timeout);
		}

		/// <summary>
		/// Test operations on a form
		/// </summary>
		/// <param name="test">Delegate to execute on the form</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Dialog<T>(Action<T> test, int timeout = DefaultTimeout)
			where T : Dialog, new()
		{
			T dialog = null;
			bool shown = false;
			try
			{
				Run((app, finished) =>
				{
					if (!Platform.Instance.Supports<Dialog>())
						Assert.Inconclusive("This platform does not support Dialog");

					dialog = new T();

					test(dialog);

					dialog.Closed += (sender, e) =>
					{
						dialog = null;
						finished();
					};
					shown = true;
					dialog.ShowModal();

				}, timeout);
			}
			catch
			{
				if (dialog != null && shown)
				{
					var application = Application;
					if (application != null)
						application.Invoke(() =>
						{
							if (dialog != null && dialog.Loaded)
								dialog.Close();
						});
					else
						dialog.Close();
				}
				throw;
			}
		}

		public static void ManualDialog(string description, Func<Dialog, Control> init)
		{
			ManualDialog(description, (form, Label) => init(form));
		}

		public static void ManualDialog(string description, Func<Dialog, Label, Control> init)
		{
			Exception exception = null;
			Dialog(dialog =>
			{
				var label = new Label { Text = description };
				var c = init(dialog, label);

				var failButton = new Button { Text = "Fail" };
				failButton.Click += (sender, e) =>
				{
					try
					{
						Assert.Fail(description);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						dialog.Close();
					}
				};

				var passButton = new Button { Text = "Pass" };
				passButton.Click += (sender, e) => dialog.Close();

				dialog.Content = new StackLayout
				{
					Spacing = 10,
					Items =
					{
						new StackLayoutItem(c, HorizontalAlignment.Stretch, true),
						label,
						new StackLayoutItem(TableLayout.Horizontal(2, failButton, passButton), HorizontalAlignment.Center)
					}
				};
			}, timeout: -1);

			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}
		/// <summary>
		/// Test operations on a form once it is shown
		/// </summary>
		/// <param name="init">Create form content and/or set other properties of the form</param>
		/// <param name="test">Delegate to execute on the form when shown</param>
		/// <param name="replay">Replay the init and test again after shown</param>
		/// <param name="timeout">Timeout to wait for the operation to complete</param>
		public static void Shown<T>(Func<Form, T> init, Action<T> test = null, bool replay = false, int timeout = DefaultTimeout)
			where T : Control
		{
			var application = Application;
			Exception exception = null;
			Form(form =>
			{
				var control = init(form);
				form.Shown += (sender, e) =>
				{
					try
					{
						if (test != null)
						{
							test(control);
							if (replay)
							{
								form.Content = null;
								control = init(form);
								if (control != null && form.Content == null && control != form)
									form.Content = control;
								if (application == null)
									test(control);
							}
						}
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						if (application == null)
							form.Close();
						else if (!replay)
							application.AsyncInvoke(form.Close);
						else
						{
							application.AsyncInvoke(() =>
							{
								try
								{
									test(control);
								}
								catch (Exception ex)
								{
									exception = ex;
								}
								finally
								{
									form.Close();
								}
							});
						}
					}
				};
				if (control != null && form.Content == null && control != form)
					form.Content = control;
			}, timeout);
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
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

		public static PropertyTestInfo PropertyTest<T>(Func<T> create, params Expression<Func<T, object>>[] param)
		{
			return new PropertyTestInfo
			{
				Type = typeof(T),
				Create = () => create(),
				Properties = param.Select(r => r.GetMemberInfo().Member.Name).ToList()
			};
		}

		public static PropertyTestInfo PropertyTest<T>(params Expression<Func<T, object>>[] param)
			where T : new()
		{
			return new PropertyTestInfo
			{
				Type = typeof(T),
				Create = () => new T(),
				Properties = param.Select(r => r.GetMemberInfo().Member.Name).ToList()
			};
		}

		public static void TestProperties<T>(Func<Form, T> create, params Expression<Func<T, object>>[] properties)
		{
			PropertyTestInfo test = null;
			Shown(form =>
			{
				var ctl = create(form);
				test = PropertyTest<T>(() => ctl, properties);
				test.Run();
				return ctl as Control;
			}, ctl => test.Run());
		}

		public class PropertyTestInfo
		{
			public Type Type { get; set; }
			public Func<object> Create { get; set; }
			public List<string> Properties { get; set; }

			public void Run()
			{
				var obj = Create != null ? Create() : Activator.CreateInstance(Type);
				foreach (var propertyName in Properties)
				{
					var propertyInfo = obj.GetType().GetRuntimeProperty(propertyName);
					var defValAttr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
					var defaultValue = defValAttr != null ? defValAttr.Value : propertyInfo.PropertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
					var val = propertyInfo.GetValue(obj);
					Assert.AreEqual(defaultValue, val, string.Format("Property '{0}' of type '{1}' is expected to be '{2}'", propertyName, Type.Name, defaultValue));
				}
			}

			public override string ToString()
			{
				return $"{Type}: {string.Join(",", Properties)}";
			}
		}

		public static IEnumerable<IControlTypeInfo<Control>> GetAllControlTypes()
		{
			foreach (var type in typeof(Control)
				.GetTypeInfo().Assembly.ExportedTypes
				.Where(r =>
				{
					var ti = r.GetTypeInfo();
					return
						r.FullName.StartsWith("Eto.Forms", StringComparison.Ordinal)
						&& Platform.Instance.Supports(r)
						&& typeof(Control).GetTypeInfo().IsAssignableFrom(ti)
						&& !ti.IsAbstract
						&& !ti.IsGenericType
						&& ti.DeclaredConstructors.Any(c => c.GetParameters().Length == 0);
				})
					 .OrderBy(r => r.FullName))
			{
				yield return new ControlTypeInfo<Control>(type);
			}
		}


		public static IEnumerable<IControlTypeInfo<Control>> GetControlTypes()
		{
			return GetAllControlTypes()
				.Where(r =>
				{
					var ti = r.Type.GetTypeInfo();
					return !typeof(Window).GetTypeInfo().IsAssignableFrom(ti)
						&& !typeof(TabPage).GetTypeInfo().IsAssignableFrom(ti)
						&& !typeof(DocumentPage).GetTypeInfo().IsAssignableFrom(ti);
				});
		}

		public interface IControlTypeInfo<out T>
			where T : Control
		{
			Type Type { get; }
			T CreateControl();
			Container CreateContainer(Control control);
			void PopulateControl(Control control);
			T CreatePopulatedControl();
		}

		public interface IContainerTypeInfo<out T> : IControlTypeInfo<T>
			where T : Container
		{
			T CreateControl(Control content);
			void SetContent(Container container, Control child);
		}

		public class ControlTypeInfo<T> : IControlTypeInfo<T>
			where T : Control
		{
			Func<T> _createControl;
			Type _type;
			public ControlTypeInfo(Func<T> createControl)
			{
				_createControl = createControl;
			}

			public ControlTypeInfo(Type type)
			{
				_type = type;
				_createControl = () => (T)Activator.CreateInstance(type);
			}

			public Type Type => _type ?? typeof(T);

			public T CreateControl() => _createControl();

			public virtual Container CreateContainer(Control control)
			{
				if (typeof(Container).GetTypeInfo().IsInstanceOfType(control))
					return (Container)control;

				return new Panel { Content = control };
			}

			public void PopulateControl(Control control)
			{
				if (control is TextControl textControl)
					textControl.Text = "Some Text";
				else if (control is ImageView imageView)
					imageView.Image = TestIcons.Logo;
				else if (control is ListControl listControl)
				{
					listControl.Items.Add("Item 1");
					listControl.Items.Add("Item 2");
					listControl.Items.Add("Item 3");
				}
				else if (control is NumericStepper numericStepper)
					numericStepper.Value = 100;
				else if (control is TabControl tabControl)
				{
					tabControl.Pages.Add(new TabPage { Text = "Tab 1", Content = new Panel { Size = new Size(100, 100), Content = "Hello" }  });
					tabControl.Pages.Add(new TabPage { Text = "Tab 2" });
					tabControl.Pages.Add(new TabPage { Text = "Tab 3" });
				}
				else if (control is DocumentControl documentControl)
				{
					documentControl.Pages.Add(new DocumentPage { Text = "Tab 1", Content = new Panel { Size = new Size(100, 100), Content = "Hello" } });
					documentControl.Pages.Add(new DocumentPage { Text = "Tab 2" });
					documentControl.Pages.Add(new DocumentPage { Text = "Tab 3" });
				}
				else if (control is Drawable drawable)
				{
					drawable.Size = new Size(100, 40);
					drawable.Paint += (sender, e) =>
					{
						var c = drawable.BackgroundColor;
						if (c.A == 0)
							c = SystemColors.ControlText;
						else
							c.Invert();
						e.Graphics.DrawText(SystemFonts.Default(), c, 0, 0, "Hello!");
					};
				}
				else if (control is SegmentedButton segmented)
				{
					segmented.Items.Add("Segment 1");
					segmented.Items.Add(TestIcons.Logo.WithSize(20, 20));
					segmented.Items.Add("Segment 3");
				}
				else if (control is CheckBoxList checkBoxList)
				{
					checkBoxList.Items.Add("Item 1");
					checkBoxList.Items.Add("Item 2");
					checkBoxList.Items.Add("Item 3");
				}
				else if (control is RadioButtonList radioButtonList)
				{
					radioButtonList.Items.Add("Item 1");
					radioButtonList.Items.Add("Item 2");
					radioButtonList.Items.Add("Item 3");
				}
				else if (control is PixelLayout pixelLayout)
				{
					pixelLayout.Add("Hello", 0, 0);
					pixelLayout.Add("World!", 24, 24);
				}
				else if (control is DynamicLayout dynamicLayout)
				{
					dynamicLayout.Add("Hello");
					dynamicLayout.Add("World!");
				}
				else if (control is StackLayout stackLayout)
				{
					stackLayout.Items.Add("Hello");
					stackLayout.Items.Add("World!");
				}
				else if (control is TableLayout tableLayout)
				{
					tableLayout.Rows.Add(new TableRow(new TableCell("Hello", true), new TableCell("World!", true)));
					tableLayout.Rows.Add(new TableRow("Row", "2"));
				}
				else if (control is Panel panel && panel.Content == null)
				{
					panel.Content = "Hello, World!";
				}
			}

			public override string ToString() => Type.Name;

			public T CreatePopulatedControl()
			{
				var c = CreateControl();
				PopulateControl(c);
				return c;
			}
		}

		public static class ContainerTypeInfo
		{
			public static ContainerTypeInfo<T> New<T>(Action<T, Control> addChild, Func<T, Container> createContainer = null)
				where T : Container, new()
			{
				return new ContainerTypeInfo<T>(() => new T(), addChild, createContainer);
			}

			public static ContainerTypeInfo<T> New<T>(Func<T> create, Action<T, Control> addChild, Func<T, Container> createContainer = null)
				where T : Container
			{
				return new ContainerTypeInfo<T>(create, addChild, createContainer);
			}
		}

		public class ContainerTypeInfo<T> : ControlTypeInfo<T>, IContainerTypeInfo<T>
			where T : Container
		{
			Action<T, Control> _setContent;
			Func<T, Container> _createContainer;

			public ContainerTypeInfo(Func<T> create, Action<T, Control> setContent, Func<T, Container> createContainer = null)
				: base(create)
			{
				_setContent = setContent;
				_createContainer = createContainer;
			}

			public override Container CreateContainer(Control control)
			{
				if (_createContainer != null)
					return _createContainer((T)control);
				return base.CreateContainer(control);
			}

			public virtual T CreateControl(Control content)
			{
				var control = CreateControl();
				SetContent(control, content);
				return control;
			}

			public virtual void SetContent(Container container, Control child)
			{
				_setContent?.Invoke((T)container, child);
			}
		}

		public static IEnumerable<IContainerTypeInfo<Container>> GetContainerTypes()
		{
			foreach (var type in GetPanelTypes())
				yield return type;
			yield return ContainerTypeInfo.New<TableLayout>((container, child) => container.Rows.Add(child));
			yield return ContainerTypeInfo.New<StackLayout>((container, child) => container.Items.Add(child));
			yield return ContainerTypeInfo.New<DynamicLayout>((container, child) => container.Add(child));
		}

		public static IEnumerable<IContainerTypeInfo<Panel>> GetPanelTypes()
		{
			yield return ContainerTypeInfo.New<Panel>((container, child) => container.Content = child);
			yield return ContainerTypeInfo.New(
				() => new Expander { Header = "Expander", Expanded = true },
				(container, child) => container.Content = child
			);
			yield return ContainerTypeInfo.New(
				() => new GroupBox { Text = "GroupBox" },
				(container, child) => container.Content = child
			);
			yield return ContainerTypeInfo.New(
				() => new TabPage { Text = "TabPage" },
				(container, child) => container.Content = child,
				container => new TabControl { Pages = { container } }
			);
			yield return ContainerTypeInfo.New(
				() => new DocumentPage { Text = "DocumentPage" },
				(container, child) => container.Content = child,
				container => new DocumentControl { Pages = { container } }
			);
			yield return ContainerTypeInfo.New<Scrollable>((container, child) => container.Content = child);
			yield return ContainerTypeInfo.New<Drawable>((container, child) => container.Content = child);
		}
	}
}
