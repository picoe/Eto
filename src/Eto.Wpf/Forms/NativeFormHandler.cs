namespace Eto.Wpf.Forms
{
	public class NativeFormHandler : WpfWindow<sw.Window, Form, Form.ICallback>, Form.IHandler
	{

		public NativeFormHandler(sw.Window window)
		{
			Control = window;
		}

		protected override bool IsAttached => true;

		public bool ShowActivated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CanFocus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void Show()
		{
			throw new NotImplementedException();
		}
	}
}
