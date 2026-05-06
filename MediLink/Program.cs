using MediLink.Data;
using Microsoft.EntityFrameworkCore;
using MediLink.Interfaces;
using MediLink.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MediLink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddHttpClient<IDrugInfoService, DrugInfoService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/ChooseLogin";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();

                var retries = 5;

                while (retries > 0)
                {
                    try
                    {
                        context.Database.Migrate();

                        // Seed Admin
                        var existingAdmin = context.Users
                            .FirstOrDefault(u => u.Email == "admin@medilink.com");

                        if (existingAdmin == null)
                        {
                            var adminUser = new MediLink.Models.User
                            {
                                Email = "admin@medilink.com",
                                Role = "Admin"
                            };

                            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<MediLink.Models.User>();
                            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

                            context.Users.Add(adminUser);
                            context.SaveChanges();
                        }

                        // Seed Doctor + linked User
                        if (!context.Doctors.Any())
                        {
                            var doctor = new MediLink.Models.Doctor
                            {
                                FirstName = "John",
                                LastName = "Smith",
                                Email = "john@medilink.com",
                                Specialization = "General Physician",
                                IsAvailable = true
                            };

                            context.Doctors.Add(doctor);
                            context.SaveChanges();
                            //Doctor email and Password
                            var doctorUser = new MediLink.Models.User
                            {
                                Email = "john@medilink.com",
                                Role = "Doctor",
                                DoctorId = doctor.Id
                            };

                            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<MediLink.Models.User>();
                            doctorUser.PasswordHash = passwordHasher.HashPassword(doctorUser, "Doctor123!");

                            context.Users.Add(doctorUser);
                            context.SaveChanges();
                        }

                        // Seed Patient + linked User
                        if (!context.Patients.Any())
                        {
                            var patient = new MediLink.Models.Patient
                            {
                                FirstName = "Jane",
                                LastName = "Doe",
                                Email = "jane@email.com",
                                PhoneNumber = "0821234567",
                                DateOfBirth = new DateTime(2000, 1, 1)
                            };

                            context.Patients.Add(patient);
                            context.SaveChanges();

                            var patientUser = new MediLink.Models.User
                            {
                                Email = "jane@email.com",
                                Role = "Patient",
                                PatientId = patient.Id
                            };

                            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<MediLink.Models.User>();
                            patientUser.PasswordHash = passwordHasher.HashPassword(patientUser, "Patient123!");

                            context.Users.Add(patientUser);
                            context.SaveChanges();
                        }

                        // Seed Appointment
                        if (!context.Appointments.Any())
                        {
                            var doctor = context.Doctors.FirstOrDefault();
                            var patient = context.Patients.FirstOrDefault();

                            if (doctor != null && patient != null)
                            {
                                var appointment = new MediLink.Models.Appointment
                                {
                                    DoctorId = doctor.Id,
                                    PatientId = patient.Id,
                                    AppointmentDate = DateTime.Now.AddDays(1),
                                    Status = "Scheduled"
                                };

                                context.Appointments.Add(appointment);
                                context.SaveChanges();
                            }
                        }

                        break;
                    }
                    catch
                    {
                        retries--;
                        Thread.Sleep(3000);
                    }
                }
            }

            app.Run();
        }
    }
}