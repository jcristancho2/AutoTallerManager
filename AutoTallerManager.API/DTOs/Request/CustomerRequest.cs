using System;
using System.ComponentModel.DataAnnotations;



namespace AutoTallerManager.API.DTOs.Request
{
    public record CustomerRequest
    {
        [Required]
        [StringLength(150)]
        public string? FullName { get; init; }

        [Required]
        [Phone]
        [StringLength(30)]
        public string? Phone { get; init; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; init; }

        [Required]
        public int CustomerTypeId { get; init; }

        [Required]
        public int AddressId { get; init; }
    }
}