namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.$if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$

type MainForm() as this = 
    inherit Form()

    do 
        $if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$Reader.Load(this)

    member this.HandleClickMe(sender:obj, e:EventArgs) =
        MessageBox.Show("Hello!") |> ignore

    member this.HandleQuit(sender:obj, e:EventArgs) =
        Application.Instance.Quit()