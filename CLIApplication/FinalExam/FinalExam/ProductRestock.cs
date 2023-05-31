using System.Text.Json;

namespace FinalExam;

public class ProductRestock
{
    private string restock_menu = "\n Enter count to restock: ";
                          
    public void ShowMenu()
    {
        Console.Write(restock_menu);
        
        string user_entry = Console.ReadLine();

        int count = 0;
        try
        {
            count = Int32.Parse(user_entry);
        }
        catch (Exception e)
        {
            count = 1;
        }
        //Looking for all product with count less than the count user entered,
        //Then you give the option to user to restock all or not
        List<Product> products = MainMenu.pl.GetProductsByCount(count);
        foreach (var product in products)
        {
            Console.WriteLine(JsonSerializer.Serialize(product));
            Console.WriteLine("\n Count: " + MainMenu.pl.GetItemCount(product.ProductId)  + "\n");
        }
        Console.WriteLine("Restock All y/n ");
        var answer = Console.ReadLine();
        if (answer.StartsWith("y") || answer.StartsWith("Y"))
        {
            foreach (var product in products)
            {
                Console.WriteLine(JsonSerializer.Serialize(product));
                Console.WriteLine("\n Count: " + MainMenu.pl.GetItemCount(product.ProductId)  + "\n");
                Console.WriteLine("How may Additional Items to purchase? ");
                var new_count = Console.ReadLine();
                int count_as_int = 0;
                try
                {
                    count_as_int = Int32.Parse(new_count);
                }
                catch (Exception e)
                {
                }

                if (count_as_int > 0)
                {
                    MainMenu.pl.Restock(product.ProductId, count_as_int);
                    Console.WriteLine("Items Purchased. New Count = " + MainMenu.pl.GetItemCount(product.ProductId) + "\n" );
                }
            }
        }
        else
        {
            foreach (var product in products)
            {
                Console.WriteLine(JsonSerializer.Serialize(product));
                Console.WriteLine("\n Count: " + MainMenu.pl.GetItemCount(product.ProductId)  + "\n");
                Console.WriteLine("Do you wish to add more Items. Enter r to return to main menu y/n/r ");
                answer = Console.ReadLine();
                if (answer.StartsWith("r") || answer.StartsWith("R"))
                    return;
                if (answer.StartsWith("y") || answer.StartsWith("Y"))
                {
                    Console.WriteLine("How may Additional Items to purchase? ");
                    var new_count = Console.ReadLine();
                    int count_as_int = 0;
                    try
                    {
                        count_as_int = Int32.Parse(new_count);
                    }
                    catch (Exception e)
                    {
                    }

                    if (count_as_int > 0)
                    {
                        MainMenu.pl.Restock(product.ProductId, count_as_int);
                        Console.WriteLine("Items Purchased. New Count = " + MainMenu.pl.GetItemCount(product.ProductId) + "\n");
                    }
                }
            }   
        }
    }
}