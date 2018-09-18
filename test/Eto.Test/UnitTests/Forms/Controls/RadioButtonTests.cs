using System;
using NUnit.Framework;
using Eto.Forms;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class RadioButtonTests : TestBase
	{
		[ManualTest]
		[TestCase(true)]
		[TestCase(false)]
		public void RadioButtonGroupsShouldOperateTogether(bool useSeparateContainers)
		{
			bool? success = null;
			ManualForm("Clicking on RadioButton 2 should uncheck RadioButton 1", form =>
			{
				var rb1 = new RadioButton { Text = "RadioButton 1", Checked = true };
				rb1.CheckedChanged += (sender, e) =>
				{
					Log.Write(rb1, $"{rb1.Text}: {rb1.Checked}");
				};

				var rb2 = new RadioButton(rb1) { Text = "RadioButton 2" };
				rb2.CheckedChanged += (sender, e) =>
				{
					Log.Write(rb2, $"{rb2.Text}: {rb2.Checked}");
					if (success == null)
						success = rb2.Checked == true && rb1.Checked == false;
				};

				if (useSeparateContainers)
				{
					return new TableLayout
					{
						Rows =
						{
							new TableLayout { Rows = { rb1 } },
							new TableLayout { Rows = { rb2 } }
						}
					};
				}
				else
				{
					return new TableLayout { Rows = { rb1, rb2 } };
				}
			});

			Assert.IsTrue(success, "Checked values are incorrect");
		}

		[Test, ManualTest]
		public void RadioButtonsInDifferentContainersShouldNotAffectOtherGroups()
		{
			ManualForm("Clicking on RadioButton 1.* should not affect RadioButton 2.* and vise versa", form =>
			{
				var rb1_1 = new RadioButton { Text = "RadioButton 1.1", Checked = true };
				rb1_1.CheckedChanged += (sender, e) => Log.Write(rb1_1, $"{rb1_1.Text}: {rb1_1.Checked}");

				var rb1_2 = new RadioButton(rb1_1) { Text = "RadioButton 1.2" };
				rb1_2.CheckedChanged += (sender, e) => Log.Write(rb1_2, $"{rb1_2.Text}: {rb1_2.Checked}");

				var rb2_1 = new RadioButton { Text = "RadioButton 2.1", Checked = true };
				rb2_1.CheckedChanged += (sender, e) => Log.Write(rb2_1, $"{rb2_1.Text}: {rb2_1.Checked}");

				var rb2_2 = new RadioButton(rb2_1) { Text = "RadioButton 2.2" };
				rb2_2.CheckedChanged += (sender, e) => Log.Write(rb2_2, $"{rb2_2.Text}: {rb2_2.Checked}");

				return new TableLayout
				{
					Rows = {
						new TableLayout { Rows = { new TableRow(rb1_1, rb2_1) } },
						new TableLayout { Rows = { new TableRow(rb1_2, rb2_2) } }
					}
				};
			});

		}

		[TestCase(true)]
		[TestCase(false)]
		public void RadioButtonGroupsShouldWorkProgrammatically(bool useSeparateContainers)
		{
			RadioButton rb1 = null, rb2 = null, rb3 = null;
			int rb1changed = 0, rb2changed = 0, rb3changed = 0;
			Shown(form =>
			{
				rb1 = new RadioButton();
				rb1.CheckedChanged += (sender, e) => rb1changed++;

				rb2 = new RadioButton(rb1);
				rb2.CheckedChanged += (sender, e) => rb2changed++;

				rb3 = new RadioButton(rb1);
				rb3.CheckedChanged += (sender, e) => rb3changed++;

				if (useSeparateContainers)
				{
					form.Content = new TableLayout
					{
						Rows =
						{
							new TextBox(), // on winforms, when a radio button gets focus it also gets checked
							new TableLayout { Rows = { rb1 } },
							new TableLayout { Rows = { rb2 } },
							new TableLayout { Rows = { rb3 } }
						}
					};
				}
				else
				{
					form.Content = new TableLayout
					{
						Rows =
						{
							new TextBox(), // on winforms, when a radio button gets focus it also gets checked
							rb1, rb2, rb3
						}
					};
				}
			}, () =>
			{
				// none checked is valid
				Assert.IsFalse(rb1.Checked, "#1.1");
				Assert.IsFalse(rb2.Checked, "#1.2");
				Assert.IsFalse(rb3.Checked, "#1.3");
				Assert.AreEqual(0, rb1changed, "#1.4");
				Assert.AreEqual(0, rb2changed, "#1.5");
				Assert.AreEqual(0, rb3changed, "#1.6");

				rb2.Checked = true;

				Assert.IsFalse(rb1.Checked, "#2.1");
				Assert.IsTrue(rb2.Checked, "#2.2");
				Assert.IsFalse(rb3.Checked, "#2.3");
				Assert.AreEqual(0, rb1changed, "#2.4");
				Assert.AreEqual(1, rb2changed, "#2.5");
				Assert.AreEqual(0, rb3changed, "#2.6");

				rb3.Checked = true;

				Assert.IsFalse(rb1.Checked, "#3.1");
				Assert.IsFalse(rb2.Checked, "#3.2");
				Assert.IsTrue(rb3.Checked, "#3.3");
				Assert.AreEqual(0, rb1changed, "#3.4");
				Assert.AreEqual(2, rb2changed, "#3.5");
				Assert.AreEqual(1, rb3changed, "#3.6");
			});
		}

	}
}
