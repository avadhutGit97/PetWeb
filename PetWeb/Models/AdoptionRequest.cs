using System;
using System.ComponentModel.DataAnnotations;

namespace PetWeb.Models
{
    public class AdoptionRequest
    {
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        [StringLength(120)]
        public string AdopterName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string AdopterEmail { get; set; } = string.Empty;

        [Phone]
        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Preferred Visit Date")]
        public DateOnly? PreferredVisitDate { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}