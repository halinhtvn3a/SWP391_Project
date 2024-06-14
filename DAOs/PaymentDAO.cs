using DAOs;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class PaymentDAO
    {
        private readonly CourtCallerDbContext _courtCallerDbContext = null;

        public PaymentDAO()
        {
            if (_courtCallerDbContext == null)
            {
                _courtCallerDbContext = new CourtCallerDbContext();
            }
        }

        public List<Payment> GetPayments()
        {
            return _courtCallerDbContext.Payments.ToList();
        }

        public Payment GetPayment(string id)
        {
            return _courtCallerDbContext.Payments.FirstOrDefault(m => m.PaymentId.Equals(id));
        }

        public Payment AddPayment(Payment Payment)
        {
            _courtCallerDbContext.Payments.Add(Payment);
            _courtCallerDbContext.SaveChanges();
            return Payment;
        }

        //public Payment UpdatePayment(string id, Payment Payment)
        //{
        //    Payment oPayment = GetPayment(id);
        //    if (oPayment != null)
        //    {
        //        oPayment.PaymentName = Payment.PaymentName;
        //        oPayment.IsNatural = Payment.IsNatural;
        //        DbContext.Update(oPayment);
        //        DbContext.SaveChanges();
        //    }
        //    return oPayment;
        //}

        public void DeletePayment(string id)
        {
            Payment oPayment = GetPayment(id);
            if (oPayment != null)
            {
                oPayment.PaymentStatus = "Cancel";
                _courtCallerDbContext.Update(oPayment);
                _courtCallerDbContext.SaveChanges();
            }
        }

        public List<Payment> SearchByDate(DateTime start, DateTime end) => _courtCallerDbContext.Payments.Where(m => m.PaymentDate >= start && m.PaymentDate <= end).ToList();


       

    }
}
