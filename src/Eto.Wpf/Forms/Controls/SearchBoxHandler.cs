using Xceed.Wpf.Toolkit;
using static Eto.Win32;
namespace Eto.Wpf.Forms.Controls
{
	public class EtoSearchTextBox : EtoWatermarkTextBox
	{
		static readonly sw.DependencyPropertyKey IsEmptyPropertyKey =
				sw.DependencyProperty.RegisterReadOnly("IsEmpty", typeof(bool), typeof(EtoSearchTextBox), new sw.PropertyMetadata(true));

		public static readonly sw.DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;


		public static readonly sw.DependencyProperty ClearCommandProperty = sw.DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(EtoSearchTextBox));

		public bool IsEmpty => (bool)GetValue(IsEmptyProperty);

		protected override void OnTextChanged(swc.TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			SetValue(IsEmptyPropertyKey, string.IsNullOrEmpty(Text));
		}

		public EtoSearchTextBox()
		{
			ClearCommand = new RelayCommand(Clear);
		}

		public ICommand ClearCommand
		{
			get => (ICommand)GetValue(ClearCommandProperty);
			set => SetValue(ClearCommandProperty, value);
		}

	}

	public class SearchBoxHandler : TextBoxHandler<xwt.WatermarkTextBox, TextBox, TextBox.ICallback>, SearchBox.IHandler
	{
		internal static object CurrentText_Key = new object();
		internal static object CurrentSelection_Key = new object();
		internal static object DisableTextChanged_Key = new object();

		protected override swc.TextBox TextBox => Control;

		protected override WatermarkTextBox CreateControl() => new EtoSearchTextBox { Handler = this, KeepWatermarkOnGotFocus = true };

		public override string PlaceholderText
		{
			get => Control.Watermark as string;
			set => Control.Watermark = value;
		}
	}
}
