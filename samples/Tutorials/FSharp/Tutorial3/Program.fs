open System
open Eto.Forms
open Eto.Drawing

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
                Spacing = Size(5, 5) // space between each cell
                , Padding = Padding(10)
            )

        [
            TableRow(
                TableCell(new Label(Text = "First Column"), true)
                , TableCell(new Label(Text = "Second Column"), true)
                , TableCell(new Label(Text = "Third Column"))
        
            )
            TableRow(
                TableCell(new TextBox(Text = "Some text"))
                , TableCell dropdown
                , TableCell(new CheckBox(Text = "A checkbox"))
            ) 
            // by default, the last row & column will get scaled. This adds a row at the end to take the extra space of the form.
            // otherwise, the above row will get scaled and stretch the TextBox/ComboBox/CheckBox to fill the remaining height.
            TableRow(ScaleHeight = true)
        ] |> List.iter layout.Rows.Add

        this.Content <- layout
        // This creates the following layout:
        //  --------------------------------
        // |First     |Second    |Third     |
        //  --------------------------------
        // |<TextBox> |<ComboBox>|<CheckBox>|
        //  --------------------------------
        // |          |          |          |
        // |          |          |          |
        //  --------------------------------
        //
        // Some notes:
        //  1. When scaling the width of a cell, it applies to all cells in the same column.
        //  2. When scaling the height of a row, it applies to the entire row.
        //  3. Scaling a row/column makes it share all remaining space with other scaled rows/columns.
        //  4. If a row/column is not scaled, it will be the size of the largest control in that row/column.
        
        this.Menu <- new MenuBar(QuitItem = (new Command((fun sender e -> Application.Instance.Quit()), MenuText = "Quit", Shortcut = (Application.Instance.CommonModifier ||| Keys.Q))).CreateMenuItem())

[<EntryPoint;STAThread>]
let main argv = 
    (new Application()).Run(new MyForm())
    0
