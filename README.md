# PROG-7311-CTRL-ALT-ELITE-ICE-TASK
# MediLink Healthcare Management System

MediLink is a web-based healthcare management system designed to connect patients, doctors, and administrators in one platform. It allows patients to book appointments, doctors to manage schedules, and administrators to oversee the system.

# Features

## Patient Features
- Register and log in securely  
- View available doctors  
- Book appointments  
- View appointment history  
- Manage profile  

## Doctor Features
- Login to doctor dashboard  
- View assigned patients  
- Manage availability  
- View scheduled appointments  

## Admin Features
- Create doctor accounts  
- Manage patients and doctors  
- Oversee system activity  

# User Roles

The system has three roles:
- Admin (Full Access)
- Doctor
- Patient

Only the Admin can create new doctor accounts.

# Admin Login Details

Email: admin@medilink.com  
Password: Admin123!

# Getting Started

git clone <your-repository-link>

Open the project in Visual Studio

dotnet restore

dotnet run

Then open the localhost link shown in the terminal.

# How to Use the App

Patient:
Register → Login → Book appointment → View appointments

Doctor:
Login → View patients → Manage schedule → View appointments

Admin:
Login → Create doctors → Manage users → Monitor system

# Rules

- Users must log in according to their role  
- Patients cannot access doctor/admin pages  
- Only admin can create doctors  
- Ensure database is running before starting the application  

# Project Purpose

This project demonstrates role-based authentication, CRUD operations, database integration, and a full web application structure.

