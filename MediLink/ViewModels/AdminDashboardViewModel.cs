using MediLink.Models;

namespace MediLink.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();
        public List<Doctor> Doctors { get; set; } = new();
        public List<Patient> Patients { get; set; } = new();
    }
}