namespace EtoApp.1.XamMac
module Program =

    open System
    open EtoApp.1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.XamMac2)
        app.Run(new MainForm())
        0