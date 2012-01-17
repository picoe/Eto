using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace Eto
{
	public interface IInstanceWidget : IWidget
	{
		string ID { get; set; }
		object ControlObject { get; }

		void HandleEvent (string handler);
	}
	
	[RuntimeNameProperty("ID")]
	public abstract class InstanceWidget : Widget, IWidget
	{
		IInstanceWidget inner;
		string style;

		public string ID
		{
			get { return inner.ID; }
			set { inner.ID = value; }
		}
		
		public string Style
		{
			get { return style; }
			set
			{
				if (style != value) {
					style = value;
					OnStyleChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler<EventArgs> StyleChanged;
		
		protected virtual void OnStyleChanged(EventArgs e)
		{
			Eto.Style.OnStyleWidget(this);
			if (StyleChanged != null) StyleChanged(this, e);
		}
		
		protected InstanceWidget (Generator generator, IWidget handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
			inner = (IInstanceWidget)Handler;
		}

		protected InstanceWidget (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			inner = (IInstanceWidget)Handler;
		}
		
		public object ControlObject {
			get { return inner.ControlObject; }
		}

		public void HandleEvent (string id)
		{
			inner.HandleEvent (id);
		}
		
		public void HandleEvent (params string[] ids)
		{
			foreach (var id in ids) {
				HandleEvent (id);
			}
		}
	}
}
