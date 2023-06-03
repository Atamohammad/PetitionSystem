using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TBMIS.Models.ViewModel;
using TeacherLoanBoxProject.Data;
using TeacherLoanBoxProject.Models;

namespace TeacherLoanBoxProject.Controllers
{
    public class BalanceController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly  int CreditAmount;
        
        //private object _db;

        public BalanceController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
           // CreditAmount = _db.LoanRecipientIndividualCard.Select(c=>c.LoanAmount.LoanAmount).Sum();
            ViewBag.creditAmount = CreditAmount;
        }

        public IEnumerable<SelectListItem> PleaseSelectList { get; set; }

        public IActionResult Index()
        {
            CreditForBlanceVM creditForBlance = new CreditForBlanceVM();
            // var one = creditForBlance.Credit = (from x in _db.LoanRecipientIndividualCard.FirstOrDefault(c => c.ActualDate == 1).LoanAmount.LoanAmount.Count;
            var one = creditForBlance.Credit = _db.LoanRecipientIndividualCard.Where(c => c.ActualDate == 1).Select(d => d.LoanAmount.LoanAmount).Count();

            ViewBag.Hamel = one;

            var list = _db.LoanRecipientIndividualCard.Select(x => x.LoanAmount.LoanAmount).Sum();
            var objList = _db.Balance.ToList();
            return View(objList);
            
        }


        public IActionResult Create()
        {


            IEnumerable<SelectListItem> CardIdSelect = _db.LoanRecipientIndividualCard.Select(i=>new SelectListItem
            {
              Text=i.LoanRecipientId.ToString()

            });
           
            
            IEnumerable<SelectListItem> name = _db.TblLoanRecipient.Select(i=>new SelectListItem
            {
              Text=i.Name,
              Value=i.Id.ToString()

            });
           
             return View(); 

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Balance obj)
        {

          

             var user= _userManager.GetUserId(User);
            obj.UserId = user;


            _db.Balance.Add(obj);
            _db.SaveChanges();

           
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();

            }
            var obj = _db.Balance.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Balance obj)
        {

            _db.Balance.Update(obj);
            _db.SaveChanges();


            return RedirectToAction("Index");


        }
       
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();

            }
            var obj = _db.Balance.Find(id);

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

            var obj = _db.Balance.Find(id);
            if (obj == null)
            {
                return NotFound();

            }

              _db.Balance.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
           




        }

        public IActionResult Details() 
        {
           
            //DetailsVM dvm = new DetailsVM()
            //{

            //    Product=_db.Product.Include(u=>u.Category).Where(u=>u.Id==id).FirstOrDefault(),
            //    ExistsInCard=false


            //};

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
