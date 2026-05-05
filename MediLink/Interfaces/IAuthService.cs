using MediLink.Models;
using MediLink.ViewModels;

namespace MediLink.Interfaces
{
    public interface IAuthService
    {
        Task RegisterPatientAsync(RegisterPatientViewModel model);
        Task RegisterDoctorAsync(RegisterDoctorViewModel model);
        Task RegisterAdminAsync(RegisterAdminViewModel model);
        Task<User?> ValidateUserAsync(string email, string password, string role);
         Task CreateDoctorByAdminAsync(AdminCreateDoctorViewModel model);
    }
}