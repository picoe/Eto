namespace Eto.Wpf.Forms;

public class FloatingFormHandler : FormHandler, FloatingForm.IHandler
{
	static readonly object Visible_Key = new object();
	
	bool _wasActive;

	protected override void Initialize()
	{
		base.Initialize();
		// defaults for a floating form
		Maximizable = false;
		Minimizable = false;
		ShowInTaskbar = false;
		Topmost = true;
	}

	public override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		Application.Instance.IsActiveChanged += Application_IsActiveChanged;
		if (!Application.Instance.IsActive)
			base.Visible = false;
	}

	public override void OnUnLoad(EventArgs e)
	{
		base.OnUnLoad(e);
		Application.Instance.IsActiveChanged -= Application_IsActiveChanged;
	}

	private void Application_IsActiveChanged(object sender, EventArgs e)
	{
		SetVisibility(true);
	}
	
	public override bool Visible
	{
		get => Widget.Properties.Get<bool>(Visible_Key, false);
		set
		{
			Widget.Properties.Set(Visible_Key, value, false);
			SetVisibility(false);
		}
	}
	
	public override void Show()
	{
		Widget.Properties.Set(Visible_Key, true, false);
		base.Show();
	}
	
	void SetVisibility(bool setActive)
	{
		var currentlyVisible = base.Visible;
		var isVisible = Application.Instance.IsActive && Visible;
		if (isVisible == currentlyVisible)
			return;
			
		if (!isVisible)
		{
			if (currentlyVisible)
			{
				_wasActive = Win32.GetThreadFocusWindow() == NativeHandle;
			}
			base.Visible = isVisible;
		}
		else if (setActive)
		{
			var oldShowActivated = Control.ShowActivated;
			Control.ShowActivated = _wasActive;
			base.Visible = isVisible;
			Control.ShowActivated = oldShowActivated;
		}
		else
			base.Visible = isVisible;
	}
}