using System;
using System.ComponentModel;
#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto
{
	public interface IInstanceWidget : IWidget
	{
		string ID { get; set; }
		object ControlObject { get; }

		void HandleEvent (string handler);
	}
	
#if DESKTOP
	[RuntimeNameProperty("ID")]
#endif
	public abstract class InstanceWidget : Widget
	{
		IInstanceWidget handler;
		string style;

		public string ID
		{
			get { return handler.ID; }
			set { handler.ID = value; }
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
			this.handler = (IInstanceWidget)Handler;
		}

		protected InstanceWidget (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			this.handler = (IInstanceWidget)Handler;
		}
		
		public object ControlObject {
			get { return handler.ControlObject; }
		}

		public void HandleEvent (string id)
		{
			handler.HandleEvent (id);
		}
		
		public void HandleEvent (params string[] ids)
		{
			foreach (var id in ids) {
				HandleEvent (id);
			}
		}
	}
}
