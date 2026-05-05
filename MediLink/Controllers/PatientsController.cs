using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using MediLink.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MediLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PatientsController : Controller
    {


        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return View(patients);
        }



        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }
            try
            {
                await _patientService.AddPatientAsync(patient);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(patient);
                }


            }
        }
public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return View(patient);
            }

            try
            {
                await _patientService.UpdatePatientAsync(patient);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(patient);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _patientService.DeletePatientAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(patient);
            }
        }
    }
}