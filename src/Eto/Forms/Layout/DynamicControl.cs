namespace Eto.Forms
{
	/// <summary>
	/// Control item for the <see cref="DynamicLayout"/>
	/// </summary>
	[ContentProperty("Control")]
	public class DynamicControl : DynamicItem
	{
		Control control;
		DynamicLayout layout;

		/// <summary>
		/// Creates the content for this item
		/// </summary>
		/// <param name="layout">Top level layout the item is being created for</param>
		public override Control Create(DynamicLayout layout)
		{
			return Control;
		}

		/// <summary>
		/// Gets or sets the control that this item contains
		/// </summary>
		/// <value>The control.</value>
		public Control Control
		{
			get { return control; }
			set
			{
				if (!ReferenceEquals(control, value))
				{
					if (layout != null && control != null)
						layout.RemoveChild(control);
					control = value;
					if (layout != null && control != null)
						layout.AddChild(control);
				}
			}
		}

		internal override System.Collections.Generic.IEnumerable<Control> Controls
		{
			get
			{
				if (Control != null)
					yield return Control;
			}
		}

		internal override void SetParent(DynamicTable table)
		{
			SetLayout(table != null ? table.layout : null);
		}

		internal override void SetLayout(DynamicLayout layout)
		{
			if (Control != null)
			{
				if (this.layout != null)
					this.layout.RemoveChild(Control);
				if (layout != null)
					layout.AddChild(Control);
			}
			this.layout = layout;
		}
	}
}
