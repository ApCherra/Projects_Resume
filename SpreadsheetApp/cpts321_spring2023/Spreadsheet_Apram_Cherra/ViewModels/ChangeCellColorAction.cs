using System;
using System.Collections.Generic;
using DynamicData;

namespace Spreadsheet_Apram_Cherra.ViewModels;

public class ChangeCellColor:CellAction
{
    private List<CellViewModel> _apply_to = new List<CellViewModel>();
    private uint _color;
    private Dictionary<string, uint> org_values = new Dictionary<string, uint>();
    public ChangeCellColor(List<CellViewModel> cells, uint c)
    {
        _apply_to.Add(cells);
        _color = c;
    }
    public void DoAction()
    {
        if (_apply_to != null)
        {
            foreach (var cell in _apply_to)
            {
                org_values.Add(cell.Cell.GetCellId(), cell.BackgroundColor);
                cell.BackgroundColor = _color;
            }
        }
    }

    public void UndoAction()
    {
        Console.WriteLine("UndoAction Called on " + ActionType());
        Console.WriteLine("ApplyTo " + _apply_to.Count);
        Console.WriteLine("Org " + org_values.Count);
        if(_apply_to != null)
        {
            foreach (var cell in _apply_to)
            {
                var ov = org_values[cell.Cell.GetCellId()];
                var tmp = cell.BackgroundColor;
                cell.BackgroundColor = ov;
                org_values[cell.Cell.GetCellId()] = tmp;
            }
        }
    }

    public string ActionType()
    {
        return "ChangeBGColor";
    }
}