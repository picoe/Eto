using System;
using System.Linq;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlEventTests
	{
		static IEnumerable<Control> GetControls()
		{
			var controls = new List<Control>();
			TestBase.Invoke(() =>
			{
				var controlTypes = typeof(Control)
					.GetTypeInfo().Assembly.ExportedTypes
					.Where(r =>
					{
						var ti = r.GetTypeInfo();
						return r.FullName.StartsWith("Eto.Forms", StringComparison.Ordinal)
							&& typeof(Control).GetTypeInfo().IsAssignableFrom(ti)
							&& !ti.IsAbstract
							&& !ti.IsGenericType
							&& ti.DeclaredConstructors.Any(c => c.GetParameters().Length == 0);
					});
				foreach (var type in controlTypes)
				{
					if (!Platform.Instance.Supports(type))
						continue;
					controls.Add((Control)Activator.CreateInstance(type));
				}
			});
			return controls;
		
		}

		/// <summary>
		/// Test to ensure all common events can be handled
		/// </summary>
		[Test]
		[TestCaseSource("GetControls")]
		public void ControlEventsShouldBeHandled(Control control)
		{
			TestBase.Invoke(() =>
			{
				try
				{
					control.SizeChanged += Control_EventHandler;
					control.EnabledChanged += Control_EventHandler;
					control.GotFocus += Control_EventHandler;
					control.LostFocus += Control_EventHandler;
					control.KeyDown += Control_EventHandler;
					control.KeyUp += Control_EventHandler;
					control.MouseUp += Control_EventHandler;
					control.MouseDown += Control_EventHandler;
					control.MouseEnter += Control_EventHandler;
					control.MouseLeave += Control_EventHandler;
					control.MouseDoubleClick += Control_EventHandler;
					control.MouseWheel += Control_EventHandler;
					//control.Shown += Control_EventHandler;
					//control.TextInput += Control_EventHandler;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Control {control.GetType().Name}:", ex);
				}
			});
		}

		static void Control_EventHandler (object sender, EventArgs e)
		{
			
		}
	}
}

