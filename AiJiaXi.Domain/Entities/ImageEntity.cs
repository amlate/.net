using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiJiaXi.Domain.Entities
{
    public class ImageEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string ImgUrl { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}