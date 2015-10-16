Imports Eto.Forms

Public Class Program
	<STAThread>
	Public Shared Sub Main(args As String())

		Dim app As New Application(${EtoPlatform})
		app.Run(New MainForm())

	End Sub
End Class