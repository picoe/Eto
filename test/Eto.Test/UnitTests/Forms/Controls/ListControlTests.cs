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
		where T: ListControl, new()
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
	}
}
