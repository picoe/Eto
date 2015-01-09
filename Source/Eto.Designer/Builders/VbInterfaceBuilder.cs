using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.VisualBasic;
using System.Collections.Generic;


namespace Eto.Designer.Builders
{
	public class VbInterfaceBuilder : CodeInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			var options = new Dictionary<string, string> { { "CompilerVersion", "v4.5" } };
			return new VBCodeProvider(options);
		}

		protected override void SetParameters(CompilerParameters parameters)
		{
			base.SetParameters(parameters);
			parameters.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
		}

		public override string GetSample()
		{
			return @"Imports Eto.Forms
Imports Eto.Drawing

Public Class MyPanel 
	Inherits Scrollable

	Public Sub New()
		Dim formLayout As New TableLayout

		With formLayout
			.Spacing = New Size(5, 5)
			.Rows.Add(New TableRow(New TableCell(LabelWithText(""TextBox"")), New TableCell(New TextBox())))
			.Rows.Add(New TableRow(New TableCell(LabelWithText(""TextArea"")), New TableCell(New TextArea())))
			.Rows.Add(New TableRow(New TableCell, New TableCell(CheckBoxWithText(""Some check box""))))
			.Rows.Add(New TableRow(New TableCell, New TableCell(New Slider)))
		End With

		Dim buttonLayout = new TableLayout
		With buttonLayout
			.Spacing = new Size(5, 5)
			.Rows.Add(new TableRow(
				Nothing,
				New TableCell(ButtonWithText(""Cancel"")),
				New TableCell(ButtonWithText(""Apply""))
			))
		End With

		Dim mainLayout = new TableLayout
		With mainLayout
			.Padding = new Padding(10)
			.Spacing = new Size(5, 5)
			.Rows.Add(New TableRow(New TableCell(formLayout)))
			.Rows.Add(New TableRow(New TableCell(buttonLayout)))
			.Rows.Add(Nothing)
		End With

		Me.Content = mainLayout
	End Sub

	' Helpers to create controls as mono's VB.NET compiler does not support With { } initializer pattern.

	Function LabelWithText(ByVal text as string) As Label
		Dim label as New Label
		label.Text = text
		Return label
	End Function

	Function CheckBoxWithText(ByVal text as string) As CheckBox
		Dim check as New CheckBox
		check.Text = text
		Return check
	End Function

	Function ButtonWithText(ByVal text as string) As Button
		Dim button as New Button
		button.Text = text
		Return button
	End Function
End Class
";
		}
	}
}

