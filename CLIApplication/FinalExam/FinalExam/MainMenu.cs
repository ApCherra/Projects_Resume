namespace FinalExam;

public class MainMenu
{
    private string main_menu = "1 : Create New Product. \n" +
                               "2 : Search Products. \n" +
                               "3 : Restock \n" +
                               "4 : Exit \n" +
                               "Option # ";

    private ProductSearch search = new ProductSearch();
    private ProductRestock restock = new ProductRestock();
    public static ProductList pl = new ProductList();

    public void ShowMenu() // our main menu class where we display main menu and depending on user interaction we proceed
    {
        string userOption = "";

        while (!userOption.Equals("4"))
        {
            Console.Write(main_menu);

            userOption = Console.ReadLine().Trim(); //trim just in case user adds space
            switch (userOption)
            {
                case "1":
                {
                    Console.Write("\n Enter Product Type: ");
                    string product_type = Console.ReadLine();
                    
                    Console.Write("\n Enter ProductID: ");
                    string product_ID = Console.ReadLine();

                    Console.Write("\n Enter Product Description: ");
                    string product_desc = Console.ReadLine();

                    Product p = new Product(product_type, product_desc);
                    p.ProductId = product_ID;
                    pl.AddProduct(p);
                    break;
                }
                case "2":
                    search.ShowMenu();
                    break;
                case "3":
                    restock.ShowMenu();
                    break;
                default:
                    break;
            }
        }
    }
}