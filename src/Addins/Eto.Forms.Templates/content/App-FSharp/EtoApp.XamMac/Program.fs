namespace EtoApp.Wpf
module Program =

    open System
    open EtoApp

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.XamMac2)
        app.Run(new MainForm())
        0