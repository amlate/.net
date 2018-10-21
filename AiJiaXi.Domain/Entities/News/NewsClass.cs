using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Domain.Entities.News
{
    public class NewsClass
    {
        public long Id { get; set; }

        public string Name { get; set; }
        
        public virtual ImageEntity ImageEntity { get; set; }

        public long? ImageEntityId { get; set; }
        
        public virtual ImageEntity ThumbNail { get; set; }
        
        public long? ThumbNailId { get; set; }

        public string Desc { get; set; }

        public virtual IEnumerable<NewsMain> NewsMains { get; set; }
    }
}