Imports Eto.Forms

Class Program

	<STAThread>
	Public Shared Sub Main(args As String())
		Dim app As New Application(Eto.Platforms.Mac64)
		app.Run(New MainForm())
	End Sub
End Class