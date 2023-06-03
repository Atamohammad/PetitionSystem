using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models;

namespace TeacherLoanBoxProject.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;


        public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {

            _db = db;
            _userManager = userManager;

        }
        public IEnumerable<SelectListItem> SelectListItem { get; set; }

        public IActionResult Index()
        {
            var userList = _db.ApplicationUser.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "none";
                    
                }
                else
                {

                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }


            }


            return View(userList);

        }
        public IActionResult Edit(String userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();


            }
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;

            }

            objFromDb.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });



            return View(objFromDb);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();


                }
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                objFromDb.Name = user.Name;
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            user.RoleList = _db.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });


            return View(user);

        }
        [HttpPost]
        public IActionResult LockUnlock(String userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();

            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
                //return RedirectToAction(nameof(Index));
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                // return RedirectToAction(nameof(Index));

            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));



        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(String userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.ApplicationUser.Remove(objFromDb);
            _db.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

    }    
}