using System;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[Handler(typeof(IHandler))]
	public class StyledWidget : Widget
	{
		new IHandler Handler => (IHandler)base.Handler;

		public bool SomeProperty
		{
			get => Handler.SomeProperty;
			set => Handler.SomeProperty = value;
		}

		public new interface IHandler : Widget.IHandler
		{
			bool SomeProperty { get; set; }
		}
	}

	public class StyledWidgetHandler : WidgetHandler<StyledWidget>, StyledWidget.IHandler
	{
		public bool SomeProperty { get; set; }
	}

	[TestFixture]
	public class DefaultStyleProviderTests : TestBase
	{
		static DefaultStyleProviderTests()
		{
			Platform.Instance.Add<StyledWidget.IHandler>(() => new StyledWidgetHandler());
		}

		[Test, InvokeOnUI]
		public void BaseClassShouldApplyDefault()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<Control>(null, c => c.Visible = false);

			var label = new Label();
			Assert.IsTrue(label.Visible);
			provider.ApplyDefault(label);
			Assert.IsFalse(label.Visible);
		}

		[Test, InvokeOnUI]
		public void BaseClassWithStyleShouldApply()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<Control>("style", c => c.Visible = false);

			var label = new Label();
			Assert.IsTrue(label.Visible);
			provider.ApplyStyle(label, "style");
			Assert.IsFalse(label.Visible);
		}

		[Test, InvokeOnUI]
		public void OtherClassShouldNotApplyDefault()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<Button>(null, c => c.Visible = false);

			var label = new Label();
			Assert.IsTrue(label.Visible);
			provider.ApplyDefault(label);
			Assert.IsTrue(label.Visible);
		}

		[Test, InvokeOnUI]
		public void OtherClassWithStyleShouldNotApply()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<Button>("style", c => c.Visible = false);

			var label = new Label();
			Assert.IsTrue(label.Visible);
			provider.ApplyStyle(label, "style");
			Assert.IsTrue(label.Visible);
		}

		[Test, InvokeOnUI]
		public void HandlerShouldApplyDefault()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<StyledWidgetHandler>(null, h => h.SomeProperty = true);

			var styledWidget = new StyledWidget();
			Assert.IsFalse(styledWidget.SomeProperty);
			provider.ApplyDefault(styledWidget.Handler);
			Assert.IsTrue(styledWidget.SomeProperty);
		}

		[Test, InvokeOnUI]
		public void HandlerWithStyleShouldApply()
		{
			var style = new DefaultStyleProvider();
			var provider = (IStyleProvider)style;
			style.Add<StyledWidgetHandler>("style", h => h.SomeProperty = true);

			var styledWidget = new StyledWidget();
			Assert.IsFalse(styledWidget.SomeProperty);
			provider.ApplyStyle(styledWidget.Handler, "style");
			Assert.IsTrue(styledWidget.SomeProperty);
		}
	}
}
