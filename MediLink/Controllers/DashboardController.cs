using System.Security.Claims;
using MediLink.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientDashboard()
        {
            var patientIdClaim = User.FindFirst("PatientId")?.Value;

            if (string.IsNullOrWhiteSpace(patientIdClaim) || !int.TryParse(patientIdClaim, out int patientId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = await _dashboardService.GetPatientDashboardAsync(patientId);
            return View(model);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var patientIdClaim = User.FindFirst("PatientId")?.Value;

            if (string.IsNullOrWhiteSpace(patientIdClaim) || !int.TryParse(patientIdClaim, out int patientId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = await _dashboardService.GetPatientDashboardAsync(patientId);
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorDashboard()
        {
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;

            if (string.IsNullOrWhiteSpace(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = await _dashboardService.GetDoctorDashboardAsync(doctorId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var model = await _dashboardService.GetAdminDashboardAsync();
            return View(model);
        }
    }
}