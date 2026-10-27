namespace Eto.GtkSharp.Forms.Controls
{
	public class CheckBoxHandler : GtkControl<Gtk.CheckButton, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		readonly Gtk.EventBox box;
		bool _alwaysShowMnemonic;

		public override Gtk.Widget ContainerControl => box;

		protected override Gtk.Widget FontControl => Control.Child ?? new Gtk.Label();

		public CheckBoxHandler()
		{
			Control = new Gtk.CheckButton { UseUnderline = true };
			box = new Gtk.EventBox { Child = Control };
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Toggled += Connector.HandleToggled;
		}

		protected new CheckBoxConnector Connector { get { return (CheckBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CheckBoxConnector();
		}

		protected class CheckBoxConnector : GtkControlConnector
		{
			bool toggling;
			public new CheckBoxHandler Handler { get { return (CheckBoxHandler)base.Handler; } }

			public void HandleToggled(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				var c = h.Control;
				if (toggling)
					return;

				toggling = true;
				try
				{
					if (h.ThreeState)
					{
						if (!c.Inconsistent && c.Active)
							c.Inconsistent = true;
						else if (c.Inconsistent)
						{
							c.Inconsistent = false;
							c.Active = true;
						}
					}
					h.Callback.OnCheckedChanged(h.Widget, EventArgs.Empty);
				}
				finally
				{
					toggling = false;
				}
			}
		}

		public override string Text
		{
			get { return Control.Label.ToEtoMnemonic(); }
			set {
				var needsFont = Control.Child == null && Widget.Properties.ContainsKey(GtkControl.Font_Key);
				Control.Label = Control.UseUnderline ? value.ToPlatformMnemonic() : value;
				var label = Control.Child as Gtk.Label;
				if (label != null)
					label.Pattern = Control.UseUnderline && _alwaysShowMnemonic ? GtkMnemonicHelper.ToPatternWithMnemonicUnderline(value) : null;
				if (needsFont)
					Control.Child?.SetFont(Font.ToPango());
			}
		}

		public bool? Checked
		{
			get { return Control.Inconsistent ? null : (bool?)Control.Active; }
			set
			{
				// GTK raises Toggled for assignments too; only clicks should advance the three-state cycle.
				var oldValue = Checked;

				Control.Toggled -= Connector.HandleToggled;

				try
				{
					Control.Inconsistent = value == null;

					if (value != null)
						Control.Active = value.Value;
				}
				finally
				{
					Control.Toggled += Connector.HandleToggled;
				}

				if (Checked != oldValue)
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			}
		}

		public bool ThreeState
		{
			get;
			set;
		}

#if GTK3
		Gtk.Widget TextColorWidget => Control;
#else
		Gtk.Widget TextColorWidget => Control.Child ?? Control;
#endif

		public Color TextColor
		{
			get { return TextColorWidget.GetForeground(); }
			set
			{
				var child = TextColorWidget;
				child.SetForeground(value, GtkStateFlags.Normal);
				child.SetForeground(value, GtkStateFlags.Active);
				child.SetForeground(value, GtkStateFlags.Prelight);
			}
		}

		public bool UseMnemonic
		{
			get => Control.UseUnderline;
			set
			{
				if (value == Control.UseUnderline)
					return; // no change
				var text = Text;
				Control.UseUnderline = value;
				Text = text;
			}
		}
		
		public bool AlwaysShowMnemonic
		{
			get => _alwaysShowMnemonic;
			set
			{
				if (_alwaysShowMnemonic == value)
					return;
				_alwaysShowMnemonic = value;
				Text = Text;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
