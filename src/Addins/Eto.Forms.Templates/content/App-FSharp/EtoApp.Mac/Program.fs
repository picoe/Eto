namespace EtoApp.Mac
module Program =

    open System
    open EtoApp

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.Mac64)
        app.Run(new MainForm())
        0