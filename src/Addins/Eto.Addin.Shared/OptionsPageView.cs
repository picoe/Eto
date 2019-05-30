using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.Shared
{
	public class OptionsPageView : BasePageView
	{
		public OptionsPageView(OptionsPageModel model)
		{
			var layout = new DynamicLayout();

			var infoLayout = new StackLayout { Spacing = 10 };

			foreach (var option in model.Options)
			{
				var currentOption = option;
				option.Selected = option.Values.FirstOrDefault();
                var infoLabel = new Label { Text = option.Selected?.Description };
				infoLayout.Items.Add(infoLabel);

                layout.Add(new Label { Text = option.Name });
				layout.BeginVertical();
				layout.BeginHorizontal();

				layout.Add(new Panel { Size = new Size(40, -1) }, xscale: false);

				var radioList = new RadioButtonList();
				radioList.Orientation = Orientation.Vertical;
				radioList.ItemTextBinding = Binding.Property((OptionValue v) => v.Name);
				radioList.DataStore = option.Values;
				radioList.SelectedValueChanged += (sender, e) =>
				{
					currentOption.Selected = radioList.SelectedValue as OptionValue;
					infoLabel.Text = currentOption.Selected?.Description;
				};
				radioList.SelectedIndex = 0;
				layout.Add(radioList);

				layout.EndHorizontal();
				layout.EndVertical();
			}

			Information = infoLayout;
            Content = layout;
		}
	}
}
