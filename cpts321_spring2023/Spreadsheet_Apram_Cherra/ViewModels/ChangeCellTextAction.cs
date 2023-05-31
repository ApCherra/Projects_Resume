using System;
using System.Collections.Generic;

namespace Spreadsheet_Apram_Cherra.ViewModels;

public class ChangeCellTextAction: CellAction
{
    private List<CellViewModel> _apply_to = new List<CellViewModel>();
    private string _value;
    private Dictionary<string, string> org_values = new Dictionary<string, string>();
    public ChangeCellTextAction(List<CellViewModel> cells, string v)
    {
        foreach (var c in cells)
        {
            _apply_to.Add(c);
        }
        _value = v;
    }
    public void DoAction()
    {
        if (_apply_to != null)
        {
            foreach (var cell in _apply_to)
            {
                org_values.Add(cell.Cell.GetCellId(), cell.Text);
                cell.Text = _value;
            }
        }
    }

    public void UndoAction()
    {
        if(_apply_to != null)
        {
            foreach (var cell in _apply_to)
            {
                var ov = org_values[cell.Cell.GetCellId()];
                var tmp = cell.Text;
                cell.Text = ov;
                cell.FireTextPropertyChanged();
                org_values[cell.Cell.GetCellId()] = tmp;
            }
        }
    }
    
    public string ActionType()
    {
        return "ChangeText";
    }
}