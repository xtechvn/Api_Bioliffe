namespace HuloToys_Service.Models.Products
{
    public class ProductBrandRequestModel
    {
      public string brand_id { get; set; }
      public int group_product_id { get; set; }
      public double amount_min { get; set; }
      public double amount_max { get; set; }
      public int page_index { get; set; }
      public int page_size { get; set; }
    }
}
