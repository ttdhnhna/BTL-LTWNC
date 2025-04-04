using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_LTWNC.Models
{
    [Table("tbl_Vehicle")]
    public class VehiclePostViewModel
    {
        [Required]
        [Key]
        public int PK_iVehicleID { get; set; }
        public string sCarName { get; set; }
        public string sCarNum { get; set; }
        public decimal fGiaThue { get; set; }
        public DateTime? dNgayThue { get; set; }
        public DateTime? dNgayTra { get; set; }
        public int sSoCho { get; set; }
        public string sLoaiNL { get; set; }
        public string sDiaDiem { get; set; }
        public string sTinhNang { get; set; }
        public string sMoTa { get; set; }
        public string sMTOther { get; set; }
        public string imgURL { get; set; }
    }
}
