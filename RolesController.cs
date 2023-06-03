using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TBMIS.Models;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models;

namespace TeacherLoanBoxProject.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RolesController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager )
        {

            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public  IEnumerable<SelectListItem> SelectListItem{ get; set; }

        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();

            return View(roles);

        } 
      
       
        public IActionResult Upsert(String id)
        {
            if (String.IsNullOrEmpty(id))
            {

                return View();
            }
            else {
                var objFromDb = _db.Roles.FirstOrDefault(u=>u.Id==id);
                return View(objFromDb);
            }

    

          

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Upsert(IdentityRole roleObj)
        {
            if (await _roleManager.RoleExistsAsync(roleObj.Name))
            {
                //erro
                TempData[WC.Error]= "Role is Exists";


            }

            if (String.IsNullOrEmpty(roleObj.Id))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = roleObj.Name });
                TempData[WC.Success] = "Role is Success.";
                return RedirectToAction(nameof(Index));


            }
            else
            {
              
                var objRoleFromDb = _db.Roles.FirstOrDefault(u => u.Id == roleObj.Id);
                if (objRoleFromDb == null) { 
                TempData[WC.Error] = "Role is not Found.";
                    return RedirectToAction(nameof(Index));

                }
                objRoleFromDb.Name = roleObj.Name;
               objRoleFromDb.NormalizedName = roleObj.Name.ToUpper();
               var result= await _roleManager.UpdateAsync(objRoleFromDb);
            }

            return RedirectToAction(nameof(Index));
        }
      
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(String id)
        {
            var objFromDb = _db.Roles.FirstOrDefault(u => u.Id ==id);
            if (objFromDb == null) {
                
            return RedirectToAction(nameof(Index));


            }
            var userRoleForThisRole = _db.UserRoles.Where(u => u.RoleId == id).Count();
            if (userRoleForThisRole > 0) { 
            
            return RedirectToAction(nameof(Index));


            }
            await _roleManager.DeleteAsync(objFromDb);

            return RedirectToAction(nameof(Index));
        }

       

    }    
}