using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IToolBarItem : IInstanceWidget
	{
		string ID { get; set; }
	}
	
	public class ToolBarItem : InstanceWidget
	{
		IToolBarItem inner;
		
		public ToolBarItem(Generator g, Type type) : base(g, type)
		{
			inner = (IToolBarItem)Handler;
		}
		
		public string ID
		{
			get { return inner.ID; }
			set { inner.ID = value; }
		}
		
	}


}
