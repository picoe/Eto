#If (UseCodePreview)
Imports Eto.Forms
Imports Eto.Drawing

Partial Class MainForm
	Inherits Form

	Public Sub InitializeComponent()
#If IsWindow
		Title = "My Eto Form"
		MinimumSize = New Size(200, 200)
#End If
		Padding = 10

		Content = New StackLayout(
			"Hello World!" ' Add more controls here
		)

#If IsForm
		' create a few commands that can be used for the menu and toolbar
		Dim clickMe As New Command With { .MenuText = "Click Me!", .ToolbarText = "Click Me!" }
		AddHandler clickMe.Executed, Sub() MessageBox.Show(Me, "I was clicked!")

		Dim quitCommand As New Command With { .MenuText = "Quit", .Shortcut = Application.Instance.CommonModifier Or Keys.Q }
		AddHandler quitCommand.Executed, Sub() Application.Instance.Quit()

		Dim aboutCommand As New Command With { .MenuText = "About..." }
		AddHandler aboutCommand.Executed, Sub()
			Dim about As New AboutDialog()
			about.ShowDialog(Me)
		End Sub

		' create menu
		Dim menuBar = New MenuBar() With {
			.QuitItem = quitCommand,
			.AboutItem = aboutCommand
        }

		' File submenu
		Dim fileMenu As New ButtonMenuItem With { .Text = "&File" }
		fileMenu.Items.Add(clickMe)
		menuBar.Items.Add(fileMenu)

		'Dim editMenu As New ButtonMenuItem With { .Text = "&Edit" }
		'editMenu.Items.Add([command/item])

		'Dim viewMenu as New ButtonMenuItem With { .Text = "&View" }
		'viewMenu.Items.Add([command/item])

		' application (OS X) or file menu (others)
		Dim preferencesMenu As New ButtonMenuItem With { .Text = "&Preferences..." }
		menuBar.ApplicationItems.Add(preferencesMenu)

		Me.Menu = menuBar

		' Create toolbar
		Dim toolBar As New ToolBar()
		toolBar.Items.Add(clickMe)
		Me.ToolBar = toolBar
#End If
	End Sub

End Class
#End If