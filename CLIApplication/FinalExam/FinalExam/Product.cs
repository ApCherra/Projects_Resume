namespace FinalExam;

public class Product
{
    public string ProductType { get; set; }
    public string Description { get; set; }
    public string ProductId { get; set; }

    public Product(string type, string desc)
    {
        this.ProductId = Guid.NewGuid().ToString();
        this.Description = desc.Trim();
        this.ProductType = type.Trim();
    }
}