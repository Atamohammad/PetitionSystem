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
    public class InsertSchoolController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
      

        public InsertSchoolController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
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
            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(x => new SelectListItem
            {
                Text = x.DistrictNameDari,
                Value = x.Id.ToString()
            });
            ViewBag.DistrictIDs = DistrictID;

            var provinces = _db.LookupSchool.Include(x=>x.District).Take(500).ToList();

           


            return View(provinces);

        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchTerm /*string Stype*/)
        {
            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(x => new SelectListItem
            {
                Text = x.DistrictNameDari,
                Value = x.Id.ToString()
            });
            ViewBag.DistrictIDs = DistrictID;


            try
            {
                if (searchTerm != null)
                {
                    // ViewData["Search"] = searchTerm;
                    // ViewData["sSearch"] = Stype;
                    var query = from x in _db.LookupSchool.Include(x=>x.District) select x;

                    if (!string.IsNullOrEmpty(searchTerm))
                    {

                        query = query.Where(x => x.SchoolNameDari.Contains(searchTerm));
                      //  && x.District.DistrictNameDari.Contains(Stype));
                    }
                  
                    return View(await query.AsNoTracking().ToListAsync());

                }
                else
                {
                  

                    return View();

                }
            }
            catch (Exception)
            {

                //ViewBag.Error = "This Name Not Found.";
                return View();

            }
        }
        public IActionResult Edit(int id)
        {

            var FindIDSchool = _db.LookupSchool.Find(id);
            if (FindIDSchool == null) {

                NotFound();
            }
            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(x => new SelectListItem
            {
                Text = x.DistrictNameDari,
                Value = x.Id.ToString()
            });
            ViewBag.DistrictIDs = DistrictID;


           


            return View(FindIDSchool);

        }
        [HttpPost]
        public IActionResult Edit(LookupSchool obj) {


            _db.LookupSchool.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");




        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(x => new SelectListItem
            {
                Text = x.DistrictNameDari,
                Value=x.Id.ToString()
            });
            ViewBag.DistrictIDs = DistrictID;


            ViewBag.ID = _db.LookupSchool.Max(x=>x.ID)+ 1;
            return View();
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LookupSchool obj)
        {
            if (obj == null) {
                NotFound();
            }
            _db.LookupSchool.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");         
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.LookupSchool.Find(id);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.LookupSchool.Remove(objFromDb);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        
    }    
}