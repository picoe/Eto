using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Serialization.Xaml;
using Eto.Forms;
using System.IO;

namespace Eto.Designer.Builders
{
	public class XamlInterfaceBuilder : IInterfaceBuilder
	{
		public void Create(string text, Action<Forms.Control> controlCreated, Action<string> error)
		{
			try
			{
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""), false))
				{
					stream.Position = 0;
					var control = XamlReader.Load<Panel>(stream);
					if (control != null)
						controlCreated(control);
				}
			}
			catch (Exception ex)
			{
				error(ex.ToString());
			}
		}

		public string GetSample()
		{
			return @"<Scrollable
	   xmlns=""http://schema.picoe.ca/eto.forms"" 
	   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	<TableLayout Padding=""10"" Spacing=""5,5"">
		<TableRow>
			<TableCell>
				<TableLayout Spacing=""5,5"">
					<TableRow>
						<TableCell><Label Text=""TextBox"" /></TableCell>
						<TableCell><TextBox /></TableCell>
					</TableRow>
					<TableRow>
						<TableCell><Label Text=""TextArea"" /></TableCell>
						<TableCell><TextArea /></TableCell>
					</TableRow>
					<TableRow>
						<TableCell />
						<TableCell><CheckBox Text=""Some check box"" /></TableCell>
					</TableRow>
					<TableRow>
						<TableCell />
						<TableCell><Slider SnapToTick=""true"" /></TableCell>
					</TableRow>
				</TableLayout>
			</TableCell>
		</TableRow>
		<TableRow>
			<TableCell>
				<TableLayout Spacing=""5, 5"">
					<TableRow>
						<TableCell ScaleWidth=""true""/>
						<TableCell><Button Text=""Cancel"" /></TableCell>
						<TableCell><Button Text=""Apply"" /></TableCell>
					</TableRow>
				</TableLayout>
			</TableCell>
		</TableRow>
		<TableRow/>
	</TableLayout>
</Scrollable>";
		}
	}
}
