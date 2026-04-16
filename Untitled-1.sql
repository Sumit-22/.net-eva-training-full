SELECT * FROM dbo.Employees;

SELECT * FROM dbo.AspNetUsers;
SELECT * FROM dbo.AspNetRoles;
SELECT * FROM dbo.AspNetUserRoles;
SELECT 
    u.Id,
    u.UserName,
    r.Name AS Role
FROM dbo.AspNetUsers u
JOIN dbo.AspNetUserRoles ur ON u.Id = ur.UserId
JOIN dbo.AspNetRoles r ON ur.RoleId = r.Id;

SELECT * FROM dbo.AspNetUserClaims;
SELECT * FROM dbo.AspNetUserLogins;
SELECT * FROM dbo.AspNetUserTokens;
SELECT * FROM dbo.AspNetRoleClaims;

SELECT 
    'User + Role + Employee' AS InfoType,

    -- USER
    u.Id AS UserId,
    u.UserName,
    u.Email,

    -- ROLE
    r.Name AS Role,

    -- EMPLOYEE
    e.Id AS EmployeeId,
    e.FirstName,
    e.LastName,
    e.Department,
    e.Salary

FROM dbo.AspNetUsers u

-- USER ↔ ROLE
LEFT JOIN dbo.AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN dbo.AspNetRoles r ON ur.RoleId = r.Id

-- USER ↔ EMPLOYEE (IMPORTANT ASSUMPTION)
LEFT JOIN dbo.Employees e ON u.Email = e.Email;
