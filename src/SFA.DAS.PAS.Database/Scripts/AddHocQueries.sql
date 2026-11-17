-- TEST DATA FOR USER AND USER SETTINGS TABLES

INSERT INTO [dbo].[User] ([UserRef], [DisplayName], [Ukprn], [Email], [IsDeleted], [UserType], [LastLogin])
SELECT v.UserRef, v.DisplayName, v.Ukprn, v.Email, v.IsDeleted, v.UserType, v.LastLogin
FROM (VALUES
    ('USR-0001', 'Alice Anderson', 10000001, 'alice.anderson@provider1.test', 0, 1, DATEADD(DAY, -1,  SYSUTCDATETIME())),
    ('USR-0002', 'Bob Brown',      10000001, 'bob.brown@provider1.test',      0, 0, DATEADD(DAY, -3,  SYSUTCDATETIME())),
    ('USR-0003', 'Carol Clark',    10000001, 'carol.clark@provider1.test',    1, 0, DATEADD(DAY, -90, SYSUTCDATETIME())),
    ('USR-0004', 'David Davies',   10000002, 'david.davies@provider2.test',   0, 1, DATEADD(HOUR, -5, SYSUTCDATETIME())),
    ('USR-0005', 'Emma Evans',     10000002, 'emma.evans@provider2.test',     0, 0, NULL),
    ('USR-0006', 'Frank Foster',   10000003, 'frank.foster@provider3.test',   0, 0, DATEADD(DAY, -7,  SYSUTCDATETIME()))
) AS v (UserRef, DisplayName, Ukprn, Email, IsDeleted, UserType, LastLogin)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[User] u WHERE u.UserRef = v.UserRef);

INSERT INTO [dbo].[UserSettings] ([UserId], [UserRef], [ReceiveNotifications])
SELECT u.Id, u.UserRef, CASE WHEN u.UserRef IN ('USR-0002', 'USR-0005') THEN 0 ELSE 1 END
FROM [dbo].[User] u
WHERE u.UserRef IN ('USR-0001','USR-0002','USR-0004','USR-0005')
  AND NOT EXISTS (SELECT 1 FROM [dbo].[UserSettings] s WHERE s.UserId = u.Id);

select * from [dbo].[User]
select * from [dbo].[UserSettings]


-- Get all active (undeleted) users for a specific provider
select p.ukprn, p.name ProviderName, u.DisplayName UserName, u.Email 
from [user] u
join Providers p on p.ukprn = u.ukprn
where u.IsDeleted = 0
and p.ukprn = 10000001
order by p.ukprn

