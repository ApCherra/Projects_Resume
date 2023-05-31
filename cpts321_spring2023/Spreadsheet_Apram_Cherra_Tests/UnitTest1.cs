using System.Data;
using Avalonia.Controls;
using Spreadsheet_Apram_Cherra.ViewModels;
using Spreadsheet_Apram_Cherra.Views;
using SpreadsheetEngine;
namespace Spreadsheet_Apram_Cherra_Tests;

using static MainWindow;

public class TestGrid
{
    [SetUp]
    public void GridTest() //this is to test whether the cells are being populated correctly
    {
        
        Spreadsheet mySpreadSheet = new Spreadsheet(5, 5);
        int column;
            int row;
            mySpreadSheet.cells[1][1].Text = "Updating " + "some" + ":" + "shit";
            mySpreadSheet.cells[1][2].Text = "Updating " + "some" + ":" + "shit";
            mySpreadSheet.cells[1][3].Text = "Updating " + "some" + ":" + "shit";
            mySpreadSheet.cells[1][4].Text = "Updating " + "some" + ":" + "shit";
            mySpreadSheet.cells[1][5].Text = "Updating " + "some" + ":" + "shit";
            Assert.AreEqual(mySpreadSheet.cells[1][1].Text, "Updating some:shit");
           // Assert.That(mySpreadSheet.cells[1][1], Is.EqualTo("Updating some:shit"));

    }

    [Test]
    public void SheetSize()
    {
        Spreadsheet mySpreadSheet = new Spreadsheet(20, 20); //checking to see if size is correct
        if (mySpreadSheet.ColumnCount == 20)
        {
            Assert.Pass();
        }
        if (mySpreadSheet.RowCount == 20)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }


    }
}


