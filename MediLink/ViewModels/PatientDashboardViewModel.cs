using MediLink.Models;

namespace MediLink.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient? Patient { get; set; }
        public List<Doctor> AvailableDoctors { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}