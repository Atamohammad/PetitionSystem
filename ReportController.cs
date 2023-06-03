using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TeacherLoanBoxProject.Models;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TBMIS.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TBMIS.Models.ViewModel;

namespace TeacherLoanBoxProject.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {

        private readonly ApplicationDbContext _db;
        //private readonly IGeneratePdf _IGenaritePDf;
        private readonly ProvinceRoleController puser;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _db = db;
            puser = new ProvinceRoleController(_db, _userManager);
//               _IGenaritePDf = generatePdf;
        }

        public void Data() {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;
            IEnumerable<SelectListItem> Month = _db.LookupMonth.Select(i => new SelectListItem
            {
                Text = i.Months,
                Value = i.Id.ToString()

            });
            ViewBag.Months = Month;
            IEnumerable<SelectListItem> amount = _db.LookupLaonAmount.Select(i => new SelectListItem
            {
                Text = i.LoanAmount.ToString(),
                Value = i.Id.ToString()

            });
            ViewBag.amounts = amount;
            IEnumerable<SelectListItem> salimali = (new List<SelectListItem>
                                            {
                                                new SelectListItem{Text="1384", Value="1384"},
                                                new SelectListItem{Text="1385", Value="1385"},
                                                new SelectListItem{Text="1386", Value="1386"},
                                                new SelectListItem{Text="1387", Value="1387"},
                                                new SelectListItem{Text="1388", Value="1388"},
                                                new SelectListItem{Text="1389", Value="1389"},
                                                new SelectListItem{Text="1390", Value="1390"},
                                                new SelectListItem{Text="1391", Value="1391"},
                                                new SelectListItem{Text="1392", Value="1392"},
                                                new SelectListItem{Text="1393", Value="1393"},
                                                new SelectListItem{Text="1394", Value="1394"},
                                                new SelectListItem{Text="1395", Value="1395"},
                                                new SelectListItem{Text="1396", Value="1396"},
                                                new SelectListItem{Text="1397", Value="1397"},
                                                new SelectListItem{Text="1398", Value="1398"},
                                                new SelectListItem{Text="1399", Value="1399"},
                                                new SelectListItem{Text="1400", Value="1400"},
                                                new SelectListItem{Text="1401", Value="1401"},
                                                new SelectListItem{Text="1402", Value="1402"},
                                                new SelectListItem{Text="1403", Value="1403"},
                                                new SelectListItem{Text="1404", Value="1404"},
                                                new SelectListItem{Text="1405", Value="1405"},

                                            });
            ViewBag.Salim = salimali;
        }



       
        public IActionResult HtmlToPdf() {

            Report1VM vn = new Report1VM()
            {

                LoanRecipientIndividualCard =new   LoanRecipientIndividualCard(),
                TblLoanRecipient = new  TblLoanRecipient()
            };



            return View(vn);
        }
        public JsonResult GetLoanDataList(int id, string type)
        {
            var list = _db.TblLoanRecipient
                               .Include(x => x.Province)
                               .Include(x => x.School)
                               .Select(x => new LoanRecipientCard
                               {
                                   Id = x.Id,
                                   NumberS = x.Numbersbt,
                                   // SaliMali = x.SaliMali,
                                   Name = x.Name,
                                   FatherName = x.FatherName,
                                   ProvinceName = x.Province.ProvinceNameDari,
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

            if (type == "p") list = list.Where(x => x.ProvinceId == id);
            else if (type == "d") list = list.Where(x => x.DistrictId == id);
            else if (type == "s") list = list.Where(x => x.SchoolId == id);




            return Json(list);
        }
        public JsonResult GetLoanDataListFemale(int id, string type)
        {
            var list = _db.TblLoanRecipient
                               .Include(x => x.Province)
                               .Include(x => x.School).Where(x => x.Gender == "Female")
                               .Select(x => new LoanRecipientCard
                               {
                                   Id = x.Id,
                                   NumberS = x.Numbersbt,
                                   // SaliMali = x.SaliMali,
                                   Name = x.Name,
                                   FatherName = x.FatherName,
                                   ProvinceName = x.Province.ProvinceNameDari,
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

            if (type == "p") list = list.Where(x => x.ProvinceId == id);
            else if (type == "d") list = list.Where(x => x.DistrictId == id);
            else if (type == "s") list = list.Where(x => x.SchoolId == id);




            return Json(list);
        }
        public JsonResult GetLoanDataListMale(int id, string type)
        {

            // var loanid = _db.TblLoanRecipient.FirstOrDefault().Id;
            var list = _db.TblLoanRecipient
                               .Include(x => x.Province)
                               .Include(x => x.School).Where(x => x.Gender == "Male")
                               .Select(x => new LoanRecipientCard
                               {
                                   Id = x.Id,
                                   NumberS = x.Numbersbt,
                                   //salimali?= x.inversCard.FirstOrDefault(x=>x.LoanReasonId==loanid).SaliMali,
                                   Name = x.Name,
                                   FatherName = x.FatherName,
                                   ProvinceName = x.Province.ProvinceNameDari,
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

            if (type == "p") list = list.Where(x => x.ProvinceId == id);
            else if (type == "d") list = list.Where(x => x.DistrictId == id);
            else if (type == "s") list = list.Where(x => x.SchoolId == id);




            return Json(list);
        }
        public JsonResult GetDataList(int id, string type)
        {
            var list = _db.TblLoanRecipient
                               .Include(x => x.Province)
                               .Include(x => x.School)
                               .OrderByDescending(x => x.Id)
                               .Select(x => new LoanRecipientCard
                               {
                                   Id = x.Id,
                                   NumberS = x.Numbersbt,
                                   // SaliMali = x.SaliMali,
                                   Name = x.Name,
                                   FatherName = x.FatherName,
                                   ProvinceName = x.Province.ProvinceNameDari,
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

            if (type == "p") list = list.Where(x => x.ProvinceId == id);
            else if (type == "d") list = list.Where(x => x.DistrictId == id);
            else if (type == "s") list = list.Where(x => x.SchoolId == id);




            return Json(list);
        }

        public JsonResult GetDistricts(int id)
        {
            var districts = _db.LookupDistrict.Where(x => x.ProvinceId == id).
                Select(x => new SelectListItem() { Text = x.DistrictNameDari, Value = x.Id.ToString() });

            return Json(districts);
        }
        public JsonResult GetMonths(int id)
        {
            var Months = _db.LookupMonth.
                Select(x => new SelectListItem() { Text = x.Months, Value = x.Id.ToString() });

            return Json(Months);
        }
        public JsonResult GetSaliMali(int id)
        {
            
                   IEnumerable < SelectListItem > salimali = (new List<SelectListItem>
                                            {
                                                new SelectListItem{Text="1384", Value="1384"},
                                                new SelectListItem{Text="1385", Value="1385"},
                                                new SelectListItem{Text="1386", Value="1386"},
                                                new SelectListItem{Text="1387", Value="1387"},
                                                new SelectListItem{Text="1388", Value="1388"},
                                                new SelectListItem{Text="1389", Value="1389"},
                                                new SelectListItem{Text="1390", Value="1390"},
                                                new SelectListItem{Text="1391", Value="1391"},
                                                new SelectListItem{Text="1392", Value="1392"},
                                                new SelectListItem{Text="1393", Value="1393"},
                                                new SelectListItem{Text="1394", Value="1394"},
                                                new SelectListItem{Text="1395", Value="1395"},
                                                new SelectListItem{Text="1396", Value="1396"},
                                                new SelectListItem{Text="1397", Value="1397"},
                                                new SelectListItem{Text="1398", Value="1398"},
                                                new SelectListItem{Text="1399", Value="1399"},
                                                new SelectListItem{Text="1400", Value="1400"},
                                                new SelectListItem{Text="1401", Value="1401"},
                                                new SelectListItem{Text="1402", Value="1402"},
                                                new SelectListItem{Text="1403", Value="1403"},

                                            });


            return Json(salimali);
        }
        public JsonResult GetSchools(int id)
        {

            var schools = _db.LookupSchool.Where(x => x.DistrictId == id).
                Select(x => new SelectListItem() { Text = x.SchoolNameDari, Value = x.ID.ToString() });


            return Json(schools);
        }
        public JsonResult GetLoanAmount(int id)
        {

            var Loanamount = _db.LookupLaonAmount.Where(x => x.Id == id).
                Select(x => new SelectListItem() { Text = x.LoanAmount.ToString(), Value = x.Id.ToString() });


            return Json(Loanamount);
        }


        public IActionResult TotalForgiven()
        {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            return View();
        }
        [HttpPost]
        public IActionResult TotalForgiven(int pid, int did, int sid)
        {
            IEnumerable<TblLoanRecipient> obj = null;
            //["SearchName"] = name;
            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pid > 0 && did == 0 && sid == 0) {
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x => x.inversCard)
                .Where(x => x.ProvinceId == pid && x.StatusType == 5).Take(200);
            }
           else if (pid > 0 && did > 0 && sid == 0)
            {
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x => x.inversCard)
                .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 5).Take(200);
            }
            else if (pid > 0 && did > 0 && sid > 0) {
                obj = _db.TblLoanRecipient
              .Include(x => x.Province)
              .Include(x => x.District)
              .Include(x => x.School)
              .Include(x => x.inversCard)
              .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 5).Take(200);
            }
            else {
                obj = null;

            }
            if (!User.IsInRole("Admin"))
            {

                if (pid > 0 && did == 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.StatusType == 5).Take(200);
                      var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);

                }
                else if (pid > 0 && did > 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 5).Take(200);
                     var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);

                }
                else if (pid > 0 && did > 0 && sid > 0)
                {
                    obj = _db.TblLoanRecipient
                  .Include(x => x.Province)
                  .Include(x => x.District)
                  .Include(x => x.School)
                  .Include(x => x.inversCard)
                  .Where(x=>puser.IsInProvince(userId, x.ProvinceId)&&x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 5).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else
                {
                    obj = null;

                }


                //list = list.Where(x => puser.IsInProvince(userId, x.ProvinceId));

            }

                if (obj == null)
            {
                TotalForgiven();
                return View();

            }
            else
            {
                TotalForgiven();

                return View(obj/*query.AsNoTracking().ToLitst()*/);

            }
        }

        public IActionResult TotalLoanComplated()
        {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;
            IEnumerable<SelectListItem> StatusType = _db.LookupLaonAmount.Select(i => new SelectListItem
            {
                Text = i.LoanAmount.ToString(),
                Value = i.Id.ToString()

            });
            ViewBag.StatusType = StatusType;

            return View();
        }
        [HttpPost]
        public IActionResult TotalLoanComplated(int pid, int did, int sid)
        {
            IEnumerable<TblLoanRecipient> obj = null;
            //["SearchName"] = name;
            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;

            //var obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x => x.District)
            //.Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 1).Take(100);

            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var typeRoles = _db.MeetTypes.Where(x => x.UserId == userId);
            ////objList = objList.Where(x => typeRoles.Where(y => y.MeetWithId == x.MeetingWithId).Count() == 1).ToList();



            if (pid > 0 && did > 0 && sid == 0)
                   {
                       obj = _db.TblLoanRecipient
                       .Include(x => x.Province)
                       .Include(x => x.District)
                       .Include(x => x.School)
                       .Include(x => x.inversCard)
                       .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 1);
              
                   }
                   else if (pid > 0 && did > 0 && sid > 0)
                   {
                       obj = _db.TblLoanRecipient
                     .Include(x => x.Province)
                     .Include(x => x.District)
                     .Include(x => x.School)
                     .Include(x => x.inversCard)
                     .Where(x => x.ProvinceId == pid && x.DistrictId == did&&x.SchoolId==sid && x.StatusType == 1);
               
              }
            else {
                  
                       obj = null;
                  
             }
            if (!User.IsInRole("Admin"))
            {


                if (pid > 0 && did > 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 1);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1).ToList();
                }
                else if (pid > 0 && did > 0 && sid > 0)
                {
                    obj = _db.TblLoanRecipient
                  .Include(x => x.Province)
                  .Include(x => x.District)
                  .Include(x => x.School)
                  .Include(x => x.inversCard)
                  .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.StatusType == 1);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else
                {

                    obj = null;

                }
            }

                if (obj == null)
                   {
                              TotalLoanComplated();
                              return View();
                         
                   }
                else 
                   {
                       TotalLoanComplated();
                   
                       return View(obj/*query.AsNoTracking().ToList()*/);
                   
                   }

        }

        public IActionResult TotalLoanNotComplated(int id)
        {

            ViewBag.totalcredit1 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 1).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum();
            ViewBag.totalcredit2 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 2).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum();
            ViewBag.totalcredit4 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 3).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum();
            ViewBag.totalcredit5 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 4).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum();
            ViewBag.totalcredit7 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 5).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum();
            ViewBag.Totalremain1 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 1).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum() - 10000;
            ViewBag.Totalremain2 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 2).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum() - 12000;
            ViewBag.Totalremain4 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 3).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum() - 18000;
            ViewBag.Totalremain5 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 4).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum() - 14000;
            ViewBag.Totalremain7 = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id).Where(c => c.LoanTypeId == 2).Where(x => x.MonthlyInstallmentId == 5).Select(x => Convert.ToInt32(x.MonthlyInstallment.MonthlyAmount)).Sum() - 0;


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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            return View();
        }
        [HttpPost]
        public IActionResult TotalLoanNotComplated(int pid, int did, int sid)
        {
           
            IEnumerable<TblLoanRecipient> obj = null;
            //["SearchName"] = name;
            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;
            //var tblID = _db.TblLoanRecipient.FirstOrDefault(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid).Id;
            //var tblID1 = from x in _db.TblLoanRecipient.Include(x => x.Province).Include(x => x.District).Include(x => x.School).Include(x => x.inversCard) select x;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pid > 0 && did > 0 && sid == 0)
            {
                ViewBag.TotalD = _db.LoanRecipientIndividualCard
               .Where(x => x.LoanRecipient.ProvinceId == pid &&x.LoanRecipient.DistrictId == did&&x.LoanRecipient.SchoolId==sid&& x.LoanTypeId == 1)
              .Select(x=>x.LoanAmount.LoanAmount).Sum();
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x=>x.inversCard)
               // .Include(x => x.LoanAmount)
                .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 2).Take(200);
            }
            else if (pid > 0 && did > 0 && sid > 0)
            {
              //  ViewBag.TotalD = _db.Tbl
              // .Where(x => x.ProvinceId == pid &&x.DistrictId==did&&x.SchoolId==sid&& x.StatusType == 1)
              //.Select(x=>x.LoanAmount.LoanAmount).Sum();

                obj = _db.TblLoanRecipient
              .Include(x => x.Province)
              .Include(x => x.District)
              .Include(x => x.School)
              .Include(x => x.inversCard)
             // .Include(x=>x.LoanAmount)
              .Where(x => x.ProvinceId == pid && x.DistrictId == did&&x.SchoolId==sid && x.StatusType == 2)
              .Take(200);
            }
            else
            {
                obj = null;
            }
            if (!User.IsInRole("Admin"))
            {


                if (pid > 0 && did > 0 && sid == 0)
                {
                    ViewBag.TotalD = _db.LoanRecipientIndividualCard
                   .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.LoanRecipient.SchoolId == sid && x.LoanTypeId == 1)
                  .Select(x => x.LoanAmount.LoanAmount).Sum();
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    //.Include(x => x.LoanAmount)
                    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 2).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else if (pid > 0 && did > 0 && sid > 0)
                {
                    ViewBag.TotalD = _db.LoanRecipientIndividualCard
                   .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.LoanRecipient.SchoolId == sid && x.LoanTypeId == 1)
                  .Select(x => x.LoanAmount.LoanAmount).Sum();

                    obj = _db.TblLoanRecipient
                  .Include(x => x.Province)
                  .Include(x => x.District)
                  .Include(x => x.School)
                  .Include(x => x.inversCard)
                  //.Include(x => x.LoanAmount)
                  .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.StatusType == 2).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else
                {
                    obj = null;
                }





            }


            if (obj == null)
            {
                TotalLoanNotComplated(1);
                ViewBag.Error = "دیتا نشته یا ولایتی کاربر اجازه نه لری.";
                return View();

            }
            else
            {
                TotalLoanNotComplated(1);

                return View(obj);

            }

        }

        public IActionResult TotalTerminated()
        {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            return View();
        }
        [HttpPost]
        public IActionResult TotalTerminated(int pid, int did, int sid)
        {
            IEnumerable<TblLoanRecipient> obj = null;
            //["SearchName"] = name;
            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pid > 0 && did == 0 && sid == 0)
            {
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x => x.inversCard)
                .Where(x => x.ProvinceId == pid  && x.StatusType == 3).Take(200);

            }
            else if (pid > 0 && did > 0 && sid == 0)
            {
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x => x.inversCard)
                .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 3).Take(200);
            }
            else if (pid > 0 && did > 0 && sid > 0)
            {
                obj = _db.TblLoanRecipient
              .Include(x => x.Province)
              .Include(x => x.District)
              .Include(x => x.School)
              .Include(x => x.inversCard)
              .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 3).Take(200);



            }
            else {
                obj = null;
            }
            if (!User.IsInRole("Admin")) {

                if (pid > 0 && did == 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.StatusType == 3).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);

                }
                else if (pid > 0 && did > 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 3).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else if (pid > 0 && did > 0 && sid > 0)
                {
                    obj = _db.TblLoanRecipient
                  .Include(x => x.Province)
                  .Include(x => x.District)
                  .Include(x => x.School)
                  .Include(x => x.inversCard)
                  .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.StatusType == 3).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);


                }
                else
                {
                    obj = null;
                }
             

            }

            if (obj == null)
            {
                TotalTerminated();
                return View();

            }
            else
            {
                TotalTerminated();


                return View(obj);

            }
        }
        public IActionResult ReportView(int? id)
        {
            // it is send the all data to this table
            ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 1)
                .Select(x => x.LoanAmount.LoanAmount).Sum();
            var TotalCreditOfCredit = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 2)
              .Select(x => x.MonthlyInstallment.MonthlyAmount)
               .Sum(x => Convert.ToInt32(x));
            var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 2)
              .Select(x => x.OtherAmounts)
               .Sum(x => Convert.ToInt32(x));

            var totalcredit = TotalCreditOfCredit + TotalCreditOtherAmount;
            ViewBag.TotalCredit = totalcredit;
            //it is to minas of total amount minas total credit

            var TotalCreditamount = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 1)
               .Select(x => x.LoanAmount.LoanAmount).Sum();

            ViewBag.TotalAmountOfRemain = totalcredit - TotalCreditamount;
            ViewBag.TotalLoanAmount = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id &&  x.LoanTypeId == 1).Count();

            

            // ViewBag.LoanAmount = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.Id == id).LoanAmount;
            ViewBag.NumberMo = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.MaktobNo;
            ViewBag.NumberDate = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.MaktobDate;
            ViewBag.SaliMali = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.SaliMali;
            ViewBag.SaliJari = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.CurrentYear;
            ViewBag.LoanAmountt = _db.LoanRecipientIndividualCard.Include(x => x.LoanAmount).FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.LoanAmount;

            ViewBag.Estihqaqno = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.EstihqaqNo;
            ViewBag.Estihqaqdate = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.EstihqaqDate;
            ViewBag.NumberMshanzda = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.NumberOfM16;
            ViewBag.NumberMshanzdaDate = _db.LoanRecipientIndividualCard.FirstOrDefault(x => x.LoanRecipientId == id && x.SId == 2)?.DateOfM16;
            //car table above
            ViewBag.Name = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).Name;
            ViewBag.image = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).ImagePath;
            ViewBag.FName = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).FatherName;
            ViewBag.GName = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).GrandFatherName;
            ViewBag.Gender = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).Gender;
            ViewBag.Register = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).RegsterNumber;
            ViewBag.Numbersb = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).Numbersbt;
            ViewBag.DateSb = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).DateSbt;
            ViewBag.IdNumSalary = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).IdnumSalary;
            ViewBag.Mlahezat = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).Comments;
            ViewBag.AccountNo = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).AccountNumber;
            ViewBag.Sawanih = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).SawanihNo;
            ViewBag.MobileNo = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).PhoneNo;
            ViewBag.StatusType = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).StatusType;
            ViewBag.SchoolType = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).SchoolType;
            ViewBag.LevelEducation = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).EducationType;
            ViewBag.JobType = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).JobType;
            ViewBag.SchoolCode = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).SchoolCode;
            ViewBag.surName = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).SureName;
            ViewBag.Tazkira = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).TazkeraNo;
            ViewBag.VolumeNo = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).VolumeNo;
            ViewBag.PageNo = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).PageNo;
            ViewBag.Province = _db.TblLoanRecipient.Include(x => x.Province).FirstOrDefault(x => x.Id == id).Province.ProvinceNameDari;
            ViewBag.Provincep = _db.TblLoanRecipient.Include(x => x.Province).FirstOrDefault(x => x.Id == id).PermanantProvince;
            ViewBag.Districtp = _db.TblLoanRecipient.Include(x => x.District).FirstOrDefault(x => x.Id == id).District.DistrictNameDari;
            ViewBag.Provincet = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).TempraryProvince;
            ViewBag.Districtt = _db.TblLoanRecipient.Include(x => x.District).FirstOrDefault(x => x.Id == id).District.DistrictNameDari;
            ViewBag.District = _db.TblLoanRecipient.Include(x => x.District).FirstOrDefault(x => x.Id == id).District.DistrictNameDari;
            ViewBag.School = _db.TblLoanRecipient.Include(x => x.School).FirstOrDefault(x => x.Id == id).School.SchoolNameDari;
            ViewBag.LoanRecipientID = _db.TblLoanRecipient.FirstOrDefault(x => x.Id == id).Id;

            var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                .Include(x => x.LoanRecipient.School)
                .Include(x => x.LoanAmount)
                .Include(x => x.AcuallDate)
                .Include(x => x.LoanType)
                .Include(x => x.MonthlyInstallment)
                .Include(x => x.LoanInstallment)
                .Where(x => x.LoanRecipientId == id)
                .Take(200);

            return View(list);
        }
       
        public IActionResult ReportView1()
        {
            var date = DateTime.Now;

            var list = _db.TblLoanRecipient
               .Include(x => x.Province)
               .Include(x => x.School)
               .Include(x => x.District)
               .Where(l => l.StatusType == 3 && l.TrackingDate == date).Take(100);
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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            //IEnumerable<TBMIS.Mod> objList = _db.ApplicationType;
            return View(list);
        }
        public IActionResult TotalMale()
        {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            return View();
        }
        [HttpPost]
        public IActionResult TotalMale(int pid, int did, int sid)
        {
            IEnumerable<TblLoanRecipient> obj = null;
            //["SearchName"] = name;
            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pid > 0 && did > 0 && sid == 0)
            {

                obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x => x.District)
               .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.Gender == "Male").Take(200)/*.AsEnumerable<TblLoanRecipient>()*/;
            }
            else if (pid > 0 && did > 0 && sid > 0)
            {

                obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x => x.District)
                   .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.Gender == "Male").Take(200)/*.AsEnumerable<TblLoanRecipient>()*/;
            }
            else 
            {
                obj = null;
            }
            if (!User.IsInRole("Admin")) 
            {

                if (pid > 0 && did > 0 && sid == 0)
                {

                    obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x => x.District)
                   .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.Gender == "Male").Take(200)/*.AsEnumerable<TblLoanRecipient>()*/;
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else if (pid > 0 && did > 0 && sid > 0)
                {

                    obj = _db.TblLoanRecipient.Include(z => z.inversCard).Include(x => x.Province).Include(x => x.School).Include(x => x.District)
                       .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.Gender == "Male").Take(200)/*.AsEnumerable<TblLoanRecipient>()*/;
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else
                {
                    obj = null;
                }

              
            }
            TotalMale();

            return View(obj);


        }

        public IActionResult TotalFemale()
        {

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
            IEnumerable<SelectListItem> School = _db.LookupSchool.Select(i => new SelectListItem
            {
                Text = i.SchoolNameDari,
                Value = i.ID.ToString()

            });
            ViewBag.School = School;

            return View();
        }
        [HttpPost]
        public IActionResult TotalFemale(int pid, int did, int sid)
        {

            IEnumerable<TblLoanRecipient> obj = null;

            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            ViewData["sSearch"] = sid;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pid > 0 && did > 0 && sid == 0)
            {
                obj = _db.TblLoanRecipient
                .Include(x => x.Province)
                .Include(x => x.District)
                .Include(x => x.School)
                .Include(x => x.inversCard)
                .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.Gender == "Female").Take(200);

            }
            else if (pid > 0 && did > 0 && sid > 0)
            {
                obj = _db.TblLoanRecipient
              .Include(x => x.Province)
              .Include(x => x.District)
              .Include(x => x.School)
              .Include(x => x.inversCard)
              .Where(x => x.ProvinceId == pid && x.DistrictId == did &&x.SchoolId==sid && x.Gender == "Female").Take(200);



            }
            else
            {
                obj = null;
            }
            if (User.IsInRole("Admin")) {
                if (pid > 0 && did > 0 && sid == 0)
                {
                    obj = _db.TblLoanRecipient
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.School)
                    .Include(x => x.inversCard)
                    .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.Gender == "Female").Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);

                }
                else if (pid > 0 && did > 0 && sid > 0)
                {
                    obj = _db.TblLoanRecipient
                  .Include(x => x.Province)
                  .Include(x => x.District)
                  .Include(x => x.School)
                  .Include(x => x.inversCard)
                  .Where(x => x.ProvinceId == pid && x.DistrictId == did && x.SchoolId == sid && x.Gender == "Female").Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj = obj.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);



                }
                else
                {
                    obj = null;
                }


            }

            if (obj == null)
            {

                TotalFemale();


                return View();
            }
            else
            {
                TotalFemale();

                return View(obj/*query.AsNoTracking().ToList()*/);

            }


        }

        public IActionResult TeacherReport1()
        {

            var list = _db.LoanRecipientIndividualCard
                             .Include(x => x.LoanReason)
                             .Include(x => x.MonthlyInstallment)
                             .Include(x => x.LoanInstallment)
                             .Include(x => x.LoanRecipient)
                             .Include(x => x.LoanAmount)
                             .Include(x => x.LoanType);



            return View(list);


        }

        //    public IActionResult Excel()
        //{
        //    byte[] fileContents;
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("sheet1");

        //        worksheet.Cells[1, 1].Value = "studentName";
        //        worksheet.Cells[1, 2].Value = "ahmad";
        //        fileContents = package.GetAsByteArray();

        //    }


        //    if (fileContents == null || fileContents.Length == 0)
        //    {

        //        return NotFound();
        //    }
        //    return File(
        //     fileContents: fileContents,
        //        contentType: "application/vnd.opensmlformates-offecedocument.spreadsheetml.sheet",
        //       fileDownloadName: "StudentName.xlsx"
        //     );


        ////}
        ///
        public IActionResult Totaldebit()
        {


            //var date1 = DateTime.Today;
            //var date2 = DateTime.Now.AddDays(-30);
            //ViewBag.TotalD = _db.LoanRecipientIndividualCard.Where(x =>x.TrackingDate > date2 && x.TrackingDate < date1 && x.LoanTypeId == 1 ).Count();
            //ViewBag.Totalamount = _db.LoanRecipientIndividualCard.Where(x => 
            //  x.TrackingDate > date2 &&x.TrackingDate<date1&& x.LoanTypeId == 1).Select(x => x.LoanAmount.LoanAmount).Sum();
         
            //var obj = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
            //   .Include(x => x.LoanRecipient.Province)
            //   .Include(x => x.LoanRecipient.District)
            //   .Include(x => x.LoanRecipient.School)
            //   .Include(x => x.LoanAmount).Include(x=>x.AcuallDate)
            //   .Where(x =>x.TrackingDate>date2 && x.TrackingDate < date1 && x.LoanTypeId == 1).Take(200);
            Data();
            return View();

        }
        [HttpPost]
        public IActionResult Totaldebit(int pid,int salId, int monthId,int aid)
        {

            // it is view bag to total
            ViewBag.TotalD = _db.LoanRecipientIndividualCard
                .Where(x =>x.LoanRecipient.ProvinceId==pid&&x.SaliMali==salId&&x.ActualDate==monthId&&x.LoanAmountId==aid&& x.LoanTypeId == 1)
                .Count();
            ViewBag.Totalamount = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanAmountId == aid && x.LoanTypeId == 1)
                .Select(x => x.LoanAmount.LoanAmount)
                .Sum();
            //loanamount 
            ViewBag.LoanAmount = _db.LoanRecipientIndividualCard
                .Include(x => x.LoanRecipient).Include(x => x.LoanAmount)
                .FirstOrDefault(x => x.LoanRecipient.ProvinceId == pid&&x.SaliMali==salId&&x.ActualDate==monthId
               )?
                .LoanAmount.LoanAmount;

            // it is the search code
            IEnumerable<LoanRecipientIndividualCard> obj = null;
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["Search"] = pid;
            //ViewData["dSearch"] = did;
            //ViewData["sSearch"] = sid;
            ViewData["aSearch"] = aid;
            ViewData["salSearch"] = salId;
            ViewData["monthSearch"] = monthId;

            if (pid > 0  && salId > 0 && monthId > 0 && aid>0)
            {
                obj = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                .Include(x => x.LoanRecipient.Province)
                .Include(x => x.LoanRecipient.District)
                .Include(x => x.LoanRecipient.School)
                .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                .Where(x => x.LoanRecipient.ProvinceId == pid&&x.SaliMali==salId&&x.ActualDate==monthId&&x.LoanAmountId==aid && x.LoanTypeId == 1).Take(200);
            }
          
            else
            {
                obj = null;
            }
            if (!User.IsInRole("Admin"))
            { 
                // it is view bag to total
            ViewBag.TotalD = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanAmountId == aid && x.LoanTypeId == 1)
                .Count();
                ViewBag.Totalamount = _db.LoanRecipientIndividualCard
                    .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanAmountId == aid && x.LoanTypeId == 1)
                    .Select(x => x.LoanAmount.LoanAmount)
                    .Sum();
                //loanamount 
                ViewBag.LoanAmount = _db.LoanRecipientIndividualCard
                    .Include(x => x.LoanRecipient).Include(x => x.LoanAmount)
                    .FirstOrDefault(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId
                   )?
                    .LoanAmount.LoanAmount;

                // it is the search code
                IEnumerable<LoanRecipientIndividualCard> obj1 = null;

                ViewData["Search"] = pid;
                //ViewData["dSearch"] = did;
                //ViewData["sSearch"] = sid;
                ViewData["aSearch"] = aid;
                ViewData["salSearch"] = salId;
                ViewData["monthSearch"] = monthId;

                if (pid > 0 && salId > 0 && monthId > 0 && aid > 0)
                {
                    obj1 = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                    .Include(x => x.LoanRecipient.Province)
                    .Include(x => x.LoanRecipient.District)
                    .Include(x => x.LoanRecipient.School)
                    .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                    .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanAmountId == aid && x.LoanTypeId == 1).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj1 = obj1.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }

                else
                {
                    obj1 = null;
                }
             



            }

            if (obj == null)
            {

                Data();


                return View();
            }
            else
            {
                Data();

                return View(obj/*query.AsNoTracking().ToList()*/);

            }




        }
        public IActionResult TotalCredit()
        {
            //var date1 = DateTime.Today;
            //var date2 = DateTime.Now.AddDays(-30);

            ////total of amounts debits and credit and otheramount
            //ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
            //  .Where(x =>x.TrackingDate > date2&&x.TrackingDate<date1&& x.LoanTypeId == 1)
            //  .Select(x => x.LoanAmount.LoanAmount)
            //  .Sum();
            //var TotalCreditOfMonthlyInstallment = _db.LoanRecipientIndividualCard
            //  .Where(x =>x.TrackingDate > date2&&x.TrackingDate<date1&& x.LoanTypeId == 2)
            //   .Select(x => x.MonthlyInstallment.MonthlyAmount)
            //   .Sum(x => Convert.ToInt32(x));
            //var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
            //    .Where(x =>x.TrackingDate > date2&&x.TrackingDate<date1&& x.LoanTypeId == 2)
            //    .Select(x => x.OtherAmounts)
            //    .Sum(x => Convert.ToInt32(x));
            //var TotalCreditandOtherAmount = TotalCreditOfMonthlyInstallment + TotalCreditOtherAmount;

            //ViewBag.TotalCreditr = TotalCreditandOtherAmount;
            //ViewBag.TotalMonthlyInstallment = TotalCreditOfMonthlyInstallment;


            //var TotalDebitamount = _db.LoanRecipientIndividualCard
            //    .Where(x =>x.TrackingDate == date2&&x.TrackingDate<date1&& x.LoanTypeId == 1)
            //    .Select(x => x.LoanAmount.LoanAmount).Sum();

            //ViewBag.TotalAmountOfRemain = TotalCreditandOtherAmount - TotalDebitamount;

            //var obj = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
            //   .Include(x => x.LoanRecipient.Province)
            //   .Include(x => x.LoanRecipient.District)
            //   .Include(x => x.LoanRecipient.School)
            //   .Include(x => x.LoanAmount).Include(x => x.MonthlyInstallment).Include(x => x.AcuallDate)
            //   .Where(x =>x.TrackingDate>date2&&x.TrackingDate<date1&& x.LoanTypeId == 2).Take(200);
            Data();
            return View();



        }
       [HttpPost]
       public IActionResult TotalCredit( int pid,int did,int salId,int monthId)
        {
           var date= DateTime.Now.AddDays(30);
            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // it is view bag to total
            if (did != 0)
            {


                // it is to count

                ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                 .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                .Select(x => x.LoanAmount.LoanAmount).Sum();

                    var TotalCreditOfMonthlyInstallment = _db.LoanRecipientIndividualCard
                   .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                   .Select(x => x.MonthlyInstallment.MonthlyAmount)
                   .Sum(x => Convert.ToInt32(x));

                   var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                        .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                    .Select(x => x.OtherAmounts)
                     .Sum(x => Convert.ToInt32(x));

                 var TotalCreditandOtherAmount = TotalCreditOfMonthlyInstallment + TotalCreditOtherAmount;
                //it is the sum total credit and total of other amount
               // ViewBag.TotalCreditr = TotalCreditandOtherAmount;


                // it is the total of credit
                ViewBag.TotalMonthlyInstallment = TotalCreditandOtherAmount;

                 //it is to minas of total amount minas total credit

                  var TotalCreditamount = _db.LoanRecipientIndividualCard
                   .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                    .Select(x => x.LoanAmount.LoanAmount).Sum();

                  ViewBag.TotalAmountOfRemain = TotalCreditandOtherAmount - TotalCreditamount;


            }
            else
            {
               

                // it is the count code
                ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
             .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
            .Select(x => x.LoanAmount.LoanAmount).Sum();

                var TotalCreditOfMonthlyInstallment = _db.LoanRecipientIndividualCard
               .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
               .Select(x => x.MonthlyInstallment.MonthlyAmount)
               .Sum(x => Convert.ToInt32(x));

                var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                     .Where(x => x.LoanRecipient.ProvinceId == pid & x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                 .Select(x => x.OtherAmounts)
                  .Sum(x => Convert.ToInt32(x));

                //   var totalcredit = TotalCreditOfCredit + TotalCreditOtherAmount;
                var TotalCreditandOtherAmount = TotalCreditOfMonthlyInstallment + TotalCreditOtherAmount;
                ViewBag.TotalMonthlyInstallment = TotalCreditOfMonthlyInstallment;


                //ViewBag.TotalCreditr = TotalCreditandOtherAmount;

                //   //it is to minas of total amount minas total credit

                var TotalCreditamount = _db.LoanRecipientIndividualCard
                 .Where(x => x.LoanRecipient.ProvinceId == pid &&x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                  .Select(x => x.LoanAmount.LoanAmount).Sum();

                ViewBag.TotalAmountOfRemain = TotalCreditandOtherAmount - TotalCreditamount;


            }

            ViewBag.LoanAmount = _db.LoanRecipientIndividualCard
                .Include(x=>x.LoanRecipient).Include(x=>x.LoanAmount)
                .FirstOrDefault(x=>x.LoanRecipient.ProvinceId==pid&&x.SaliMali==salId&&x.ActualDate==monthId)?
                .LoanAmount.LoanAmount;

            // it is the search code
            IEnumerable<LoanRecipientIndividualCard> obj = null;

            ViewData["Search"] = pid;
            ViewData["dSearch"] = did;
            //ViewData["sSearch"] = sid;
            ViewData["salSearch"] = salId;
            ViewData["monthSearch"] = monthId;

            if (pid > 0 &&  did > 0 && salId > 0 && monthId > 0)
            {
                obj = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                .Include(x => x.LoanRecipient.Province)
                .Include(x => x.LoanRecipient.District)
                .Include(x => x.LoanRecipient.School)
                .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                .Where(x => x.LoanRecipient.ProvinceId == pid&& x.LoanRecipient.DistrictId==did&& x.SaliMali==salId&&x.ActualDate==monthId&& x.LoanTypeId == 2).Take(200);
            }
            else if (pid > 0 && did == 0  && salId > 0 && monthId > 0)
            {
                obj = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                   .Include(x => x.LoanRecipient.Province)
                   .Include(x => x.LoanRecipient.District)
                   .Include(x => x.LoanRecipient.School)
                   .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                   .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2).Take(200);
            }
 
            else
            {
                obj = null;
            }
            if (!User.IsInRole("Admin")) {

                if (did != 0)
                {

                    ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                      .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                      .Select(x => x.LoanAmount.LoanAmount)
                      .Sum();
                    var TotalCreditOfMonthlyInstallment = _db.LoanRecipientIndividualCard
                       .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                      .Select(x => x.MonthlyInstallment.MonthlyAmount)
                       .Sum(x => Convert.ToInt32(x));
                    var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                        .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                      .Select(x => x.OtherAmounts)
                       .Sum(x => Convert.ToInt32(x));
                    var TotalCreditandOtherAmount = TotalCreditOfMonthlyInstallment + TotalCreditOtherAmount;

                    ViewBag.TotalCreditr = TotalCreditandOtherAmount;
                    ViewBag.TotalMonthlyInstallment = TotalCreditOfMonthlyInstallment;


                    var TotalDebitamount = _db.LoanRecipientIndividualCard
                       .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                       .Select(x => x.LoanAmount.LoanAmount).Sum();

                    ViewBag.TotalAmountOfRemain = TotalCreditandOtherAmount - TotalDebitamount;


                }
                else
                {


                    ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                     .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                     .Select(x => x.LoanAmount.LoanAmount)
                     .Sum();
                    var TotalCreditOfMonthlyInstallment = _db.LoanRecipientIndividualCard
                       .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                      .Select(x => x.MonthlyInstallment.MonthlyAmount)
                       .Sum(x => Convert.ToInt32(x));
                    var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                        .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2)
                      .Select(x => x.OtherAmounts)
                       .Sum(x => Convert.ToInt32(x));
                    var TotalCreditandOtherAmount = TotalCreditOfMonthlyInstallment + TotalCreditOtherAmount;

                    ViewBag.TotalCreditr = TotalCreditandOtherAmount;
                    ViewBag.TotalMonthlyInstallment = TotalCreditOfMonthlyInstallment;


                    var TotalDebitamount = _db.LoanRecipientIndividualCard
                       .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 1)
                       .Select(x => x.LoanAmount.LoanAmount).Sum();

                    ViewBag.TotalAmountOfRemain = TotalCreditandOtherAmount - TotalDebitamount;
                }

                ViewBag.LoanAmount = _db.LoanRecipientIndividualCard
                    .Include(x => x.LoanRecipient).Include(x => x.LoanAmount)
                    .FirstOrDefault(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId)?
                    .LoanAmount.LoanAmount;

                // it is the search code
                IEnumerable<LoanRecipientIndividualCard> obj1 = null;

                ViewData["Search"] = pid;
                ViewData["dSearch"] = did;
                //ViewData["sSearch"] = sid;
                ViewData["salSearch"] = salId;
                ViewData["monthSearch"] = monthId;

                if (pid > 0 && did > 0 && salId > 0 && monthId > 0)
                {
                    obj1 = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                    .Include(x => x.LoanRecipient.Province)
                    .Include(x => x.LoanRecipient.District)
                    .Include(x => x.LoanRecipient.School)
                    .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                    .Where(x => x.LoanRecipient.ProvinceId == pid && x.LoanRecipient.DistrictId == did && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj1 = obj1.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }
                else if (pid > 0 && did == 0 && salId > 0 && monthId > 0)
                {
                    obj1 = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                       .Include(x => x.LoanRecipient.Province)
                       .Include(x => x.LoanRecipient.District)
                       .Include(x => x.LoanRecipient.School)
                       .Include(x => x.AcuallDate).Include(x => x.LoanAmount)
                       .Where(x => x.LoanRecipient.ProvinceId == pid && x.SaliMali == salId && x.ActualDate == monthId && x.LoanTypeId == 2).Take(200);
                    var typeRoles = _db.UserProvince.Where(x => x.UserId == userId);
                    obj1 = obj1.Where(x => typeRoles.Where(x => x.ProvinceId == pid).Count() == 1);
                }

                else
                {
                    obj1 = null;
                }
               
            }

            if (obj == null)
            {

                Data();


                return View();
            }
            else
            {
                Data();

                return View(obj/*query.AsNoTracking().ToList()*/);

            }


        

        }

    }
}