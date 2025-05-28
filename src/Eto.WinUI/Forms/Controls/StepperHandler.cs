using Eto.WinUI.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.WinUI.Forms.Controls;

public class StepperHandler : WinUIControl<SpinButton, Stepper, Stepper.ICallback>, Stepper.IHandler
{
	StepperValidDirections _validDirection = StepperValidDirections.Both;
	public StepperValidDirections ValidDirection
	{
		get => _validDirection;
		set
		{
			_validDirection = value;
			Control.UpEnabled = value.HasFlag(StepperValidDirections.Up);
			Control.DownEnabled = value.HasFlag(StepperValidDirections.Down);
		}
	}

	protected override SpinButton CreateControl() => new();
}
