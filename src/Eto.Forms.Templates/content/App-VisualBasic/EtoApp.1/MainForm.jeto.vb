#If (UseJeto)
Imports Eto.Forms
Imports Eto.Drawing
Imports Eto.Serialization.Json

Public Class MainForm
	Inherits Form

	Public Sub New()
		JsonReader.Load(Me)
	End Sub
#If IsForm

	Protected Sub HandleClickMe(sender As Object, e As EventArgs)
		MessageBox.Show("I was clicked!")
	End Sub

	Protected Sub HandleAbout(sender As Object, e As EventArgs)
		Dim dlg As New AboutDialog()
		dlg.ShowDialog(Me)
	End Sub

	Protected Sub HandleQuit(sender As Object, e As EventArgs)
		Application.Instance.Quit()
	End Sub
#End If

End Class
#End If