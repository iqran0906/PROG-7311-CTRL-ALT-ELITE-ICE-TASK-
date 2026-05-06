using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MediLink.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [Authorize(Roles = "Admin")]

        // Displays the details page for a specific appointment.
        // The appointment ID is used to retrieve the correct appointment record.
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();

            if (User.IsInRole("Admin"))
            {
                ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
            }

            return View();
        }

        // Handles the submitted appointment creation form.
        // Only Admin and Patient users can create appointments.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (User.IsInRole("Patient"))
            {
                var patientIdClaim = User.FindFirst("PatientId")?.Value;

                if (string.IsNullOrWhiteSpace(patientIdClaim) || !int.TryParse(patientIdClaim, out int patientId))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                appointment.PatientId = patientId;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();

                if (User.IsInRole("Admin"))
                {
                    ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
                }

                return View(appointment);
            }

            try
            {
                await _appointmentService.AddAppointmentAsync(appointment);

                if (User.IsInRole("Patient"))
                {
                    return RedirectToAction("PatientDashboard", "Dashboard");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();

                if (User.IsInRole("Admin"))
                {
                    ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
                }

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(appointment);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
            ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();
            return View(appointment);
        }

        // Handles the submitted appointment edit form.
        // Only Admin users are allowed to update appointment details.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
            // Checks whether the updated appointment form passed validation.
            // If validation fails, the patient and doctor dropdown lists are loaded again
            // before returning the user to the edit page.
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
                ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();
                return View(appointment);
            }

            try
            {
                await _appointmentService.UpdateAppointmentAsync(appointment);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Patients = await _appointmentService.GetPatientSelectListAsync();
                ViewBag.Doctors = await _appointmentService.GetDoctorSelectListAsync();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(appointment);
            }
        }

        // Only Admin users are allowed to access the appointment delete page.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // Handles the final delete confirmation after the Admin confirms they want to remove the appointment.
        // ActionName("Delete") allows this method to respond to the Delete form post.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(appointment);
            }
        }

        // Only Patient users are allowed to cancel their own appointments.
        [Authorize(Roles = "Patient")]
           
            public async Task<IActionResult> Cancel(int id)
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (appointment == null)
                {
                    return NotFound();
                }

                var patientIdClaim = User.FindFirst("PatientId")?.Value;

                if (!int.TryParse(patientIdClaim, out int patientId) || appointment.PatientId != patientId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                return View(appointment);
            }

        // Handles the final cancellation after the patient confirms they want to cancel the appointment.
        // ActionName("Cancel") allows this method to respond to the Cancel form post.
        [HttpPost, ActionName("Cancel")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Patient")]
            public async Task<IActionResult> CancelConfirmed(int id)
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

                if (appointment == null)
                {
                    return NotFound();
                }

                var patientIdClaim = User.FindFirst("PatientId")?.Value;

                if (!int.TryParse(patientIdClaim, out int patientId) || appointment.PatientId != patientId)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                await _appointmentService.DeleteAppointmentAsync(id);

                return RedirectToAction("MyAppointments", "Dashboard");
            }
        }
    }
