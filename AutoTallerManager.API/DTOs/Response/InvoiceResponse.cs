using System;

namespace AutoTallerManager.API.DTOs.Response
{
    public record InvoiceResponse
    {
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int ServiceOrderId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
    }
}