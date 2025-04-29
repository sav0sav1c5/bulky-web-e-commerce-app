using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {

            // Push migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            // Create roles if they are not created
            // This is Async method so we need to await that - used .GetAwaiter().GetResult()
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                // If standard 'CUSTOMER' role does not exist we want to create that role
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                // If roles are not created, we will need to create ADMIN user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@savosavic.com",
                    Email = "savo.savic.in@gmail.com",
                    Name = "Savo Savic",
                    PhoneNumber = "0659514423",
                    StreetAddress = "Bul. Oslobodjenja 66",
                    State = "Serbia",
                    PostalCode = "21000",
                    City = "Novi Sad",
                    EmailConfirmed = true
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser adminUser = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "savo.savic.in@gmail.com");

                _userManager.AddToRoleAsync(adminUser, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
