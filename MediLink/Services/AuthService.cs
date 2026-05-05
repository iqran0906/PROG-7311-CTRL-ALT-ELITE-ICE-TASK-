using MediLink.Data;
using MediLink.Interfaces;
using MediLink.Models;
using MediLink.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task RegisterPatientAsync(RegisterPatientViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                throw new Exception("An account with this email already exists.");
            }

            var patient = new Patient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = model.Email,
                Role = "Patient",
                PatientId = patient.Id
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterDoctorAsync(RegisterDoctorViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                throw new Exception("An account with this email already exists.");
            }

            var doctor = new Doctor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Specialization = model.Specialization,
                IsAvailable = model.IsAvailable
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = model.Email,
                Role = "Doctor",
                DoctorId = doctor.Id
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterAdminAsync(RegisterAdminViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                throw new Exception("An account with this email already exists.");
            }

            var user = new User
            {
                Email = model.Email,
                Role = "Admin"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> ValidateUserAsync(string email, string password, string role)
        {
            var user = await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == role);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return result == PasswordVerificationResult.Success ? user : null;
        }
        public async Task CreateDoctorByAdminAsync(AdminCreateDoctorViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                throw new Exception("An account with this email already exists.");
            }

            var doctor = new Doctor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Specialization = model.Specialization,
                IsAvailable = model.IsAvailable
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = model.Email,
                Role = "Doctor",
                DoctorId = doctor.Id
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}