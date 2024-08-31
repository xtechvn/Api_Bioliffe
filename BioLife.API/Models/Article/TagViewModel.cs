using System;
using System.Collections.Generic;

namespace HuloToys_Service.Models.Article;

public partial class TagViewModel
{
    public long Id { get; set; }

    public string? TagName { get; set; }

    public DateTime? CreatedOn { get; set; }

}
