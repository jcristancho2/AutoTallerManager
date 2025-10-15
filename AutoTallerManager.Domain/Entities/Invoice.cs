using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTallerManager.Domain.Entities;

    public class Invoice : BaseEntity
{
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        
        public string? Observations { get; set; }
        
        public decimal Taxes { get; set; }

        public int ServiceOrderId { get; set; }
        public ServiceOrder? ServiceOrder { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int PaymentTypeId { get; set; }
        public PaymentType? PaymentType { get; set; }
    }