using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_LTWNC.Models
{
    [Table("tbl_Post")]
    public class Post
    {
        [Key]
        [Column("PK_iPostID")]
        public int Id { get; set; }
        [Column("FK_iUserID")]
        public int userID { get; set; }
        [Column("FK_iVehicleID")]
        public int vehicleID { get; set; }
        [Column("dTgDangbai")]
        public DateTime? dTgDangbai { get; set; }
    }
}
