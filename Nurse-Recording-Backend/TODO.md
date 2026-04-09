# Backend Continuation Plan for Nurse Recording System

## Status: In Progress

1. [x] Update .csproj with required NuGet packages
2. [x] Update appsettings.json with ConnectionString and JWT settings
3. [x] Create Models: Patient.cs, Appointment.cs, PatientRecord.cs, Followup.cs, Nurse.cs
4. [x] Create Data/AppDbContext.cs
5. [x] Update Program.cs: Web API setup, CORS, JWT, Swagger
6. [x] Create Controllers: AuthController.cs (login/logout)
7. [x] Create PatientsController.cs (CRUD)
8. [x] Create other Controllers: Appointments, Records, Followups, Dashboard
9. [x] Remove MVC files (HomeController, Views, wwwroot)
10. [x] Install packages, add-migration InitialCreate, update-database
11. [x] Test with dotnet run and Swagger

Next step after each: Update this TODO.md with [x] for completed.

