using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Formats.Asn1;
using System.Reflection;

namespace SpreadsheetEngine;

using System.ComponentModel;

public class Spreadsheet : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public Dictionary<string, List<Cell>> cell_x_ref = new Dictionary<string, List<Cell>>();
    public int RowCount { get; }
    public int ColumnCount { get; }
    
    public ObservableCollection<Cell[]> cells = new ObservableCollection<Cell[]>();
    private List<string> calculation_stack = new List<string>();
    public Spreadsheet(int rows, int columns)
    {
        RowCount = rows;
        ColumnCount = columns;
        InitCells();
    }

    protected void InitCells()
    {
        for (int r = 0; r < RowCount; r++)
        {
            Cell[] row = new Cell[ColumnCount + 1]; //One more cell for representing row Index.
            //Add one cell for row Index.
            Cell indexCell = new Cell(r, 0);
            indexCell.Text = "" + (r + 1);
            row[0] = indexCell;
            for (int c = 1; c <= ColumnCount; c++)
            {
                Cell myCell = new Cell(r, c);
                myCell.PropertyChanged += MyCellOnPropertyChanged;
                myCell.Text = "";
                row[c] = myCell;
            }

            cells.Add(row);
        }
    }

    private void MyCellOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //Handle special processing of Text of cell which was changed.
        try
        {
            calculation_stack.Clear();
            HandleEvaluateCellValue((Cell)sender);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return;
        }

        OnPropertyChanged(this, (Cell)sender, e);
    }

    private bool CircularRef(Cell c)
    {
        if (cell_x_ref.ContainsKey(c.GetCellId()))
        {
            
        }

       return false;
    }

    private void HandleEvaluateCellValue(Cell c)
    {
       /*
        * B1 is refrenced by [D1, D2]
        * D1 is referenced by [A1]
        * A1 is referenced by [B1]
        */

       if (c.Text.StartsWith("="))
        {
            Console.WriteLine("Evaluating cell Text Change " + c.RowIndex + " : " + c.ColumnIndex);
            //expression is after char '=' 
            string expression = c.Text.Substring(1).Trim();
            
            ExpressionTree exp_tree = new ExpressionTree(expression);
            List<string> vars_in_expression = exp_tree.getExpressionVars();

            RemoveXRefrences(c);

            foreach (var ex_var in vars_in_expression)
            {
                Console.WriteLine("Processing Var " + ex_var);
                int columnNumber = -1;
                int rowNumber = -1;
                try
                {
                    columnNumber = ((int)ex_var[0]) - 64;
                    rowNumber = Int32.Parse(ex_var.Substring(1)) - 1;
                    if (columnNumber > 26 || columnNumber < 1 || rowNumber < 0 || rowNumber > 49)
                    {
                        c.Value = "!( Bad Reference)";
                        return;
                    }
                }
                catch (Exception e)
                {
                    c.Value = "!(Bad Reference)";
                    return;
                }
                Cell refCell = GetCell(rowNumber, columnNumber);
                if (refCell.GetCellId() == c.GetCellId())
                {
                    c.Value = "!(Self Reference)";
                    return;
                }
                exp_tree.SetVariable(ex_var, refCell.Value);
                if (cell_x_ref.ContainsKey(refCell.GetCellId()))
                {
                    cell_x_ref[refCell.GetCellId()].Add(c);
                    Console.WriteLine("Added " + c.GetCellId() + " to " + refCell.GetCellId());
                }
                else
                {
                    var l = new List<Cell>();
                    l.Add(c);
                    cell_x_ref.Add(refCell.GetCellId(), l);
                    Console.WriteLine("Added " + c.GetCellId() + " to " + refCell.GetCellId());
                }
            }

            c.Value = exp_tree.Evaluate().ToString();
            ChangeXrefCells(c);
        }
        else
        {
            c.Value = c.Text;
            ChangeXrefCells(c);
        }
    }

    private void RemoveXRefrences(Cell cell) //when have cell =b2 - 2 , cell refer to b2, change that b2 to c1, we still end up keeping reference, doing this will remove any reference
    {
        foreach (var expvar in cell_x_ref.Values)
        {
            expvar.Remove(cell);
        }

        
    }

    private void ChangeXrefCells(Cell c)
    {
        if (calculation_stack.Contains(c.GetCellId())) //detecting circular reference
        {
            c.Value = "!(Circular Ref)";
            calculation_stack.Clear(); //first check
            return;
        }
        else
        {
            calculation_stack.Add(c.GetCellId()); //eval cell, added to stack
        }
            
        if (!cell_x_ref.ContainsKey(c.GetCellId()))
        {
            Console.WriteLine("No ref found for " + c.GetCellId());
            return;
        }

        var lst = cell_x_ref[c.GetCellId()];
        foreach (var cl in lst.ToArray())
        {
            
            HandleEvaluateCellValue(cl);
        }
    }

    public Cell GetCell(int row, int column)
    {
        return cells[row][column];
    }
    public Cell SetCell(int row, int column, Cell c)
    {
        return cells[row][column] = c;
    }

    public ObservableCollection<string[]> CellValues()
    {
        var values = new ObservableCollection<string[]>();
        var i = 0;
        foreach (Cell[] _row in cells)
        {
            string[] stringRow = new string[ColumnCount];
            stringRow[i] = _row[i].Text;
            values.Add(stringRow);
        }

        return values;
    }

    protected static void OnPropertyChanged(Spreadsheet? parent, Object? sender, PropertyChangedEventArgs? e) =>
        parent.PropertyChanged?.Invoke(sender, e);
}