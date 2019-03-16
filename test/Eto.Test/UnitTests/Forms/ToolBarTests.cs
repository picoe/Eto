using System;
using System.Threading;
using Eto.Forms;
using NUnit.Framework;
using Eto.Drawing;
namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ToolBarTests : TestBase
	{
		[Test, InvokeOnUI]
		public void AddingDividerSeparatorShouldNotCrash()
		{
			var form = new Dialog { Size = new Size(800, 300) };

			var tb = new ToolBar();
			form.ToolBar = tb;
			for (int i = 0; i < 20; i++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Thread.Sleep(10);
				var item = new ButtonToolItem { Text = i.ToString() };
				tb.Items.Add(item);

				tb.Items.AddSeparator(type: SeparatorToolItemType.Divider);
			}

			form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			form.ShowModal();
		}
	}
}
