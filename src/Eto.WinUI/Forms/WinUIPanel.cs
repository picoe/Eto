using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinUI.Forms
{
	public abstract class WinUIPanel<TControl, TWidget, TCallback> : WinUIContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl : class
		where TWidget : Panel
		where TCallback : Panel.ICallback
	{
		Control _content;
		public virtual Control Content
		{
			get => _content;
			set
			{
				if (_content != value)
				{
					_content = value;
					_border.Child = _content.ToWinUI();
				}
			}
		}
		public Padding Padding { get; set; }
		public Size MinimumSize { get; set; }
		public ContextMenu ContextMenu { get; set; }

		muc.Border _border;

		public WinUIPanel()
		{
			_border = new muc.Border
			{
				AllowFocusOnInteraction = false
			};
		}
		public override mux.FrameworkElement ContainerControl => _border;

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(_border);
		}

		public abstract void SetContainerContent(mux.FrameworkElement content);

	}
}
