using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_LTWNC.Models
{
    [Table("tbl_Post")]
    public class PostModel
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int userID { get; set; }
        public int vehicleID { get; set; }
        public DateTime? dTgDangbai { get; set; }
    }
}
