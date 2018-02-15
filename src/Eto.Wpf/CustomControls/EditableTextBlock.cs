using Eto.Wpf.Forms.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System;

namespace Eto.Wpf.CustomControls
{
#pragma warning disable CS0618 // Type or member is obsolete
	public class EditableTextBlock : UserControl
	{
		string oldText;

		public EditableTextBlock()
		{
			var textBox = new FrameworkElementFactory(typeof(TextBox));
			textBox.SetValue(Control.PaddingProperty, new Thickness(1)); // 1px for border
			textBox.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(TextBox_Loaded));
			textBox.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
			textBox.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
			textBox.AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));
			textBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Text") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			var editTemplate = new DataTemplate { VisualTree = textBox };

			var textBlock = new FrameworkElementFactory(typeof(TextBlock));
			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(2));
			textBlock.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(TextBlock_MouseDown));
			textBlock.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Text") { Source = this });
			var viewTemplate = new DataTemplate { VisualTree = textBlock };

			var style = new System.Windows.Style(typeof(EditableTextBlock));
			var trigger = new Trigger { Property = IsInEditModeProperty, Value = true };
			trigger.Setters.Add(new Setter { Property = ContentTemplateProperty, Value = editTemplate });
			style.Triggers.Add(trigger);

			trigger = new Trigger { Property = IsInEditModeProperty, Value = false };
			trigger.Setters.Add(new Setter { Property = ContentTemplateProperty, Value = viewTemplate });
			style.Triggers.Add(trigger);
			Style = style;
		}

		void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var args = new RoutedEventArgs(TreeViewHandler.EtoTreeViewItem.LabelEditingEvent, this);
			this.GetVisualParent<TreeViewHandler.EtoTreeViewItem>().RaiseEvent(args);
			args.Handled = args.Handled;
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(EditableTextBlock),
			new PropertyMetadata(""));

		public bool IsEditable
		{
			get { return (bool)GetValue(IsEditableProperty); }
			set { SetValue(IsEditableProperty, value); }
		}

		public static readonly DependencyProperty IsEditableProperty =
			DependencyProperty.Register(
			"IsEditable",
			typeof(bool),
			typeof(EditableTextBlock),
			new PropertyMetadata(true));

		public bool IsInEditMode
		{
			get
			{
				return IsEditable && (bool)GetValue(IsInEditModeProperty);
			}
			set
			{
				if (IsEditable)
				{
					if (value) oldText = Text;
					SetValue(IsInEditModeProperty, value);
				}
			}
		}
		public static readonly DependencyProperty IsInEditModeProperty =
			DependencyProperty.Register(
			"IsInEditMode",
			typeof(bool),
			typeof(EditableTextBlock),
			new PropertyMetadata(false));

		static void TextBox_Loaded(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			textBox.Focus();
			textBox.SelectAll();
		}

		void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!settingFocus)
			{
				e.Handled = SetParentFocus();
				if (!e.Handled)
					IsInEditMode = false;
			}
        }

		bool settingFocus;
		bool SetParentFocus()
		{
			if (settingFocus)
				return true;
			settingFocus = true;
			var item = this.GetVisualParent<TreeViewItem>();
			if (item != null)
				item.Focus();
			settingFocus = false;
			RaiseEvent(new RoutedEventArgs(LostFocusEvent, this));

			var args = new RoutedEventArgs(TreeViewHandler.EtoTreeViewItem.LabelEditedEvent, this);
			this.GetVisualParent<TreeViewHandler.EtoTreeViewItem>().RaiseEvent(args);
			return args.Handled;
		}


		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (!SetParentFocus())
					IsInEditMode = false;
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				var prev = Text;
				Text = oldText;
				if (!SetParentFocus())
					IsInEditMode = false;
				else
					Text = prev;
				e.Handled = true;
			}
		}

		void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Pressed && e.ClickCount >= 2 && e.ChangedButton == MouseButton.Left)
			{
				IsInEditMode = true;
				e.Handled = true;
			}
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
}
