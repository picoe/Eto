namespace ${Namespace}
module Program =

    open System
    open ${BaseProjectName}

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(${EtoPlatform})
        app.Run(new MainForm())
        0