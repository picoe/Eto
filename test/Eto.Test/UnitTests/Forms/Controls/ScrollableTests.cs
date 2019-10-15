using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ScrollableTests : TestBase
	{
		[Test, ManualTest]
		public void TwoScrollablesShouldNotClipControls()
		{
			ManualForm(
				"When resizing on macOS with System Preferences > General > Show Scroll Bars set to 'Always', the scrollbars should not obscure content when resizing the form to a smaller size.\nAlso, the top panel should never get a vertical scroll bar, only horizontal.",
				form =>
			{
				form.ClientSize = new Size(400, -1);
				form.Padding = 20;
				var pixelLayout1 = new PixelLayout { BackgroundColor = Colors.Green };
				pixelLayout1.Add(new Panel { BackgroundColor = Colors.Blue, Size = new Size(200, 30) }, Point.Empty);
				var scrollable1 = new Scrollable { Content = pixelLayout1 };

				var pixelLayout2 = new PixelLayout { BackgroundColor = Colors.Green };
				pixelLayout2.Add(new Panel { BackgroundColor = Colors.Blue, Size = new Size(300, 300) }, Point.Empty);
				var scrollable2 = new Scrollable { Content = pixelLayout2 };
				return new TableLayout
				{
					Rows = {
						scrollable1,
						scrollable2
					}
				};
			});
		}

		[Test, TestCaseSource(nameof(GetControlTypes)), ManualTest]
		public void AllControlsShouldExpandWidth(IControlTypeInfo<Control> info)
		{
			ManualForm("Control and blue background should expand to width of scrollable", form =>
			{
				var control = info.CreateControl();
				info.PopulateControl(control);
				control.BackgroundColor = Colors.Blue;

				return new Scrollable
				{
					Size = new Size(300, 200),
					Content = control,
					ExpandContentWidth = true,
					ExpandContentHeight = false
				};
			});
		}

		[Test, ManualTest]
		public void WidthOfContentShouldAffectScrollableRegionWhenLabelsWrap()
		{
			void CreateAndAddEmptySpace(DynamicLayout container)
			{
				container.AddRow(new TableRow(new Label { Text = "", Height = 10 }));
			}

			Label CreateAndAddLabelRow(DynamicLayout container, string text)
			{
				var label = new Label { Text = text, Font = SystemFonts.Bold(null, FontDecoration.None), Wrap = WrapMode.Word };
				container.AddRow(new TableRow(label));
				CreateAndAddEmptySpace(container);
				return label;
			}

			void CreateAndAddDescriptionRow(DynamicLayout container, string text)
			{
				container.AddRow(new TableRow(new Label { Text = text, Wrap = WrapMode.Word, Font = SystemFonts.Label(SystemFonts.Default().Size - 2.0f) }));
				CreateAndAddEmptySpace(container);
			}

			DropDown CreateAndAddDropDownRow(DynamicLayout container, string text, List<string> options, int position, Action<DropDown, EventArgs> command)
			{
				var txt = new Label { Text = text, VerticalAlignment = VerticalAlignment.Center };
				var drop = new DropDown { Width = 200 };

				foreach (var item in options)
				{
					drop.Items.Add(item);
				}

				drop.SelectedIndex = position;

				if (command != null) drop.SelectedIndexChanged += (sender, e) => command.Invoke((DropDown)sender, e);

				var tr = new TableRow(txt, null, drop);

				container.AddRow(tr);
				CreateAndAddEmptySpace(container);

				return drop;
			}


			ManualForm(
				"You should be able to scroll down to the 'BOTTOM' label, and long paragraphs should be wrapped.",
				form =>
				{
					var container = new DynamicLayout();

					container.BeginVertical();

					container.Padding = 0;

					CreateAndAddLabelRow(container, "Header 1");

					CreateAndAddDescriptionRow(container, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras suscipit malesuada ex, ut iaculis nisl. Donec eros est, rutrum ac molestie vitae, euismod mollis enim. Integer eget turpis sit amet nulla laoreet dapibus. Curabitur sit amet nulla sed erat rutrum gravida. Donec vel erat ex. Aenean sit amet massa at ante suscipit fringilla. Sed bibendum tempor sem et congue. Maecenas diam neque, dictum id ligula eget, vulputate ornare massa. Integer lobortis dolor risus, a eleifend ante feugiat ac. Aenean egestas bibendum fermentum. In ac elit vitae augue convallis blandit eu eget ligula.");

					CreateAndAddDropDownRow(container, "Some Drop Down", new List<string>() { "item1", "item2" }, 0, (sender, e) => { });

					CreateAndAddLabelRow(container, "Header 2");

					CreateAndAddLabelRow(container, "Header 3");

					CreateAndAddDescriptionRow(container, "Vestibulum dignissim in ipsum sed condimentum. Etiam vitae ullamcorper dui. Pellentesque non imperdiet lacus. Maecenas ullamcorper sapien enim, sit amet commodo mauris scelerisque non. Nullam facilisis ipsum laoreet, gravida eros in, egestas odio. Morbi id ex vitae dui viverra ornare et vel metus. Phasellus lobortis finibus ex. Nulla tristique malesuada eros eget maximus. Donec a nisi facilisis sapien porta mattis. Curabitur sodales, magna sit amet aliquet commodo, eros quam congue felis, sit amet dictum orci neque nec risus. Aenean id auctor tellus. Etiam ac imperdiet nunc. Aliquam suscipit quam nec velit sollicitudin, posuere tempor ante vehicula. Suspendisse vitae massa tempus, maximus urna vitae, bibendum arcu.");

					CreateAndAddDropDownRow(container, "Other Drop Down", new List<string>(), -1, (sender, e) => { });

					container.AddRow(new Label { Text = "BOTTOM", Font = SystemFonts.Bold() });

					container.AddSpace();

					container.EndVertical();

					container.Width = 300;// - container.Padding.Value.Left * 2 - container.Padding.Value.Right * 2;

					var tabs = new List<TabPage>
					{
						new TabPage(new Scrollable { Content = container, Border = BorderType.None }) { Text = "Tab 1" },
						new TabPage(new Panel()) { Text = "Tab 2" }
					};

					var tabctrl = new TabControl();
					foreach (var tab in tabs)
					{
						tabctrl.Pages.Add(tab);
					}

					form.ClientSize = new Size(500, 300);

					return tabctrl;
				});
		}

	}
}
