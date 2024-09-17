namespace HuloToys_Service.Models.Article
{
    public class CategoryArticleModel // box tin
    {
        public long id { get; set; }

        public string title { get; set; } = null!;

        public string lead { get; set; } = null!;

        public string image169 { get; set; } = null!;
        public string image43 { get; set; } = null!;
        public string image11 { get; set; } = null!;
        public DateTime publishdate { get; set; }
        public string category_name { get; set; }

    }
}
