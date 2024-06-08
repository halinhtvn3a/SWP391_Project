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
        private readonly CourtCallerDbContext DbContext = null;

        public PaymentDAO()
        {
            if (DbContext == null)
            {
                DbContext = new CourtCallerDbContext();
            }
        }

        public List<Payment> GetPayments()
        {
            return DbContext.Payments.ToList();
        }

        public Payment GetPayment(string id)
        {
            return DbContext.Payments.FirstOrDefault(m => m.PaymentId.Equals(id));
        }

        public Payment AddPayment(Payment Payment)
        {
            DbContext.Payments.Add(Payment);
            DbContext.SaveChanges();
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
                DbContext.Update(oPayment);
                DbContext.SaveChanges();
            }
        }

        public List<Payment> SearchByDate(DateTime start, DateTime end) => DbContext.Payments.Where(m => m.PaymentDate >= start && m.PaymentDate <= end).ToList();


    }
}
