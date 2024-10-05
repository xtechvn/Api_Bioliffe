using HuloToys_Service.ElasticSearch;
using HuloToys_Service.Models.Article;
using HuloToys_Service.Models.ElasticSearch;
using HuloToys_Service.Utilities.Lib;

namespace HuloToys_Service.Controllers.News.Business
{
    public partial class NewsBusiness
    {
        public IConfiguration configuration;
        public ArticleService article_service;
        
        public ArticleCategoryService articleCategoryESService;
        
        public GroupProductESService groupProductESService;
        public NewsBusiness(IConfiguration _configuration)
        {

            configuration = _configuration;
            article_service = new ArticleService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        
            articleCategoryESService = new ArticleCategoryService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
            
            groupProductESService = new GroupProductESService(_configuration["DataBaseConfig:Elastic:Host"], _configuration);
        }

        public async Task<ArticleModel> getArticleDetail(long article_id)
        {
            try
            {
                var article_list = article_service.GetArticleDetailById(article_id);
                return article_list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetArticleCategoryByParentID -GroupProductRepository : " + ex);
            }
            return null;
        }


        /// <summary>
        /// cuonglv
        /// Lấy ra danh sách các bài thuộc 1 chuyên mục
        /// </summary>
        /// <param name="cate_id"></param>
        /// <returns></returns>
        //public async Task<List<CategoryArticleModel>> getArticleListByCategoryId(int cate_id,int size)
        //{
        //    var list_article = new List<CategoryArticleModel>();
        //    try
        //    {  
        //        // Lấy ra danh sách id các bài viết theo chuyên mục
        //        var List_articleCategory = articleCategoryESService.GetArticleIdByCategoryId(cate_id, size);
        //        if (List_articleCategory != null && List_articleCategory.Count > 0)
        //        {
        //            foreach (var item in List_articleCategory)
        //            {
        //                // Chi tiết bài viết
        //                var article = articleESService.GetArticleDetailForCategoryById((long)item.articleid);
        //                if (article == null)
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    list_article.Add(article);
        //                }
        //            }                    
        //        }
        //        return list_article;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "getArticleListByCategoryId - ArticleDAL: " + ex);
        //        return list_article;
        //    }
        //}


        /// <summary>
        /// cuonglv
        /// Lấy ra bài mới nhất
        /// </summary>
        /// <param name="category_parent_id">Là id cha của các chuyên mục  tin tức</param>
        /// <returns></returns>
        public async Task<List<CategoryArticleModel>> getListNews(int category_id, int take)
        {
            var list_article = new List<CategoryArticleModel>();
            try
            {
                // Lấy ra danh sách id các bài viết mới nhất
                var obj_top_story = article_service.getListNews( category_id,take);

                return obj_top_story;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "getArticleListByCategoryId - ArticleDAL: " + ex);
                return list_article;
            }
        }

        public List<GroupProductModel> GetByParentId(long parent_id)
        {
            try
            {
                var obj_cate = new List<GroupProductModel>();
                // List chuyên mục cha
                var obj_cate_parent = groupProductESService.GetListGroupProductByParentId(parent_id);

                foreach (var item in obj_cate_parent)
                {                  
                    var obj_cate_child = new List<GroupProductModel>();
                    var cate_child_detail =  groupProductESService.GetListGroupProductByParentId(item.id);
                    if (cate_child_detail.Count > 0)
                    {
                        foreach (var item_child in cate_child_detail)
                        {
                            var cate_child = new GroupProductModel
                            {
                                parentid = item.id,
                                id = item_child.id,
                                name = item_child.name,                                
                                imagepath = item_child.imagepath,
                                path = item_child.path,
                            };
                            obj_cate_child.Add(cate_child);
                        }                        
                    }
                    var cate_parent = new GroupProductModel
                    {
                        id = item.id,
                        name = item.name,
                        imagepath = item.imagepath,
                        path = item.path,
                        group_product_child = obj_cate_child
                    };
                    obj_cate.Add(cate_parent);
                }

                return obj_cate;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetByParentId-GroupProductDAL" + ex);
            }
            return null;
        }

        public GroupProductModel GetById(long id)
        {
            try
            {
                var data = groupProductESService.GetDetailGroupProductById(id);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetById-GroupProductDAL" + ex);

            }
            return null;
        }
       
        public async Task<List<GroupProductModel>> GetArticleCategoryByParentID(long parent_id)
        {
            try
            {
                var article_list = GetByParentId(parent_id).ToList();                
                return article_list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetArticleCategoryByParentID -GroupProductRepository : " + ex);
            }
            return null;
        }
        //public async Task<List<GroupProductModel>> GetFooterCategoryByParentID(long parent_id)
        //{
        //    try
        //    {
        //        //var group = GetByParentId(parent_id);
        //        //group = group.Where(x => x.isshowfooter == true).ToList();
        //        //var list = new List<GroupProductModel>();

        //        // list.AddRange(group.Select(x => new ArticleGroupViewModel() { id = x.Id, image_path = x.ImagePath, name = x.Name, order_no = (int)x.OrderNo, url_path = x.Path }).OrderBy(x => x.order_no).ToList());
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetFooterCategoryByParentID -GroupProductRepository : " + ex);
        //    }
        //    return null;
        //}

        /// <summary>
        /// cuonglv
        /// Lấy ra tổng tin theo chuyên mục
        /// </summary>
        /// <param name="cate_id"></param>
        /// <returns></returns>
        public async Task<int> getTotalItemNewsByCategoryId(int cate_id)
        {
            var list_article = new List<CategoryArticleModel>();
            try
            {
                // Lấy ra danh sách id các bài viết theo chuyên mục
                var total = article_service.getTotalItemNewsByCategoryId(cate_id);
                
                return total;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "getArticleListByCategoryId - ArticleDAL: " + ex);
                return 0;
            }
        }


    }
}
