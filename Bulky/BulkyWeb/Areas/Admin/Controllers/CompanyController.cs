using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all companies from db into list and return them to view 
        public IActionResult Index()
        {
            List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
            return View(companyList);
        }

        // Check if action contain id and based on that return what is needed
        public IActionResult Upsert(int? id)
        {
            if (id == 0 || id == null)
            {
                // Create action - if id=0 or null create and return 'Company' obj
                return View(new Company());
            }
            else
            {
                // Update action - if not id=0 or null return 'Company' obj with that id
                Company obj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(obj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company obj) 
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(obj);
            }
        }

        // Region for API calls
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objForDelete = _unitOfWork.Company.Get(u => u.Id == id);

            if (objForDelete == null)
            {
                return Json(new { success = false, message = "Error while trying to delete company!" });
            }

            _unitOfWork.Company.Remove(objForDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Company deleted successfully!" });
        }

        #endregion

    }
}
