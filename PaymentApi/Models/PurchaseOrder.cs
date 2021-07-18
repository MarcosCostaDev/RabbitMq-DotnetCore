using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentApi.Models
{
    public class PurchaseOrder
    {
        public decimal AmountToPay;
        public string PoNumber;
        public string CompanyName;
        public int PaymentDayTerms;
    }
}
