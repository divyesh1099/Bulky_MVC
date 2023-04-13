using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        [BindProperty]
        public ShoppingCart ShoppingCart { get; set; }
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository, IShoppingCartRepository shoppingCartRepository, IUnitOfWork unitOfWork)
        {
            _db = db;
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _productRepository.GetAll(includeProperties: "Category");

            return View(productList);
        }

        public IActionResult Details(int id)
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart = new ShoppingCart();
            ShoppingCart.Product = _productRepository.Get(u => u.Id == id, includeProperties: "Category");
            ShoppingCart.Count = 1;
            ShoppingCart.ProductId = id;
            ShoppingCart.ApplicationUserId = userId;
            return View(ShoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            // shoppingCart.ApplicationUserId = userId;

            ShoppingCart shoppingCartFromDb = _shoppingCartRepository.Get(u=>u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if (shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCart.Count;
                _shoppingCartRepository.Update(shoppingCartFromDb);
            } else
            {
                _shoppingCartRepository.Add(shoppingCart);
            }
            using (var transaction = _db.Database.BeginTransaction())
            {
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ShoppingCarts ON");
                _unitOfWork.Save();
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ShoppingCarts OFF");
                transaction.Commit();
            }
            //_unitOfWork.Save();
            TempData["success"] = "Item Added To Cart Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}