using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models;
using TeacherLoanBoxProject.Models.ViewModel;

namespace TeacherLoanBoxProject.Controllers
{
    [Authorize]
    public class LoanRecipientIndividualCardController : Controller
    {
       private readonly ApplicationDbContext _db;
        private readonly  UserManager<IdentityUser> _userManager;
        public LoanRecipientIndividualCardController(ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {

            _db = db;
                _userManager = userManager;

        }
        public JsonResult GetService(int id)

        {
            var GetLoanType = _db.LookupServicePeriodType.Where(x => x.LoanAmountId == id).
                Select(x => new SelectListItem() { Text = x.ServicePeriodType, Value = x.Id.ToString() });
            
            return Json(GetLoanType);
        }  
        public JsonResult LoanInstallmentID(int id)

        {
            var LoanInstallmentID = _db.LookupLoanInstallment.Where(x => x.Id == id).
                Select(x => new SelectListItem() { Text = x.InstallmentsNameDari, Value = x.Id.ToString() });
            
            return Json(LoanInstallmentID);
        }
        public JsonResult GetMontInstallmnetID(int id)

        {
            var GetMont = _db.LookupLoanMonthlyInstallment.Where(x => x.LoanAmountId == id).
                Select(x => new SelectListItem() { Text = x.MonthlyAmount, Value = x.Id.ToString() });




            return Json(GetMont);
        }
        public IActionResult IndexByID(int? id)
        {
            //total debit
            ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.SId == 2 && x.LoanTypeId == 1)
                .Select(x => x.LoanAmount.LoanAmount).Sum();
            //total installemt and otheramount
            var TotalCreditOfCredit = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id&& x.LoanTypeId == 2 && x.SId == 2)
              .Select(x => x.MonthlyInstallment.MonthlyAmount)
               .Sum(x => Convert.ToInt32(x));
            var TotalCreditOtherAmount = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 2 && x.SId == 2)
              .Select(x => x.OtherAmounts)
               .Sum(x => Convert.ToInt32(x));
          
                var totalcredit= TotalCreditOfCredit + TotalCreditOtherAmount;
            ViewBag.TotalCredit = totalcredit;
              //it is to minas of total amount minas total credit

            var TotalCreditamount = _db.LoanRecipientIndividualCard
                .Where(x => x.LoanRecipientId == id && x.LoanTypeId == 1 && x.SId == 2)
               .Select(x => x.LoanAmount.LoanAmount).Sum();

            ViewBag.TotalAmountOfRemain = totalcredit - TotalCreditamount;
            ViewBag.TotalLoanAmount = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id && x.LoanTypeId == 1).Count();


            var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
                .Include(x => x.LoanRecipient.School)
                .Include(x => x.LoanAmount)
                .Include(x => x.LoanType)
                .Include(x => x.AcuallDate)
                .Include(x => x.MonthlyInstallment)
                .Include(x => x.LoanInstallment)
                .Where(x => x.LoanRecipientId == id && x.SId==2).OrderBy(x => x.SaliMali)
                .Take(20);

            return View(list);
        }

       
        public IActionResult ShowAllData(int id)
        {
            ViewBag.TotalDebit = _db.LoanRecipientIndividualCard
                .Where(x=>x.LoanRecipientId==id&&x.LoanTypeId==1)
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
            ViewBag.TotalLoanAmount = _db.LoanRecipientIndividualCard.Where(x => x.LoanRecipientId == id && x.LoanTypeId == 1).Count();


            var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
               .Include(x => x.LoanRecipient.School)
               .Include(x => x.LoanAmount)
               .Include(x => x.LoanType)
               .Include(x => x.AcuallDate)
               .Include(x => x.MonthlyInstallment)
               .Include(x => x.LoanInstallment)
               .Where(x => x.LoanRecipientId == id).OrderBy(x=>x.SaliMali)
               .Take(200);

            return View(list);

        }
        public IActionResult Create(int? id)
        {

            ViewBag.Name = _db.TblLoanRecipient.SingleOrDefault(x => x.Id == id).Name;

            ViewBag.LoanRecipientID = _db.TblLoanRecipient
             .SingleOrDefault(x => x.Id == id).Id;

            //ViewBag.Name = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient)
            // .SingleOrDefault(x => x.Id == id).LoanRecipient.Name;
            //ViewBag.LoanRecipientID = _db.LoanRecipientIndividualCard.Include(x => x.LoanRecipient).SingleOrDefault(x => x.Id == id).LoanRecipient.Id;

            IEnumerable<SelectListItem> Stayus = _db.LookupStatusType.Select(s => new SelectListItem
            {
                Text=s.StatusTypeDari,
                Value=s.Id.ToString()


            });
            ViewBag.StatusType = Stayus;

            IEnumerable<SelectListItem> yearMaliAndJari = _db.LookupStatusYearType.Select(s => new SelectListItem
            {
                Text = s.StatusYearType,
                Value = s.StatusYearType.ToString()


            });
            ViewBag.yearMaliAndJari = yearMaliAndJari;
           
            IEnumerable<SelectListItem> mysalimali = (new List<SelectListItem>
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
            ViewBag.SaliMali = mysalimali;
            IEnumerable<SelectListItem> LoanTypeID = _db.LookupLoanType.Select(i => new SelectListItem
            {
                Text = i.LoanType,
                Value = i.Id.ToString()

            });

            ViewBag.LoanTypeID = LoanTypeID;

            IEnumerable<SelectListItem> ServiceP = _db.LookupServicePeriodType.Select(i => new SelectListItem
            {
                Text = i.ServicePeriodType,
                Value = i.Id.ToString()

            });

            ViewBag.ServiceP = ServiceP;

            IEnumerable<SelectListItem> LoanAmount = _db.LookupLaonAmount.Select(i => new SelectListItem
            {
                Text = i.LoanAmount.ToString(),
                Value = i.Id.ToString()

            });

            ViewData["LoanAmount"] = LoanAmount;
            IEnumerable<SelectListItem> ActualDate = _db.LookupMonth.Select(i => new SelectListItem
            {
                Text = i.Months,
                Value = i.Id.ToString()

            });
            ViewBag.ActualDate = ActualDate;

            //ViewData["ActualDate"] = new SelectList(_db.LookupMonth, "Id", "ActualDate");

            IEnumerable<SelectListItem> LoanInstallmentId = _db.LookupLoanInstallment.Select(i => new SelectListItem
            {
                Text = i.InstallmentsNameDari,
                Value = i.Id.ToString()

            });

            ViewBag.LoanInstallmentId = LoanInstallmentId;

            IEnumerable<SelectListItem> LoanReasonID = _db.LookupReasonType.Select(i => new SelectListItem
            {
                Text = i.ReasonType,
                Value = i.Id.ToString()

            });

            ViewBag.LoanReasonID = LoanReasonID;

            IEnumerable<SelectListItem> MonthlyInstallment = _db.LookupLoanMonthlyInstallment.Select(i => new SelectListItem
            {
                Text = i.MonthlyAmount,
                Value = i.Id.ToString()

            });

            ViewBag.MonthlyInstallment = MonthlyInstallment;


            return View();
        }
        public IActionResult GoAhead() {

            return View();
        
        }
       
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoanRecipientIndividualCard obj,int id)
        {
            try
            {
                obj.Id = 0;
               // if (ModelState.IsValid)
               // {

                    var user = _userManager.GetUserId(User);
                    obj.UserId = user;
                    obj.TrackingDate = DateTime.Now;
                    _db.LoanRecipientIndividualCard.Add(obj);
                    _db.SaveChanges();
                TempData["Success"] = "Successfully Inserted.";
                return RedirectToAction(nameof(IndexByID), new { id = obj.LoanRecipientId });

            }
            catch (Exception) {


                ViewBag.Error = "شما کادرها را خالی گذاشتید.";
                    return View();

            }
            //return View();


            // IndexByID();
        }

        public IActionResult Delete(int? id)
        {

            var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanAmount).FirstOrDefault(x => x.Id == id);
            return View(list);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.LoanRecipientIndividualCard.FirstOrDefault(u => u.Id ==id);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.LoanRecipientIndividualCard.Remove(objFromDb);
            _db.SaveChanges();
            TempData["Success"] = "Update Successfullty";

            return RedirectToAction(nameof(IndexByID),new { id=objFromDb.LoanRecipientId});
        }
        public IActionResult ShowAllDataDelete(int? id)
        {

            var list = _db.LoanRecipientIndividualCard.Include(x => x.LoanAmount).FirstOrDefault(x => x.Id == id);
            return View(list);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShowAllDataDelete(int id)
        {
            var objFromDb = _db.LoanRecipientIndividualCard.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {

                return NotFound();

            }
            _db.LoanRecipientIndividualCard.Remove(objFromDb);
            _db.SaveChanges();
            TempData["Info"] = "Deleted Successfullty";
            return RedirectToAction(nameof(ShowAllDataDelete), new { id = objFromDb.LoanRecipientId });
        }
        public IActionResult Edit(int id)
        {
            //ViewBag.LoanRecipientID = _db.TblLoanRecipient
            //.SingleOrDefault(x => x.Id == id).Id;
            if (id == 0)
            {
                return NotFound();

            }
            var objFromDb = _db.LoanRecipientIndividualCard.Find(id);
            if (objFromDb == null)
            {
                NotFound();
            }

           IEnumerable<SelectListItem> mysalijari=( new List<SelectListItem>
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
            ViewBag.SaliJari = mysalijari;
            IEnumerable<SelectListItem> mysalimali = (new List<SelectListItem>
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
            ViewBag.SaliMali = mysalimali;
            IEnumerable<SelectListItem> LoanTypeId = _db.LookupLoanType.Select(i => new SelectListItem
            {
                Text = i.LoanType,
                Value = i.Id.ToString()

            });
            ViewBag.LoanTypeId = LoanTypeId;

            IEnumerable<SelectListItem> LoanAmountID = _db.LookupLaonAmount.Select(i => new SelectListItem
            {
                Text = i.LoanAmount.ToString(),
                Value = i.Id.ToString()

            });
            ViewBag.LoanAmountID = LoanAmountID;
            IEnumerable<SelectListItem> Stayus = _db.LookupStatusType.Select(s => new SelectListItem
            {
                Text = s.StatusTypeDari,
                Value = s.Id.ToString()


            });
            ViewBag.StatusType = Stayus;

            IEnumerable<SelectListItem> ServicePeriodID = _db.LookupServicePeriodType.Select(i => new SelectListItem
            {
                Text = i.ServicePeriodType,
                Value = i.Id.ToString()

            });

            ViewBag.ServicePeriodID = ServicePeriodID;
            //Where(m => m.Id == objFromDb.ActualDate)
            IEnumerable<SelectListItem> ActualDate = _db.LookupMonth.Select(i => new SelectListItem
            {
                Text = i.Months, 
                Value = i.Id.ToString()

            });
            ViewBag.ActualDate = ActualDate;

            IEnumerable<SelectListItem> LoanReasonID = _db.LookupReasonType.Select(i => new SelectListItem
            {
                Text = i.ReasonType,
                Value = i.Id.ToString()

            });
            ViewBag.LoanReasonID = LoanReasonID;
            //
            IEnumerable<SelectListItem> LoanInstallmentID = _db.LookupLoanInstallment.Select(i => new SelectListItem
            {
                Text = i.InstallmentsNameDari,
                Value = i.Id.ToString()

            });
            ViewBag.LoanInstallmentID = LoanInstallmentID;

            IEnumerable<SelectListItem> MonthlyInstallmentID = _db.LookupLoanMonthlyInstallment.Select(i => new SelectListItem
            {
                Text = i.MonthlyAmount,
                Value = i.Id.ToString()

            });
            ViewBag.MonthlyInstallmentID = MonthlyInstallmentID;

          
           
            return View(objFromDb);
        } 
           [HttpPost]
           [ValidateAntiForgeryToken]
          public IActionResult Edit(LoanRecipientIndividualCard card)
          {
           

            _db.LoanRecipientIndividualCard.Update(card);
           _db.SaveChanges();

           return RedirectToAction("IndexByID",new { id=card.LoanRecipientId});

           }
        public IActionResult ShowAllDataEdit(int id)
        {
            //ViewBag.LoanRecipientID = _db.TblLoanRecipient
            //.SingleOrDefault(x => x.Id == id).Id;
            if (id == 0)
            {
                return NotFound();

            }
            var objFromDb = _db.LoanRecipientIndividualCard.Find(id);
            if (objFromDb == null)
            {
                NotFound();
            }

            IEnumerable<SelectListItem> mysalijari = (new List<SelectListItem>
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
            ViewBag.SaliJari = mysalijari;
            IEnumerable<SelectListItem> mysalimali = (new List<SelectListItem>
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
            ViewBag.SaliMali = mysalimali;
            IEnumerable<SelectListItem> LoanTypeId = _db.LookupLoanType.Select(i => new SelectListItem
            {
                Text = i.LoanType,
                Value = i.Id.ToString()

            });
            ViewBag.LoanTypeId = LoanTypeId;

            IEnumerable<SelectListItem> LoanAmountID = _db.LookupLaonAmount.Select(i => new SelectListItem
            {
                Text = i.LoanAmount.ToString(),
                Value = i.Id.ToString()

            });
            ViewBag.LoanAmountID = LoanAmountID;
            IEnumerable<SelectListItem> Stayus = _db.LookupStatusType.Select(s => new SelectListItem
            {
                Text = s.StatusTypeDari,
                Value = s.Id.ToString()


            });
            ViewBag.StatusType = Stayus;

            IEnumerable<SelectListItem> ServicePeriodID = _db.LookupServicePeriodType.Select(i => new SelectListItem
            {
                Text = i.ServicePeriodType,
                Value = i.Id.ToString()

            });

            ViewBag.ServicePeriodID = ServicePeriodID;
            //Where(m => m.Id == objFromDb.ActualDate)
            IEnumerable<SelectListItem> ActualDate = _db.LookupMonth.Select(i => new SelectListItem
            {
                Text = i.Months,
                Value = i.Id.ToString()

            });
            ViewBag.ActualDate = ActualDate;

            IEnumerable<SelectListItem> LoanReasonID = _db.LookupReasonType.Select(i => new SelectListItem
            {
                Text = i.ReasonType,
                Value = i.Id.ToString()

            });
            ViewBag.LoanReasonID = LoanReasonID;
            //
            IEnumerable<SelectListItem> LoanInstallmentID = _db.LookupLoanInstallment.Select(i => new SelectListItem
            {
                Text = i.InstallmentsNameDari,
                Value = i.Id.ToString()

            });
            ViewBag.LoanInstallmentID = LoanInstallmentID;

            IEnumerable<SelectListItem> MonthlyInstallmentID = _db.LookupLoanMonthlyInstallment.Select(i => new SelectListItem
            {
                Text = i.MonthlyAmount,
                Value = i.Id.ToString()

            });
            ViewBag.MonthlyInstallmentID = MonthlyInstallmentID;



            return View(objFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShowAllDataEdit(LoanRecipientIndividualCard card)
        {


            _db.LoanRecipientIndividualCard.Update(card);
            _db.SaveChanges();
            TempData["Success"] = "Update Successfullty";


            return RedirectToAction("ShowAllData", new { id = card.LoanRecipientId });

        }


    }
}