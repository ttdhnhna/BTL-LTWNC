using BTL_LTWNC.Data;
using BTL_LTWNC.Models;
using BTL_LTWNC.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace BTL_LTWNC.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IPostRepository _postRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(IPostRepository postRepository, IVehicleRepository vehicleRepository, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _postRepository = postRepository;
            _vehicleRepository = vehicleRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Posts()
        {
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

        [HttpGet]
        public IActionResult AddPost()
        {
            return View();
        }
        //Khong su dung ajax
        /*[HttpPost]
        public async Task<IActionResult> AddPost(VehicleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }

                    ViewBag.ErrorMessage = "Có lỗi xảy ra! Vui lòng kiểm tra lại thông tin.";
                    return View("AddPost", model);
                }

                if (ModelState.IsValid)
                {
                    string uniqueFileName = null;
                    // Xử lý ảnh
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        var exists = await _context.tbl_Vehicle.AnyAsync(v => v.sCarNum == model.CarNum);
                        if (exists)
                        {
                            ModelState.AddModelError("CarNum", "Biển số xe đã tồn tại.");
                            return View("AddPost", model);
                        }

                        var vehicle = new Vehicle
                        {
                            imgURL = uniqueFileName,
                            sCarName = model.CarName,
                            sCarNum = model.CarNum,
                            fGiaThue = model.GiaThue,
                            dNgayThue = model.NgayThue,
                            dNgayTra = model.NgayTra,
                            sSoCho = model.SoCho,
                            sLoaiNL = model.LoaiNL,
                            sDiaDiem = model.DiaDiem,
                            sTinhNang = model.TinhNang,
                            sMoTa = model.MoTa,
                            sMTOther = model.MTOther
                        };
                        _vehicleRepository.AddVehicle(vehicle);
                        await _context.SaveChangesAsync();

                        var userId = HttpContext.Session.GetInt32("UserId");

                        if (userId == null)
                        {
                            return RedirectToAction("Login", "Account");
                        }

                        var post = new Post
                        {
                            userID = userId.Value,
                            vehicleID = vehicle.PK_iVehicleID,
                            dTgDangbai = DateTime.Now
                        };
                        _postRepository.AddPost(post);
                        await _context.SaveChangesAsync();


                    }
                    return RedirectToAction("Posts");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                ViewBag.ErrorMessage = "Không thể lưu bài đăng. Vui lòng thử lại!";
            }
            return View(model);
        }
*/
        //Co su dung ajax(Thanh cong)
        [HttpPost]
        public async Task<IActionResult> AddPost(VehicleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return Json(new { success = false, message = string.Join("<br>", errors) });
                }

                string uniqueFileName = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    var exists = await _context.tbl_Vehicle.AnyAsync(v => v.sCarNum == model.CarNum);
                    if (exists)
                    {
                        return Json(new { success = false, message = "Biển số xe đã tồn tại." });
                    }

                    var vehicle = new Vehicle
                    {
                        imgURL = uniqueFileName,
                        sCarName = model.CarName,
                        sCarNum = model.CarNum,
                        fGiaThue = model.GiaThue,
                        dNgayThue = model.NgayThue,
                        dNgayTra = model.NgayTra,
                        sSoCho = model.SoCho,
                        sLoaiNL = model.LoaiNL,
                        sDiaDiem = model.DiaDiem,
                        sTinhNang = model.TinhNang,
                        sMoTa = model.MoTa,
                        sMTOther = model.MTOther
                    };
                    _vehicleRepository.AddVehicle(vehicle);
                    await _context.SaveChangesAsync();

                    var userId = HttpContext.Session.GetInt32("UserId");
                    if (userId == null)
                    {
                        return Json(new { success = false, message = "Vui lòng đăng nhập." });
                    }

                    var post = new Post
                    {
                        userID = userId.Value,
                        vehicleID = vehicle.PK_iVehicleID,
                        dTgDangbai = DateTime.Now
                    };
                    _postRepository.AddPost(post);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Vui lòng chọn ảnh." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return Json(new { success = false, message = "Không thể lưu bài đăng. Vui lòng thử lại!" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == post.vehicleID);
            if (vehicle == null)
            {
                return NotFound();
            }

            var viewModel = new PostvsVehicle
            {
                Post = post,
                Vehicle = vehicle,
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> PostDetail(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == post.vehicleID);
            if (vehicle == null)
            {
                return NotFound();
            }

            var viewModel = new PostvsVehicle
            {
                Post = post,
                Vehicle = vehicle,
            };
            return View(viewModel);
        }

        //Khong su dung ajax(Thanh cong)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);

            var userId = HttpContext.Session.GetInt32("UserId");

            if (post == null || post.userID != userId)
            {
                return Unauthorized();
            }

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

            return RedirectToAction("Posts");
        }

        //Co su dung ajax(Loi)
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);
            var userId = HttpContext.Session.GetInt32("UserId");

            if (post == null || post.userID != userId)
            {
                return Unauthorized();
            }

            try
            {
                var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == post.vehicleID);
                if (vehicle != null)
                {
                    _context.tbl_Vehicle.Remove(vehicle);
                }

                _context.tbl_Post.Remove(post);
                await _context.SaveChangesAsync();

                return Ok(); // Trả về 200 nếu thành công
            }
            catch (Exception ex)
            {
                // Có thể log lỗi ex.Message nếu cần
                return StatusCode(500, "Lỗi khi xóa bài viết");
            }
        }*/

        
        [HttpGet]
        public async Task<IActionResult> UpdatePost(int id)
        {
            var post = await _context.tbl_Post.FirstOrDefaultAsync(p => p.Id == id);

            var userId = HttpContext.Session.GetInt32("UserId");

            if (post == null || post.userID != userId)
            {
                return Unauthorized();
            }

            var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == post.vehicleID);
            if (vehicle == null)
                return NotFound();

            var model = new VehicleDto
            {
                Id = vehicle.PK_iVehicleID,
                CarName = vehicle.sCarName,
                CarNum = vehicle.sCarNum,
                GiaThue = vehicle.fGiaThue,
                NgayThue = vehicle.dNgayThue,
                NgayTra = vehicle.dNgayTra,
                SoCho = vehicle.sSoCho,
                LoaiNL = vehicle.sLoaiNL,
                DiaDiem = vehicle.sDiaDiem,
                TinhNang = vehicle.sTinhNang,
                MoTa = vehicle.sMoTa,
                MTOther = vehicle.sMTOther,
            };
            ViewData["imgURL"] = vehicle.imgURL;

            return View(model);
        }

        //Khong su dung ajax
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(VehicleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }

                    ViewBag.ErrorMessage = "Có lỗi xảy ra! Vui lòng kiểm tra lại thông tin.";
                    return View("UpdatePost",model);
                }

                if (ModelState.IsValid)
                {
                    var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == model.Id);
                    if (vehicle == null)
                        return NotFound();

                    
                    // Xử lý ảnh
                    if (model.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        vehicle.imgURL = uniqueFileName;                      
                    }
                    vehicle.sCarName = model.CarName;
                    vehicle.sCarNum = model.CarNum;
                    vehicle.fGiaThue = model.GiaThue;
                    vehicle.dNgayThue = model.NgayThue;
                    vehicle.dNgayTra = model.NgayTra;
                    vehicle.sSoCho = model.SoCho;
                    vehicle.sLoaiNL = model.LoaiNL;
                    vehicle.sDiaDiem = model.DiaDiem;
                    vehicle.sTinhNang = model.TinhNang;
                    vehicle.sMoTa = model.MoTa;
                    vehicle.sMTOther = model.MTOther;

                    _vehicleRepository.UpdateVehicle(vehicle);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Cập nhật thành công, chuyển hướng đến Posts");
                    return RedirectToAction("Posts", "Posts");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                ViewBag.ErrorMessage = "Không thể cập nhật bài đăng. Vui lòng thử lại!";
            }
            return View(model);
        }*/

        //Co su dung ajax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePost(VehicleDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return BadRequest(string.Join(", ", errors));
                    }

                    ViewBag.ErrorMessage = "Có lỗi xảy ra! Vui lòng kiểm tra lại thông tin.";
                    return View("UpdatePost", model);
                }

                var vehicle = await _context.tbl_Vehicle.FirstOrDefaultAsync(v => v.PK_iVehicleID == model.Id);
                if (vehicle == null)
                    return NotFound();

                bool isCarNumExist = await _context.tbl_Vehicle
                    .AnyAsync(v => v.sCarNum == model.CarNum && v.PK_iVehicleID != model.Id);

                if (isCarNumExist)
                {
                    ViewBag.ErrorMessage = "Số xe đã tồn tại. Vui lòng nhập số xe khác.";
                    return View("UpdatePost", model);
                }

                // Xử lý ảnh
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    vehicle.imgURL = uniqueFileName;
                }

                vehicle.sCarName = model.CarName;
                vehicle.sCarNum = model.CarNum;
                vehicle.fGiaThue = model.GiaThue;
                vehicle.dNgayThue = model.NgayThue;
                vehicle.dNgayTra = model.NgayTra;
                vehicle.sSoCho = model.SoCho;
                vehicle.sLoaiNL = model.LoaiNL;
                vehicle.sDiaDiem = model.DiaDiem;
                vehicle.sTinhNang = model.TinhNang;
                vehicle.sMoTa = model.MoTa;
                vehicle.sMTOther = model.MTOther;

                _vehicleRepository.UpdateVehicle(vehicle);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Posts", "Posts") });
                }

                return RedirectToAction("Posts", "Posts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return StatusCode(500, "Lỗi server: " + ex.Message);
                }

                ViewBag.ErrorMessage = "Không thể cập nhật bài đăng. Vui lòng thử lại!";
                return View(model);
            }
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
            return View("Posts", results);
        }

    }
}
