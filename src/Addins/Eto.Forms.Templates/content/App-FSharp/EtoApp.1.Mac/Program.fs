namespace EtoApp.1.Mac
module Program =

    open System
    open EtoApp.1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.Mac64)
        app.Run(new MainForm())
        0