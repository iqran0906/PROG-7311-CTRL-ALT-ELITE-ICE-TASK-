using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using Microsoft.EntityFrameworkCore;


namespace MediLink.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetAllPatientsAsync() //Gets all patient records
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id) // Finds one patient by id.
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPatientAsync(Patient patient) //Adds a new patient and saves it to the database.
        {

            bool emailExists = await _context.Patients
      .AnyAsync(p => p.Email == patient.Email);

            if (emailExists)
            {
                throw new Exception("A patient with this email already exists.");
            }

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(patient.Id);

            if (existingPatient == null)
            {
                throw new Exception("Patient not found.");
            }

            bool emailExists = await _context.Patients
                .AnyAsync(p => p.Email == patient.Email && p.Id != patient.Id);

            if (emailExists)
            {
                throw new Exception("Another patient with this email already exists.");
            }

            existingPatient.FirstName = patient.FirstName;
            existingPatient.LastName = patient.LastName;
            existingPatient.Email = patient.Email;
            existingPatient.PhoneNumber = patient.PhoneNumber;
            existingPatient.DateOfBirth = patient.DateOfBirth;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }

            bool hasAppointments = await _context.Appointments
                .AnyAsync(a => a.PatientId == id);

            if (hasAppointments)
            {
                throw new Exception("Cannot delete patient with existing appointments.");
            }

            var linkedUser = await _context.Users
                .FirstOrDefaultAsync(u => u.PatientId == id);

            if (linkedUser != null)
            {
                _context.Users.Remove(linkedUser);
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }
}

