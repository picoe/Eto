open System
open System.Diagnostics
open Eto.Forms
open Eto.Drawing

type MyObject() =
    let mutable textProperty = ""
    member this.TextProperty
      with get () = textProperty
      and set value = 
        textProperty <- value
        Console.WriteLine(sprintf "Set TextProperty to %s" value)

type MyForm() as this =
    inherit Form()

    do
        this.ClientSize <- Size(600, 400)
        this.Title <- "Table Layout"

        let dropdown = new DropDown()
        
        ["Item 1"; "Item 2"; "Item 3"]
        |> List.iter dropdown.Items.Add

        // The main layout mechanism for Eto.Forms is a TableLayout.
        // This is recommended to allow controls to keep their natural platform-specific size.
        // You can layout your controls declaratively using rows and columns as below, or add to the TableLayout.Rows and TableRow.Cell directly.
        let layout = 
            new TableLayout(
                Spacing = new Size(5, 5) // space between each cell
                , Padding = new Padding(10)
            )

        [
            TableRow(new Label(Text = "DataContext Binding") |> TableCell, this.DataContextBinding() |> TableCell)
            TableRow(new Label(Text = "Object Binding") |> TableCell, this.ObjectBinding() |> TableCell)
            TableRow(new Label(Text = "Direct Binding") |> TableCell, this.DirectBinding() |> TableCell)
            TableRow(ScaleHeight = true)
        ] |> List.iter layout.Rows.Add

        this.Content <- layout
        
        // Set data context so it propegates to all child controls
        this.DataContext <- MyObject(TextProperty = "Initial Value 1")

        let quitItem = 
            new Command(
                (fun sender e -> Application.Instance.Quit()),
                MenuText = "Quit",
                Shortcut = (Application.Instance.CommonModifier ||| Keys.Q)
            )

        this.Menu <- new MenuBar(QuitItem = quitItem.CreateMenuItem())

    member this.DataContextBinding () =
        let textBox = new TextBox()
        // bind to the text property using delegates
        textBox.TextBinding.BindDataContext<_>(
            Func<MyObject,_>(fun (r) -> r.TextProperty)
            , Action<MyObject,_>(fun (r) value -> r.TextProperty <- value)
        ) |> ignore
        // You can also bind using reflection
        // textBox.TextBinding.BindDataContext<MyObject>(fun r -> r.TextProperty) |> ignore
        // or, if the data context type is unknown
        //textBox.TextBinding.BindDataContext(new PropertyBinding<string>("TextProperty"));
        textBox

    member this.ObjectBinding() =
        // object instance we want to bind to
        let obj = MyObject(TextProperty = "Initial Value 2")
        let textBox = new TextBox()
        // bind to the text property of a specific object instance using reflection
        textBox.TextBinding.Bind(obj, fun r -> r.TextProperty) |> ignore
        textBox

    member this.DirectBinding() =
        let textBox = new TextBox()

        // bind to the text property using delegates
        textBox.TextBinding.Bind(Func<_>(fun () -> "some value"), Action<_>(fun value -> Console.WriteLine(sprintf "Set value to %s directly" value))) |> ignore
        textBox

[<EntryPoint;STAThread>]
let main argv = 
    (new Application()).Run(new MyForm())
    0
