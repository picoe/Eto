using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ToggleButton))]
	public class ToggleButtonSection : BaseButtonSection<ToggleButton>
	{
		protected override string DefaultText => "Toggle Me";

		protected override void LogEvents(ToggleButton button)
		{
			base.LogEvents(button);
			button.CheckedChanged += (sender, e) => Log.Write(sender, $"CheckedChanged, Checked:{button.Checked}");
		}

		protected override void AddAdditionalOptions()
		{
			base.AddAdditionalOptions();

			var checkedCheck = new CheckBox { Text = "Checked" };
			checkedCheck.CheckedBinding.Bind(Control, b => b.Checked);

			AddSeparateRow(null, checkedCheck, null);
		}
	}
	[Section("Controls", typeof(Button), "Button (new)")]
	public class NewButtonSection : BaseButtonSection<Button>
	{
		protected override string DefaultText => "Click Me";
	}

	public abstract class BaseButtonSection<T> : DynamicLayout
		where T : Button, new()
	{
		protected T Control { get; private set; }

		protected abstract string DefaultText { get; }

		Bitmap CreatePlainImage()
		{
			var b = new Bitmap(32, 32, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(b))
			{
				g.FillRectangle(Colors.Blue, new Rectangle(b.Size));
			}
			return b;
		}

		public BaseButtonSection()
		{
			Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Center);

			Image image = TestIcons.TestIcon.WithSize(16, 16);
			Image plainImage = CreatePlainImage();
			Image largeImage = TestIcons.TestIcon.WithSize(100, 100);

			DefaultSpacing = new Size(2, 2);

			var button = new T();
			button.Text = DefaultText;
			button.Image = image;
			Control = button;

			var originalMinimumSize = button.MinimumSize;

			LogEvents(button);

			var textBox = new TextBox();
			textBox.TextBinding.Bind(button, b => b.Text);

			var enabledCheck = new CheckBox { Text = "Enabled" };
			enabledCheck.CheckedBinding.Bind(button, b => b.Enabled);

			var visibleCheck = new CheckBox { Text = "Visible" };
			visibleCheck.CheckedBinding.Bind(button, b => b.Visible);

			var withTextCheck = new CheckBox { Text = "With Text" };
			withTextCheck.CheckedBinding
				.Convert(r => r == true ? textBox.Text : null, s => !string.IsNullOrEmpty(s))
				.Bind(button, c => c.Text);
			withTextCheck.CheckedBinding.Bind(textBox, b => b.Enabled);

			var withImageSelection = new RadioButtonList { Items = { "Image", "Large Image", "Plain Image", "No Image" } };
			withImageSelection.SelectedIndexBinding
				.Convert(
					i => i == 0 ? image : i == 1 ? largeImage : i == 2 ? plainImage : null,
					img => img == image ? 0 : img == largeImage ? 1 : img == plainImage ? 2 : 3
					)
				.Bind(button, c => c.Image);

			var performClick = new Button { Text = "PerformClick" };
			performClick.Click += (sender, e) => button.PerformClick();

			var minimumSizeEntry = new SizeEntry();
			minimumSizeEntry.ValueBinding.Bind(button, b => b.MinimumSize);

			var clearMinimumSize = new CheckBox { Text = "Clear MinimumSize" };
			clearMinimumSize.CheckedBinding
				.Convert(ischecked => ischecked == true ? Size.Empty : originalMinimumSize, size => size.IsEmpty)
				.Bind(button, c => c.MinimumSize);
			clearMinimumSize.CheckedBinding.Inverse().Bind(minimumSizeEntry, m => m.Enabled);

			var sizeEntry = new SizeEntry();
			sizeEntry.ValueBinding.Bind(button, b => b.Size, DualBindingMode.OneWayToSource);

			var imagePositionDropDown = new EnumDropDown<ButtonImagePosition>();
			imagePositionDropDown.SelectedValueBinding.Bind(button, c => c.ImagePosition);

			var backgroundColorPicker = new ColorPicker { AllowAlpha = true };
			backgroundColorPicker.ValueBinding.Bind(button, b => b.BackgroundColor);

			var textColorPicker = new ColorPicker { AllowAlpha = true };
			textColorPicker.ValueBinding.Bind(button, b => b.TextColor);

			BeginVertical(padding: 10);
			AddSeparateRow(null, enabledCheck, visibleCheck, null);
			AddSeparateRow(null, "With:", withImageSelection, imagePositionDropDown, null);
			AddSeparateRow(null, withTextCheck, textBox, null);
			AddSeparateRow(null, clearMinimumSize, minimumSizeEntry, null);
			AddSeparateRow(null, "Size:", sizeEntry, null);
			AddSeparateRow(null, "BackgroundColor", backgroundColorPicker, "TextColor", textColorPicker, null);
			AddSeparateRow(null, performClick, null);
			AddAdditionalOptions();
			EndVertical();

			AddAutoSized(button, centered: true);
		}

		protected virtual void LogEvents(T button)
		{
			button.Click += (sender, e) => Log.Write(sender, $"Click");
		}

		protected virtual void AddAdditionalOptions()
		{
		}
	}
}
