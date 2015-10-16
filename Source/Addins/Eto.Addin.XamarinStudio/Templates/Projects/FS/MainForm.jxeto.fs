namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.$if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$

type MainForm() as this = 
    inherit Form()
    do 
        $if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$Reader.Load(this)

    member this.HandleClickMe(sender:Object, e:EventArgs) =
        ignore(MessageBox.Show("Hello!"))

    member this.HandleQuit(sender:Object, e:EventArgs) =
        Application.Instance.Quit()