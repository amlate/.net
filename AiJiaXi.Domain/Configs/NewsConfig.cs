using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Project.Domain.Entities.News;

namespace Project.Domain.Configs
{
    public class NewsClassConfig : EntityTypeConfiguration<NewsClass>
    {
        public NewsClassConfig()
        {
            this.HasKey(item => item.Id)
                .Property(item => item.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            this.HasOptional(item => item.ImageEntity).WithMany().HasForeignKey(item => item.ImageEntityId).WillCascadeOnDelete(false);
            this.HasOptional(item => item.ThumbNail).WithMany().HasForeignKey(item => item.ThumbNailId).WillCascadeOnDelete(false);
        }
    }

    public class NewsMainConfig : EntityTypeConfiguration<NewsMain>
    {
        public NewsMainConfig()
        {
            this.HasKey(item => item.Id)
                .Property(item => item.Id)
                .HasColumnName("Id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            this.HasOptional(item => item.ImageEntity).WithMany().HasForeignKey(item => item.ImageEntityId).WillCascadeOnDelete(false);
            this.HasOptional(item => item.ThumbNail).WithMany().HasForeignKey(item => item.ThumbNailId).WillCascadeOnDelete(false);
        }
    }
}