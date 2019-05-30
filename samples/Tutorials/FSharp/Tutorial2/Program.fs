open System
open Eto.Forms
open Eto.Drawing

/// Custom command
/// <remarks>
/// You can create your own command subclasses, or create instances of Command directly.
/// Commands can be used for either the menu or toolbar.
/// Otherwise, you can use MenuItem or ToolItem classes directly.
/// </remarks>
type MyCommand() as this =
    inherit Command()
    do
        this.MenuText <- "C&lick Me, Command"
        this.ToolBarText <- "Click Me"
        this.ToolTip <- "This shows a dialog for no reason"
        this.Shortcut <- Application.Instance.CommonModifier ||| Keys.M // control+M or cmd+M

    override this.OnExecuted e =
        base.OnExecuted e
        MessageBox.Show(Application.Instance.MainForm, "You clicked me!", "Tutorial 2", MessageBoxButtons.OK) |> ignore

type MyForm() as this =
    inherit Form()

    do
        this.ClientSize <- Size(600, 400)
        this.Title <- "Menus and Toolbars"
        this.Menu <- new MenuBar()
        let fileItem = new ButtonMenuItem(Text = "&File")
        
        this.Menu.Items.Add fileItem

        fileItem.Items.Add (new MyCommand()) |> ignore
        fileItem.Items.Add (new ButtonMenuItem(Text = "Click Me, MenuItem")) |> ignore
        
        // quit item (goes in Application menu on OS X, File menu for others)
        this.Menu.QuitItem <- (new Command((fun sender e -> Application.Instance.Quit()) , MenuText = "Quit", Shortcut = (Application.Instance.CommonModifier ||| Keys.Q))).CreateMenuItem()

        // about command (goes in Application menu on OS X, Help menu for others)
        this.Menu.AboutItem <- (new Command((fun sender e -> (new Dialog(Content = (new Label(Text = "About my app...")), ClientSize = Size(200, 200))).ShowModal(this)), MenuText = "About my app")).CreateMenuItem()

        // create toolbar
        this.ToolBar <- new ToolBar()
        this.ToolBar.Items.Add(new MyCommand())
        this.ToolBar.Items.Add(new SeparatorToolItem())
        this.ToolBar.Items.Add(new ButtonToolItem(Text = "Click Me, ToolItem"))

[<EntryPoint;STAThread>]
let main argv = 
    (new Application()).Run(new MyForm())
    0
