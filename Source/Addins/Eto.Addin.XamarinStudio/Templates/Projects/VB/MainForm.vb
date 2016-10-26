Imports System
Imports Eto.Forms
Imports Eto.Drawing

''' <summary>
''' Your application's main form
''' </summary>
Public Class MainForm
	Inherits Form

	Public Sub New()
		Title = "My Eto Form"
		ClientSize = New Size(400, 350)

		Dim helloLabel As New Label()
		helloLabel.Text = "Hello World!"

		Dim layout As New StackLayout()
		layout.Items.Add(New StackLayoutItem(helloLabel))
		' Add more controls here

		Content = layout

		' create a few commands that can be used for the menu and toolbar
		Dim clickMe As New Command()
		With clickMe
			.MenuText = "Click Me!"
			.ToolBarText = "Click Me!"
			AddHandler .Executed, AddressOf ClickMeClicked
		End With

		Dim quitCommand As New Command()
		With quitCommand
			.MenuText = "Quit"
			.Shortcut = Application.Instance.CommonModifier Or Keys.Q
			AddHandler .Executed, AddressOf QuitClicked
		End With

		Dim aboutCommand As New Command()
		With aboutCommand
			.MenuText = "About..."
			AddHandler .Executed, AddressOf AboutClicked
		End With


		' create menu
			' File submenu
				' new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
				' new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
			' application (OS X) or file menu (others)
		Menu = New MenuBar()
		With Menu
			Dim fileItem As New ButtonMenuItem()
			With fileItem
				.Text = "&File"
				.Items.Add(clickMe)
			End With
			.Items.Add(fileItem)

			Dim preferencesItem As New ButtonMenuItem()
			preferencesItem.Text = "&Preferences..."
			.ApplicationItems.Add(preferencesItem)
			.QuitItem = quitCommand
			.AboutItem = aboutCommand
		End With

		' create toolbar			
		ToolBar = New ToolBar()
		ToolBar.Items.Add(clickMe)
	End Sub

	Sub QuitClicked(ByVal sender As Object, ByVal e As EventArgs)
		Application.Instance.Quit()
	End Sub

	Sub AboutClicked(ByVal sender As Object, ByVal e As EventArgs)
		MessageBox.Show(Me, "About my app...")
	End Sub

	Sub ClickMeClicked(ByVal sender As Object, ByVal e As EventArgs)
		MessageBox.Show(Me, "I was clicked!")
	End Sub
End Class
