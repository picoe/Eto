using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ICheckToolBarButton : IToolBarActionItem
	{
		bool Checked { get; set; }
	}
	
	public class CheckToolBarButton : ToolBarActionItem
	{
		ICheckToolBarButton inner;
		
		public event EventHandler<EventArgs> Click;
		public event EventHandler<EventArgs> CheckedChanged;
		
		public CheckToolBarButton(Generator g) : base(g, typeof(ICheckToolBarButton))
		{
			inner = (ICheckToolBarButton)Handler;
		}
		
		public bool Checked
		{
			get { return inner.Checked; }
			set { inner.Checked = value; }
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
