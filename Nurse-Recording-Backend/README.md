# Nurse Recording System - Backend

This repository contains the backend database scripts and configurations for the **Nurse Recording System**, an application designed to streamline the management of patient records, medical appointments, and clinic staff accounts.

## Database Schema

The system uses SQL Server (typically configured for `(localdb)\MSSQLLocalDB` for local development) with the database name `NurseRecordingDb`. 

### Core Tables:
- **Nurses**: Stores authentication and account information for system users (Admins and Nurses).
- **Patients**: Contains patient demographic details, emergency contacts, and social media links.
- **Appointments**: Tracks scheduled visits and the reasons for appointments.
- **PatientRecords**: Holds detailed medical records including diagnoses, symptoms, treatments, and clinical notes.
- **Followups**: Tracks subsequent follow-up notes for specific patients.

## Setup Instructions

To initialize the database locally, execute the SQL scripts in the following order using SQL Server Management Studio (SSMS) or Azure Data Studio:

1. **Create Database & Tables**:
   Run `Database/CreateDatabase.sql`. This script will:
   - Create the `NurseRecordingDb` database.
   - Generate all necessary tables with their respective relationships (Foreign Keys, Cascading deletes).
   - Seed the database with an initial default nurse account.
   - Create database indexes to optimize query performance.

2. **Create Stored Procedures**:
   Run `Database/StoredProcedures.sql`. This establishes reusable CRUD (Create, Read, Update, Delete) operations for the application backend to interface with, such as:
   - `sp_GetPatient`
   - `sp_AddNurse`
   - `sp_AddPatient`
   - `sp_UpdatePatient`
   - `sp_DeletePatient`
   - `sp_SearchPatients`

## Useful Queries

The `Database/QueryNurses.sql` file contains helpful queries for verifying your setup, checking registered accounts, and counting the number of administrative users.

## Frontend Integration

This backend is designed to be consumed by the **Nurse-Recording-System-Web** frontend application. Ensure your connection string in the frontend environment configuration matches your local SQL Server instance.

Example Connection String:
`Server=(localdb)\MSSQLLocalDB;Database=NurseRecordingDb;Trusted_Connection=True;`