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
   public class AuditBusiness : IAuditBusiness
   {
       public List<AuditView> ViewAll()
       {
           using (var rep = new AuditRepository())
           {
               return rep.GetAll().Select(x => new AuditView
               {
                   AuditID = x.AuditID,
                   UserName = x.UserName,
                   Event = x.Event,
                   TimeAccessed = x.TimeAccessed,
               }).ToList();
           }
       }

       public List<AuditView> Search(string SearchString)
       {
           using (var patientrepo = new AuditRepository())
           {
               return patientrepo.GetAll().Select(x => new AuditView() { UserName = x.UserName, Event = x.Event, TimeAccessed = x.TimeAccessed, AuditID = x.AuditID }).Where(s => s.UserName.ToUpper().Contains(SearchString.ToUpper())
               || s.UserName.ToUpper().Contains(SearchString.ToUpper())).ToList();
           }
       }


       private static Audit ConvertToAudit(AuditView modelView)
       {
           var dev = new Audit
           {
               AuditID = modelView.AuditID,
               UserName = modelView.UserName,
               Event = modelView.Event,
               TimeAccessed = modelView.TimeAccessed,
           };
           return dev;
       }

       public void Insert(AuditView model)
       {
           using (var rep = new AuditRepository())
           {
               rep.Insert(ConvertToAudit(model)); 
           }
       }


       //public void Update(AuditViewModel modelView)
       //{
       //    using (var rep = new AuditRepository())
       //    {
       //        var dev = rep.GetById(modelView.AuditID);

       //        if (dev != null)
       //        {
       //         dev.UserName = modelView.UserName;
       //         dev.Event = modelView.Event;
       //         dev.TimeAccessed = modelView.TimeAccessed;
       //            rep.Update(dev);
       //        }
       //    }
       //}

       

       public void Delete(int model)
       {
           using (var rep = new AuditRepository())
           {
               var dev = rep.GetById(model);
               rep.Delete(dev);
           }
       }


       public AuditView JustGetOne(int id)
       {
           return ViewAll().SingleOrDefault(x => x.AuditID.Equals(id));
       }
    }
   }

