using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaKhan.Data;
using ZaKhan.Model;
using ZaKhan.Service;

namespace ZaKhan.BusinessLogic
{
    public class BookingBusiness : IBookingBusiness
    {
        public List<BookingModel> GetAllBookings()
        {
            using (var bookingrepo = new BookingRepository())
            {
                return bookingrepo.GetAll().Select(x => new BookingModel() { BookingID = x.BookingID, Username = x.Username, BranchName = x.BranchName, OptometristName = x.OptometristName, BookedTime = x.BookedTime, BookedDate = x.BookedDate, TotalCost = x.TotalCost, Status = x.Status }).ToList();
            }
        }

        public void Insert(BookingModel bookingModel)
        {
            using (var repo = new BookingRepository())
            {
                PatientViewModel pa = new PatientViewModel();
                var dis = new Booking();
                if (true)
                {
                    //dis.BookingID = bookingModel.BookingID;

                    dis.Username = bookingModel.Username;
                    dis.BranchName = bookingModel.BranchName;
                    dis.OptometristName = bookingModel.OptometristName;
                    dis.BookedTime = bookingModel.BookedTime;
                    dis.BookedDate = bookingModel.BookedDate;
                    dis.Status = bookingModel.Status;

                    dis.TotalCost = bookingModel.TotalCost;

                }
                repo.Insert(dis);
            }
        }

        public void Update(BookingModel bookingModel)
        {
            using (var repo = new BookingRepository())
            {
                PatientViewModel pa = new PatientViewModel();
                var dis = new Booking();
                dis = repo.GetById(bookingModel.BookingID);
                if (true)
                {
                    //dis.BookingID = bookingModel.BookingID;

                    dis.Username = bookingModel.Username;
                    dis.BranchName = bookingModel.BranchName;
                    dis.OptometristName = bookingModel.OptometristName;
                    dis.BookedTime = bookingModel.BookedTime;
                    dis.BookedDate = bookingModel.BookedDate;
                    dis.TotalCost = bookingModel.TotalCost;
                    dis.Status = bookingModel.Status;
                }
                repo.Update(dis);


            }
        }

        public void Delete(BookingModel bookingModel)
        {
            using (var repo = new BookingRepository())
            {
                var dis = new Booking();
                dis = repo.GetById(bookingModel.BookingID);
                repo.Delete(dis);
            }

        }
        public BookingModel GetById(int id)
        {
            using (var repo = new BookingRepository())
            {
                return GetAllBookings().Find(x => x.BookingID == id);
            }
        }
        public BookingModel GetByUser(string user)
        {
            using (var repo = new BookingRepository())
            {
                return GetAllBookings().Find(x => x.Username == user);
            }
        }
        public void Dispose(bool disposing)
        {
            using (var repo = new BookingRepository())
            {
                repo.Dispose();
            }
        }
    }
}
