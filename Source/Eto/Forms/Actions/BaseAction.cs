using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public interface ICopyFromAction
	{
		void CopyFrom(BaseAction action);
	}

	public class BaseAction : InstanceWidget
	{
		#region Members
		
		bool enabled = true;
		
		#endregion
		
		#region Events
		
		public event EventHandler<EventArgs> Activated;
		public event EventHandler<EventArgs> EnabledChanged;
		public event EventHandler<EventArgs> Removed;
		
		protected virtual void OnRemoved(EventArgs e)
		{
			if (Removed != null) Removed(this, e);
		}
		
		#endregion

		#region Constructors

		protected BaseAction (Generator g, Type type, bool initialize = true) 
			: base(g, type, initialize)
		{
		}

		public BaseAction(string id, string text, EventHandler<EventArgs> activated, params Keys[] accelerators)
			: base(null, null as IWidget, initialize: false)
		{
			this.ID = id;
			this.text = text;
			this.Activated = activated;
			this.Accelerators = accelerators;
		}

		#endregion

		#region Properties

		public virtual bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					OnEnabledChanged(EventArgs.Empty);
				}
			}
		}

		public int Order { get; set; }

		public object Tag { get; set; }

		string text;
		public virtual string Text
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}|{3}", MenuText, ToolBarText, ToolTip, Description);
			}
			set
			{
				MenuText = ToolBarText = value;				
				string[] vals = value.Split('|');
				if (vals.Length > 0) MenuText = vals[0];
				if (vals.Length > 1) ToolBarText = vals[1];
				if (vals.Length > 2) ToolTip = vals[2];
				if (vals.Length > 3) Description = vals[3];
			}
		}

		public string MenuText { get; set; }
		public string ToolBarText { get; set; }
		
		public virtual string ToolTip { get; set; }
		public string Description { get; set; }
		
		public bool ShowLabel { get; set; }
		
		public virtual Image Image { get; set; }

		public Keys Accelerator
		{
			get { return (Accelerators != null && Accelerators.Length > 0) ? Accelerators[0] : Keys.None; }
			set { Accelerators = new Keys[] { value }; }
		}
		
		public Keys[] Accelerators { get; set; }
		
		public string AcceleratorText
		{
			get
			{
				if (Accelerators == null) return string.Empty;
				if (Accelerator != Keys.None)
				{
					string val = string.Empty;
					Keys modifier = (Accelerator & Keys.ModifierMask);
					if (modifier != Keys.None) val += modifier.ToString();
					Keys mainKey = (Accelerator & Keys.KeyMask);
					if (mainKey != Keys.None)
					{
						if (val.Length > 0) val += "+";
						val += mainKey.ToString();
					}
					return val;
				}
				return string.Empty;
			}
		}
		
		#endregion
		
		protected BaseAction(string id, string text, Image image, EventHandler<EventArgs> activated)
			: this(id, text, activated)
		{
			this.Image = image;

		}

		protected BaseAction(string id, string text, EventHandler<EventArgs> activated)
			: this(id, text)
		{
			Activated += activated;
		}
		
		protected BaseAction(string id, string text) : base(null, null as IWidget)
		{
			this.ID = id;
			this.Text = text;
		}
		
		protected BaseAction() : base(null, null as IWidget)
		{
		}

		public virtual void CopyFrom(BaseAction a)
		{
			this.Accelerator = a.Accelerator;
			this.Accelerators = a.Accelerators;
			this.enabled = a.Enabled;
			this.ID = a.ID;
			this.Image = a.Image;
			this.Order = a.Order;
			this.ShowLabel = a.ShowLabel;
			this.Style = a.Style;
			this.Tag = a.Tag;			
			this.Text = a.Text;
			// set Text-derived properties after Text is set for consistency, although this order should not matter.
			this.Description = a.Description;
			this.MenuText = a.MenuText;
			this.ToolBarText = a.ToolBarText;
			this.ToolTip = a.ToolTip;

			var c = this.Handler as ICopyFromAction;
			if (c != null)
				c.CopyFrom(a);
		}

		internal void Remove()
		{
			OnRemoved(EventArgs.Empty);
		}
		
		public virtual void OnEnabledChanged(EventArgs e)
		{
			if (EnabledChanged != null) EnabledChanged(this, e);
		}
		

		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) Activated(this, e);
		}

		public void Activate()
		{
			OnActivated(EventArgs.Empty);
		}

#if MENU_TOOLBAR_REFACTORING
		public virtual ToolBarItem GenerateToolBarItem(ActionItem actionItem, Generator generator, ToolBarTextAlign textAlign)
		{
			throw new NotImplementedException();
		}
#endif

		public MenuItem CreateMenuItem()
		{
			var result = new ImageMenuItem();
			result.CopyFrom(this);
			return result;
		}

		public ToolBarItem CreateToolBarItem()
		{
			var result = new ToolBarButton();
			result.CopyFrom(this);
			return result;
		}
	}
}
