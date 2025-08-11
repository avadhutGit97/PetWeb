using System;
using System.ComponentModel.DataAnnotations;

namespace PetWeb.Models
{
    public enum PetType
    {
        Dog = 0,
        Cat = 1,
        Rabbit = 2,
        Horse = 3,
        Other = 4
    }

    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Type")]
        public PetType Type { get; set; } = PetType.Dog;

        [Required]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Range(0, 40)]
        [Display(Name = "Age (years)")]
        public int? AgeYears { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(150)]
        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }

        [EmailAddress]
        [Display(Name = "Owner Email")]
        public string? OwnerEmail { get; set; }

        [Url]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}