using System;

namespace Eto.Forms
{
	public interface ICheckToolItem : IToolItem
	{
		bool Checked { get; set; }
	}
	
	public class CheckToolItem : ToolItem
	{
		new ICheckToolItem Handler { get { return (ICheckToolItem)base.Handler; } }
		
		public event EventHandler<EventArgs> CheckedChanged;
		
		public CheckToolItem(Generator g) : base(g, typeof(ICheckToolItem))
		{
		}
		
		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
		
		public void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null) CheckedChanged(this, e);
		}
	}
}
