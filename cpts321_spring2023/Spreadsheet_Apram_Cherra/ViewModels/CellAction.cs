using System.Collections.Generic;

namespace Spreadsheet_Apram_Cherra.ViewModels;

public interface CellAction
{
    public void DoAction();
    public void UndoAction();
    public string ActionType();
}