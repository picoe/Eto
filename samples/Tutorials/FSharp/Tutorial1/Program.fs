open System
open Eto.Forms
open Eto.Drawing

type MyForm() as this =
    inherit Form()
    do
        this.ClientSize <- Size(600, 400)
        this.Title <- "Hello, Eto.Forms"
        this.Content <- new Label(Text = "Some content", VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center)

[<EntryPoint;STAThread>]
let main argv = 
    (new Application()).Run(new MyForm())
    0

