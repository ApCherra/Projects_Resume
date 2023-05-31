namespace FinalExam;

public class Item
{
    public string ItemId { get; set; }
    public string ProductId { get; set; }

    public Item(string product_id)
    {
        ItemId = Guid.NewGuid().ToString();
        ProductId = product_id;
    }
}