﻿namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing

type MainForm$if$($UseCodePreview$==True)Base$endif$ () as this = 
    inherit Form()
    $if$($UseCodePreview$==True)member this.InitializeComponent() =$endif$$if$($UseCode$==True)do$endif$ 
        base.Title <- "My Eto Form"
        base.ClientSize <- new Size(400, 350)

        // table with three rows
        let layout = new StackLayout()
        layout.Items.Add(new StackLayoutItem(new Label(Text = "Hello World!")))
        // Add more controls here

        base.Content <- layout;

        // create a few commands that can be used for the menu and toolbar
        let clickMe = new Command(MenuText = "Click Me!", ToolBarText = "Click Me!")
        clickMe.Executed.Add(fun e -> MessageBox.Show(this, "I was clicked!") |> ignore)

        let quitCommand = new Command(MenuText = "Quit")
        quitCommand.Shortcut <- Application.Instance.CommonModifier ||| Keys.Q
        quitCommand.Executed.Add(fun e -> Application.Instance.Quit())

        let aboutCommand = new Command(MenuText = "About...")
        aboutCommand.Executed.Add(fun e -> MessageBox.Show(this, "About my app...") |> ignore)

        base.Menu <- new MenuBar()
        let fileItem = new ButtonMenuItem(Text = "&File")
        fileItem.Items.Add(clickMe) |> ignore
        base.Menu.Items.Add(fileItem)

        (* add more menu items to the main menu...
        let editItem = new ButtonMenuItem(Text = "&Edit")
        base.Menu.Items.Add(editItem)
        let viewItem = new ButtonMenuItem(Text = "&View")
        base.Menu.Items.Add(viewItem)
        *)

        base.Menu.ApplicationItems.Add(new ButtonMenuItem(Text = "&Preferences..."))
        base.Menu.QuitItem <- quitCommand.CreateMenuItem()
        base.Menu.AboutItem <- aboutCommand.CreateMenuItem()

        base.ToolBar <- new ToolBar()
        base.ToolBar.Items.Add(clickMe)