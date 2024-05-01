namespace Eto.GtkSharp.Forms.ToolBar
{
	public interface IToolBarItemHandler
	{
		void CreateControl(ToolBarHandler handler, int index);
		
	}

	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler, IToolBarItemHandler
		where TControl: Gtk.ToolItem
		where TWidget: ToolItem
	{
		bool enabled = true;
		bool visible = true;
		string text;
		string toolTip;
		Image image;

		protected Gtk.Image GtkImage { get; set; }

		public abstract void CreateControl(ToolBarHandler handler, int index);
		
		public string Text
		{
			get => text;
			set
			{
				if (text != value)
				{
					text = value;
					SetText();
				}
			}
		}

		public virtual string ToolTip
		{
			get => toolTip;
			set 
			{
				if (toolTip != value)
				{
					toolTip = value;
					SetToolTip();
				}
			}
		}

		protected virtual void SetText()
		{
			if (Control is Gtk.ToolButton button)
				button.Label = Text;
		}
		
		protected virtual void SetToolTip()
		{
			if (Control is Gtk.ToolButton button)
				button.TooltipText = ToolTip;
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				GtkImage = image.ToGtk (Gtk.IconSize.Button);
			}
		}

		public bool Enabled 
		{
			get { return enabled; }
			set { 
				enabled = value;
				if (Control != null)
					Control.Sensitive = value;
			}
		}

		public bool Visible
		{
			get => visible;
			set
			{
				visible = value;
				if (Control != null)
					Control.Visible = value;
			}
		}

		public void CreateFromCommand(Command command)
		{
		}

		public void OnLoad(System.EventArgs e)
		{
		}

		public void OnPreLoad(System.EventArgs e)
		{
		}

		public void OnUnLoad(System.EventArgs e)
		{
		}
	}
}
