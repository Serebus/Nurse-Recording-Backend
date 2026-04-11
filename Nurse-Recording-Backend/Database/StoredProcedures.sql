-- Stored Procedures for NurseRecordingDb
-- Common CRUD operations

USE NurseRecordingDb;
GO

-- SP: Get Patient by ID
CREATE OR ALTER PROCEDURE sp_GetPatient
    @Id INT
AS
BEGIN
    SELECT * FROM Patients WHERE Id = @Id;
END
GO

-- SP: Add Nurse/Admin
CREATE OR ALTER PROCEDURE sp_AddNurse
    @Username NVARCHAR(100),
    @Password NVARCHAR(255),
    @Email NVARCHAR(255),
    @Role NVARCHAR(50) = 'Nurse'
AS
BEGIN
    INSERT INTO Nurses (Username, Password, Email, Role)
    VALUES (@Username, @Password, @Email, @Role);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Add Patient
CREATE OR ALTER PROCEDURE sp_AddPatient
    @Firstname NVARCHAR(100),
    @Middlename NVARCHAR(100) = '',
    @Lastname NVARCHAR(100),
    @Address NVARCHAR(500) = '',
    @Password NVARCHAR(255) = '',
    @Facebook NVARCHAR(500) = '',
    @Email NVARCHAR(255) = '',
    @EmergencyContact NVARCHAR(20) = ''
AS
BEGIN
    INSERT INTO Patients (Firstname, Middlename, Lastname, Address, Password, Facebook, Email, EmergencyContact)
    VALUES (@Firstname, @Middlename, @Lastname, @Address, @Password, @Facebook, @Email, @EmergencyContact);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Update Patient
CREATE OR ALTER PROCEDURE sp_UpdatePatient
    @Id INT,
    @Firstname NVARCHAR(100),
    @Middlename NVARCHAR(100),
    @Lastname NVARCHAR(100),
    @Address NVARCHAR(500),
    @Password NVARCHAR(255),
    @Facebook NVARCHAR(500),
    @Email NVARCHAR(255),
    @EmergencyContact NVARCHAR(20)
AS
BEGIN
    UPDATE Patients SET
        Firstname = @Firstname,
        Middlename = @Middlename,
        Lastname = @Lastname,
        Address = @Address,
        Password = @Password,
        Facebook = @Facebook,
        Email = @Email,
        EmergencyContact = @EmergencyContact
    WHERE Id = @Id;
END
GO

-- SP: Delete Patient
CREATE OR ALTER PROCEDURE sp_DeletePatient
    @Id INT
AS
BEGIN
    DELETE FROM Patients WHERE Id = @Id;
END
GO

-- SP: Search Patients
CREATE OR ALTER PROCEDURE sp_SearchPatients
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Patients
    WHERE Firstname LIKE '%' + @SearchTerm + '%' 
       OR Lastname LIKE '%' + @SearchTerm + '%'
       OR Email LIKE '%' + @SearchTerm + '%';
END
GO

-- Schema patch: align existing Followups table with current backend model
IF COL_LENGTH('dbo.Followups', 'RecordId') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD RecordId INT NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'Diagnosis') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD Diagnosis NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'New_Diagnostic') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD New_Diagnostic NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'Symptom') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD Symptom NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'New_Symptom') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD New_Symptom NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'Treatment') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD Treatment NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.Followups', 'Additional_Treatment') IS NULL
BEGIN
    ALTER TABLE dbo.Followups ADD Additional_Treatment NVARCHAR(1000) NULL;
END
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Followups')
      AND name = 'RecordId'
      AND is_nullable = 1
)
BEGIN
    UPDATE dbo.Followups
    SET RecordId = 1
    WHERE RecordId IS NULL;
END
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Followups')
      AND name = 'RecordId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Followups ALTER COLUMN RecordId INT NOT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Followups_PatientRecords_RecordId'
)
BEGIN
    ALTER TABLE dbo.Followups
    ADD CONSTRAINT FK_Followups_PatientRecords_RecordId
    FOREIGN KEY (RecordId) REFERENCES dbo.PatientRecords(Id);
END
GO

-- Similar SPs for Appointments, Records can be added

PRINT 'Stored procedures and schema patch created successfully.';
GO
