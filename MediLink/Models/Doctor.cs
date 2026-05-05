using System.ComponentModel.DataAnnotations;

namespace MediLink.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}