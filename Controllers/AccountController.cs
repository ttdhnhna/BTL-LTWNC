using BTL_LTWNC.Models;
using BTL_LTWNC.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace BTL_LTWNC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private const string AdminEmail = "admin@gmail.com";
        private const string AdminPassword = "admin123"; 

        public AccountController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!", errors });
            }

            if (user.sPW != user.sConfirmPW)
            {
                return Json(new { success = false, message = "Mật khẩu nhập lại không khớp!" });
            }

            if (!_userRepo.Register(user))
            {
                return Json(new { success = false, message = "Email đã tồn tại!" });
            }

            return Json(new { success = true, message = "Đăng ký thành công!" });
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            if (user.sEmail == AdminEmail && user.sPW == AdminPassword)
            {
                HttpContext.Session.SetString("Admin", "true");
                return Json(new { success = true, message = "Đăng nhập Admin thành công!", redirectUrl = "/Admin/TrangChu_Admin" });
            }

            var loggedInUser = _userRepo.Login(user.sEmail, user.sPW);
            if (loggedInUser != null)
            {
                HttpContext.Session.SetString("User", loggedInUser.sEmail);
                HttpContext.Session.SetInt32("UserId", loggedInUser.PK_iUserID);
                return Json(new { success = true, message = "Đăng nhập thành công!", redirectUrl = "/Account/Home" });
            }

            return Json(new { success = false, message = "Email hoặc mật khẩu không đúng!" });
        }

        /*public IActionResult TrangChu()
        {
            if (HttpContext.Session.GetString("User") == null)
                return RedirectToAction("Login");

            return View("~/Views/Post/TrangChu.cshtml");
        }*/

        [HttpGet]
        public IActionResult Home()
        {
            if (HttpContext.Session.GetString("User") == null)
                return RedirectToAction("Login");

            return View("~/Views/Project/Home.cshtml");
        }

        public IActionResult Admin()
        {
            if (HttpContext.Session.GetString("Admin") == null)
                return RedirectToAction("Login");

            var users = _userRepo.GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword([FromBody] Dictionary<string, string> data)
        {
            if (!data.ContainsKey("email") || !data.ContainsKey("newPassword"))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            string email = data["email"];
            string newPassword = data["newPassword"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });
            }

            var user = _userRepo.GetByEmail(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Email không tồn tại trong hệ thống." });
            }

            bool result = _userRepo.UpdateNewPassUser(user, newPassword);

            return Json(new { success = result, message = result ? "Đặt lại mật khẩu thành công!" : "Lỗi khi cập nhật mật khẩu!" });
        }
    }
}
