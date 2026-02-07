# GridView

A GridView is bound by assigning its `DataStore` property to a collection.  The control supports MVVM in such that if the collection implements `INotifyCollectionChanged`, such as the `ObservableCollection<T>`, it will update as changes are made to the underlying collection.

There are a number of available Cell types:

* CheckBoxCell
* ComboBoxCell
* ImageTextCell
* ImageViewCell
* TextBoxCell

When creating cells, you must set its binding which can be done via index, property via reflection, or delegate/lambda methods.

[API Documentation](http://api.etoforms.picoe.ca/html/T_Eto_Forms_GridView.htm)

## POCO

The best way to use the GridView is to use a [POCO](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object) or [DTO](http://en.wikipedia.org/wiki/Data_transfer_object).  Cells can reference properties of your objects to populate its data.

As an example:

```C#
using Eto.Forms;
using Eto;
using System.Collections.ObjectModel;

class MyPoco
{
    public string Text { get; set; }

    public bool Check { get; set; }
}

class MyForm : Form
{
    public MyForm()
    {
        var collection = new ObservableCollection<MyPoco>();
        collection.Add(new MyPoco { Text = "Row 1", Check = true });
        collection.Add(new MyPoco { Text = "Row 2", Check = false });

        var grid = new GridView { DataStore = collection };

        grid.Columns.Add(new GridColumn {
            DataCell = new TextBoxCell { Binding = Binding.Property<MyPoco, string>(r => r.Text) },
            HeaderText = "Text"
        });

        grid.Columns.Add(new GridColumn {
            DataCell = new CheckBoxCell { Binding = Binding.Property<MyPoco, bool?>(r => r.Check) },
            HeaderText = "Check"
        });
        Content = grid;
    }
}
```

## DataTable

To bind to a `DataTable` (as commonly provided by SQL sources), you can use the following strategy by converting each row to an [anonymous type](http://msdn.microsoft.com/en-us/library/bb397696.aspx) (or POCO).

```C#
using System.Linq;
using Eto.Forms;


class MyForm : Form
{
    public MyForm()
    {
        var table = new DataTable();
        table.Columns.Add("TextColumn");
        table.Columns.Add("CheckColumn", typeof(bool));

        table.Rows.Add("Row 1", true);
        table.Rows.Add("Row 2", false);

        var collection = table.Rows.Cast<DataRow>()
            .Select(x => new { Text = x[0], Check = x[1] })
            .ToList();

        var grid = new GridView { DataStore = collection };

        grid.Columns.Add(new GridColumn
        { 
            DataCell = new TextBoxCell("Text"),
            HeaderText = "Text"
        });
        grid.Columns.Add(new GridColumn
        { 
            DataCell = new CheckBoxCell("Check"),
            HeaderText = "Check"
        });

        Content = grid;
    }
}
```
