using Bulky.DataAccess.Repository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly CompanyRepository _companyRepository;
        public CompanyController(CompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public IActionResult Index()
        {
            List<Company> companyList = _companyRepository.GetAll().ToList();
            return View(companyList);
        }

        public IActionResult Upsert(int? id) 
        {
            if(id != null && id != 0)
            {
                Company? company = _companyRepository.Get(u => u.Id == id);
                if(company != null)
                {
                    return View(company);
                } else
                {
                    return NotFound();
                }
            } else
            {
                Company newCompany = new Company { Name = "" };
                return View(newCompany);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    _companyRepository.Add(obj);
                    TempData["success"] = "Congratulations, New Company was Created Successfully";
                } else
                {
                    Company companyfromDb = _companyRepository.Get(u => u.Id == obj.Id);
                    companyfromDb.Name = obj.Name;
                    companyfromDb.PhoneNumber = obj.PhoneNumber;
                    TempData["success"] = "Congratulations, New Company was Created Successfully";
                }
                _companyRepository.Save();
                return RedirectToAction("Index", "Company");
            }
            return View();
        }



        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = _companyRepository.GetAll().ToList();
            return Json(new { data = Companies });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company? companyToDelete = _companyRepository.Get(u => u.Id == id);
            if (companyToDelete == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _companyRepository.Remove(companyToDelete);
            _companyRepository.Save();

            List<Company> Companies = _companyRepository.GetAll().ToList();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
        #endregion
    }
}
