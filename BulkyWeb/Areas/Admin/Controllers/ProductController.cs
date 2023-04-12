using Bulky.DataAccess.Repository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ProductRepository productRepository, CategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> Products = _productRepository.GetAll(includeProperties:"Category").ToList();
            
            return View(Products);
        }
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _categoryRepository.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()

            };
            if(id==null || id == 0){
                return View(productVM);

            }
            else
            {
                productVM.Product = _productRepository.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwrootpath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwrootpath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                        // Delete Old Image
                        var oldImagePath = Path.Combine(wwwrootpath, productVM.Product.ImageURL.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageURL = @"\images\product\" + filename;
                }
                if(productVM.Product.Id == 0)
                {
                    _productRepository.Add(productVM.Product);
                } else
                {
                    _productRepository.Update(productVM.Product);
                }
                _productRepository.Save();
                TempData["success"] = "Congratulations your Product was created successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        
        #region API Calls
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> Products = _productRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = Products });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product? productToDelete = _productRepository.Get(u => u.Id == id);
            if(productToDelete == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _productRepository.Remove(productToDelete);
            _productRepository.Save();

            List<Product> Products = _productRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new { success = true, message = "Deleted Successful" });
        }
        #endregion
    }
}
