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
    public class YearController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
      

        public YearController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {

            _db = db;
            _userManager = userManager;

        }
       
        public IActionResult Index()
        {
           

            var years = _db.LookupStatusYearType.ToList();

           


            return View(years);

        }
        
        public IActionResult Edit(int id)
        {

            var FindIDSchool = _db.LookupStatusYearType.Find(id);
            if (FindIDSchool == null) {

                NotFound();
            }
           
           


            return View(FindIDSchool);

        }
        [HttpPost]
        public IActionResult Edit(LookupStatusYearType obj) {


            _db.LookupStatusYearType.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");




        }
        public IActionResult Create()
        {
            


           // ViewBag.ID = _db.LookupStatusYearType.Max(x=>x.ID)+ 1;
            return View();
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LookupStatusYearType obj)
        {
            if (obj == null) {
                NotFound();
            }
            _db.LookupStatusYearType.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");         
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.LookupStatusYearType.Find(id);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.LookupStatusYearType.Remove(objFromDb);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
    }    
}