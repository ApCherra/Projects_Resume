using FinalExam;

namespace FinalExamTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    //Test 1 is searching the product
    public void Test1()
    {
        ProductList pl = new ProductList();

        Product p1 = new Product("p", "p1");
        Product p2 = new Product("p", "p2");

        pl.AddProduct(p1);
        pl.AddProduct(p2);

        string[] tags = new string[1];
        tags[0] = "p1";

        List<Product> sp = pl.SearchProducts(tags, 0);
        
        if (sp.Count == 1)
        {
            Assert.Pass();
        }
        else
        {
            Assert.Fail();
        }
    }
    
    [Test]
    //Looking for item count for a product
    public void Test2()
    {
        ProductList pl = new ProductList();

        Product p1 = new Product("p", "p1");
        Product p2 = new Product("p", "p2");

        pl.AddProduct(p1);
        pl.AddProduct(p2);

        string[] tags = new string[1];
        tags[0] = "p1";

        int count = pl.GetItemCount(p1.ProductId);

        if (count == 0)
            Assert.Pass();
        else
            Assert.Fail();
    }
}