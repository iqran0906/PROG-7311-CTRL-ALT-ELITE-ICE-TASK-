using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;

        public DoctorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync() // Gets all Doctors Records
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)  // Finds one doctor by id.
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddDoctorAsync(Doctor doctor) //Adds a new doctor and saves it to the database.
        {
            bool emailExists = await _context.Doctors
                .AnyAsync(d => d.Email == doctor.Email);

            if (emailExists)
            {
                throw new Exception("A doctor with this email already exists."); // Ensures doctos arent duplicated
            }

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var existingDoctor = await _context.Doctors.FindAsync(doctor.Id);

            if (existingDoctor == null)
            {
                throw new Exception("Doctor not found.");
            }

            bool emailExists = await _context.Doctors
                .AnyAsync(d => d.Email == doctor.Email && d.Id != doctor.Id);

            if (emailExists)
            {
                throw new Exception("Another doctor with this email already exists."); // Ensures that emails cant be duplicated for doctors
            }

            existingDoctor.FirstName = doctor.FirstName;
            existingDoctor.LastName = doctor.LastName;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.Email = doctor.Email;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }

            bool hasAppointments = await _context.Appointments
                .AnyAsync(a => a.DoctorId == id);

            if (hasAppointments)
            {
                throw new Exception("Cannot delete doctor with existing appointments.");
            }

            var linkedUser = await _context.Users
                .FirstOrDefaultAsync(u => u.DoctorId == id);

            if (linkedUser != null)
            {
                _context.Users.Remove(linkedUser);
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }
}
    
