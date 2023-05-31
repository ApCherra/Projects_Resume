using System;
using SpreadsheetEngine;

namespace HW5;

public class Menu
{
   bool displayMenu = true;

   //string userInput;
   string userExpression;
   private ExpressionTree ex;

   public void MenuInterface()
   {
      while (displayMenu) //to display menu
      {
         ShowMenu();
        
      }
   }

   public void ShowMenu() //menu options
   {
     
      Console.WriteLine("Menu (current expression: )");
      Console.WriteLine("1 = Enter a new expression");
      Console.WriteLine("2 = Set a variable value");
      Console.WriteLine("3 = Evaluate tree");
      Console.WriteLine("4 = Quit");

      switch (Console.ReadLine())
      {
         case "4":
            displayMenu = false;
            break;
         case "1":
            InputExpression();
            ex = new ExpressionTree(userExpression);
            break;
            
         case "2":
            InputExpression();
            string[] tags = userExpression.Split("=");
            if (ex != null)
            {
               ex.SetVariable(tags[0].Trim(),tags[1].Trim());
            }
            break;
         case "3":
            Evaluate();
            break;
      }
   }

   public void InputExpression()
   {
      Console.Write("input new expression: ");
      userExpression = Console.ReadLine();
      
   }

   public void Evaluate()
   {
      int value = ex.Evaluate();
   Console.WriteLine("Final Tree Expression output is " + value.ToString());
   }



}

