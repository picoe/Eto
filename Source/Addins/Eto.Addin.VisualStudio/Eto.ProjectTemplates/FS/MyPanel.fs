namespace $safeprojectname$

open System
open Eto.Forms
open Eto.Drawing

type MyPanel () as this =
    inherit Panel ()

    do
        base.Content <- new Label (Text = "Some Content")
