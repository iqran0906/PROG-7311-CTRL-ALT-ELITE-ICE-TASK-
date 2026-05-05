using MediLink.Models;

namespace MediLink.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public Doctor? Doctor { get; set; }
        public List<Appointment> Appointments { get; set; } = new();
    }
}