Imports Eto.Forms

Class Program

	<STAThread>
	Public Shared Sub Main(args As String())
		Dim app As New Application(Eto.Platform.Detect)
		' Wpf doesn't follow the system light/dark theme unless asked to
		If app.Platform.IsWpf Then
			app.Theme = Themes.System
		End If
		app.Run(New MainForm())
	End Sub
End Class