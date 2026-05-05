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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
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
