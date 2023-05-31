namespace FinalExam;

public class ProductList
{
    //creating dictionary to help organize and store data
    private Dictionary<string, Product> products_by_id = new Dictionary<string, Product>();
    
    private Dictionary<string, Product> products_by_desc = new Dictionary<string, Product>();
    
    private Dictionary<string, List<Item>> itemsByProduct = new Dictionary<string, List<Item>>();
    
    public void AddProduct(Product p)
    {
        products_by_id.Add(p.ProductId, p);
        products_by_desc.Add(p.ProductId + " " + p.Description, p);
        if (!itemsByProduct.ContainsKey(p.ProductId))
        {
            itemsByProduct.Add(p.ProductId, new List<Item>());
        }
    }

    public void AddItem(string product_id, Item itm)
    {
        //electronic products -- no items needs to be added.
        if (products_by_id.ContainsKey(product_id))
        {
            var p = products_by_id[product_id];
            if (p.ProductType.Equals("e"))
            {
                return;
            }
        }
        if (itemsByProduct.ContainsKey(product_id))
        {
            itemsByProduct[product_id].Add(itm);
        }
        else
        {
            List<Item> itm_list = new List<Item>();
            itm_list.Add(itm);
            itemsByProduct.Add(product_id, itm_list);
        }
    }

    public int GetItemCount(string product_id)
    {
        //Does product exist. 
        //IF electronic product ... items are unlimited. -1 represent unlimited
        if (products_by_id.ContainsKey(product_id))
        {
            var product = products_by_id[product_id];
            if (product.ProductType.Equals("e"))
                return -1;
        }
        if (itemsByProduct.ContainsKey(product_id))
        {
            return itemsByProduct[product_id].Count;
        }

        return 0;
    }

    public List<Item> GetItems(string product_id)
    {
        if (itemsByProduct.ContainsKey(product_id))
            return itemsByProduct[product_id];
        else
        {
            return new List<Item>();
        }
    }
    public List<Product> GetProductsByCount(int threashold)
    {
        
        List<Product> products = new List<Product>(); 
        foreach (var record in itemsByProduct)
        {
            if (record.Value.Count < threashold)
            {
                products.Add(products_by_id[record.Key]);
            }
        }

        return products;
    }
    /*
     * searchType 0 for OR Search, 1 for AND search
     */
    public List<Product> SearchProducts(string[] tags, int searchType)
    {
        if (tags == null || tags.Length == 0)
        {
            return products_by_id.Values.ToList();
        }

        if (searchType == 0)
        {
            return OrSearch(tags);
        }

        return AndSearch(tags);
    }

    private List<Product> OrSearch(string[] tags)
    {
        List<string> matchingKeys = new List<string>();
        List<string> keys = products_by_desc.Keys.ToList();
        foreach(var key in  keys) {
            foreach (var tag in tags)
            {
                if (key.ToLower().Contains(tag.ToLower()))
                {
                    matchingKeys.Add(key);
                }
            }
        }

        List<Product> matchingProducts = new List<Product>();

        foreach (var mk in matchingKeys)
        {
            matchingProducts.Add(products_by_desc[mk]);
        }

        return matchingProducts;
    }
    private List<Product> AndSearch(string[] tags)
    {
        List<string> matchingKeys = new List<string>();
        List<string> keys = products_by_desc.Keys.ToList();
        foreach(var key in  keys)
        {
            bool allTagsMatched = true;
            foreach (var tag in tags)
            {
                if (!key.ToLower().Contains(tag.ToLower()))
                {
                    allTagsMatched = false;
                }
            }
            if (allTagsMatched)
            {
                matchingKeys.Add(key);
            }
        }
        List<Product> matchingProducts = new List<Product>();
        foreach (var mk in matchingKeys)
        {
            matchingProducts.Add(products_by_desc[mk]);
        }
        return matchingProducts;
    }

    public void Restock(string product_id, int count)
    {
        for (int item_count = 0; item_count < count; item_count++)
        {
            AddItem(product_id, new Item(product_id));
        }
    }
    
}