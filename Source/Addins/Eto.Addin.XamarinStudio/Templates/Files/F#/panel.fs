namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing


type ${EscapedIdentifier} () =
    inherit Panel ()

    do
        base.Content <- new Label (Text = "Some Content")
