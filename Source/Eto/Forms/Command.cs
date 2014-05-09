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

		#pragma warning disable 672,612,618

		public override MenuItem CreateMenuItem(Generator generator)
		{
			return new CheckMenuItem(this);
		}

		public override ToolItem CreateToolItem(Generator generator)
		{
			return new CheckToolItem(this);
		}

		#pragma warning restore 672,612,618
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

		#pragma warning disable 672,612,618

		public override MenuItem CreateMenuItem(Generator generator)
		{
			return menuItem = new RadioMenuItem(this, Controller != null ? Controller.menuItem : null);
		}

		public override ToolItem CreateToolItem(Generator generator)
		{
			throw new NotSupportedException();
		}

		#pragma warning restore 672,612,618
	}

	public class Command
	{
		#region Events

		public event EventHandler<EventArgs> EnabledChanged;

		protected virtual void OnEnabledChanged(EventArgs e)
		{
			if (EnabledChanged != null) EnabledChanged(this, e);
		}

		public event EventHandler<EventArgs> Executed;

		protected virtual void OnExecuted(EventArgs e)
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
		
		public string ToolTip { get; set; }

		public bool ShowLabel { get; set; }
		
		public Image Image { get; set; }

		public Keys Shortcut { get; set; }

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


		#pragma warning disable 612,618

		public virtual ToolItem CreateToolItem()
		{
			return CreateToolItem(null);
		}

		public virtual MenuItem CreateMenuItem()
		{
			return CreateMenuItem(null);
		}

		[Obsolete("Use variation without generator instead")]
		public virtual ToolItem CreateToolItem(Generator generator)
		{
			return new ButtonToolItem(this, generator);
		}

		[Obsolete("Use variation without generator instead")]
		public virtual MenuItem CreateMenuItem(Generator generator)
		{
			return new ButtonMenuItem(this, generator);
		}

		#pragma warning restore 612,618
	}
}
