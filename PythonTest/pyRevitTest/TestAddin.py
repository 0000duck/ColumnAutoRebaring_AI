import clr
clr.AddReference("RevitAPI")
clr.AddReference("RevitAPIUI")

from Autodesk.Revit.DB import *
from Autodesk.Revit.UI import *
from Autodesk.Revit.UI.Selection import *

class ABC:
    i =123

uidoc = commandData.Application.ActiveUIDocument
doc = uidoc.Document
sel = uidoc.Selection

tx = Transaction(doc, "Python")
tx.Start()

frm.ShowDialog()

elem = doc.GetElement(sel.PickObject(ObjectType.Element))
elem.LookupParameter("comments").Set("Python Add-in")

tx.Commit()