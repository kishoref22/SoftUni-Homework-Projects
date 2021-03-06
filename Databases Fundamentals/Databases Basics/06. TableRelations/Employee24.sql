USE SoftUni
GO

SELECT e.EmployeeID,
	   e.FirstName,
	   CASE
			WHEN p.StartDate > '2005-01-01' THEN NULL
			ELSE p.Name
	   END AS ProjectName
FROM Employees AS e
	LEFT OUTER JOIN EmployeesProjects AS ep
		ON e.EmployeeID = ep.EmployeeID
	LEFT OUTER JOIN Projects AS p
		ON p.ProjectID = ep.ProjectID
WHERE e.EmployeeID = 24
ORDER BY e.EmployeeID