using System.Linq;
using BTL_LTWNC.Models;
using BTL_LTWNC.Data;

namespace BTL_LTWNC.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Register(User user)
        {
            if (EmailExists(user.sEmail))
            {
                return false;
            }
            // user.sPlainPW = user.sPW; 
            user.sPW = BCrypt.Net.BCrypt.HashPassword(user.sPW); 

            _context.tbl_User.Add(user);
            _context.SaveChanges();
            return true;
        }


        public User Login(string email, string password)
        {
            var user = _context.tbl_User.FirstOrDefault(u => u.sEmail == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.sPW))
            {
                return user;
            }
            return null;
        }

        public User GetByEmail(string email)
        {
            return _context.tbl_User.FirstOrDefault(u => u.sEmail == email);
        }

        public bool EmailExists(string email)
        {
            return _context.tbl_User.Any(u => u.sEmail == email);
        }

        public List<User> GetAllUsers()
        {
            return _context.tbl_User.ToList();
        }

        public bool UpdateUser(User updatedUser, string newPassword = null)
        {
            var user = _context.tbl_User.FirstOrDefault(u => u.PK_iUserID == updatedUser.PK_iUserID);
            if (user == null) return false;

            user.sUserName = updatedUser.sUserName;
            user.sSDT = updatedUser.sSDT;
            user.sEmail = updatedUser.sEmail;
            user.dNS = updatedUser.dNS;
            user.sCCCD = updatedUser.sCCCD;
            user.sPW = updatedUser.sPW; 

            if (!string.IsNullOrEmpty(newPassword))
                {
                    user.sPW = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }

            _context.SaveChanges();
            return true;
        }

        public bool DeleteUser(int id)
        {
            var user = _context.tbl_User.Find(id);
            if (user == null) return false;

            _context.tbl_User.Remove(user);
            _context.SaveChanges();
            return true;
        }


        public List<User> SearchUsers(string keyword)
        {
            return _context.tbl_User.Where(u => 
                                            // u.sUserName.Contains(keyword)||
                                            u.sEmail.Contains(keyword)
                                        ).ToList();
        }

        public User GetUserById(int userId)
        {
            return _context.tbl_User.FirstOrDefault(u => u.PK_iUserID == userId);
        }

        public bool AddUser(User user)
        {
            if (string.IsNullOrEmpty(user.sPW)) 
            {
                throw new ArgumentNullException(nameof(user.sPW), "Mật khẩu không được để trống.");
            }

            if (EmailExists(user.sEmail))
            {
                return false;
            }

            user.sPW = BCrypt.Net.BCrypt.HashPassword(user.sPW); 

            _context.tbl_User.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateUser(User updatedUser)
        {
            try
            {
                // Tìm người dùng trong cơ sở dữ liệu
                var user = _context.tbl_User.FirstOrDefault(u => u.PK_iUserID == updatedUser.PK_iUserID);
                if (user == null) return false;

                user.sUserName = updatedUser.sUserName;
                user.sSDT = updatedUser.sSDT;
                user.sEmail = updatedUser.sEmail;
                user.dNS = updatedUser.dNS;
                user.sCCCD = updatedUser.sCCCD;

                if (!string.IsNullOrWhiteSpace(updatedUser.sPW))
                {
                    user.sPW = BCrypt.Net.BCrypt.HashPassword(updatedUser.sPW);
                }

                var result = _context.SaveChanges() > 0;
                
                if (!result)
                {
                    Console.WriteLine("Cập nhật không thành công!");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật người dùng: " + ex.Message);
                return false;
            }
        }

        public bool UpdateNewPassUser(User user, string newPassword)
        {
            try
            {
                user.sPW = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.tbl_User.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }        
        }
    }
}


// public IActionResult Details(int id)
        // {
        //     var post = _postRepository.GetPostById(id);
        //     if (post == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(post);
        // }*/