using MediLink.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediLink.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int id);
        Task<List<SelectListItem>> GetPatientSelectListAsync(); // links to patient 
        Task<List<SelectListItem>> GetDoctorSelectListAsync(); // links to doctor
    }
}
