using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public interface ICopyFromAction
	{
		void CopyFrom(BaseAction action);
	}

	/// <summary>
	/// The base class for menu and toolbar items.
	/// Holds the model properties of these items,
	/// and exposes events such as Clicked.
	/// 
	/// This is not an abstract class. When instantiated
	/// directly, the use case is to set model properties
	/// and call CreateMenuItem or CreateToolbarItem. One
	/// scenario that uses this is Application.GetSystemActions(),
	/// which returns a list of system actions. The desired
	/// system actions can be turned into menu or toolbar items
	/// as needed.
	/// 
	/// CreateMenuItem and CreateToolbarItem call CopyFrom, which
	/// derived types can extend to copy properties defined
	/// by them. If a handler implements ICopyFromAction, it
	/// can also copy its extended properties.
	/// </summary>
	public class BaseAction : InstanceWidget
	{
		#region Members
		
		bool enabled = true;
		
		#endregion
		
		#region Events

		public event EventHandler<EventArgs> Clicked;
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
			this.Clicked = activated;
			this.Accelerators = accelerators;
		}

		#endregion

		#region Properties

		string id;
		public override string ID 
		{
			get { return id; }
			set { this.id = value; }
		}

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

		public virtual Keys Shortcut
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
				if (Shortcut != Keys.None)
				{
					string val = string.Empty;
					Keys modifier = (Shortcut & Keys.ModifierMask);
					if (modifier != Keys.None) val += modifier.ToString();
					Keys mainKey = (Shortcut & Keys.KeyMask);
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
			Clicked += activated;
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
			this.Shortcut = a.Shortcut;
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
		
		public void OnClick(EventArgs e)
		{
			if (Clicked != null) Clicked(this, e);
		}

		public void Activate()
		{
			OnClick(EventArgs.Empty);
		}

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
