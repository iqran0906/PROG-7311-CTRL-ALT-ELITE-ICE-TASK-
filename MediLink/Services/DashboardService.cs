using MediLink.Data;
using MediLink.Interfaces;
using MediLink.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PatientDashboardViewModel> GetPatientDashboardAsync(int patientId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);

            var availableDoctors = await _context.Doctors
                .Where(d => d.IsAvailable)
                .ToListAsync();

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return new PatientDashboardViewModel
            {
                Patient = patient,
                AvailableDoctors = availableDoctors,
                Appointments = appointments
            };
        }

        public async Task<DoctorDashboardViewModel> GetDoctorDashboardAsync(int doctorId)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return new DoctorDashboardViewModel
            {
                Doctor = doctor,
                Appointments = appointments
            };
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            var doctors = await _context.Doctors
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();

            var patients = await _context.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            return new AdminDashboardViewModel
            {
                Appointments = appointments,
                Doctors = doctors,
                Patients = patients
            };
        }
    }
}