using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public abstract partial class BaseAction
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
		
		public string ID { get; set; }
		
		public object Tag { get; set; }
		
		public string Text
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}|{3}", MenuText, ToolBarText, TooltipText, Description);
			}
			set
			{
				string[] vals = value.Split('|');
				if (vals.Length > 0) MenuText = vals[0];
				if (vals.Length > 1) ToolBarText = vals[1];
				if (vals.Length > 2) TooltipText = vals[2];
				if (vals.Length > 3) Description = vals[3];
			}
		}
		
		public string MenuText { get; set; }

		public string MenuItemStyle { get; set; }
		
		public string ToolBarText { get; set; }
		
		public string ToolBarItemStyle { get; set; }
		
		public string TooltipText { get; set; }
		
		public string Description { get; set; }
		
		public bool ShowLabel { get; set; }
		
		public Image Image { get; set; }

		[Obsolete ("Use Image")]
		public Icon Icon
		{
			get { return Image as Icon; }
			set { Image = value; }
		}

		public Key Accelerator
		{
			get { return (Accelerators != null && Accelerators.Length > 0) ? Accelerators[0] : Key.None; }
			set { Accelerators = new Key[] { value }; }
		}
		
		public Key[] Accelerators { get; set; }
		
		public string AcceleratorText
		{
			get
			{
				if (Accelerators == null) return string.Empty;
				if (Accelerator != Key.None)
				{
					string val = string.Empty;
					Key modifier = (Accelerator & Key.ModifierMask);
					if (modifier != Key.None) val += modifier.ToString();
					Key mainKey = (Accelerator & Key.KeyMask);
					if (mainKey != Key.None)
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
		
		protected BaseAction(string id, string text)
		{
			this.ID = id;
			this.Text = text;
		}
		
		protected BaseAction()
		{
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

		public abstract ToolBarItem GenerateToolBarItem(ActionItem actionItem, Generator generator, ToolBarTextAlign textAlign);
	}

}
