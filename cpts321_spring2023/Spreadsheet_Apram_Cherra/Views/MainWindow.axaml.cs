using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using DynamicData.Binding;
using DynamicData.Kernel;
using Spreadsheet_Apram_Cherra.ViewModels;
using SpreadsheetEngine;
using DataGridRow = Avalonia.Controls.DataGridRow;

namespace Spreadsheet_Apram_Cherra.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    EditDataTemplate editTmplate = new EditDataTemplate();
    ReadDataTemplate readTmplate = new ReadDataTemplate();
    public MainWindow()
    {
        InitializeComponent();
        // Initialize the Grid with right number of Rows and Columns
        InitializeDatagrid();
        //The Read Template may need to know the State of the Grid for which it is
        //creating cell template.. 
        readTmplate.targetGrid = SpreadsheetDataGrid;
    }

    public void InitializeDatagrid()
    {
        //Add 26 Columns from A to Z and one for showing Row Number
        // Each Column will be editable and associated with
        //Text property of the corresponding Object of Cell class
        // readonly cell will be associated with Value property
        for (int i = 0; i < 27; i++)
        {
            char nm = (char)(64 + i);
            if (i == 0) //force 0 column header to ' '
                nm = ' ';
            //Create The column, set Templates for editing mode and read mode.
            // setup the header and add the column to DataGrid columns collection.
            var columnTemplate = new DataGridTemplateColumn
            {
                Header = nm,
                CellStyleClasses = new Classes { "SpreadsheetCellClass" },
                // CellTemplate = new FuncDataTemplate<RowViewModel>((value, namescope) =>
                //     new TextBlock
                //     {
                //         [!TextBlock.TextProperty] =
                //             new Binding("[" + i/27 + "].Value"),
                //         TextAlignment = TextAlignment.Left,
                //         //VerticalAlignment = VerticalAlignment.Center,
                //         Padding = Thickness.Parse("5,0,5,0")
                //     }),
                // CellEditingTemplate = new FuncDataTemplate<RowViewModel>((value, namescope) =>
                //     new TextBox()
                //     {
                //         Text = value[i/27].Text
                //     }
                // ),
                CellTemplate = readTmplate,
                CellEditingTemplate = editTmplate
                
            };
            SpreadsheetDataGrid.Columns.Add(columnTemplate);
        }
         SpreadsheetDataGrid.PreparingCellForEdit += (sender, args) =>
            {
                if (args.EditingElement is not TextBox textInput) return;
                var rowIndex = args.Row.GetIndex();
                var columnIndex = args.Column.DisplayIndex;
                textInput.Text = ((MainWindowViewModel)this.DataContext).GetCellText(rowIndex, columnIndex);
            };

            SpreadsheetDataGrid.CellEditEnding += (sender, args) =>
            {
                Console.WriteLine("Editing called");
                if (args.EditingElement is not TextBox textInput) return;
                var rowIndex = args.Row.GetIndex();
                var columnIndex = args.Column.DisplayIndex;
                
                ((MainWindowViewModel)this.DataContext).SetCellText(rowIndex, columnIndex, textInput.Text);
            };

            // we use the following event to write our own selection logic
            SpreadsheetDataGrid.CellPointerPressed += (sender, args) =>
            {
                // get the pressed cell
                var rowIndex = args.Row.GetIndex();
                var columnIndex = args.Column.DisplayIndex;

                // are we selected multiple cells
                var multipleSelection = args.PointerPressedEventArgs.KeyModifiers != KeyModifiers.None;
                if (multipleSelection == false)
                {
                    ((MainWindowViewModel)this.DataContext).SelectCell(rowIndex, columnIndex);
                }
                else
                {
                    ((MainWindowViewModel)this.DataContext).ToggleCellSelection(rowIndex, columnIndex);
                }
            };

            SpreadsheetDataGrid.BeginningEdit += (sender, args) =>
            {
                // get the pressed cell
                var rowIndex = args.Row.GetIndex();
                var columnIndex = args.Column.DisplayIndex;

                var cell = ((MainWindowViewModel)this.DataContext).GetCell(rowIndex, columnIndex);
                if (false == cell.CanEdit)
                {
                    args.Cancel = true;
                }
                else
                {
                    ((MainWindowViewModel)this.DataContext).ResetSelection();
                }
            };
            SpreadsheetDataGrid.VerticalScroll += (sender, args) =>
            {
                //Console.WriteLine("Vertical Scroll Handler Called");
                SpreadsheetDataGrid.Items = null;
                readTmplate.callIndex = 0;
                editTmplate.callIndex = 0;
                SpreadsheetDataGrid.Items = ((MainWindowViewModel)this.DataContext).GetSpreadSheet();
            };

        //Try showing the Grid Lines in the DataGrid on screen
        SpreadsheetDataGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;
    }

    public void ShowGridRows()
    {
        //This method can be called to reset the Grid from the Model Data.
        
        //Listen to model spreadsheet changes, ie ChangeCellText method is called when anything changes in model spreadsheet.
        ((MainWindowViewModel)this.DataContext).SpreadsheetData.PropertyChanged += ChangeCellText;

        ResetGrid();
    }

    private void ResetGrid()
    {
        //Reset the callIndex on edit and read templates to their init value...
        SpreadsheetDataGrid.Items = null;
        readTmplate.callIndex = 0;
        editTmplate.callIndex = 0;
        //Set items property if the DataGrid to Model spreadsheet cells. This will force the DataGrid to reBuild itself.
        // SpreadsheetDataGrid.Items = ((MainWindowViewModel)this.DataContext).SpreadsheetData.cells;
        SpreadsheetDataGrid.Items = ((MainWindowViewModel)this.DataContext).GetSpreadSheet();
    }

    private void ChangeCellText(object? sender, PropertyChangedEventArgs e)
    {
        //This method is called when anything changes on model side, so we force repaint/rebuild
        // of the DataGrid .. first make Items =null and then set it to model spreadsheet Cells
        SpreadsheetDataGrid.Items = null;
        readTmplate.callIndex = 0;
        editTmplate.callIndex = 0;
        SpreadsheetDataGrid.Items = ((MainWindowViewModel)this.DataContext).GetSpreadSheet();
    }

    private void SpreadsheetDataGrid_OnCellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        int column_index = e.Column.DisplayIndex;
        int row_index = e.Row.GetIndex();
        ((MainWindowViewModel)this.DataContext).GetCell(row_index, column_index).FireTextPropertyChanged();
    }
    public async void PickColor(object? sender, RoutedEventArgs routedEventArgs)
    {
        ColorPickerPopup dialog = new ColorPickerPopup();
        var fn =   await dialog.ShowDialog<uint>((Window) this);
        ((MainWindowViewModel)this.DataContext).ApplyCellAction("ChangeBGColor", dialog.PickedColor);
        ResetGrid();
    }
    public async void LoadFile(object? sender, RoutedEventArgs routedEventArgs)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        var fn =   await dialog.ShowAsync((Window) this);
        if (fn == null || fn.Length <= 0)
            return;
        StreamReader sd = new StreamReader("" + fn[0]);
        ((MainWindowViewModel)this.DataContext).LoadFileContent(sd);
        ResetGrid();
    }
    public async void SaveFile(object? sender, RoutedEventArgs routedEventArgs)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        var fn =   await dialog.ShowAsync((Window) this);
        if (fn == null || fn.Length <= 0)
            return;
        Console.WriteLine("File_name " + fn);
        StreamWriter sd = new StreamWriter("" + fn);
        ((MainWindowViewModel)this.DataContext).SaveToFile(sd);
    }

    private void RedoAction(object? sender, RoutedEventArgs e)
    {
        ((MainWindowViewModel)this.DataContext).Redo();
        ResetGrid();
    }

    private void UndoAction(object? sender, RoutedEventArgs e)
    {
        ((MainWindowViewModel)this.DataContext).Undo();
        ResetGrid();
    }


    private void EditSubMenu(object? sender, PointerPressedEventArgs e)
    {
        CellAction u = ((MainWindowViewModel)this.DataContext).UndoPeek();
        CellAction r = ((MainWindowViewModel)this.DataContext).RedoPeek();
       
        IEnumerator en = ((MenuItem)sender).Items.GetEnumerator();
        
        while (en.MoveNext())
        {
            var mi = (MenuItem) en.Current;
            if (mi.Header.ToString().StartsWith("_Undo"))
            {
                if (u == null)
                {
                    mi.Header = "_Undo";
                    mi.IsEnabled = false;
                }
                else
                {
                    mi.Header = "_Undo" + "- " + u.ActionType();
                    mi.IsEnabled = true;
                }
                    
            }
            else
            {
                if (r == null)
                {
                    mi.Header = "_Redo";
                    mi.IsEnabled = false;
                }
                else
                {
                    mi.Header = "_Redo" + "- " + r.ActionType();
                    mi.IsEnabled = true;
                }
            }
            
        }
        

    }
}

public class EditDataTemplate : IDataTemplate
{
    public int callIndex = 0;

    public IControl Build(object param)
    {
        // build the control to display
        RowViewModel c = ((RowViewModel)param);
        TextBox tb = new TextBox()
        {
            Text = c[callIndex % 27].Text,
          //  [!TextBox.TextProperty] = new Binding("[" + (callIndex % 27) + "].Text"),
        };
        callIndex++;
        return tb;
    }

    public bool Match(object data)
    {
        // Check if we can accept the provided data
        return data is string;
    }
}

public class ReadDataTemplate : IDataTemplate
{
    public int callIndex = 0;
    public DataGrid targetGrid;

    public IControl Build(object param)
    {
        // build the control to display
        RowViewModel c = ((RowViewModel)param);

        int index = callIndex % 27;
        if (targetGrid.CurrentColumn != null && targetGrid.SelectedIndex != -1)
        {
            char nm = (char)targetGrid.CurrentColumn.Header;
            if (((int)nm) - 64 > 0)
            {
                index = (int)nm - 64;
            }
        }

        TextBlock tb = new TextBlock()
        {
            Text = c[index].Value,
            Background = new SolidColorBrush(c[index].BackgroundColor)
            //  [!TextBox.TextProperty] = new Binding("[" + (callIndex % 27) + "].Text"),
        };
        callIndex++;
        return tb;
    }

    public bool Match(object data)
    {
        Console.WriteLine("Match " + data);
        // Check if we can accept the provided data
        return data is string;
    }
}