using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_LTWNC.Models
{
    public class PostViewModel
    {
        [Required]
        public string CarName { get; set; }

        [Required]
        public string CarNum { get; set; }

        [Required]
        public decimal GiaThue { get; set; }

        public DateTime? NgayThue { get; set; }
        public DateTime? NgayTra { get; set; }

        public int SoCho { get; set; }
        public string LoaiNL { get; set; }
        public string DiaDiem { get; set; }
        public string TinhNang { get; set; }
        public string MoTa { get; set; }
        public string MTOther { get; set; }

        public int UserID { get; set; }
        public string imgURL { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
