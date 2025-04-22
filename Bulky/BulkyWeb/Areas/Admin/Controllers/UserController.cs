using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Also we can use DbContext
        // private readonly ApplicationDbContext _db;
        // private readonly UserManager<IdentityUser> _userManager;

        // public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        // {
            // _db = db;
            // _userManager = userManager;
        // }

        // Get all companies from db into list and return them to view 
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            // We need role id from AspNetUserRoles
            // string userRoleId = _unitOfWork.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            // We need to populate View Model
            RoleManagementViewModel roleManagementViewModel = new RoleManagementViewModel()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),

                // Now we need to populate teo remaining things - dropdowns
                // For that will be used projections
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            // GetRolesAsync - will get roles assigned to user
            roleManagementViewModel.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
            
            return View(roleManagementViewModel);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementViewModel roleManagementViewModel)
        {
            // We need role id that will be stored in View Model -> ApplicationUser.Id
            // string userRoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementViewModel.ApplicationUser.Id).RoleId;

            // Also we need to get old role
            // string oldUserRole = _db.Roles.FirstOrDefault(u => u.Id == userRoleId).Name;

            string oldUserRole = roleManagementViewModel.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementViewModel.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementViewModel.ApplicationUser.Id);

            if (!(roleManagementViewModel.ApplicationUser.Role == oldUserRole))
            {
                // If current role is different than old role that means it's updated
                // ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementViewModel.ApplicationUser.Id);

                // If current role is company we need to assign company to that user
                if (roleManagementViewModel.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementViewModel.ApplicationUser.CompanyId;
                }

                // If old role was company we need to remove company id in application user
                if (oldUserRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                //_db.SaveChanges();
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                // We need to remove old role and add new
                _userManager.RemoveFromRoleAsync(applicationUser, oldUserRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementViewModel.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                // If role stay same but we need to change just company
                if (oldUserRole == SD.Role_Company && applicationUser.CompanyId != roleManagementViewModel.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementViewModel.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }
            
            return View(roleManagementViewModel);
        }

        // Region for API calls
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> usersList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();
            //List<ApplicationUser> usersList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            // var userRolesList = _db.UserRoles.ToList();
            // var rolesList = _db.Roles.ToList();

            // Because company is null for some users, we will create company object and set name to ""
            foreach (var user in usersList)
            {

                // var roleId = userRolesList.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                user.Role = role;

                if (user.Company == null)
                {
                    user.Company = new()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = usersList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            // In AspNetUsers there is column LockoutEnd, if != null and in future account will be locked till that date
            var userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            
            if (userFromDb == null)
            {
                return Json(new {success = false, message = "Error while Locking/Unlocking user!"});
            }

            if (userFromDb != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                // User is locked and need to be unlocked
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            
            // _db.SaveChanges();
            _unitOfWork.ApplicationUser.Update(userFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Operation successfull!" });
        }

        #endregion

    }
}
