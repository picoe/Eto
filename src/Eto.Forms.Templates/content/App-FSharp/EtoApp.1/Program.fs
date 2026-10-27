namespace EtoApp._1.Desktop
module Program =

    open System
    open EtoApp._1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platform.Detect)
        // Wpf doesn't follow the system light/dark theme unless asked to
        if app.Platform.IsWpf then
            app.Theme <- Eto.Forms.Themes.System
        app.Run(new MainForm())
        0