-- Query to view all Nurses in NurseRecordingDb
USE NurseRecordingDb;
GO

SELECT Id, Username, Email, Role, IsAuthenticated, Password -- Remove Password in prod
FROM Nurses
ORDER BY Username;

-- Count
SELECT 'Nurse Count' AS Info, COUNT(*) AS Total FROM Nurses;

-- Admins only
SELECT * FROM Nurses WHERE Role = 'Admin';

PRINT 'Nurses loaded.';
GO
