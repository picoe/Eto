namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing

type ${EscapedIdentifier}Base () as this =
    inherit Dialog ()

    do
        base.Title <- "My ${EscapedIdentifier} dialog"
        base.Content <- new Label (Text = "Some Content")
