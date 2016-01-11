import clr

clr.AddReference('Eto')
clr.AddReference('Eto.Wpf')

from Eto.Forms import *
from Eto import *
from MainForm import *

app = Application($EtoPlatform$)
form = MainForm()
app.Run(form)
