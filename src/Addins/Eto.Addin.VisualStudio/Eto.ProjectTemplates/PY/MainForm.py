from Eto.Forms import *
from Eto.Drawing import *

class MainForm(Form):
    def __init__(self):
        # Create child controls and initialize form
        self.Size = Size(400, 400)
        self.Title = 'My Form'
        self.Content = Label(Text = 'Hello')
        pass
