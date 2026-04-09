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

-- Similar SPs for Appointments, Records can be added

PRINT 'Stored procedures created successfully.';
GO
