using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities;

    public class Invoice : BaseEntity
    {
        public DateTime Invoice_Date { get; set; }
        public decimal Total { get; set; }
        public string Invoice_Number { get; set; } = string.Empty;
        
        public decimal Taxes { get; set; }

        public int Service_Order_Id { get; set; }
        public ServiceOrder? ServiceOrder { get; set; }

        public int Customer_Id { get; set; }
        public Customer? Customer { get; set; }

        public int PaymentTypeId { get; set; }
        public PaymentType? PaymentType { get; set; }
    }