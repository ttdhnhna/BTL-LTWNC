using BTL_LTWNC.Data;
using BTL_LTWNC.Models;
using BTL_LTWNC.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
            var posts = _context.tbl_Post.ToList();
            return View(posts);
        }

        [HttpGet]
        public IActionResult AddPost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(PostViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Có lỗi xảy ra! Vui lòng kiểm tra lại thông tin.";
                    return View("AddPost", model);
                }

                if (ModelState.IsValid)
                {
                    // Xử lý ảnh
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        //string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                        //string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    model.ImageFile.CopyTo(fileStream);
                        //}
                        //model.imgURL = "/images/" + uniqueFileName;
                        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        newFileName += Path.GetExtension(model.ImageFile!.FileName);

                        string imageFullPath = _webHostEnvironment.WebRootPath + "/uploads/" + newFileName;
                        using(var stream = System.IO.File.Create(imageFullPath))
                        {
                            model.ImageFile.CopyTo(stream);
                        }
                        var vehicle = new VehiclePostViewModel
                        {
                            imgURL = newFileName,
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

                        var post = new PostModel
                        {
                            userID = 1,
                            vehicleID = vehicle.PK_iVehicleID,
                            dTgDangbai = DateTime.Now
                        };
                        _postRepository.AddPost(post);

                    }
                    return RedirectToAction("Posts","Posts");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                ViewBag.ErrorMessage = "Không thể lưu bài đăng. Vui lòng thử lại!";
            }
            return View(model);
        }
     }
}
