using MediLink.ViewModels;

namespace MediLink.Interfaces
{
    public interface IDashboardService
    {
        Task<PatientDashboardViewModel> GetPatientDashboardAsync(int patientId);
        Task<DoctorDashboardViewModel> GetDoctorDashboardAsync(int doctorId);
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
    }
}