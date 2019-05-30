using System;
using System.Linq;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ControlEventTests : TestBase
	{
		/// <summary>
		/// Test to ensure all common events can be handled
		/// </summary>
		[Test]
		[TestCaseSource(nameof(GetAllControlTypes))]
		public void ControlEventsShouldBeHandled(IControlTypeInfo<Control> controlType)
		{
			TestBase.Invoke(() =>
			{
				var control = controlType.CreateControl();
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

