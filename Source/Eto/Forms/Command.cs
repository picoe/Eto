using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public class CheckCommand : Command
	{
		#region Events

		public event EventHandler<EventArgs> CheckedChanged;

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

		#endregion

		#region Properties

		bool ischecked;

		public bool Checked
		{
			get { return ischecked; }
			set
			{
				if (ischecked != value)
				{
					ischecked = value;
					OnCheckedChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		public CheckCommand()
		{
		}

		public CheckCommand(EventHandler<EventArgs> execute)
			: base(execute)
		{
		}

		public override MenuItem CreateMenuItem(Generator generator = null)
		{
			return new CheckMenuItem(this, generator);
		}

		public override ToolItem CreateToolItem(Generator generator = null)
		{
			return new CheckToolItem(this, generator);
		}
	}

	public class RadioCommand : CheckCommand
	{
		public RadioCommand Controller { get; set; }
		RadioMenuItem menuItem;

		public RadioCommand()
		{
		}

		public RadioCommand(EventHandler<EventArgs> execute)
			: base(execute)
		{
		}

		public override MenuItem CreateMenuItem(Generator generator = null)
		{
			return menuItem = new RadioMenuItem(this, Controller != null ? Controller.menuItem : null, generator);
		}

		public override ToolItem CreateToolItem(Generator generator = null)
		{
			throw new NotSupportedException();
		}
	}

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

		public bool ShowLabel { get; set; }
		
		public virtual Image Image { get; set; }

		public virtual Keys Shortcut { get; set; }

		#endregion

		public Command()
		{
		}

		public Command(EventHandler<EventArgs> execute)
		{
			Executed += execute;
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
