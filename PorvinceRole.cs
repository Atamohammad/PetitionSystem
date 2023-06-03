using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeacherLoanBoxProject.Data;
using TBMIS.Models;
using TeacherLoanBoxProject.Models;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TBMIS.Controllers
{
    [Authorize]
    public class ProvinceRoleController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
      

        public ProvinceRoleController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {

            _db = db;
            _userManager = userManager;

        }

      

        public IEnumerable<SelectListItem> SelectListItem { get; set; }


        public bool IsInProvince(string userId, int provinceId)
        {
            int isInProvince = _db.UserProvince.Where(x => x.UserId == userId && x.ProvinceId == provinceId).Count();
            if (isInProvince == 0) return false;
            return true;
        }
      
        public IActionResult Index()
        {
            
            var userProvinces = _db.UserProvince.Include(m=>m.Province).Include(i=>i.User).ToList();

           


            return View(userProvinces);

        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> provice = _db.LookupProvince.Select(x => new SelectListItem
            {
                Text = x.ProvinceNameDari,
                Value=x.Id.ToString()
            });
            ViewBag.provice = provice;
            IEnumerable<SelectListItem> userId = _db.Users.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value=x.Id.ToString()
            });
            ViewBag.userId = userId;

            return View();
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PorvinceRole obj)
        {
            _db.UserProvince.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");         
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.UserProvince.Find(id);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.UserProvince.Remove(objFromDb);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
    }    
}