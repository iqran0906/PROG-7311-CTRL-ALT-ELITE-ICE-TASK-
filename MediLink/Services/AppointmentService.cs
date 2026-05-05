using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync() //Finds all appointments 
        {
            return await _context.Appointments
                .Include(a => a.Patient) //So names show instead of ID 
                .Include(a => a.Doctor) //So names show instead of ID 
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id) //Finds appointment by id
        {
            return await _context.Appointments
                .Include(a => a.Patient) //So names show instead of ID 
                .Include(a => a.Doctor) //So names show instead of ID 
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAppointmentAsync(Appointment appointment) 
        {
            if (appointment.AppointmentDate < DateTime.Now)
            {
                throw new Exception("Appointment date cannot be in the past."); //Ensures that appointments are created for the future 
            }

            bool patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            if (!patientExists)
            {
                throw new Exception("Selected patient does not exist."); // Message if patient is not created or doesnt exist
            }

            bool doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);
            if (!doctorExists)
            {
                throw new Exception("Selected doctor does not exist."); // Message if doctor is not created or doesnt exist
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetPatientSelectListAsync() //Dropdown for Patients
        {
            return await _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FirstName + " " + p.LastName
                })
                .ToListAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = await _context.Appointments.FindAsync(appointment.Id);

            if (existingAppointment == null)
            {
                throw new Exception("Appointment not found.");
            }

            if (appointment.AppointmentDate < DateTime.Now)
            {
                throw new Exception("Appointment date cannot be in the past.");
            }

            bool patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            if (!patientExists)
            {
                throw new Exception("Selected patient does not exist.");
            }

            bool doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);
            if (!doctorExists)
            {
                throw new Exception("Selected doctor does not exist.");
            }

            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.Status = appointment.Status;
            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.DoctorId = appointment.DoctorId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetDoctorSelectListAsync() //Dropdown for doctors
        {
            return await _context.Doctors
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + " " + d.LastName + " - " + d.Specialization
                })
                .ToListAsync();
        }
    }
}