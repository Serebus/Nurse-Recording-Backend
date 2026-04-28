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

-- SP: Update Alarm State (IoT)
CREATE OR ALTER PROCEDURE sp_UpdateAlarmState
    @State INT,
    @DeviceId NVARCHAR(100) = NULL,
    @NurseId INT = NULL
AS
BEGIN
    INSERT INTO Alarms (State, DeviceId, NurseId)
    VALUES (@State, @DeviceId, @NurseId);
    
    SELECT SCOPE_IDENTITY() AS Id;
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

IF COL_LENGTH('dbo.PatientRecords', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.PatientRecords ADD Status NVARCHAR(20) DEFAULT 'Open' WITH VALUES;
END
GO

PRINT 'Stored procedures and schema patch created successfully.';
GO

-- =============================================================
-- ADMIN
-- =============================================================

-- SP: Get Admin profile by Id
CREATE OR ALTER PROCEDURE sp_GetAdmin
    @Id INT
AS
BEGIN
    SELECT Id, Username, Email, Role, IsAuthenticated
    FROM Nurses
    WHERE Id = @Id AND Role = 'Admin';
END
GO

-- SP: Update Admin (Username / Email / Password)
CREATE OR ALTER PROCEDURE sp_UpdateAdmin
    @Id       INT,
    @Username NVARCHAR(100) = NULL,
    @Email    NVARCHAR(255) = NULL,
    @Password NVARCHAR(255) = NULL   -- Pass pre-hashed value from the API layer
AS
BEGIN
    UPDATE Nurses SET
        Username = COALESCE(NULLIF(@Username, ''), Username),
        Email    = COALESCE(NULLIF(@Email,    ''), Email),
        Password = COALESCE(NULLIF(@Password, ''), Password)
    WHERE Id = @Id AND Role = 'Admin';
END
GO

-- =============================================================
-- NURSES
-- =============================================================

-- SP: Get all Nurses
CREATE OR ALTER PROCEDURE sp_GetAllNurses
AS
BEGIN
    SELECT Id, Username, Email, Role, IsAuthenticated FROM Nurses;
END
GO

-- SP: Get Nurse by Id
CREATE OR ALTER PROCEDURE sp_GetNurse
    @Id INT
AS
BEGIN
    SELECT Id, Username, Email, Role, IsAuthenticated
    FROM Nurses WHERE Id = @Id;
END
GO

-- SP: Update Nurse
CREATE OR ALTER PROCEDURE sp_UpdateNurse
    @Id       INT,
    @Username NVARCHAR(100),
    @Email    NVARCHAR(255),
    @Password NVARCHAR(255),
    @Role     NVARCHAR(50)
AS
BEGIN
    UPDATE Nurses SET
        Username = @Username,
        Email    = @Email,
        Password = @Password,
        Role     = @Role
    WHERE Id = @Id;
END
GO

-- SP: Delete Nurse
CREATE OR ALTER PROCEDURE sp_DeleteNurse
    @Id INT
AS
BEGIN
    DELETE FROM Nurses WHERE Id = @Id;
END
GO

-- =============================================================
-- APPOINTMENTS
-- =============================================================

-- SP: Get all Appointments (with patient name)
CREATE OR ALTER PROCEDURE sp_GetAllAppointments
AS
BEGIN
    SELECT a.*, p.Firstname, p.Lastname
    FROM Appointments a
    INNER JOIN Patients p ON a.PatientId = p.Id;
END
GO

-- SP: Get Appointment by Id
CREATE OR ALTER PROCEDURE sp_GetAppointment
    @Id INT
AS
BEGIN
    SELECT a.*, p.Firstname, p.Lastname
    FROM Appointments a
    INNER JOIN Patients p ON a.PatientId = p.Id
    WHERE a.Id = @Id;
END
GO

-- SP: Add Appointment
CREATE OR ALTER PROCEDURE sp_AddAppointment
    @AppointmentId NVARCHAR(20),
    @PatientId     INT,
    @Date          DATE,
    @Time          NVARCHAR(10),
    @Reason        NVARCHAR(1000) = NULL,
    @Status        NVARCHAR(20)   = 'Pending'
AS
BEGIN
    INSERT INTO Appointments (AppointmentId, PatientId, Date, Time, Reason, Status)
    VALUES (@AppointmentId, @PatientId, @Date, @Time, @Reason, @Status);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Update Appointment
CREATE OR ALTER PROCEDURE sp_UpdateAppointment
    @Id            INT,
    @AppointmentId NVARCHAR(20),
    @PatientId     INT,
    @Date          DATE,
    @Time          NVARCHAR(10),
    @Reason        NVARCHAR(1000) = NULL,
    @Status        NVARCHAR(20)   = 'Pending'
AS
BEGIN
    UPDATE Appointments SET
        AppointmentId = @AppointmentId,
        PatientId     = @PatientId,
        Date          = @Date,
        Time          = @Time,
        Reason        = @Reason,
        Status        = @Status
    WHERE Id = @Id;
END
GO

-- SP: Delete Appointment
CREATE OR ALTER PROCEDURE sp_DeleteAppointment
    @Id INT
AS
BEGIN
    DELETE FROM Appointments WHERE Id = @Id;
END
GO

-- =============================================================
-- PATIENT RECORDS
-- =============================================================

-- SP: Get all PatientRecords
CREATE OR ALTER PROCEDURE sp_GetAllPatientRecords
AS
BEGIN
    SELECT pr.*, p.Firstname, p.Lastname
    FROM PatientRecords pr
    INNER JOIN Patients p ON pr.PatientId = p.Id;
END
GO

-- SP: Get PatientRecord by Id
CREATE OR ALTER PROCEDURE sp_GetPatientRecord
    @Id INT
AS
BEGIN
    SELECT pr.*, p.Firstname, p.Lastname
    FROM PatientRecords pr
    INNER JOIN Patients p ON pr.PatientId = p.Id
    WHERE pr.Id = @Id;
END
GO

-- SP: Add PatientRecord
CREATE OR ALTER PROCEDURE sp_AddPatientRecord
    @RecordId  NVARCHAR(20),
    @PatientId INT,
    @Date      DATE,
    @Diagnosis NVARCHAR(1000) = NULL,
    @Symptom   NVARCHAR(1000) = NULL,
    @Treatment NVARCHAR(1000) = NULL,
    @Notes     NVARCHAR(MAX)  = NULL,
    @Status    NVARCHAR(20)   = 'Open'
AS
BEGIN
    INSERT INTO PatientRecords (RecordId, PatientId, Date, Diagnosis, Symptom, Treatment, Notes, Status)
    VALUES (@RecordId, @PatientId, @Date, @Diagnosis, @Symptom, @Treatment, @Notes, @Status);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Update PatientRecord
CREATE OR ALTER PROCEDURE sp_UpdatePatientRecord
    @Id        INT,
    @RecordId  NVARCHAR(20),
    @PatientId INT,
    @Date      DATE,
    @Diagnosis NVARCHAR(1000) = NULL,
    @Symptom   NVARCHAR(1000) = NULL,
    @Treatment NVARCHAR(1000) = NULL,
    @Notes     NVARCHAR(MAX)  = NULL,
    @Status    NVARCHAR(20)   = 'Open'
AS
BEGIN
    UPDATE PatientRecords SET
        RecordId  = @RecordId,
        PatientId = @PatientId,
        Date      = @Date,
        Diagnosis = @Diagnosis,
        Symptom   = @Symptom,
        Treatment = @Treatment,
        Notes     = @Notes,
        Status    = @Status
    WHERE Id = @Id;
END
GO

-- SP: Delete PatientRecord
CREATE OR ALTER PROCEDURE sp_DeletePatientRecord
    @Id INT
AS
BEGIN
    DELETE FROM PatientRecords WHERE Id = @Id;
END
GO

-- =============================================================
-- FOLLOWUPS
-- =============================================================

-- SP: Get all Followups
CREATE OR ALTER PROCEDURE sp_GetAllFollowups
AS
BEGIN
    SELECT f.*, p.Firstname, p.Lastname
    FROM Followups f
    INNER JOIN Patients p ON f.PatientId = p.Id;
END
GO

-- SP: Get Followup by Id
CREATE OR ALTER PROCEDURE sp_GetFollowup
    @Id INT
AS
BEGIN
    SELECT f.*, p.Firstname, p.Lastname
    FROM Followups f
    INNER JOIN Patients p ON f.PatientId = p.Id
    WHERE f.Id = @Id;
END
GO

-- SP: Add Followup
CREATE OR ALTER PROCEDURE sp_AddFollowup
    @PatientId            INT,
    @RecordId             INT,
    @Date                 DATE,
    @Diagnosis            NVARCHAR(1000) = NULL,
    @New_Diagnostic       NVARCHAR(1000) = NULL,
    @Symptom              NVARCHAR(1000) = NULL,
    @New_Symptom          NVARCHAR(1000) = NULL,
    @Treatment            NVARCHAR(1000) = NULL,
    @Additional_Treatment NVARCHAR(1000) = NULL,
    @Notes                NVARCHAR(MAX)  = NULL
AS
BEGIN
    INSERT INTO Followups (PatientId, RecordId, Date, Diagnosis, New_Diagnostic, Symptom, New_Symptom, Treatment, Additional_Treatment, Notes)
    VALUES (@PatientId, @RecordId, @Date, @Diagnosis, @New_Diagnostic, @Symptom, @New_Symptom, @Treatment, @Additional_Treatment, @Notes);
    SELECT SCOPE_IDENTITY() AS Id;
END
GO

-- SP: Update Followup
CREATE OR ALTER PROCEDURE sp_UpdateFollowup
    @Id                   INT,
    @PatientId            INT,
    @RecordId             INT,
    @Date                 DATE,
    @Diagnosis            NVARCHAR(1000) = NULL,
    @New_Diagnostic       NVARCHAR(1000) = NULL,
    @Symptom              NVARCHAR(1000) = NULL,
    @New_Symptom          NVARCHAR(1000) = NULL,
    @Treatment            NVARCHAR(1000) = NULL,
    @Additional_Treatment NVARCHAR(1000) = NULL,
    @Notes                NVARCHAR(MAX)  = NULL
AS
BEGIN
    UPDATE Followups SET
        PatientId            = @PatientId,
        RecordId             = @RecordId,
        Date                 = @Date,
        Diagnosis            = @Diagnosis,
        New_Diagnostic       = @New_Diagnostic,
        Symptom              = @Symptom,
        New_Symptom          = @New_Symptom,
        Treatment            = @Treatment,
        Additional_Treatment = @Additional_Treatment,
        Notes                = @Notes
    WHERE Id = @Id;
END
GO

-- SP: Delete Followup
CREATE OR ALTER PROCEDURE sp_DeleteFollowup
    @Id INT
AS
BEGIN
    DELETE FROM Followups WHERE Id = @Id;
END
GO

-- =============================================================
-- DEVICES
-- =============================================================

-- SP: Register (or confirm) a Device
CREATE OR ALTER PROCEDURE sp_RegisterDevice
    @DeviceId    NVARCHAR(100),
    @Description NVARCHAR(255) = 'Auto-registered'
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Devices WHERE DeviceId = @DeviceId)
    BEGIN
        INSERT INTO Devices (DeviceId, Description, RegisteredAt)
        VALUES (@DeviceId, @Description, GETUTCDATE());
    END
    SELECT * FROM Devices WHERE DeviceId = @DeviceId;
END
GO

-- SP: Get all Devices
CREATE OR ALTER PROCEDURE sp_GetAllDevices
AS
BEGIN
    SELECT * FROM Devices;
END
GO

-- =============================================================
-- ALARMS
-- =============================================================

-- SP: Get latest alarm state per device
CREATE OR ALTER PROCEDURE sp_GetLatestAlarms
AS
BEGIN
    SELECT a.*
    FROM Alarms a
    INNER JOIN (
        SELECT DeviceId, MAX(Timestamp) AS LatestTs
        FROM Alarms
        GROUP BY DeviceId
    ) latest ON a.DeviceId = latest.DeviceId AND a.Timestamp = latest.LatestTs;
END
GO

-- SP: Get latest alarm for a specific device
CREATE OR ALTER PROCEDURE sp_GetLatestAlarmByDevice
    @DeviceId NVARCHAR(100)
AS
BEGIN
    SELECT TOP 1 *
    FROM Alarms
    WHERE DeviceId = @DeviceId
    ORDER BY Timestamp DESC;
END
GO

PRINT 'All stored procedures created successfully.';
GO
