namespace HuloToys_Service.Models.Article
{
    public class ArticleModel: CategoryArticleModel
    {
       
        public string body { get; set; } = null!;

        public int status { get; set; }

        public int articletype { get; set; }

        public int? pageview { get; set; }

        public DateTime? publishdate { get; set; }

        public int? authorid { get; set; }

        public string image169 { get; set; } = null!;

        public string? image43 { get; set; }

        public string? image11 { get; set; }

        public DateTime? createdon { get; set; }

        public DateTime? modifiedon { get; set; }

        public DateTime? downtime { get; set; }

        public DateTime? uptime { get; set; }

        public short? position { get; set; }
    }
}
