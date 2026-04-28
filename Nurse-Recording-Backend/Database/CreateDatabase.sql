-- Create Database Script for NurseRecordingDb
-- Run in SQL Server Management Studio or Azure Data Studio

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NurseRecordingDb')
BEGIN
    CREATE DATABASE NurseRecordingDb;
END
GO

USE NurseRecordingDb;
GO

-- Create Tables (matching EF models)
CREATE TABLE Nurses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL, -- Hash in prod
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IsAuthenticated BIT DEFAULT 1,
    Role NVARCHAR(50) DEFAULT 'Nurse' -- Admin/Nurse
);

CREATE TABLE Patients (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Firstname NVARCHAR(100) NOT NULL,
    Middlename NVARCHAR(100),
    Lastname NVARCHAR(100) NOT NULL,
    Address NVARCHAR(500),
    Password NVARCHAR(255),
    Facebook NVARCHAR(500),
    Email NVARCHAR(255) UNIQUE,
    EmergencyContact NVARCHAR(20)
);

CREATE TABLE Appointments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AppointmentId NVARCHAR(20) NOT NULL,
    PatientId INT NOT NULL,
    Date DATE NOT NULL,
    Time NVARCHAR(10) NOT NULL,
    Reason NVARCHAR(1000),
    Status NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE CASCADE
);

CREATE TABLE PatientRecords (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RecordId NVARCHAR(20) NOT NULL,
    PatientId INT NOT NULL,
    Date DATE NOT NULL,
    Diagnosis NVARCHAR(1000),
    Symptom NVARCHAR(1000),
    Treatment NVARCHAR(1000),
    Notes NVARCHAR(MAX),
    Status NVARCHAR(20) DEFAULT 'Open',
    FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE CASCADE
);

CREATE TABLE Followups (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PatientId INT NOT NULL,
    RecordId INT NOT NULL,
    Date DATE NOT NULL,
    Diagnosis NVARCHAR(1000),
    New_Diagnostic NVARCHAR(1000),
    Symptom NVARCHAR(1000),
    New_Symptom NVARCHAR(1000),
    Treatment NVARCHAR(1000),
    Additional_Treatment NVARCHAR(1000),
    Notes NVARCHAR(MAX),
    FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE CASCADE,
    FOREIGN KEY (RecordId) REFERENCES PatientRecords(Id) ON DELETE NO ACTION
);

-- Devices table to track registered hardware
CREATE TABLE Devices (
    DeviceId NVARCHAR(100) PRIMARY KEY,
    Description NVARCHAR(255),
    RegisteredAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Alarms table for IoT
CREATE TABLE Alarms (
    Id INT PRIMARY KEY IDENTITY(1,1),
    State INT NOT NULL, -- 0=Idle,1=Calling,2=Coming,3=Ended
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DeviceId NVARCHAR(100),
    NurseId INT NULL,
    FOREIGN KEY (NurseId) REFERENCES Nurses(Id),
    FOREIGN KEY (DeviceId) REFERENCES Devices(DeviceId) ON DELETE CASCADE
);

-- Indexes
CREATE INDEX IX_Patients_Email        ON Patients(Email);
CREATE INDEX IX_Appointments_Date     ON Appointments(Date);
CREATE INDEX IX_Appointments_PatientId ON Appointments(PatientId);
CREATE INDEX IX_PatientRecords_Date   ON PatientRecords(Date);
CREATE INDEX IX_PatientRecords_PatientId ON PatientRecords(PatientId);
CREATE INDEX IX_Followups_PatientId   ON Followups(PatientId);
CREATE INDEX IX_Followups_RecordId    ON Followups(RecordId);
CREATE INDEX IX_Alarms_State          ON Alarms(State);
CREATE INDEX IX_Alarms_Timestamp      ON Alarms(Timestamp);
CREATE INDEX IX_Alarms_DeviceId       ON Alarms(DeviceId);
GO

-- -------------------------------------------------------
-- Seed Data (mirrors EF Core OnModelCreating HasData)
-- -------------------------------------------------------
-- Default nurse account (password stored as plain-text in dev seed; hash it in production)
IF NOT EXISTS (SELECT 1 FROM Nurses WHERE Email = 'aclcnurse@gmail.com')
BEGIN
    SET IDENTITY_INSERT Nurses ON;
    INSERT INTO Nurses (Id, Username, Password, Email, IsAuthenticated, Role)
    VALUES (1, 'aclcnurse', 'aclcnurse123', 'aclcnurse@gmail.com', 1, 'Nurse');
    SET IDENTITY_INSERT Nurses OFF;
END
GO
