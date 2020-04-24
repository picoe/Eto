using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
    [Section("Behaviors", typeof(Taskbar))]
    public class TaskbarSection : Panel
    {
        EnumDropDown<TaskbarProgressState> dropTaskbarState;
        NumericStepper numericStepper;
        Button buttonSetTaskbar;

        public TaskbarSection()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(15, 6);
            layout.DefaultPadding = new Padding(10);
            layout.BeginVertical();

            layout.BeginHorizontal();
            layout.Add(null, false, false);
            layout.Add(null, true, true);
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(new Label { Text = "State:", VerticalAlignment = VerticalAlignment.Center }, false, false);
            dropTaskbarState = new EnumDropDown<TaskbarProgressState>();
            dropTaskbarState.SelectedValue = TaskbarProgressState.Progress;
            layout.Add(dropTaskbarState, true, false);
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(new Label { Text = "Value:", VerticalAlignment = VerticalAlignment.Center }, false, false);
            numericStepper = new NumericStepper();
            numericStepper.MinValue = 0;
            numericStepper.MaxValue = 100;
            numericStepper.Value = 50;
            layout.Add(numericStepper, true, false);
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(null, false, false);
            buttonSetTaskbar = new Button();
            buttonSetTaskbar.Text = "Set Taskbar";
            layout.Add(buttonSetTaskbar, true, false);
            layout.EndHorizontal();

            layout.BeginHorizontal();
            layout.Add(null, false, false);
            layout.Add(null, true, true);
            layout.EndHorizontal();

            Content = layout;

            buttonSetTaskbar.Click += ButtonShowNot_Click;
        }

        private void ButtonShowNot_Click(object sender, EventArgs e)
        {
            Taskbar.SetProgress(dropTaskbarState.SelectedValue, (float)numericStepper.Value / 100f);
        }
    }
}
