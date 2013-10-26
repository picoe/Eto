using System;

namespace Eto.Forms
{
	public interface ICheckToolBarButton : IToolBarActionItem
	{
		bool Checked { get; set; }
	}
	
	public class CheckToolBarButton : ToolBarActionItem
	{
		new ICheckToolBarButton Handler { get { return (ICheckToolBarButton)base.Handler; } }
		
		public event EventHandler<EventArgs> Click;
		public event EventHandler<EventArgs> CheckedChanged;
		
		public CheckToolBarButton(Generator g) : base(g, typeof(ICheckToolBarButton))
		{
		}
		
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
		
		public void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}
		
		public void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null) CheckedChanged(this, e);
		}
		
	}
	
	
}
