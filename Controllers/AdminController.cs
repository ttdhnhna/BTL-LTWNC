using BTL_LTWNC.Data;
using BTL_LTWNC.Models;
using BTL_LTWNC.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BTL_LTWNC.Controllers
{
    public class AdminController : Controller
    {
        //Can bo xung doan nay de co the lay duoc thong tin xe theo bai viet
        private readonly IUserRepository _userRepo;
        private readonly ApplicationDbContext _context;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IPostRepository _postRepository;

        public AdminController(IUserRepository userRepo, ApplicationDbContext context, IPostRepository postRepository, IVehicleRepository vehicleRepository)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _context = context;
            _postRepository = postRepository;
            _vehicleRepository = vehicleRepository;
        }

        public IActionResult TrangChu_Admin()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult QL_User()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Account");

            var users = _userRepo.GetAllUsers();
            return View(users);
        }
        //Chuc nang xem bai viet o phan admin
        public IActionResult QL_Posts()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login", "Account");

            /*var posts = _context.tbl_Post.ToList();
            var vehicles = _context.tbl_Vehicle.ToList();
            var postvsvehicleList = posts.Select(post => new PostvsVehicle
            {
                Post = post,
                Vehicle = vehicles.FirstOrDefault(v => v.PK_iVehicleID == post.vehicleID)
            }).ToList();*/
            var postvsvehicleList = _postRepository.GetAllPostsWithVehicle();
            return View(postvsvehicleList);
        }
        //Chuc nang xoa bai viet cua admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == post.vehicleID);
                if (vehicle != null)
                {
                    _context.tbl_Vehicle.Remove(vehicle);
                }

                _context.tbl_Post.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("QL_Posts");
        }

        [HttpGet]
        public IActionResult SearchPost(string carName, string price, string location)
        {
            //Phần repo này có cách hoạt động tương tự với phần lấy posts ở trên chức năng posts nhưng thôi để thế này cho đỡ bị lặp lại
            var postvsvehicleList = _postRepository.GetAllPostsWithVehicle();
            if (!string.IsNullOrEmpty(carName))
            {
                postvsvehicleList = postvsvehicleList.Where(p => p.Vehicle.sCarName.Contains(carName)).ToList();
            }

            if (!string.IsNullOrEmpty(price) && decimal.TryParse(price, out decimal maxPrice))
            {
                postvsvehicleList = postvsvehicleList.Where(p => p.Vehicle.fGiaThue <= maxPrice).ToList();
            }

            if (!string.IsNullOrEmpty(location))
            {
                postvsvehicleList = postvsvehicleList.Where(p => p.Vehicle.sDiaDiem.Contains(location)).ToList();
            }

            var results = postvsvehicleList.ToList();
            return View("QL_Posts", results);
        }



        [HttpGet]
        public IActionResult SearchUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập email để tìm kiếm." });
            }

            var users = _userRepo.SearchUsers(email)
                .Where(u => u.sEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                .Select(u => new
                {
                    u.PK_iUserID,
                    u.sUserName,
                    u.sEmail,
                    dNS = u.dNS?.ToString("yyyy-MM-dd"),
                    u.sCCCD,
                    u.sSDT
                }).ToList();

            if (users.Count == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy kết quả!" });
            }

            return Json(new { success = true, users });
        }

        [HttpPost]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { success = false, message = "ID không hợp lệ!" });
            }

            bool deleted = _userRepo.DeleteUser(id);
            return Json(new { success = deleted, message = deleted ? "Xóa thành công!" : "Xóa thất bại!" });
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.sPW) || string.IsNullOrEmpty(user.sEmail))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ hoặc thiếu thông tin bắt buộc." });
            }

            bool success = _userRepo.AddUser(user);
            return Json(new { success });
        }

        [HttpGet]
        public IActionResult GetUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { success = false, message = "ID không hợp lệ!" });
            }

            var user = _userRepo.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng!" });
            }
            return Json(user);
        }

        [HttpPost]
        [Route("Admin/UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || id != user.PK_iUserID)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            var existingUser = _userRepo.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound(new { success = false, message = "Người dùng không tồn tại!" });
            }

            if (string.IsNullOrEmpty(user.sPW))
            {
                user.sPW = existingUser.sPW;
            }
            else
            {
                user.sPW = BCrypt.Net.BCrypt.HashPassword(user.sPW);
            }

            bool updated = _userRepo.UpdateUser(user);
            return Json(new { success = true, message = updated ? "Cập nhật thành công!" : "Cập nhật thất bại!" });
        }
    }
}
