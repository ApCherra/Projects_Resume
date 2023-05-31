using System.Text.Json;

namespace FinalExam;

public class ProductSearch
{
    private string search_menu = "Enter product id or text to search \n" +
                                 " # ";

    private string search_option_menu = "Enter 0 for OR Search \n" +
                                        "Enter 1 for AND Search \n" +
                                        "Option # ";
    public void ShowMenu()
    {
        Console.Write(search_menu); //display product search menu
        string user_text = Console.ReadLine();
        string[] search_tags = user_text.Split();
        Console.Write(search_option_menu);
        string user_search_option = Console.ReadLine();

        int option = 0;
        try
        {
            option = Int32.Parse(user_search_option);
        }
        catch (Exception e)
        {
            option = 1;
        }
        //pl represents product list,
        // here we are searching for the product by description or ID
        List<Product> products = MainMenu.pl.SearchProducts(search_tags, option);
        foreach (var product in products)
        {
            Console.WriteLine(JsonSerializer.Serialize(product));
            Console.WriteLine("Item Count :: " + MainMenu.pl.GetItemCount(product.ProductId));
            foreach (var itm in MainMenu.pl.GetItems(product.ProductId))
            {
                Console.WriteLine("     " + JsonSerializer.Serialize(itm) );
            }
        }
        
    }
}