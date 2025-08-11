using System;
using System.ComponentModel.DataAnnotations;

namespace PetWeb.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(150)]
        public string? OwnerName { get; set; }

        [Url]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}