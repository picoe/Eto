using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Test.UnitTests;
using Eto.Wpf.Forms;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class ContainerTests : TestBase
{
	[Test]
	public void RemovingChildShouldClearParent() => Invoke(() =>
	{
		var container = new Eto.Forms.Panel();
		var containerHandler = container.Handler as Eto.Wpf.Forms.Controls.PanelHandler;
		var child = new Eto.Forms.Label { Text = "Child" };
		var childHandler = child.Handler as Eto.Wpf.Forms.Controls.LabelHandler;

		Assert.That(containerHandler, Is.Not.Null, "#1.1");
		Assert.That(childHandler, Is.Not.Null, "#1.2");

		container.Content = child;

		var borderField = typeof(WpfPanel<swc.Border, Panel, Panel.ICallback>).GetField("_border", BindingFlags.NonPublic | BindingFlags.Instance);
		var border = borderField?.GetValue(containerHandler) as swc.Border;

		Assert.That(border, Is.Not.Null, "#2.1");
		Assert.That(border.Child, Is.EqualTo(childHandler.ContainerControl), "#2.2");
		Assert.That(container.Content, Is.EqualTo(child), "#2.3");
		Assert.That(child.Parent, Is.EqualTo(container), "#2.4");

		container.Content = null;

		Assert.That(container.Content, Is.Null, "#3.1");
		Assert.That(child.Parent, Is.Null, "#3.2");

		Assert.That(containerHandler.Control.Child, Is.Null, "#3.3");
		Assert.That(childHandler.Control.Parent, Is.Null, "#3.4");
	});

}