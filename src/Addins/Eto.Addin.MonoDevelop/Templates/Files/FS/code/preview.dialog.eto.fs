namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing

type ${EscapedIdentifier}Base () as this =
    inherit Panel ()

    member this.InitializeComponent() =
        base.Title <- "My ${EscapedIdentifier} dialog"
        base.Content <- new Label (Text = "Some Content")
