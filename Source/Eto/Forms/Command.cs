using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public class Command
	{
		#region Events

		public event EventHandler<EventArgs> EnabledChanged;

		public virtual void OnEnabledChanged(EventArgs e)
		{
			if (EnabledChanged != null) EnabledChanged(this, e);
		}

		public event EventHandler<EventArgs> Executed;

		public virtual void OnExecuted(EventArgs e)
		{
			if (Executed != null) Executed(this, e);
		}

		public event EventHandler<EventArgs> Removed;

		protected virtual void OnRemoved(EventArgs e)
		{
			if (Removed != null) Removed(this, e);
		}
		
		#endregion

		#region Properties

		public string ID { get; set; }

		bool enabled = true;
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

		public object Tag { get; set; }

		public string MenuText { get; set; }

		public string ToolBarText { get; set; }
		
		public virtual string ToolTip { get; set; }

		public string Description { get; set; }
		
		public bool ShowLabel { get; set; }
		
		public virtual Image Image { get; set; }

		public virtual Keys Shortcut
		{
			get { return (ShortcutKeys != null && ShortcutKeys.Length > 0) ? ShortcutKeys[0] : Keys.None; }
			set { ShortcutKeys = new [] { value }; }
		}
		
		public Keys[] ShortcutKeys { get; set; }
		
		public string ShortcutText
		{
			get
			{
				if (ShortcutKeys == null) return string.Empty;
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

		public Command()
		{
		}

		public Command(EventHandler<EventArgs> execute)
		{
			Executed += execute;
		}

		public Command(string id, string text, string tooltip, EventHandler<EventArgs> execute)
		{
			ID = id;
			MenuText = ToolBarText = text;
			ToolTip = tooltip;
			Executed += execute;
		}
		
		internal void Remove()
		{
			OnRemoved(EventArgs.Empty);
		}
		
		public void Execute()
		{
			OnExecuted(EventArgs.Empty);
		}

		public virtual ToolItem CreateToolItem(Generator generator = null)
		{
			return new ButtonToolItem(this, generator);
		}

		public virtual MenuItem CreateMenuItem(Generator generator = null)
		{
			return new ButtonMenuItem(this, generator);
		}
	}
}
