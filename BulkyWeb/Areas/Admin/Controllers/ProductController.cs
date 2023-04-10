using Bulky.DataAccess.Repository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private ProductRepository _productRepository;
        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public IActionResult Index()
        {
            List<Product> Products = _productRepository.GetAll().ToList();
            return View(Products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Add(product);
                _productRepository.Save();
                TempData["success"] = "Congratulations your Product was created successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id != null && id != 0)
            {
                Product Product = _productRepository.Get(u=>u.Id == id);
                return View(Product);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Update(product);
                _productRepository.Save();
                TempData["success"] = "Congratulations, Your Product was Edited Successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if(id != null && id != 0)
            {
                Product Product = _productRepository.Get(u => u.Id == id);
                return View(Product);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            if(id != null && id != 0)
            {
                Product? productFromDb = _productRepository.Get(u => u.Id == id);
                if(productFromDb != null)
                {
                    _productRepository.Remove(productFromDb);
                    _productRepository.Save();
                    TempData["success"] = "Congratulations, the product was Deleted Successfully";
                    return RedirectToAction("Index", "Product");

                }
            }
            return NotFound();
        }
    }
}
