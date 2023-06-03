using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TBMIS.Controllers;
using TBMIS.Models;
using TeacherLoanBoxProject;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models;
using TeacherLoanBoxProject.Models.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TeacherLoanBoxProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly       ApplicationDbContext _db;
        private readonly IWebHostEnvironment  _webHostEnvironment;
        private readonly ProvinceRoleController puser;
        private readonly IHtmlLocalizer<HomeController> _localizer;
        // private readonly   _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db,IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, IHtmlLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _localizer=localizer;
            puser  = new ProvinceRoleController(_db, _userManager);
        }

        public IEnumerable<SelectListItem> TblLoanRecipientSelectList { get; set; }

        [HttpPost]
        public IActionResult CultureManagement(string culture,string returnUrl)
        {
            var str = "";
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
             new CookieOptions { Expires = DateTime.Now.AddDays(30)});
            str = culture;
           // ViewBage.culture = culture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            if (culture == "fa") { return LocalRedirect(returnUrl); }
            else{ return LocalRedirect(returnUrl); }

           // return LocalRedirect(returnUrl);
        }
        public IActionResult CultureManagement1(string culture)
        {
            var str = "";
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
             new CookieOptions { Expires = DateTime.Now.AddDays(30) });
            str = culture;
            // ViewBage.culture = culture;
         
            if (culture == "fa") { return Redirect(Request.Headers["Referer"].ToString()); }
            else { return Redirect(Request.Headers["Referer"].ToString());}

            // return LocalRedirect(returnUrl);
        }

        public IActionResult GoBack()
        {
            var name = _db.TblLoanRecipient.FirstOrDefault().Name;
            var status = _db.TblLoanRecipient.FirstOrDefault().StatusType;

            Index();

            return RedirectToAction("Index");

        }
        [Authorize]
        public JsonResult GetDistricts(int id)
        {
            var districts = _db.LookupDistrict.Where(x => x.ProvinceId == id).
                Select(x => new SelectListItem() { Text = x.DistrictNameDari, Value = x.Id.ToString() });

            return Json(districts);
        }
        public JsonResult GetSchools(int id)
        {

            var schools = _db.LookupSchool.Where(x => x.DistrictId==id).
                Select(x => new SelectListItem() { Text = x.SchoolNameDari, Value = x.ID.ToString() });
            

            return Json(schools);
        } 
        public JsonResult GetSchoolCode(int id)
        {

            var schoolCode = _db.LookupSchool.Where(x => x.ID == id).
                Select(x => new SelectListItem() { Text = x.SchoolCode.ToString(), Value = x.SchoolCode.ToString() });

            return Json(schoolCode);
        }
        [Authorize]
        public JsonResult GetPermanantP(int id)
        {

            var permanatD = _db.LookupDistrict.Where(x => x.ProvinceId == id).
                Select(x => new SelectListItem() { Text = x.DistrictNameDari.ToString(), Value = x.Id.ToString() });


            return Json(permanatD);
        }  
        public JsonResult GetTempraryP(int id)
        {

            var TemprarayP = _db.LookupDistrict.Where(x => x.ProvinceId == id).
                Select(x => new SelectListItem() { Text = x.DistrictNameDari.ToString(), Value = x.Id.ToString() });


            return Json(TemprarayP);
        }
        public JsonResult GetSchool(int id)
        {

            var schools = _db.LookupSchool.Where(x => x.DistrictId == id).
                Select(x => new SelectListItem() { Text = x.SchoolNameDari, Value = x.ID.ToString() });


            return Json(schools);
        }
        public JsonResult GetName(int id)
        {

            var Name = _db.TblLoanRecipient.Where(x => x.SchoolId == id).
                Select(x => new SelectListItem() { Text = x.Name.ToString(), Value = x.Id.ToString() });

            return Json(Name);
        }
        public JsonResult GetFatherName(int id)
        {

            var FatherName = _db.TblLoanRecipient.Where(x => x.Id == id).
                Select(x => new SelectListItem() { Text = x.FatherName.ToString(), Value = x.Id.ToString() });


            return Json(FatherName);
        }
        public JsonResult GetStatus(int id)
        {

            var Status = _db.TblLoanRecipient.Where(x => x.Id == id).
                Select(x => new SelectListItem() { Text = x.StatusTypeNavigation.StatusTypeDari, Value = x.Id.ToString() });


            return Json(Status);
        }
        [Authorize]
        public JsonResult GetDataList(int id,string type)
        {
            // var loanid = _db.TblLoanRecipient.FirstOrDefault().Id;
            var list = _db.TblLoanRecipient
                                .Include(x => x.Province)
                                .Include(x => x.School)
                                .Include(x => x.District)
                                .Select(x => new LoanRecipientCard
                                {
                                    Id = x.Id,
                                    NumberS = x.Numbersbt,
                                    SaliMali =x.inversCard.FirstOrDefault().SaliMali,
                                    Name = x.Name,
                                    FatherName = x.FatherName,
                                    ProvinceName = x.Province.ProvinceNameDari,
                                    DistrictName = x.District.DistrictNameDari,
                                    SchoolName = x.School.SchoolNameDari,
                                    TazkeraNo = x.TazkeraNo,
                                    AccountNumber = x.AccountNumber,
                                    IdnumSalary = x.IdnumSalary,
                                    StatusType = x.StatusType,
                                    ProvinceId = x.ProvinceId,
                                    DistrictId = x.DistrictId,
                                    SchoolId = x.SchoolId
                                }).AsEnumerable<LoanRecipientCard>();


            if (!User.IsInRole("Admin"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                list = list.Where(x => puser.IsInProvince(userId, x.ProvinceId));
            }

            //if (type == "p") list = list.Where(x => x.ProvinceId == id);
            if (type == "d")
            {
                list = list.Where(x =>x.DistrictId == id);
            }
            else if (type == "s")
            {
                list = list.Where(x => x.SchoolId == id);
            }
           
            else if (type == "n")
            {
                list = list.Where(x => x.Id == id);
            }
            else if (type == "fn")
            {
                list = list.Where(x => x.Id == id);
            }
            else if (type == "st")
            {
                list = list.Where(x => x.Id == id);
            }


            //if (type == "d") list = list.Where(x => x.DistrictId == id);
            //else if (type == "s") list = list.Where(x => x.SchoolId == id);
            //else if (type == "n")
            //{
            //    var aa = list.Where(x => x.Id == id).FirstOrDefault(x=>x.Id==id);
            //    list = list.Where(x => x.Name.Contains(aa.Name));
            //}
            //else if (type == "fn") list = list.Where(x => x.Id == id);
            //else if (type == "st") list = list.Where(x => x.StatusType == id);

            return Json(list.ToList());
        }
        [Authorize]
        public IActionResult Index()
        {
            //int pageNumber = 1
            //int rows = 100;
            //int startIndex = (pageNumber - 1) * rows;
            //int pages = 1;
            //var date = DateTime.Today;
            //var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
            //       .Include(x => x.LoanRecipient.School)
            //       .Include(x => x.LoanRecipient.Province)
            //       .Where(x => x.TrackingDate >= date)
            //       .Take(100).AsEnumerable<LoanRecipientIndividualCard>();
            //if (!User.IsInRole("Admin"))
            //{
            //    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //    list = list.Where(x => puser.IsInProvince(userId, x.LoanRecipient.ProvinceId)).Where(x => x.TrackingDate >= date)
            //    .Take(100).OrderByDescending(x => x.Id);
            //}
            var cardID = _db.LoanRecipientIndividualCard.FirstOrDefault().Id;
            ViewBag.Toaldebit = _db.LoanRecipientIndividualCard.Where(x => x.Id==cardID&&x.LoanTypeId == 1).Count();
            
           // var SaliMali = _db.LoanRecipientIndividualCard.FirstOrDefault()?.SaliMali;
            var LoanrecreipientId = _db.TblLoanRecipient.FirstOrDefault().Id;
            ViewBag.SaliMali = LoanrecreipientId;
            


            var date = DateTime.Today;

            var list = _db.TblLoanRecipient.Include(y=> y.inversCard).Include(x=>x.Province).Include(x=>x.School).Include(x=>x.District)
                   .Where(x => x.TrackingDate >= date)
                   .Take(100).AsEnumerable<TblLoanRecipient>();
            if (!User.IsInRole("Admin")) {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                list = list.Where(x => puser.IsInProvince(userId, x.ProvinceId)).Where(x => x.TrackingDate >= date)
                .Take(100).OrderByDescending(x => x.Id);
            }


            // it is to show the salimali in the indext view
            //var id = _db.LoanRecipientIndividualCard.Include(x=>x.LoanRecipient);
            //if (id.Count() > 0)
            //{
            //    foreach (var i in id)
            //    {

            //        ViewBag.SaliMali = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == i.Id)?.SaliMali;

            //    }
            //}
            //pages = (int )Math.Ceiling((float)list.Count() / (float)100);

            //list = list.Skip(startIndex).Take(rows);

            var rId = _db.TblLoanRecipient;

            IEnumerable<SelectListItem> Province = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Value = i.Id.ToString()
               // Selected = (i.Id == rId.FirstOrDefault().ProvinceId) ? true : false
                // Selected = (x.Id == obj.SubjectId) ? true : false



            });
            ViewBag.Province = Province;


            IEnumerable<SelectListItem> District = _db.LookupDistrict.Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Value = i.Id.ToString()
                //Selected = (i.Id == rId.FirstOrDefault().DistrictId) ? true : false



            });
            ViewBag.District = District;

            IEnumerable<SelectListItem> Name = _db.TblLoanRecipient.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            

            });
            ViewBag.Name = Name;

            IEnumerable<SelectListItem> Father = _db.TblLoanRecipient.Select(i => new SelectListItem
            {
                Text = i.FatherName,
                Value = i.Id.ToString()
              

            });
            ViewBag.Father = Father;

            IEnumerable<SelectListItem> Status = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Value = i.Id.ToString()
              

            });
            ViewBag.Status = Status;
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()
                //Selected = (i.Id == rId.FirstOrDefault().SchoolId) ? true : false



            });
            ViewBag.School = School;
            IEnumerable<SelectListItem> Statuss = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Value = i.StatusTypeDari


            });
            ViewBag.StatusTypeData = Statuss;

            return View(list);

        }
        public void TheData() {
            IEnumerable<SelectListItem> Province = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Value = i.Id.ToString()
               

            });
            ViewBag.Province = Province;

            IEnumerable<SelectListItem> District = _db.LookupDistrict.Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Value = i.Id.ToString()
                

            });
            ViewBag.District = District;

            IEnumerable<SelectListItem> Name = _db.TblLoanRecipient.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
              

            });
            ViewBag.Name = Name;

            IEnumerable<SelectListItem> Father = _db.TblLoanRecipient.Select(i => new SelectListItem
            {
                Text = i.FatherName,
                Value = i.Id.ToString()
               

            });
            ViewBag.Father = Father;

            IEnumerable<SelectListItem> Status = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Value = i.Id.ToString()
               

            });
            ViewBag.Status = Status;

            IEnumerable<SelectListItem> Statuss = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Value = i.StatusTypeDari
               

            });
            ViewBag.StatusTypeData = Statuss;
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()
              

            });
            ViewBag.School = School;

        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchTerm, string Stype)
        {


            //var cardID = _db.TblLoanRecipient.FirstOrDefault().Id;
           // ViewBag.Toaldebit = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId ==x.LoanRecipientId && x.LoanTypeId == 1).Count();
           // var SaliMali = _db.LoanRecipientIndividualCard.LastOrDefault()?.SaliMali;
            //x => Convert.ToInt32(x)

            //var ToTalComplated = _db.LoanRecipientIndividualCard
            //    .Where(x=>x.LoanTypeId==2)
            //    .Select(x => x.MonthlyInstallment.MonthlyAmount).Sum(x=>Convert.ToInt32(x));
            //var SubTotal = _db.LoanRecipientIndividualCard
            //    .Where(x => x.LoanTypeId == 2)
            //    .Select(x => x.OtherAmounts).Sum(x => Convert.ToInt32(x));
            //ViewBag.Alltotal = ToTalComplated + SubTotal;
            //var amountofLoan = _db.LoanRecipientIndividualCard.Select(x => x.LoanAmount);

            //if(ViewBag.Alltotal)
            // var selectCardId = _db.LoanRecipientIndividualCard.Select(s=>s.Id);
            // ViewBag.SelectAmount = _db.LoanRecipientIndividualCard.Where(x=>x.LoanRecipientId==selectCardId).Select(x=>x.LoanAmount.LoanAmount).Sum();
            try
            {
                        if (searchTerm != null)
                    {
                       // ViewData["Search"] = searchTerm;
                       // ViewData["sSearch"] = Stype;
                        var query = from x in _db.TblLoanRecipient.Include(x => x.Province).Include(x => x.District).Include(x => x.School).Include(x => x.inversCard) select x;
                        
                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                  
                                query = query.Where(x => x.Name.Contains(searchTerm)&&x.StatusTypeNavigation.StatusTypeDari.Contains(Stype));
                            }
                            TheData();
                            return View(await query.AsNoTracking().ToListAsync());
                  
                    }
                    else
                    {
                        TheData();
                        return View();
                  
                    }
            }
            catch (Exception)
            {

                //ViewBag.Error = "This Name Not Found.";
                return View();

            }
        }
        //[HttpPost]
        //public IActionResult Index(int pid, int did, int sid, int Stype, int NName, int Fname)
        //{

        //    ViewData["Search"] = pid;
        //    ViewData["dSearch"] = did;
        //    ViewData["sSearch"] = sid;
        //    ViewData["nSearch"] = NName;
        //    ViewData["fSearch"] = Fname;
        //    ViewData["stSearch"] = Stype;

        //    var obj = _db.TblLoanRecipient.Include(x => x.inversCard).Include(x => x.Province).Include(x => x.District).Include(x => x.School).Where(x => x.ProvinceId == pid && x.DistrictId == did).Take(200);
        //    //var obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x=>x.District)
        //    //    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.Id == NName && x.Id == Fname && x.StatusType == Stype).AsEnumerable<TblLoanRecipient>().Take(100);


        //    if (sid > 0)
        //        obj = obj.Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid);
        //    if (Stype > 0)
        //        obj = obj.Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid &&x.StatusType == Stype);
        //    if (NName > 0)
        //        obj = obj.Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.StatusType == Stype && x.Id == NName);
        //    if (Fname > 0)
        //        obj = obj.Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.StatusType == Stype && x.Id == NName && x.Id == Fname);
          
        //    //it is used to get the province and district and school data
        //    TheData();

        //    return View(obj.ToList()/*query.AsNoTracking().ToList()*/);

        //}
        [Authorize]
        [HttpGet]
        public IActionResult Dashboard(ReportVM report)
        {
            var totalrecip = _db.TblLoanRecipient.Where(l => l.StatusType == 1).Count();
            var notcomplate = _db.TblLoanRecipient.Where(l => l.StatusType == 2).Count();

            var terminated = _db.TblLoanRecipient.Where(l => l.StatusType == 3).Count();
            var forgeven = _db.TblLoanRecipient.Where(l => l.StatusType == 5).Count();
            var totalmale = _db.TblLoanRecipient.Where(l =>l.Gender == "Male").Count();
            var totalfemale = _db.TblLoanRecipient.Where(l =>l.Gender == "Female").Count();


            var totalcredit = _db.LoanRecipientIndividualCard.Where(c => c.LoanTypeId == 2).Count();
           
            var totaldebit = _db.LoanRecipientIndividualCard.Where(l=>l.LoanTypeId==1).Count();

            var totalOfRecipient = _db.TblLoanRecipient.Count();
            var idOfRecipient = _db.LoanRecipientIndividualCard.Select(x=>x.LoanRecipientId);

            report = new ReportVM()
            {

                TotalLoanRecipientStatusCompleted = totalrecip,
                TotalLoanRecipientStatusPending = notcomplate,
                TotalLoanRecipientStatusTerminated = terminated,
                TotalLoanRecipientStatusForgiven = forgeven,
                TotalLoanRecipient = totalOfRecipient,
                TotalMale = totalmale,
                TotalFemale = totalfemale,
                TotalCredit = totalcredit,
                TotalDebit = totaldebit,

            };
           return View(report);
        }
        public IActionResult Create(int? id)

        {

            //ViewData[]=SelectList(_db.LookupReasonType"ProvinceNameDari","Id")
            //ViewData["ProvinceID"] = new SelectList(_db.LookupProvince, "Id", "ProvinceNameDari");
            IEnumerable<SelectListItem> ProvinceID = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.ProvinceID = ProvinceID;

            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(i=> new SelectListItem { 
               Text = i.DistrictNameDari,
                Selected = true,
                Value = i.Province.ToString()
                 });
            ViewBag.DistrictID = DistrictID; 
            IEnumerable<SelectListItem> SchoolID = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Selected = true,
                Value = i.ID.ToString()

            });

            ViewBag.SchoolID = SchoolID;


            IEnumerable<SelectListItem> SchoolType = _db.LookupSchoolType.Select(i => new SelectListItem
            {
                Text = i.SchoolTypeDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.SchoolType = SchoolType;

            IEnumerable<SelectListItem> TazkiraType = _db.LookupTazkeeraType.Select(i => new SelectListItem
            {
                Text = i.TazkeeraTypeDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.TazkiraType = TazkiraType;
            
            IEnumerable<SelectListItem> PermanantProvince = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.PermanantProvince = PermanantProvince;
           
            IEnumerable<SelectListItem> PermanantDistrict = _db.LookupDistrict.Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.PermanantDistrict = PermanantDistrict;


            IEnumerable<SelectListItem> TempraryProvince = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.TempraryProvince = TempraryProvince;
           
            IEnumerable<SelectListItem> TempraryDistrict = _db.LookupDistrict.Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.TempraryDistrict = TempraryDistrict;


            IEnumerable<SelectListItem> EducationType = _db.LookupEducationType.Select(i => new SelectListItem
            {
                Text = i.EducationType,
                Selected = true,
                Value = i.Id.ToString()
            });

            ViewBag.EducationType = EducationType;
            IEnumerable<SelectListItem> JobType = _db.LookupJobType.Select(i => new SelectListItem
            {
                Text = i.JobTypeDari,
                Selected = true,
                Value = i.Id.ToString()

            });

            ViewBag.JobType = JobType;
            IEnumerable<SelectListItem> StatusType = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Selected = true,
                Value = i.Id.ToString()

            });
            ViewBag.StatusType = StatusType;
            IEnumerable<SelectListItem> StatusYear = _db.LookupStatusYearType.Select(i => new SelectListItem
            {
                Text = i.StatusYearType,
                Selected = true,
                Value = i.Id.ToString()
            });

            ViewBag.StatusYear = StatusYear;

            IEnumerable<SelectListItem> Schoolcode = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolCode.ToString(),
           
                Value = i.ID.ToString()

            });

            ViewBag.Schoolcode = Schoolcode;
    

            TblLoanRecipient tbl = new TblLoanRecipient();
            if (id == null)
            {
                return View(tbl);

            }
            else
            {
                tbl = _db.TblLoanRecipient.Find(id);
                if (tbl == null)
                {
                    return NotFound();
                }
                return View(tbl);
            }

           
        
        }
        [Authorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TblLoanRecipient tbl)

        {

            try
            {
                if (ModelState.IsValid)
                {

                    // creating 

                    var files = HttpContext.Request.Form.Files;
                    String WebRootPath = _webHostEnvironment.WebRootPath;
                    if (tbl.Id == 0)
                    {
                        if (files.Count() != 0)
                        {
                            String upload = WebRootPath + WC.ImagePath;
                            String fileName = Guid.NewGuid().ToString();
                            String extension = Path.GetExtension(files[0].FileName);
                            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                            {
                                files[0].CopyTo(fileStream);
                            }

                            tbl.ImagePath = fileName + extension;

                        }

                        tbl.TrackingDate = DateTime.Now;
                        var user = _userManager.GetUserId(User);
                        tbl.UserId = user;
                        _db.TblLoanRecipient.Add(tbl);

                    }

                    else
                    {

                        //updating 
                        var objFromDb = _db.TblLoanRecipient.AsNoTracking().FirstOrDefault(u => u.Id == tbl.Id);
                        if (files.Count > 0)
                        {
                            string upload = WebRootPath + WC.ImagePath;
                            String fileName = Guid.NewGuid().ToString();
                            String extension = Path.GetExtension(files[0].FileName);

                            var oldfile = Path.Combine(upload, objFromDb.ImagePath);

                            if (System.IO.File.Exists(oldfile))
                            {
                                System.IO.File.Delete(oldfile);
                            }
                            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                            {
                                files[0].CopyTo(fileStream);
                            }
                            tbl.ImagePath = objFromDb.ImagePath;
                        }
                        else
                        {
                            tbl.ImagePath = objFromDb.ImagePath;

                        }

                        var user = _userManager.GetUserId(User);
                        tbl.UserId = user;
                        _db.TblLoanRecipient.Update(tbl);

                    }

                    _db.SaveChanges();
                    TempData["Success"] = "Created successfully";


                    return RedirectToAction("Index", new { id = tbl.Id });
                }
                else
                {
                    IEnumerable<SelectListItem> ProvinceID = _db.LookupProvince.Select(i => new SelectListItem
                    {
                        Text = i.ProvinceNameDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.ProvinceID = ProvinceID;

                    IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Select(i => new SelectListItem
                    {
                        Text = i.DistrictNameDari,
                        Selected = true,
                        Value = i.Province.ToString()
                    });
                    ViewBag.DistrictID = DistrictID;
                    IEnumerable<SelectListItem> SchoolID = _db.LookupSchool.Select(i => new SelectListItem
                    {
                        Text = i.SchoolNameDari,
                        Selected = true,
                        Value = i.ID.ToString()

                    });

                    ViewBag.SchoolID = SchoolID;


                    IEnumerable<SelectListItem> SchoolType = _db.LookupSchoolType.Select(i => new SelectListItem
                    {
                        Text = i.SchoolTypeDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.SchoolType = SchoolType;

                    IEnumerable<SelectListItem> TazkiraType = _db.LookupTazkeeraType.Select(i => new SelectListItem
                    {
                        Text = i.TazkeeraTypeDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.TazkiraType = TazkiraType;

                    IEnumerable<SelectListItem> PermanantProvince = _db.LookupProvince.Select(i => new SelectListItem
                    {
                        Text = i.ProvinceNameDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.PermanantProvince = PermanantProvince;

                    IEnumerable<SelectListItem> PermanantDistrict = _db.LookupDistrict.Select(i => new SelectListItem
                    {
                        Text = i.DistrictNameDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.PermanantDistrict = PermanantDistrict;


                    IEnumerable<SelectListItem> TempraryProvince = _db.LookupProvince.Select(i => new SelectListItem
                    {
                        Text = i.ProvinceNameDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.TempraryProvince = TempraryProvince;

                    IEnumerable<SelectListItem> TempraryDistrict = _db.LookupDistrict.Select(i => new SelectListItem
                    {
                        Text = i.DistrictNameDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.TempraryDistrict = TempraryDistrict;


                    IEnumerable<SelectListItem> EducationType = _db.LookupEducationType.Select(i => new SelectListItem
                    {
                        Text = i.EducationType,
                        Selected = true,
                        Value = i.Id.ToString()
                    });

                    ViewBag.EducationType = EducationType;
                    IEnumerable<SelectListItem> JobType = _db.LookupJobType.Select(i => new SelectListItem
                    {
                        Text = i.JobTypeDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });

                    ViewBag.JobType = JobType;
                    IEnumerable<SelectListItem> StatusType = _db.LookupStatusType.Select(i => new SelectListItem
                    {
                        Text = i.StatusTypeDari,
                        Selected = true,
                        Value = i.Id.ToString()

                    });
                    ViewBag.StatusType = StatusType;
                    IEnumerable<SelectListItem> StatusYear = _db.LookupStatusYearType.Select(i => new SelectListItem
                    {
                        Text = i.StatusYearType,
                        Selected = true,
                        Value = i.Id.ToString()
                    });

                    ViewBag.StatusYear = StatusYear;
                    
                    IEnumerable<SelectListItem> Schoolcode = _db.LookupSchool.Select(i => new SelectListItem
                    {
                        Text = i.SchoolCode.ToString(),
                        Selected = true,
                        Value = i.ID.ToString()

                    });

                    ViewBag.Schoolcode = Schoolcode;

                    TempData[WC.Error] = " Error Occurs.";

                    return View();
                }
            }
            catch (Exception) {

                TempData["Danger"] = " Error Occurs.";
                ViewBag.Error = "شما کادرسها را خالی گذاشتید.";
                return View();

            }


        }
        
        [HttpGet]
        public IActionResult Delete(int id)

         {
            if (id == null || id == 0)
            {


                return NotFound();
            }
            // var objs = _db.TblLoanRecipient;

            var obj = _db.TblLoanRecipient.Find(id);
            
                if (obj == null)
            {

                return NotFound();
            }


            return View(obj);
            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            //var obj2 = _db.LoanRecipientIndividualCard.FirstOrDefault(i=>i.Id==id);
            //if (obj2 != null)
            //{
            //    _db.LoanRecipientIndividualCard.Remove(obj2);
            //    _db.SaveChanges();
            //}
            var objFromDb = _db.TblLoanRecipient.FirstOrDefault(u => u.Id == id);
            try
            {

                if (objFromDb == null)
                {

                    return NotFound();

                }
                string upload = _webHostEnvironment.WebRootPath = WC.ImagePath;

                if (upload == null)
                {
                    String fileName = Guid.NewGuid().ToString();


                    var oldfile = Path.Combine(upload, objFromDb.ImagePath);

                    if (System.IO.File.Exists(oldfile))
                    {
                        System.IO.File.Delete(oldfile);

                    }
                }
                _db.TblLoanRecipient.Remove(objFromDb);
                _db.SaveChanges();

                return RedirectToAction(nameof(Index), new { id = objFromDb.Id });
            }
            catch (Exception ex)
            {

                ViewBag.FError = "شما نه میتوانید که این را حذف کنید اول  شما کارت را حذف کنید" + ex;

            }
                return RedirectToAction(nameof(Index), new { id = objFromDb.Id });


        }

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var objFromDb = _db.TblLoanRecipient.Find(id);

            if (objFromDb == null)
            {
                NotFound();
            }
            IEnumerable<SelectListItem> ProvinceID = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.ProvinceID = ProvinceID;

            IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Where(p=>p.ProvinceId== objFromDb.ProvinceId).Select(i => new SelectListItem
            {


                Text = i.DistrictNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.DistrictID = DistrictID;
            IEnumerable<SelectListItem> SchoolID = _db.LookupSchool.Where(p => p.DistrictId == objFromDb.DistrictId).Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });

            ViewBag.SchoolID = SchoolID;


            IEnumerable<SelectListItem> SchoolType = _db.LookupSchoolType.Select(i => new SelectListItem
            {
                Text = i.SchoolTypeDari,
                Value = i.Id.ToString()

            });

            ViewBag.SchoolType = SchoolType;

            IEnumerable<SelectListItem> TazkiraType = _db.LookupTazkeeraType.Select(i => new SelectListItem
            {
                Text = i.TazkeeraTypeDari,
                Value = i.Id.ToString()

            });

            ViewBag.TazkiraType = TazkiraType;


            IEnumerable<SelectListItem> PermanantProvince = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.PermanantProvince = PermanantProvince;

            IEnumerable<SelectListItem> PermanantDistrict = _db.LookupDistrict.Where(p => p.ProvinceId == objFromDb.PermanantProvince).Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.PermanantDistrict = PermanantDistrict;


            IEnumerable<SelectListItem> TempraryProvince = _db.LookupProvince.Select(i => new SelectListItem
            {
                Text = i.ProvinceNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.TempraryProvince = TempraryProvince;

            IEnumerable<SelectListItem> TempraryDistrict = _db.LookupDistrict.Where(p => p.ProvinceId == objFromDb.TempraryProvince).Select(i => new SelectListItem
            {
                Text = i.DistrictNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.TempraryDistrict = TempraryDistrict;


            IEnumerable<SelectListItem> EducationType = _db.LookupEducationType.Select(i => new SelectListItem
            {
                Text = i.EducationType,
                Value = i.Id.ToString()
            });

            ViewBag.EducationType = EducationType;
            IEnumerable<SelectListItem> JobType = _db.LookupJobType.Select(i => new SelectListItem
            {
                Text = i.JobTypeDari,
                Value = i.Id.ToString()

            });

            ViewBag.JobType = JobType;
            IEnumerable<SelectListItem> StatusType = _db.LookupStatusType.Select(i => new SelectListItem
            {
                Text = i.StatusTypeDari,
                Value = i.Id.ToString()

            });
            ViewBag.StatusType = StatusType;
            IEnumerable<SelectListItem> StatusYear = _db.LookupStatusYearType.Select(i => new SelectListItem
            {
                Text = i.StatusYearType,
                Value = i.Id.ToString()

            });

            ViewBag.StatusYear = StatusYear;

            IEnumerable<SelectListItem> Schoolcode = _db.LookupSchool.Where(s=>s.ID==objFromDb.SchoolId).Select(i => new SelectListItem
            {
                Text = i.SchoolCode.ToString(),
                Value = i.SchoolCode.ToString()

            });

            ViewBag.Schoolcode = Schoolcode;


           
           
            return View(objFromDb);
        } 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TblLoanRecipient tbl)
        {
            var files = HttpContext.Request.Form.Files;
            String WebRootPath = _webHostEnvironment.WebRootPath;
            var objFromDb = _db.TblLoanRecipient.AsNoTracking().FirstOrDefault(u => u.Id == tbl.Id);
            if (ModelState.IsValid)
            {

                
                if (files.Count > 0)
                {

                    string upload = WebRootPath + WC.ImagePath;
                    if (objFromDb.ImagePath !=null)
                    {
                        var oldfile = Path.Combine(upload, objFromDb.ImagePath);

                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);

                        }
                    }
                    String fileName = Guid.NewGuid().ToString();
                    String extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {

                        files[0].CopyTo(fileStream);
                    }

                    tbl.ImagePath = fileName + extension;
                    //objFromDb.ImagePath;


                }
                else
                {
                    tbl.ImagePath = objFromDb.ImagePath;

                }

                var user = _userManager.GetUserId(User);
                tbl.UserId = user;

                _db.TblLoanRecipient.Update(tbl);
                _db.SaveChanges();

                return RedirectToAction("Index", new { id = tbl.Id });
            }
            else {
                IEnumerable<SelectListItem> ProvinceID = _db.LookupProvince.Select(i => new SelectListItem
                {
                    Text = i.ProvinceNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.ProvinceID = ProvinceID;

                IEnumerable<SelectListItem> DistrictID = _db.LookupDistrict.Where(p => p.ProvinceId == objFromDb.ProvinceId).Select(i => new SelectListItem
                {


                    Text = i.DistrictNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.DistrictID = DistrictID;
                IEnumerable<SelectListItem> SchoolID = _db.LookupSchool.Where(p => p.DistrictId == objFromDb.DistrictId).Select(i => new SelectListItem
                {
                    Text = i.SchoolNameDari,
                    Value = i.ID.ToString()

                });

                ViewBag.SchoolID = SchoolID;


                IEnumerable<SelectListItem> SchoolType = _db.LookupSchoolType.Select(i => new SelectListItem
                {
                    Text = i.SchoolTypeDari,
                    Value = i.Id.ToString()

                });

                ViewBag.SchoolType = SchoolType;

                IEnumerable<SelectListItem> TazkiraType = _db.LookupTazkeeraType.Select(i => new SelectListItem
                {
                    Text = i.TazkeeraTypeDari,
                    Value = i.Id.ToString()

                });

                ViewBag.TazkiraType = TazkiraType;


                IEnumerable<SelectListItem> PermanantProvince = _db.LookupProvince.Select(i => new SelectListItem
                {
                    Text = i.ProvinceNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.PermanantProvince = PermanantProvince;

                IEnumerable<SelectListItem> PermanantDistrict = _db.LookupDistrict.Where(p => p.ProvinceId == objFromDb.PermanantProvince).Select(i => new SelectListItem
                {
                    Text = i.DistrictNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.PermanantDistrict = PermanantDistrict;


                IEnumerable<SelectListItem> TempraryProvince = _db.LookupProvince.Select(i => new SelectListItem
                {
                    Text = i.ProvinceNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.TempraryProvince = TempraryProvince;
                
                IEnumerable<SelectListItem> TempraryDistrict = _db.LookupDistrict.Where(p => p.ProvinceId == objFromDb.TempraryProvince).Select(i => new SelectListItem
                {
                    Text = i.DistrictNameDari,
                    Value = i.Id.ToString()

                });

                ViewBag.TempraryDistrict = TempraryDistrict;


                IEnumerable<SelectListItem> EducationType = _db.LookupEducationType.Select(i => new SelectListItem
                {
                    Text = i.EducationType,
                    Value = i.Id.ToString()
                });

                ViewBag.EducationType = EducationType;
                IEnumerable<SelectListItem> JobType = _db.LookupJobType.Select(i => new SelectListItem
                {
                    Text = i.JobTypeDari,
                    Value = i.Id.ToString()

                });

                ViewBag.JobType = JobType;
                IEnumerable<SelectListItem> StatusType = _db.LookupStatusType.Select(i => new SelectListItem
                {
                    Text = i.StatusTypeDari,
                    Value = i.Id.ToString()

                });
                ViewBag.StatusType = StatusType;
                IEnumerable<SelectListItem> StatusYear = _db.LookupStatusYearType.Select(i => new SelectListItem
                {
                    Text = i.StatusYearType,
                    Value = i.Id.ToString()

                });

                ViewBag.StatusYear = StatusYear;

                IEnumerable<SelectListItem> Schoolcode = _db.LookupSchool.Where(s => s.ID == objFromDb.SchoolId).Select(i => new SelectListItem
                {
                    Text = i.SchoolCode.ToString(),
                    Value = i.SchoolCode.ToString()

                });

                ViewBag.Schoolcode = Schoolcode;
              // Dashboard(ReportVM vm);
                return View(tbl);
            }
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
