using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZaKhan.BusinessLogic;
using ZaKhan.Data;
using ZaKhan.Model;
using Microsoft.AspNet.Identity;
using ZaKhan.Service;

namespace ZaKhan.Controllers
{
    [Audit]
    public class BookingController : Controller
    {
        private readonly IBookingBusiness _bookingBusiness = new BookingBusiness();
        private readonly IPatientBusiness _patientBusiness = new PatientBusiness();
        private readonly IOptometristBusiness _optometristBusiness = new OptometristBusiness();
        private readonly IStoreLocationBusiness _storelocationBusiness = new StoreLocationBusiness();
        private readonly IFeeBusiness _feeBusiness = new FeeBusiness();


        public ActionResult Index()
        {
           
            return View(_bookingBusiness.GetAllBookings());
        }

        // GET: Booking/Details/5
      

        // GET: Booking/Create
        public ActionResult Create()
        {
            ApplicationDbContext db = new ApplicationDbContext();
        
           
            string user = User.Identity.GetUserName();
            List<Patient> lst = db.Patients.ToList().FindAll(r => r.UserName == user);
            ViewBag.listdetails = lst;

           

            List<SelectListItem> branchNames = new List<SelectListItem>();
            BookingModel book = new BookingModel();

            List<StoreLocation> branch = db.StorLocations.ToList();
            branch.ForEach(x =>
                {
                    branchNames.Add(new SelectListItem { Text = x.Description, Value = x.Description});
                });
            //book.Branch = branchNames;
            book.Branch = branchNames;

           
            return View(book);
        }



        [HttpPost]
        public ActionResult GetOptometrist(StoreLocationView storeModel,string Description)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //string Description = "Musgrave Mall";
            int statId;
            string LocationId = db.StorLocations.ToList().Find(x => x.Description == Description).LocationId.ToString();
            List<SelectListItem> optometristNames = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(LocationId))
            {
                statId = Convert.ToInt32(LocationId);
                List<Optometrist> optometrists = db.Optometrists.Where(x => x.LocationId == statId).ToList();
                optometrists.ForEach(x =>
                {
                    optometristNames.Add(new SelectListItem { Text = x.OptometristName, Value = x.OptometristName.ToString() });
                });
            }
            return Json(optometristNames, JsonRequestBehavior.AllowGet);
        }



        // POST: Booking/Create
        [HttpPost]
        [Audit]
        public ActionResult Create(BookingModel bookingModel, string Username, string optometristNames, string Status = "Pending")
        {
            OptometristModel optom = new OptometristModel();
            StoreLocationView sl = new StoreLocationView();
            ApplicationDbContext db = new ApplicationDbContext();
          
            string user = User.Identity.GetUserName();
            List<Patient> lst = db.Patients.ToList().FindAll(r => r.UserName == user);
            ViewBag.listdetails = lst;


            

            ViewBag.op = optometristNames;
        

            if (ModelState.IsValid)
            {
                List<SelectListItem> branchNames = new List<SelectListItem>();


                List<StoreLocation> branch = db.StorLocations.ToList();
                branch.ForEach(x =>
                {
                    branchNames.Add(new SelectListItem { Text = x.Description, Value = x.Description });
                });
                bookingModel.Branch = branchNames;
               
                if (db.Bookings.Any(p => p.Username == user))
                {
                    ModelState.AddModelError("", "*You have already made a booking, View Booking Details");
                }
                else
                {
                    if (db.Bookings.Any(k => k.BookedDate == bookingModel.BookedDate && db.Bookings.Any(s => s.BookedTime == bookingModel.BookedTime)))
                    {
                        ModelState.AddModelError("", "*Time has already been booked, Please select a new time");

                    }
                    else
                    {
                       
                        foreach (Patient p in lst)
                        {
                            TempData["message"] = string.Format("{0} Please Confirm Your Booking", p.FirstName + " " + p.LastName);
                        }

                        bookingModel.Username = User.Identity.GetUserName();
                        bookingModel.TotalCost = _feeBusiness.CalTot();
                        bookingModel.Status = Status;
                        _bookingBusiness.Insert(bookingModel);
                        return RedirectToAction("Confirm","Booking");
                    }
                }

            }

            else
            {
                List<SelectListItem> branchNames = new List<SelectListItem>();


                List<StoreLocation> branch = db.StorLocations.ToList();
                branch.ForEach(x =>
                {
                    branchNames.Add(new SelectListItem { Text = x.Description, Value = x.Description });
                });
                bookingModel.Branch = branchNames;
                ModelState.AddModelError("", "Please Fill In Your Details Correctly");
            }

           
          

            return View(bookingModel);
        }

        // GET: Booking/Edit/5
        public ActionResult Edit(int id)
        {
            ViewData["optometristBranch"] = new SelectList(_storelocationBusiness.GetAllLocations(), "Description", "Description");
            ViewData["optometrists"] = new SelectList(_optometristBusiness.GetOptometrists(), "OptometristName", "OptometristName");
            var booking = _bookingBusiness.GetById(id);
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [Audit]
        public ActionResult Edit(int id, BookingModel bookingModel)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string user = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
                ViewData["optometristBranch"] = new SelectList(_storelocationBusiness.GetAllLocations(), "Description", "Description");
                ViewData["optometrists"] = new SelectList(_optometristBusiness.GetOptometrists(), "OptometristName", "OptometristName");

                if (db.Bookings.Any(k => k.BookedDate == bookingModel.BookedDate && db.Bookings.Any(s => s.BookedTime == bookingModel.BookedTime && bookingModel.Username!=user)))
                {
                    ModelState.AddModelError("", "*Time has already been booked, Please select a new time");

                }
                else
                {
                    bookingModel.Username = User.Identity.GetUserName();
                    _bookingBusiness.Update(bookingModel);

                    return RedirectToAction("Bookings");
                }
               

            }
            return View();
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int id)
        {
            var booking = _bookingBusiness.GetById(id);
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var booking = _bookingBusiness.GetById(id);
            _bookingBusiness.Delete(booking);
            if (User.IsInRole("Admin").Equals(true))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Bookings");
            }
        }

        public ActionResult Bookings()
        {
            BookingModel bookingModel = new BookingModel();

            ApplicationDbContext db = new ApplicationDbContext();
            string user = User.Identity.GetUserName();
          //  List<Booking> lst2 = db.Bookings.Find().Username.ToList();
            ViewBag.Books = db.Bookings.ToList();

            return View(_bookingBusiness.GetAllBookings().FindAll(r => r.Username == user));
            //return View();
        }

        [HttpPost]
        public ActionResult Bookings(BookingModel bookingModel)
        {
            

            ApplicationDbContext db = new ApplicationDbContext();
            string user = User.Identity.GetUserName();
            List<Booking> lst2 = db.Bookings.ToList().FindAll(r => r.Username == user);
            ViewBag.Books = lst2;
           
            return View(_bookingBusiness.GetAllBookings().FindAll(r => r.Username == user));
            //return View();
        }



       // [HttpPost]
        public ActionResult Invoice()
        {
            string user = User.Identity.GetUserName();
            ApplicationDbContext db = new ApplicationDbContext();

            List<Patient> lst = db.Patients.ToList().FindAll(r => r.UserName == user);
            ViewBag.listdetails = lst;
            List<Booking> lst2 = db.Bookings.ToList().FindAll(r => r.Username == user);
            ViewBag.listbooks = lst2;
            return View();
        }


        public ActionResult Calendar(BookingModel bookingModel)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Booking> lst = db.Bookings.ToList();
            ViewBag.All = lst;
           

            ViewData["optometristBranch"] = new SelectList(_storelocationBusiness.GetAllLocations(), "Description", "Description");
            return View("Calendar",bookingModel);
        }

        public ActionResult MusgraveCal()
        {

            
            ApplicationDbContext db = new ApplicationDbContext();

             List<Booking> lst = db.Bookings.ToList();
             ViewBag.All = lst;

            List<Booking> lst1 = db.Bookings.ToList().FindAll(r => r.BranchName == "Musgrave Mall");
            ViewBag.Musgrave = lst1;
            return View();
        }

        public ActionResult GatewayCal()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Booking> lst = db.Bookings.ToList();
            ViewBag.All = lst;
            List<Booking> lst2 = db.Bookings.ToList().FindAll(r => r.BranchName == "Gateway Mall");
            ViewBag.Gateway = lst2;

            return View();
        }

        public ActionResult PavilionCal()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Booking> lst = db.Bookings.ToList();
            ViewBag.All = lst;
            List<Booking> lst3 = db.Bookings.ToList().FindAll(r => r.BranchName == "Pavilion Mall");
            ViewBag.Pavilion = lst3;
            return View();
        }



        //Step One of Two for creating Booking Confirm , Getting data
        public ActionResult Confirm()
        {

            ApplicationDbContext db = new ApplicationDbContext();
            Booking booking = db.Bookings.SingleOrDefault(x => x.Username.Equals(User.Identity.Name));
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);

        }

        //Step Two of Two for creating Booking Confirm , Posting data
        [HttpPost]
        public ActionResult Confirm(Booking bookingView, string Status = "Confirmed")
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                string user = User.Identity.GetUserName();
                List<Patient> lst = db.Patients.ToList().FindAll(r => r.UserName == user);

                foreach (Patient p in lst)
                {
                    string email = p.Email;
                    var mail = new Email();
                    var message = "Hello" + " " + p.FirstName + " " + p.LastName + " " +
                    "This is a notification to inform you that you have made a booking with ZaKhan Optometrists. If This is not you, please contact us itstudents.dut.ac.za/201615";
                    mail.SendEmail(email, message);
                }

                foreach (Patient p in lst)
                {
                    TempData["message"] = string.Format("{0} Your booking was successfully", p.FirstName + " " + p.LastName);
                }


                bookingView.Status = Status;
                db.Entry(bookingView).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Bookings","Booking");

            }
            return View();
        }
    }
}
