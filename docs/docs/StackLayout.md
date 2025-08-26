    open System
    open Eto.Forms
    open Eto.Drawing
    
    type MainForm () as this =
        inherit Form()
        let textBoxName = new TextBox()
        let okButton = new Button(Text = "Ok")
        do
            this.Title <- "Задача"
            okButton.Click.Add(fun e -> MessageBox.Show(textBoxName.Text) |> ignore)
         
            let layout = new StackLayout(Orientation = Orientation.Vertical,HorizontalContentAlignment =          HorizontalAlignment.Center,Spacing = 5,Padding = new Padding(10))
            layout.Items.Add(new StackLayoutItem(new Label(Text = "Тема:")))
            layout.Items.Add(new StackLayoutItem(textBoxName))
            layout.Items.Add(new StackLayoutItem(okButton))
            base.Content <- layout;