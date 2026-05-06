using MediLink.Interfaces;
using MediLink.Models;
using MediLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    /// Handles doctor management operations such as creating, editing,
    /// viewing, and deleting doctor records within the system.

    [Authorize(Roles = "Admin")]
    public class DoctorsController : Controller
    {
        // Service responsible for doctor-related business operations
        private readonly IDoctorService _doctorService;
        // Service responsible for authentication and account creation
        private readonly IAuthService _authService;


        /// Initializes the doctors controller with required services.
        /// <param name="doctorService">Service used for doctor management operations.</param>
        /// <param name="authService">Service used for authentication-related operations.</param>
        public DoctorsController(IDoctorService doctorService, IAuthService authService)
        {
            _doctorService = doctorService;
            _authService = authService;
        }

        /// Displays a list of all registered doctors.
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return View(doctors);
        }

        /// Displays detailed information for a specific doctor.
        /// <param name="id">Unique identifier of the doctor.</param>
        public async Task<IActionResult> Details(int id)
        {
            // Retrieves doctor information based on the provided identifier
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            // Returns a not found result if the doctor does not exist
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        /// Displays the form used to create a new doctor account.
        public IActionResult Create()
        {
            return View();
        }

        /// Processes the submitted doctor creation form.
        /// <param name="model">Doctor registration information submitted by the administrator.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateDoctorViewModel model)
        {
            // Validates submitted form data before processing
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Creates a new doctor account using the authentication service
                await _authService.CreateDoctorByAdminAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Displays validation or processing errors to the administrator
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        /// Displays the edit form for an existing doctor record.
        /// <param name="id">Unique identifier of the doctor.</param>
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieves doctor information for editing
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            // Returns a not found result if the doctor does not exist
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        /// Processes updates made to an existing doctor record.
        /// <param name="doctor">Updated doctor information.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            // Validates submitted doctor information before updating
            if (!ModelState.IsValid)
            {
                return View(doctor);
            }

            try
            {
                // Updates doctor information in the database
                await _doctorService.UpdateDoctorAsync(doctor);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Displays validation or processing errors to the administrator
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(doctor);
            }
        }

        /// Displays the delete confirmation page for a doctor.
        /// <param name="id">Unique identifier of the doctor.</param>
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieves doctor information before deletion
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            // Returns a not found result if the doctor does not exist
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        /// Permanently removes a doctor record from the system.
        /// <param name="id">Unique identifier of the doctor.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Deletes the selected doctor from the database
                await _doctorService.DeleteDoctorAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Retrieves doctor information again if deletion fails
                var doctor = await _doctorService.GetDoctorByIdAsync(id);

                // Displays validation or processing errors to the administrator
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(doctor);
            }
        }
    }
}