using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
    public class NativeControlHostTests : TestBase
    {
		
		[ManualTest]
		[TestCaseSource(typeof(NativeHostControls), nameof(NativeHostControls.GetNativeHostTests))]
		public void NativeHostShouldShowControl(NativeHostTest test)
		{
			ManualForm("Control should show something", form =>
			{
				form.Resizable = true;
				return new NativeControlHost(test.CreateControl());
			});
		}
        
		[ManualTest]
		[TestCaseSource(typeof(NativeHostControls), nameof(NativeHostControls.GetNativeHostTests))]
		public void NativeHostInScrollableShouldBeClipped(NativeHostTest test)
		{
			ManualForm("Native control should not show when scrolled out of view", form =>
			{
				form.Resizable = true;
				var scrollable = new Scrollable
				{
					Size = new Size(400, 400),
					Content = new TableLayout
					{
						Rows = {
						new Panel { Content = "Before", Size = new Size(600, 400) },
						new NativeControlHost(test.CreateControl()),
						new Panel { Content = "After", Size = new Size(600, 400) },
					}
					}
				};

				return scrollable;
			});
		}
		
		class MyControl : NativeControlHost
		{
			public NativeHostTest Test { get; set; }

			protected override void OnCreateNativeControl(CreateNativeControlArgs e)
			{
				base.OnCreateNativeControl(e);
				e.NativeControl = Test.CreateControl();
			}
		}
		
		[ManualTest]
		[TestCaseSource(typeof(NativeHostControls), nameof(NativeHostControls.GetNativeHostTests))]
		public void NetiveHostInSubclassShouldWork(NativeHostTest test)
		{
			ManualForm("Control should show something", form =>
			{
				form.Resizable = true;
				var control = new MyControl { Test = test };
				control.Size = new Size(100, 20);
				return control;
			});
		}
    }
}