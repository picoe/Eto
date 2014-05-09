using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;
using Eto.Drawing;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ComboBoxHandler : IosControl<UILabel, ComboBox>, IComboBox
	{
		IListStore dataStore;
		int selectedIndex = -1;
		string emptyText;

		public static string DefaultEmptyText = "<select>";

		public string EmptyText { get { return emptyText ?? DefaultEmptyText; } set { emptyText = value; } }

		public class EtoLabel : UILabel
		{
			UIView inputView;
			UIView accessoryView;
			WeakReference handler;

			public ComboBoxHandler Handler { get { return (ComboBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override bool CanBecomeFirstResponder { get { return true; } }

			public override UIView InputView
			{
				get { return inputView ?? (inputView = Handler.CreatePicker()); }
			}

			public override UIView InputAccessoryView
			{
				get { return accessoryView ?? (accessoryView = Handler.CreateAccessoryView()); }
			}

			public override void TouchesEnded(NSSet touches, UIEvent evt)
			{
				base.TouchesEnded(touches, evt);
				var picker = (UIPickerView)InputView;
				picker.ReloadAllComponents();
				picker.Select(Math.Max(0, Handler.SelectedIndex), 0, false);

				BecomeFirstResponder();
			}

		}

		static UIColor ButtonTextColor = new UIButton(UIButtonType.RoundedRect).TitleColor(UIControlState.Normal);
		static UIColor DisabledTextColor = new UIButton(UIButtonType.RoundedRect).TitleColor(UIControlState.Disabled);

		public ComboBoxHandler()
		{
			Control = new EtoLabel { Handler = this };
			Control.UserInteractionEnabled = true;
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Control.Text = EmptyText;
			Control.TextColor = ButtonTextColor;
		}

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				Control.TextColor = value ? ButtonTextColor : DisabledTextColor;
			}
		}

		class DataSource : UIPickerViewDataSource
		{
			WeakReference handler;

			public ComboBoxHandler Handler { get { return (ComboBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override int GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override int GetRowsInComponent(UIPickerView pickerView, int component)
			{
				var data = Handler.dataStore;
				return data != null ? data.Count : 0;
			}
		}

		class Delegate : UIPickerViewDelegate
		{
			WeakReference handler;

			public ComboBoxHandler Handler { get { return (ComboBoxHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override string GetTitle(UIPickerView pickerView, int row, int component)
			{
				var data = Handler.dataStore;
				return data != null ? data[row].Text : string.Empty;
			}
		}

		public UIPickerView CreatePicker()
		{
			var picker = new UIPickerView();
			picker.ShowSelectionIndicator = true;
			picker.DataSource = new DataSource { Handler = this };
			picker.Delegate = new Delegate { Handler = this };
			return picker;
		}

		public UIToolbar CreateAccessoryView()
		{
			var tools = new UIToolbar();
			tools.BarStyle = UIBarStyle.Default;

			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, ee) => {
				var picker = (UIPickerView)Control.InputView;
				SelectedIndex = picker.SelectedRowInComponent(0);
				Control.ResignFirstResponder();
			});
			var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (s, ee) => Control.ResignFirstResponder());

			tools.Items = new [] { cancelButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), doneButton };
			tools.SizeToFit();
			return tools;
		}

		public int SelectedIndex
		{
			get	{ return selectedIndex; }
			set
			{
				if (value != selectedIndex)
				{
					selectedIndex = value;
					SetText();
					Widget.OnSelectedIndexChanged(EventArgs.Empty);
				}
			}
		}

		void SetText()
		{
			var oldSize = GetPreferredSize(SizeF.MaxValue);
			if (dataStore != null && selectedIndex >= 0 && selectedIndex < dataStore.Count)
			{
				var item = dataStore[selectedIndex];
				Control.Text = item.Text;
			}
			else
				Control.Text = EmptyText;
			LayoutIfNeeded(oldSize);
		}


		public IListStore DataStore
		{
			get { return dataStore; }
			set
			{
				var index = selectedIndex;
				selectedIndex = -1;
				dataStore = value;
				SelectedIndex = index;
			}
		}
	}
}
