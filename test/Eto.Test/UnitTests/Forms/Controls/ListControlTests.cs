using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	public enum SetBindingMode
	{
		Before,
		After,
		Async
	}

	public class ListControlTests<T> : TestBase
		where T : ListControl, new()
	{
		[ManualTest]
		[TestCase(SetBindingMode.Before)]
		[TestCase(SetBindingMode.After)]
		[TestCase(SetBindingMode.Async)]
		public void ItemTextBindingShouldWorkRegardlessOfOrder(SetBindingMode mode)
		{
			ManualForm("Should show 'Item x' for each item", form =>
			{
				var dropDown = new T();
				if (dropDown is ListBox)
					dropDown.Size = new Size(150, 200);

				var binding = Binding.Delegate((string d) => "Item " + d);
				if (mode == SetBindingMode.Before)
					dropDown.ItemTextBinding = binding;
				dropDown.DataStore = new[] { "1", "2", "3" };
				dropDown.SelectedIndex = 0;
				if (mode == SetBindingMode.After)
					dropDown.ItemTextBinding = binding;

				if (mode == SetBindingMode.Async)
				{
					var updateBindingButton = new Button { Text = "Update ItemTextBinding" };
					updateBindingButton.Click += (sender, e) => dropDown.ItemTextBinding = binding;

					return new StackLayout
					{
						Spacing = 4,
						Items = { new StackLayoutItem(dropDown, true), updateBindingButton }
					};
				}
				else
					return dropDown;
			});
		}

		[Test]
		public void SettingDataStoreToNullAfterPopulatedShouldNotCrash()
		{
			Form(form =>
			{
				var list = new T();

				list.DataStore = new[] { "Item 1", "Item 2", "Item 3" };
				form.Content = list;

				form.Shown += (sender, e) =>
				{
					Application.Instance.AsyncInvoke(() =>
					{
						list.DataStore = null;
						list.Invalidate();
						new UITimer((sender2, e2) => { form.Close(); }) { Interval = 1 }.Start();
					});
				};
			});
		}
		
		[Test, InvokeOnUI]
		public void ChangingSelectedIndexMultipleTimesBeforeLoadShouldTriggerChanged()
		{
			var list = new T();
			int changed = 0;
			list.SelectedIndexChanged += (sender, e) => changed++;
			list.DataStore = new [] { "Item 1", "Item 2", "Item 3" };
			
			Assert.AreEqual(0, changed, "1.1 - Setting data store should not fire selected index");
			Assert.AreEqual(-1, list.SelectedIndex, "1.2");
			
			list.SelectedIndex = 0;
			Assert.AreEqual(1, changed, "2.1 - Setting selected index should trigger event");
			Assert.AreEqual(0, list.SelectedIndex, "2.2");
			
			list.SelectedIndex = 1;
			Assert.AreEqual(2, changed, "3.1 - Setting selected index again should trigger event again");
			Assert.AreEqual(1, list.SelectedIndex, "3.2");
		}
	}
}
