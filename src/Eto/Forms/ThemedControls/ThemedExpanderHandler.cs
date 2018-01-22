using System;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="Expander"/> control.
	/// </summary>
	public class ThemedExpanderHandler : ThemedContainerHandler<StackLayout, Expander, Expander.ICallback>, Expander.IHandler
	{
		Panel header;
		Button toggle;
		Panel content;

		static readonly object ExpandedButtonText_Key = new object();

		/// <summary>
		/// Gets or sets the text of the button when <see cref="Expanded"/> is <c>true</c>.
		/// </summary>
		public string ExpandedButtonText
		{
			get { return Widget.Properties.Get<string>(ExpandedButtonText_Key, "\x25B2"); }
			set { Widget.Properties.Set(ExpandedButtonText_Key, value, SetButtonText); }
		}

		static readonly object CollapsedButtonText_Key = new object();

		/// <summary>
		/// Gets or sets the text of the button when <see cref="Expanded"/> is <c>false</c>.
		/// </summary>
		public string CollapsedButtonText
		{
			get { return Widget.Properties.Get<string>(CollapsedButtonText_Key, "\x25BC"); }
			set { Widget.Properties.Set(CollapsedButtonText_Key, value, SetButtonText); }
		}

		/// <summary>
		/// Initializes a new instance of the ThemedExpanderHandler.
		/// </summary>
		public ThemedExpanderHandler()
		{
			header = new Panel();
			toggle = new Button();
			toggle.Size = new Size(24, 24);
			content = new Panel();
			content.Visible = false;

			var headerLayout = new StackLayout
			{
				VerticalContentAlignment = VerticalAlignment.Center,
				Orientation = Orientation.Horizontal,
				Items = { toggle, header }
			};

			Control = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items = { headerLayout, new StackLayoutItem(content, expand: true) }
			};

			toggle.Click += (sender, e) => Expanded = !Expanded;
			header.MouseDown += (sender, e) =>
			{
				Expanded = !Expanded;
				e.Handled = true;
			};
		}

		/// <summary>
		/// Initializes the control after attached to a widget.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			SetButtonText();
		}

		void SetButtonText()
		{
			toggle.Text = Expanded ? ExpandedButtonText : CollapsedButtonText;
		}

		/// <summary>
		/// Gets or sets whether the expander content is visible (expanded).
		/// </summary>
		public bool Expanded
		{
			get { return content.Visible; }
			set
			{
				if (content.Visible != value)
				{
					content.Visible = value;
					SetButtonText();
					Callback.OnExpandedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the content of the header
		/// </summary>
		public virtual Control Header
		{
			get { return header.Content; }
			set { header.Content = value; }
		}

		/// <summary>
		/// Gets or sets the content of the control when expanded.
		/// </summary>
		public virtual Control Content
		{
			get { return content.Content; }
			set { content.Content = value; }
		}

		/// <summary>
		/// Gets or sets the padding around the <see cref="Content"/>.
		/// </summary>
		public Padding Padding
		{
			get { return content.Padding; }
			set { content.Padding = value; }
		}

		/// <summary>
		/// Gets or sets the minimum size of this control.
		/// </summary>
		public Size MinimumSize
		{
			get { return Control.MinimumSize; }
			set { Control.MinimumSize = value; }
		}

		/// <summary>
		/// Gets or sets the context menu of the expander.
		/// </summary>
		public ContextMenu ContextMenu
		{
			get { return Control.ContextMenu; }
			set { Control.ContextMenu = value; }
		}

		/// <summary>
		/// Attaches the handler events.
		/// </summary>
		/// <param name="id">Identifier of the event to attach</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Expander.ExpandedChangedEvent:
					// handled explicitly
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
