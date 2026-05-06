using System.Security.Claims;
using MediLink.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    /// Handles dashboard-related requests for patients, doctors, and administrators.
    /// Responsible for retrieving and displaying role-specific dashboard information.

    public class DashboardController : Controller
    {

        // Service used to retrieve dashboard data for different user roles
        private readonly IDashboardService _dashboardService;

        /// Initializes the dashboard controller with the required dashboard service.
        /// <param name="dashboardService">Service responsible for dashboard operations.</param>

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// Displays the patient dashboard containing patient-specific information and statistics.
        /// <returns>The patient dashboard view.</returns>

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> PatientDashboard()
        {
            // Retrieves the patient identifier from the authenticated user's claims
            var patientIdClaim = User.FindFirst("PatientId")?.Value;

            if (string.IsNullOrWhiteSpace(patientIdClaim) || !int.TryParse(patientIdClaim, out int patientId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Retrieves patient dashboard data from the dashboard service
            var model = await _dashboardService.GetPatientDashboardAsync(patientId);
            return View(model);
        }

        /// Displays appointment information associated with the logged-in patient.
        /// <returns>The appointments view for the patient.</returns>

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            // Retrieves the patient identifier from the authenticated user's claims
            var patientIdClaim = User.FindFirst("PatientId")?.Value;

            // Validates the patient identifier before loading dashboard data
            if (string.IsNullOrWhiteSpace(patientIdClaim) || !int.TryParse(patientIdClaim, out int patientId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Retrieves patient dashboard data from the dashboard service
            var model = await _dashboardService.GetPatientDashboardAsync(patientId);
            return View(model);
        }

        /// Displays dashboard information relevant to doctors.
        /// <returns>The doctor dashboard view.</returns>

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DoctorDashboard()
        {
            // Retrieves the doctor identifier from the authenticated user's claims
            var doctorIdClaim = User.FindFirst("DoctorId")?.Value;

            // Validates the doctor identifier before loading dashboard data
            if (string.IsNullOrWhiteSpace(doctorIdClaim) || !int.TryParse(doctorIdClaim, out int doctorId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Retrieves doctor dashboard data from the dashboard service
            var model = await _dashboardService.GetDoctorDashboardAsync(doctorId);
            return View(model);
        }

        /// Displays administrative dashboard information and system statistics.
        /// <returns>The administrator dashboard view.</returns>

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            // Retrieves administrator dashboard data from the dashboard service
            var model = await _dashboardService.GetAdminDashboardAsync();
            return View(model);
        }
    }
}