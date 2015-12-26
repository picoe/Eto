Imports Eto.Forms
Imports Eto.Drawing

Public Class MyPanel
  Inherits Panel

  Public Sub New()

	Dim label As New Label()
	label.Text = "Some Content"

	Me.Content = label
  End Sub

End Class