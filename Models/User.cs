using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_LTWNC.Models
{
    public class User
    {
        [Key]
        public int PK_iUserID { get; set; }

        [Required]
        public string sUserName { get; set; }

        public DateOnly? dNS { get; set; }

        [Required, EmailAddress]
        public string sEmail { get; set; }

        [Required]
        public string sCCCD { get; set; }

        [Required]
        public string sSDT { get; set; }

        [Required]
        public string sPW { get; set; }
        
        [NotMapped] 
        [Compare("sPW", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string sConfirmPW { get; set; }
        
        public bool IsAdmin { get; set; } = false;
    }
}
