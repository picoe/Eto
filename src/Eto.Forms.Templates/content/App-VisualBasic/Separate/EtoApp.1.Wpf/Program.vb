Imports Eto.Forms

Class Program

	<STAThread>
	Public Shared Sub Main(args As String())
		Dim app As New Application(Eto.Platforms.Wpf)
		' Wpf doesn't follow the system light/dark theme unless asked to
		app.Theme = Themes.System
		app.Run(New MainForm())
	End Sub
End Class