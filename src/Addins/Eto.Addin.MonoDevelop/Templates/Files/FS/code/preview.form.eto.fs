namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing

type ${EscapedIdentifier}Base () as this =
    inherit Form ()

    member this.InitializeComponent() =
        base.Title <- "My ${EscapedIdentifier} form"
        base.Content <- new Label (Text = "Some Content")
