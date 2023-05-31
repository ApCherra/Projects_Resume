using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using DynamicData;
using SpreadsheetEngine;

namespace Spreadsheet_Apram_Cherra.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private List<RowViewModel> _spreadsheetData = null;
    private readonly List<CellViewModel> _selectedCells = new();
    private Stack<CellAction> un_do_actions = new Stack<CellAction>();
    private Stack<CellAction> re_do_actions = new Stack<CellAction>();
    
    public Spreadsheet SpreadsheetData = new Spreadsheet(50, 26);

    public MainWindowViewModel()
    {
        InitSheet();
    }

    private void InitSheet()
    {
        _spreadsheetData = new List<RowViewModel>();
        foreach (Cell[] cArray in SpreadsheetData.cells)
        {
            List<CellViewModel> cvm_list = new List<CellViewModel>();
            foreach (var cell in cArray)
            {
                cvm_list.Add(new CellViewModel(cell));
            }
            _spreadsheetData.Add(new RowViewModel(cvm_list));
        }
    }

    public List<RowViewModel> GetSpreadSheet()
    {
        return _spreadsheetData;
    }
    public CellViewModel GetCell(int ri, int ci)
    {
        return _spreadsheetData[ri][ci];
    }
    public string GetCellText(int ri, int ci)
    {
        return _spreadsheetData[ri][ci].Text;
    }
    public void SetCellText(int rowIndex, int columnIndex, string text_val)
    {
        //_spreadsheetData[rowIndex][columnIndex].Text = text_val;
        ResetSelection();
        SelectCell(rowIndex, columnIndex);
        ApplyCellAction("ChangeText", text_val);
    }
    public void SelectCell(int rowIndex, int columnIndex)
    {
        var clickedCell = GetCell(rowIndex, columnIndex);

        var shouldEditCell = clickedCell.IsSelected;

        ResetSelection();

        // add the pressed cell back to the list
        _selectedCells.Add(clickedCell);

        clickedCell.IsSelected = true;
        if (shouldEditCell)
            clickedCell.CanEdit = true;
    }

    public void ToggleCellSelection(int rowIndex, int columnIndex)
    {
        var clickedCell = GetCell(rowIndex, columnIndex);
        if (false == clickedCell.IsSelected)
        {
            _selectedCells.Add(clickedCell);
            clickedCell.IsSelected = true;
        }
        else
        {
            _selectedCells.Remove(clickedCell);
            clickedCell.IsSelected = false;
        }
    }

    public void ResetSelection()
    {
        // clear current selection
        foreach (var cell in _selectedCells)
        {
            cell.IsSelected = false;
            cell.CanEdit = false;
        }
        _selectedCells.Clear();
    }

    public void ApplyCellAction(string actionType, object value)
    {
        CellAction action = null;
        if(actionType == "ChangeBGColor")
            action = new ChangeCellColor(_selectedCells, (uint) value);
        else if(actionType == "ChangeText")
        {
            action = new ChangeCellTextAction(_selectedCells, (string) value);
        }
        if (null != action)
        {
            action.DoAction();
            un_do_actions.Push(action);
        }

        ResetSelection();
    }

    public CellAction UndoPeek()
    {
        try
        {
            return un_do_actions.Peek();
        }
        catch (Exception e)
        {
            return null;
        }
    }
    public void Undo()
    {
        try
        {
            CellAction the_action = un_do_actions.Pop();
            Console.WriteLine("Undoing " + the_action.ActionType());
            the_action.UndoAction();
            re_do_actions.Push(the_action);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void Redo()
    {
        try
        {
            CellAction the_action = re_do_actions.Pop();
            the_action.UndoAction();
        }
        catch (Exception e)
        {
           Console.WriteLine(e);
        }
    }
    public CellAction RedoPeek()
    {
        try
        {
            return re_do_actions.Peek();
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public void LoadFileContent(StreamReader r)
    {
        var serializer = new XmlSerializer(typeof(SerilizableSpreadsheet));
       
        var sheet = (SerilizableSpreadsheet) serializer.Deserialize(r);
        SpreadsheetData = new Spreadsheet(50, 26);
        foreach (var cell in sheet.cells)
        {
            SpreadsheetData.SetCell(cell.RowIndex, cell.ColumnIndex, cell);
        }
        un_do_actions = new Stack<CellAction>();
        re_do_actions = new Stack<CellAction>();
        InitSheet();
    }
    public async void SaveToFile(StreamWriter sw)
    {
        SerilizableSpreadsheet sp = new SerilizableSpreadsheet();
        foreach (var ca in SpreadsheetData.cells)
        {
            foreach (var c in ca)
            {
                if(!c.IsDefault())
                    sp.cells.Add(c);
            }
        }
        var serializer = new XmlSerializer(typeof(SerilizableSpreadsheet));
        serializer.Serialize(sw, sp );
    }
}