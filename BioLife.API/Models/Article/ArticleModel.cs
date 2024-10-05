using Nest;

namespace HuloToys_Service.Models.Article
{
    public class ArticleModel:CategoryArticleModel // Kế thừa thuộc tính của box tin
    {
        [PropertyName("Body")]
        public string body { get; set; }
    }
}
