MediLink Healthcare System – Setup Guide

Requirements:
- Docker Desktop installed and running
- Visual Studio 2022 (with .NET support)

Steps to Run the Application:

1. Open the MediLink solution (.sln) in Visual Studio
2. Ensure Docker Desktop is running
3. In Visual Studio, select:
   "Container (Dockerfile)" or "Docker Compose" profile
4. Click Run 

The application will:
- Automatically build Docker containers
- Start SQL Server inside Docker
- Apply database migrations
- Seed an Admin user

Admin Login:
Email: admin@medilink.com  
Password: Admin123!


Notes:
- The system uses Docker containers for both the web application and database

Features:
- Patient registration and login
- Admin creates doctors
- Appointment booking system
- Role-based access (Admin / Patient / Doctor)