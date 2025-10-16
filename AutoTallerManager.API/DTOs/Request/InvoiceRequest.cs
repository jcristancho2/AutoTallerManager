using System;
using System.ComponentModel.DataAnnotations;

namespace AutoTallerManager.API.DTOs.Request
{
    public record InvoiceRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        [Required]
        public int ServiceOrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }
    }
}